using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach(Sound snd in sounds)
        {
            snd.source = gameObject.AddComponent<AudioSource>();
            snd.source.clip = snd.clip;

            snd.source.volume = snd.volume;
            snd.source.pitch = snd.pitch;
            snd.source.loop = snd.loop;
        }
    }

    public void Play(string name)
    {
        Play(name, 1.0f);
    }

    public void Play(string name,  float pitch)
    {
        Sound snd = getSource(name);
        if(snd == null)
        {
            return;
        }

        snd.source.pitch = pitch;

        snd.source.Play();
    }

    public void Stop()
    {
        foreach(Sound snd in sounds)
        {
            if(snd != null)
            {
                snd.source.Stop();
            }
        }
    }

    public void Stop(string name)
    {
        
        Sound snd = getSource(name);
        if(snd == null)
        {
            return;
        }
        snd.source.Stop();
    }

    public void changeVolume(string name, float vol)
    {
        Sound snd = getSource(name);
        if(snd == null)
        {
            return;
        }
        vol = Mathf.Clamp(vol, 0f, 1f);
        snd.source.volume = vol;
        snd.volume = vol;
    }

    private Sound getSource(string name)
    {
        Sound snd = null;

        snd = Array.Find(sounds, sound => sound.name == name);

        if(snd == null)
            Debug.LogWarning("Could not find sound: " + name);
        return snd;
    }
}