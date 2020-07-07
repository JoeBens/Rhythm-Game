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
    public float bpm = 130;
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


    public bool isDBZ = false;
    private void Awake()
    {
       
    }
    private void Start()
    {
        panelActivated = true;
        secPerBeat = 60f / bpm;
        arrayNote = FindObjectOfType<MidiReader>().GetComponent<MidiReader>().GetArray();
        eventsBeatsArr = FindObjectOfType<MidiReader>().GetComponent<MidiReader>().GetEventsBeat();


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

            Note musicNote;

            if (isDBZ == true)
            {
                int rand = UnityEngine.Random.Range(2, 9);

                musicNote = ObjectPooler.SharedInstance.GetPooledObject(rand).GetComponent<Note>();
            }
            else
            {
                musicNote = ObjectPooler.SharedInstance.GetPooledObject(0).GetComponent<Note>();
            }


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


            currentIndex++;
            progressSlider.value = currentIndex;




        }
    }

}
