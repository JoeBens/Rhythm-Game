using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

using NAudio.Midi;
using UnityEngine.UI;
using System.IO;
using System.Globalization;

public class MidiReader : MonoBehaviour
{

    public float bpm;
    public float secPerBeat;


    public long[] eventsBeatsArr;

    public decimal[] arrayNote;

    public decimal[] arrayNoteTextAsset;

    public TextAsset textFileNotes;
    public TextAsset textFileBeats;
    private void Awake()
    {
        secPerBeat = 60f / bpm;
        //List<decimal> noteTimeHolder = CalculateMidiRealTime();
        //arrayNote = ReturnArray(noteTimeHolder);
        //WriteNotes();
        //WriteBeats();
        arrayNoteTextAsset = ReadTextAssetNotes();
        eventsBeatsArr = ReadTextAssetBeats();

    }

    public decimal[] GetArray()
    {
        return arrayNoteTextAsset;
    }

    public long[] GetEventsBeat()
    {
        return eventsBeatsArr;
    }
    public void WriteNotes()
    {
        List<string> notes = new List<string>();
        for (int i = 0; i < arrayNote.Length; i++)
        {
            arrayNote[i] = arrayNote[i] / 1.4m;
            notes.Add(arrayNote[i].ToString());
        }

        string[] arrayNoteV2 = notes.ToArray();
        System.IO.File.WriteAllLines("Assets/Resources/AgainNotes.txt", arrayNoteV2);
    }

    public void WriteBeats()
    {
        List<string> notes = new List<string>();
        for (int i = 0; i < eventsBeatsArr.Length; i++)
        {
            notes.Add(eventsBeatsArr[i].ToString());
        }
        System.IO.File.WriteAllLines("Assets/Resources/AgainBeats.txt", notes);
    }

    //public decimal[] Read()
    //{

    //    string[] lines = System.IO.File.ReadAllLines("Assets/Audios/Jojo.txt");
        
    //    decimal[] notes = new decimal[lines.Length];
    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        decimal x = decimal.Parse(lines[i]);
    //        notes[i] = x;
    //    }
    //    return notes;
    //}
    public decimal[] ReadTextAssetNotes()
    {

        List<string> notesList;
        notesList = new List<string>(textFileNotes.text.Split('\n'));

        string[] lines = notesList.ToArray();
        decimal[] notes = new decimal[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            decimal x = decimal.Parse(lines[i]);
            notes[i] = x;
            //Debug.Log(lines[i]);
        }
        return notes;
    }
    public long[] ReadTextAssetBeats()
    {

        List<string> notesList;
        notesList = new List<string>(textFileBeats.text.Split('\n'));

        string[] lines = notesList.ToArray();
        long[] notes = new long[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {            
            long x = long.Parse(lines[i].Replace(",","."), CultureInfo.InvariantCulture);
            notes[i] = x;
            //Debug.Log(lines[i]);
        }
        return notes;
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
        var mf = new MidiFile("Assets/Audios/Midi Files/AgainYui.mid", strictMode);
        mf.Events.MidiFileType = 0;

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
                    //Debug.Log("Beat: " + beat);

                    if (midiEvent is TempoEvent)
                    {
                        //Debug.Log("Absolute Time " + (midiEvent as TempoEvent).AbsoluteTime);
                    }
                }
            }
        }


        List<decimal> eventsTimesArr = new List<decimal>();


        decimal lastRealTime = 0m;
        decimal lastAbsoluteTime = 0m;


        decimal currentMicroSecondsPerTick = 0m;

        for (int i = 0; i < midiEvents.Count; i++)
        {
            MidiEvent midiEvent = midiEvents[i];
            TempoEvent tempoEvent = midiEvent as TempoEvent;

            if (midiEvent.AbsoluteTime > lastAbsoluteTime)
            {
                lastRealTime += ((decimal)midiEvent.AbsoluteTime - lastAbsoluteTime) * currentMicroSecondsPerTick;
            }

            lastAbsoluteTime = midiEvent.AbsoluteTime;

            if (tempoEvent != null)
            {
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

            Debug.Log("Time: " + lastRealTime / 1000000m);
        }
        eventsBeatsArr = beatsList.ToArray();
        //Debug.Log("Length: " + eventsBeatsArr.Length);
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
