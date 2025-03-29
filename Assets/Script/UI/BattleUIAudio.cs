using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIAudio : MonoBehaviour
{
    [SerializeField] private Slider _mainVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

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
    }

    private void OnDisable()
    {
        GameDataManager.Instance.SaveGame();
    }
}
