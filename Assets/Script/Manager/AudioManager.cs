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
	private Dictionary<string, List<AudioSource>> _nowPlayAudio = new Dictionary<string, List<AudioSource>>();
	private AudioSource _bgmAudioSource;
	private AudioSource _nextAudioSource;

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
		
		gameObject.AddComponent<AudioListener>();
	}

	private void Start()
	{
		_mainVolume = GameDataManager.Instance.gameData.mainVolume;
		_bgmVolume = GameDataManager.Instance.gameData.bgmVolume;
		_sfxVolume = GameDataManager.Instance.gameData.sfxVolume;

        if (_bgmAudioSource != null)
		{
            PlayBGM("BackGroundMusic");
        }
        
        _nextAudioSource = gameObject.AddComponent<AudioSource>();
        //_nextAudioSource
	}

	private void OnValidate()
	{
		if(_bgmAudioSource != null)
		{
			_bgmAudioSource.volume = _mainVolume * _bgmVolume;
		}
		
		if(GameDataManager.Instance != null)
		{
		    GameDataManager.Instance.gameData.mainVolume = _mainVolume;
			GameDataManager.Instance.gameData.bgmVolume = _bgmVolume;
			GameDataManager.Instance.gameData.sfxVolume = _sfxVolume;
			
			GameDataManager.Instance.SaveGame();
		}
		
	}

	public void PlayBGM(string key, float volume = 0.5f)
	{
		AudioClip audioClip = _audioLibrarySO.GetAudioClip(key);

		if(audioClip == null)
		{
			Debug.Log("AudioManager PlaySound 輸入的 Key 有錯");
			return;
		}
		
		if(_bgmAudioSource.clip == audioClip) return;

		_bgmAudioSource.clip = audioClip;
		_bgmAudioSource.loop = true;
		_bgmAudioSource.volume = _mainVolume * _bgmVolume;
		_bgmAudioSource.Play();
	}
	
	private IEnumerator CrossFadeBGM()
	{
	    yield return null;
	}

	public void PlaySound(string key, Vector3 position, bool isLoop = false, float playTimer = 0f)
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
		audioSource.pitch = Random.Range(0.95f, 1.05f);
		audioSource.Play();

		if(!_nowPlayAudio.ContainsKey(key))
		{
			_nowPlayAudio[key] = new List<AudioSource>();
		}
		_nowPlayAudio[key].Add(audioSource);

		if(playTimer == 0f)
		{
			StartCoroutine(RecycleAudioToPool(key, audioSource, audioClip.length));
		}
		else
		{
			StartCoroutine(RecycleAudioToPool(key, audioSource, playTimer));
		}
		
	}

	/// <summary>
	/// 停止指定的音效播放
	/// </summary>
	/// <param name="key"></param>
	public void StopSound(string key)
	{
		if(_nowPlayAudio.ContainsKey(key))
		{
			foreach(AudioSource audioSource in _nowPlayAudio[key])
			{
				audioSource.Stop();
				audioSource.clip = null;
				audioSource.loop = false;
			}
			_nowPlayAudio.Remove(key);
		}
	}

	// 將 AudioSource 播完後, 回收進物件池中
	private IEnumerator RecycleAudioToPool(string key, AudioSource audioSource, float audioTime)
	{
		yield return new WaitForSeconds(audioTime);

		audioSource.Stop();
		audioSource.clip = null;
		audioSource.loop = false;

		if(_nowPlayAudio.ContainsKey(key))
		{
			_nowPlayAudio[key].Remove(audioSource);
		}

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
