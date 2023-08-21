using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioMixer mixer;
    public Slider slider1, slider2;

    void Start()
    {
        slider1.value = PlayerPrefs.GetFloat("Bgm",0.75f);
        slider2.value = PlayerPrefs.GetFloat("Sfx",0.75f);
    }

    public void SetBGMVolume(float sliderValue)
    {
        mixer.SetFloat("Bgm",Mathf.Log10(sliderValue)*20);
        PlayerPrefs.SetFloat("Bgm",sliderValue);
    }

    public void SetSfxVolume(float sliderValue)
    {
        mixer.SetFloat("Sfx",Mathf.Log10(sliderValue)*20);
        PlayerPrefs.SetFloat("Sfx",sliderValue);
    }
}
