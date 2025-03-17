using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance {get; private set;}

	[SerializeField] private AudioLibrarySO _audioLibrarySO;
	[Range(0, 1)][SerializeField] private float _mainVolume;
	[Range(0, 1)][SerializeField] private float _bgmVolume;
	[Range(0, 1)][SerializeField] private float _sfxVolume;

	[SerializeField] private int _audioPoolSize;
	private Queue<AudioSource> _audioPool = new Queue<AudioSource>();
	private Dictionary<string, AudioSource> _nowPlayAudio;

	private AudioSource _bgmAudioSource;

	private void Awake()
	{
		// 單例模式
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

        Instance = this;
		DontDestroyOnLoad(gameObject);

        _bgmAudioSource = gameObject.AddComponent<AudioSource>();

		// 預先做好池子
		for(int i = 0; i < _audioPoolSize; i++)
		{
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			_audioPool.Enqueue(audioSource);
		}
	}

	private void Start()
	{
		// mainVolume = GameDataManager.Instance.gameData.mainVolume;
		// bgmVolume = GameDataManager.Instance.gameData.bgmVolume;
		// sfxVolume = GameDataManager.Instance.gameData.sfxVolume;

        if (_bgmAudioSource != null)
		{
            PlayBGM("BackGroundMusic");
        }
	}

	private void OnValidate()
	{
		if(_bgmAudioSource != null)
		{
			_bgmAudioSource.volume = _mainVolume * _bgmVolume;
		}
	}

	public void PlayBGM(string key, float volume = 0.5f)
	{
		AudioClip audioClip = _audioLibrarySO.GetAudioClip(key);

		if(audioClip != null)
		{
			if(_bgmAudioSource.clip == audioClip) return;

			_bgmAudioSource.clip = audioClip;
			_bgmAudioSource.loop = true;
			_bgmAudioSource.volume = _mainVolume * _bgmVolume;
			_bgmAudioSource.Play();
		}
		else
		{
			Debug.Log("AudioManager PlaySound 輸入的 Key 有錯");
		}
	}

	public void PlaySound(string key, Vector3 position, bool isLoop = false)
	{
		// 檢查輸入的 key 是否正確
		AudioClip audioClip = _audioLibrarySO.GetAudioClip(key);

		if(audioClip == null)
		{
			Debug.Log("AudioManager PlaySound 輸入的 Key 有錯, key 為" + key);
			return;
		}

		AudioSource audioSource;

		if(_audioPool.Count > 0)
		{
			audioSource = _audioPool.Dequeue();
		}
		else
		{
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}

		audioSource.transform.position = position;
		audioSource.clip = audioClip;
		audioSource.volume = _mainVolume * _sfxVolume;
		audioSource.loop = isLoop;
		audioSource.Play();

		StartCoroutine(RecycleAudioToPool(key, audioSource, audioClip.length));
	}

	public void StopSound(string key)
	{
		
	}

	// 將 AudioSource 播完後, 回收進物件池中
	private IEnumerator RecycleAudioToPool(string key, AudioSource audioSource, float audioTime)
	{
		yield return new WaitForSeconds(audioTime);

		_audioPool.Enqueue(audioSource);
	}

	public void SetMainVolume(float mainVolume)
	{
		_mainVolume = mainVolume;
		_bgmAudioSource.volume = mainVolume * _bgmVolume;

		GameDataManager.Instance.gameData.mainVolume = mainVolume;
	}

	public void SetBGMVolume(float bgmVolume)
	{
		_bgmVolume = bgmVolume;
		_bgmAudioSource.volume = _mainVolume * bgmVolume;

		GameDataManager.Instance.gameData.bgmVolume = bgmVolume;
	}

	public void SetSFXVolume(float sfxVolume)
	{
		_sfxVolume = sfxVolume;

		GameDataManager.Instance.gameData.sfxVolume = sfxVolume;
	}
	
}
