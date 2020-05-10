using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

using NAudio.Midi;
using UnityEngine.UI;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameObject note;
    public Transform[] spawnPositions;

    public int currentIndex = 0;

    public float songPosition;
    public float songPosInBeats;
    float dsptimesong;
    float bpm = 130;
    public float secPerBeat;

    
    public long[] eventsBeatsArr;

    public decimal[] arrayNote;

    int notePosMusic = 0;

    public float startLineY;
    public float posX;
    public float finishLineY;
    public float removeLineY;


    public float beatsShownOnScreen = 10f;

    private Queue<Note> notesOnScreen;

    public bool hitNote = false;

    public float songOffset;

    private bool startSong = false;
    private bool startToPlay = false;


    public AudioSource songAudioSource;

    public Slider progressSlider;

    public GameObject tapPanel;
    
    public bool panelActivated = true;

    private void Awake()
    {
        panelActivated = true;
        secPerBeat = 60f / bpm;
        List<decimal> noteTimeHolder = CalculateMidiRealTime();
        arrayNote = ReturnArray(noteTimeHolder);
    }
    private void Start()
    {
        notesOnScreen = new Queue<Note>();

        dsptimesong = (float)AudioSettings.dspTime;
        
        currentIndex = 0;
        Debug.Log(arrayNote.Length);
        //Invoke("StartPlaying", 5f);

        progressSlider.maxValue = arrayNote.Length;
        progressSlider.value = currentIndex;

    }

  

    private void StartPlaying()
    {
        startSong = true;
        dsptimesong = (float)AudioSettings.dspTime;
        songAudioSource.Play();
        AudioManager.instance.Play("loop");

    }

    private void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            if(startToPlay == false)
            {
                startToPlay = true;
                StartPlaying();
            }
            tapPanel.SetActive(false);
            panelActivated = false;

        }
        if (Input.GetMouseButtonUp(0))
        {
            tapPanel.SetActive(true);
            panelActivated = true;
        }


        if (startSong == false)
        {
            return;
        }

        if (hitNote == true)
        {
            hitNote = false;
            if (notesOnScreen.Count > 0)
            {
                notesOnScreen.Dequeue();
            }
        }

        songPosition = (float)(AudioSettings.dspTime - dsptimesong - songOffset);
        float beatToShow = songPosition / secPerBeat + beatsShownOnScreen;
        songPosInBeats = songPosition / secPerBeat;
        int beatsShownInAdvance = currentIndex - notePosMusic;

        if (currentIndex < arrayNote.Length && arrayNote[currentIndex] < (decimal)beatToShow)
        {
            // Instantiate a new music note. (Search "Object Pooling" for more information if you wish to minimize the delay when instantiating game objects.)
            // We don't care about the position and rotation because we will set them later in MusicNote.Initialize(...).

            //Enemy musicNote = ((GameObject)Instantiate(note, transform.position, Quaternion.identity)).GetComponent<Enemy>();

            Note musicNote = ObjectPooler.SharedInstance.GetPooledObject(0).GetComponent<Note>();
            musicNote.gameObject.SetActive(true);
            if (eventsBeatsArr[currentIndex] == 1)
            {
                posX = -2f;
            }
            else if (eventsBeatsArr[currentIndex] == 2)
            {
                posX = -0.6666667f;
            }
            else if (eventsBeatsArr[currentIndex] == 3)
            {
                posX = 0.6666667f;
            }
            else
            {
                posX = 2f;
            }
            musicNote.Initialize(this, startLineY, finishLineY, removeLineY, posX, arrayNote[currentIndex], currentIndex + "", currentIndex);

            // The note is push into the queue for reference.
            notesOnScreen.Enqueue(musicNote);


            // Update the next index.
            currentIndex++;
            progressSlider.value = currentIndex;




        }
    }

    private long GetBeat(long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
    {
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
        return beat;
    }


    private List<decimal> CalculateMidiRealTime()
    {
        var strictMode = false;
        var mf = new MidiFile("Assets/Audios/Midi Files/Jojo.mid", strictMode);
        mf.Events.MidiFileType = 0;

        // Have just one collection for both non-note-off and tempo change events
        List<MidiEvent> midiEvents = new List<MidiEvent>();
        List<long> beatsList = new List<long>();

        for (int n = 0; n < mf.Tracks; n++)
        {
            foreach (var midiEvent in mf.Events[n])
            {
                if (!MidiEvent.IsNoteOff(midiEvent))
                {
                    var timeSignature = mf.Events[0].OfType<TimeSignatureEvent>().FirstOrDefault();
                    long beat = GetBeat(midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature);
                    beatsList.Add(beat);
                    midiEvents.Add(midiEvent);
                    Debug.Log("Beat: " + beat);

                    // Instead of causing stack unwinding with try/catch,
                    // we just test if the event is of type TempoEvent
                    if (midiEvent is TempoEvent)
                    {
                        //Debug.Log("Absolute Time " + (midiEvent as TempoEvent).AbsoluteTime);
                    }
                }
            }
        }

        // Switch to decimal from float.
        // decimal has 28-29 digits percision
        // while float has only 6-9
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types

        // Now we have only one collection of both non-note-off and tempo events
        // so we cannot be sure of the size of the time values array.
        // Just employ a List<float>
        List<decimal> eventsTimesArr = new List<decimal>();
        

        // Keep track of the last absolute time and last real time because
        // tempo events also can occur "between" events
        // which can cause incorrect times when calculated using AbsoluteTime
        decimal lastRealTime = 0m;
        decimal lastAbsoluteTime = 0m;

        // instead of keeping the tempo event itself, and
        // instead of multiplying every time, just keep
        // the current value for microseconds per tick
        decimal currentMicroSecondsPerTick = 0m;

        for (int i = 0; i < midiEvents.Count; i++)
        {
            MidiEvent midiEvent = midiEvents[i];
            TempoEvent tempoEvent = midiEvent as TempoEvent;

            // Just append to last real time the microseconds passed
            // since the last event (DeltaTime * MicroSecondsPerTick
            if (midiEvent.AbsoluteTime > lastAbsoluteTime)
            {
                lastRealTime += ((decimal)midiEvent.AbsoluteTime - lastAbsoluteTime) * currentMicroSecondsPerTick;
            }

            lastAbsoluteTime = midiEvent.AbsoluteTime;

            if (tempoEvent != null)
            {
                // Recalculate microseconds per tick
                currentMicroSecondsPerTick = (decimal)tempoEvent.MicrosecondsPerQuarterNote / (decimal)mf.DeltaTicksPerQuarterNote;

                // Remove the tempo event to make events and timings match - index-wise
                // Do not add to the eventTimes
                midiEvents.RemoveAt(i);
                beatsList.RemoveAt(i);
                i--;
                continue;
            }

            // Add the time to the collection.
            eventsTimesArr.Add(lastRealTime / 1000000m);

            //Debug.Log("Time: " + lastRealTime / 1000000m);
        }
        eventsBeatsArr = beatsList.ToArray();
        Debug.Log("Length: " + eventsBeatsArr.Length);
        return eventsTimesArr;

    }
    private decimal[] ReturnArray(List<decimal> noteTimeHolder)
    {
        decimal[] arr = noteTimeHolder.Distinct().ToArray();

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i] / (decimal)secPerBeat;
            //arr[i] = (decimal)Math.Round(arr[i] / (decimal)secPerBeat, 1);
            //arr[i] = (decimal)Math.Round(arr[i], 1);
            //Debug.Log("Time" + arr[i]);
        }
        return arr;
    }

}
