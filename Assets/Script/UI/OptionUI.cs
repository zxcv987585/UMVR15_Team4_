using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider _mainVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Button _settingFinishButton;

    private void OnEnable()
    {
        if(GameDataManager.Instance == null) return;
        _mainVolumeSlider.value = GameDataManager.Instance.gameData.mainVolume;
        _bgmVolumeSlider.value = GameDataManager.Instance.gameData.bgmVolume;
        _sfxVolumeSlider.value = GameDataManager.Instance.gameData.sfxVolume;

        if(AudioManager.Instance == null) return;
        _mainVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMainVolume);
        _bgmVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        _settingFinishButton.onClick.AddListener(GameDataManager.Instance.SaveGame);
    }

    private void OnDisable()
    {
        _mainVolumeSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetMainVolume);
        _bgmVolumeSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetBGMVolume);
        _sfxVolumeSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetSFXVolume);
        _settingFinishButton.onClick.RemoveListener(GameDataManager.Instance.SaveGame);
    }
}
