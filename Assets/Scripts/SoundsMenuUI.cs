using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundsMenuUI : MonoBehaviour
{
    AudioSource menuSource;
    AudioSource backSource;
    AudioSource nextSource;

    public AudioClip menuClip;
    public AudioClip backClip;
    public AudioClip nextClip;
    public AudioMixerGroup mixerGroup;

    void Start() {
        menuSource = AddAudio (true, true, menuClip);
        backSource = AddAudio (false, false, backClip);
        nextSource = AddAudio (false, false, nextClip);
        StartPlayingSounds ();
    }
    public AudioSource AddAudio(bool loop, bool playAwake,AudioClip clip ) 
    { 
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip; 
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = 1f; 
        newAudio.outputAudioMixerGroup = mixerGroup;
        return newAudio; 
    }
    void StartPlayingSounds(){
        menuSource.Play();
    }
    public void playBack(){
        backSource.PlayOneShot(backSource.clip, 1f);
    }
    public void playNext(){
        nextSource.PlayOneShot(nextSource.clip, 1f);
    }
    
}
