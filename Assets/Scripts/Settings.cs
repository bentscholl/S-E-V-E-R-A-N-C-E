using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    AudioMixer Mixer;
    Slider volSlider;
    Toggle vSyncToggle;
    Toggle fullscreenToggle;
    // Start is called before the first frame update
    void Start()
    {
        Mixer = Resources.Load<AudioMixer>("AudioMixer");
        volSlider = GameObject.Find("VolSlider").GetComponent<Slider>();
        vSyncToggle = GameObject.Find("VSync").GetComponent<Toggle>();
        fullscreenToggle = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        volSlider.value = PlayerPrefs.GetFloat("Volume");
        vSyncToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("VSync"));
        fullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume()
    {
        float vol = Mathf.Log(volSlider.value) * 20;
        Mixer.SetFloat("MVolume", vol);
        PlayerPrefs.SetFloat("Volume", volSlider.value);
    }

    public void SetVSync()
    {
        PlayerPrefs.SetInt("VSync", Convert.ToInt32(vSyncToggle.isOn));
        QualitySettings.vSyncCount = Convert.ToInt32(vSyncToggle.isOn);
    }

    public void SetFullscreen()
    {
        PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(fullscreenToggle.isOn));
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}
