using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;


    
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    public Sound[] sounds;



    private void Awake()
    {
        

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        //Charger tous les sons qui peuvent être
        //joués au cours d'une partie
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;

            s.source.volume = s.volume;

            s.source.pitch = s.pitch;

            s.source.loop = s.loop;

            s.source.playOnAwake = s.playOnAwake;
        }
    }



    // Use this for initialization
    void Start()
    {
       
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Doesn't exist");

            return;
        }
        Debug.Log("Exists!");
        s.source.Play();
    }
    public void Pause(string name)
    {
        //chercher le son avec le "name" et le jouer
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Pause();
    }
    public void PauseEverything()
    {
        //chercher le son avec le "name" et le pauser
        foreach (Sound s in sounds)
        {
            s.source.Pause();
        }
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }
    public void StopEverything()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }
}
