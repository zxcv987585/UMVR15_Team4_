using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        mainSlider.value = GameDataManager.Instance.gameData.mainVolume;
        bgmSlider.value = GameDataManager.Instance.gameData.bgmVolume;
        sfxSlider.value = GameDataManager.Instance.gameData.sfxVolume;

        mainSlider.onValueChanged.AddListener(AudioManager.Instance.SetMainVolume);
        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);

        exitButton.onClick.AddListener(() => 
        {
            GameDataManager.Instance.SaveGame();
            Hide();
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    
    
}
