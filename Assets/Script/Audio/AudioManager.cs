using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance {get; private set;}

	[SerializeField] private AudioLibrarySO audioLibrarySO;
	
	[Range(0, 1)][SerializeField] private float mainVolume;
	[Range(0, 1)][SerializeField] private float bgmVolume;
	[Range(0, 1)][SerializeField] private float sfxVolume;

	private AudioSource bgmAudioSource;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);	
		}
		
		
		bgmAudioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		mainVolume = GameDataManager.Instance.gameData.mainVolume;
		bgmVolume = GameDataManager.Instance.gameData.bgmVolume;
		sfxVolume = GameDataManager.Instance.gameData.sfxVolume;

		PlayBGM("BackGroundMusic");
	}

	public void PlayBGM(string key, float volume = 0.5f)
	{
		AudioClip audioClip = audioLibrarySO.GetAudioClip(key);

		if(audioClip != null)
		{
			if(bgmAudioSource.clip == audioClip) return;

			bgmAudioSource.clip = audioClip;
			bgmAudioSource.loop = true;
			bgmAudioSource.volume = mainVolume * bgmVolume;
			bgmAudioSource.Play();
		}
		else
		{
			Debug.LogError("AudioManager PlaySound 輸入的 Key 有錯");
		}
	}

	public void PlaySound(string key, Vector3 position, float volume = 0.5f)
	{
		AudioClip audioClip = audioLibrarySO.GetAudioClip(key);
		
		if(audioClip != null)
		{
			AudioSource.PlayClipAtPoint(audioClip, position, mainVolume * sfxVolume);
		}
		else
		{
			Debug.LogError("AudioManager PlaySound 輸入的 Key 有錯");
		}
	}

	public void SetMainVolume(float mainVolume)
	{
		this.mainVolume = mainVolume;
		bgmAudioSource.volume = mainVolume * bgmVolume;

		GameDataManager.Instance.gameData.mainVolume = mainVolume;
	}

	public void SetBGMVolume(float bgmVolume)
	{
		this.bgmVolume = bgmVolume;
		bgmAudioSource.volume = mainVolume * bgmVolume;

		GameDataManager.Instance.gameData.bgmVolume = bgmVolume;
	}

	public void SetSFXVolume(float sfxVolume)
	{
		this.sfxVolume = sfxVolume;

		GameDataManager.Instance.gameData.sfxVolume = sfxVolume;
	}
	
}
