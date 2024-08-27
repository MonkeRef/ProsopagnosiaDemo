using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour {
    public static SettingUI Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button closeButton;

    private void Awake () {
        Instance = this;

        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start () {
        if (PlayerPrefs.HasKey("musicVolume")){
            LoadVolume();
        } else {
            SetMusicVolume();
            SetSFXVolume();
        }

        Hide();
    }

    public void SetMusicVolume () {
        float musicVolume = musicSlider.value; // Take the value of the slider
        audioMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20); // Set music volume to audioMixer
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }

    public void SetSFXVolume () {
        float sfxVolume = sfxSlider.value; // Take the value of the slider
        audioMixer.SetFloat("Sfx", Mathf.Log10(sfxVolume) * 20); // Set music volume to audioMixer
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    private void LoadVolume () {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetMusicVolume();
    }

    public void Show () {
        gameObject.SetActive(true);
    }

    private void Hide () {
        gameObject.SetActive(false);
    }
}
