using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundsPauseUI : MonoBehaviour
{
    AudioSource pausaSource;
    AudioSource backSource;
    AudioSource nextSource;

    public AudioClip pausaClip;
    public AudioClip backClip;
    public AudioClip nextClip;
    public AudioMixerGroup mixerGroup;


    void Start() {
        pausaSource = AddAudio (false, false, pausaClip);
        backSource = AddAudio (false, false, backClip);
        nextSource = AddAudio (false, false, nextClip);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playPause();
        }
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

    public void playBack(){
        backSource.PlayOneShot(backSource.clip, 1f);
    }
    public void playNext(){
        nextSource.PlayOneShot(nextSource.clip, 1f);
    }
    public void playPause(){
        pausaSource.PlayOneShot(pausaSource.clip, 1f);
    }
}
