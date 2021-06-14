using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    
    public GameObject BG;

    public Sprite fondo;
    public Sprite opciones;

    public Slider slider;


    bool openSetting=false;
    
    void Awake()
    {
        audioMixer.GetFloat("volume", out var vol);
        slider.value = vol;

        gameObject.SetActive(false);
        GameObject pausa = GameObject.Find("Canvas").transform.Find("PauseMenu")?.gameObject as GameObject;
        if (pausa != null) pausa.gameObject.SetActive(false);
    }

    public void setVolume (float volume) {
        audioMixer.SetFloat("volume", volume);
    }
    public void setQuality (int index) {
        QualitySettings.SetQualityLevel(index);
    }
    public void SetFullscreen (bool isFully){
        Screen.fullScreen = isFully;
    }
    public void changeBg(){
        openSetting=!openSetting;
        if (openSetting)
        {
            BG.GetComponent<Image>().sprite =  opciones;
        }else{
            BG.GetComponent<Image>().sprite =  fondo;
        }
    }
}
