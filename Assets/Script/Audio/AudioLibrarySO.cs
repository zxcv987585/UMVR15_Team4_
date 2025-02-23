using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()]
public class AudioLibrarySO : ScriptableObject
{
    
    [System.Serializable]
    public class AudioClipObject
    {
        public string key;
        public AudioClip audioClip;
    }

    [SerializeField] private List<AudioClipObject> audioClipObjectList;
    private Dictionary<string, AudioClip> audioClipDictionary;

    private void OnEnable()
    {
        audioClipDictionary = new Dictionary<string, AudioClip>();
        foreach(AudioClipObject audioClipObject in audioClipObjectList)
        {
            if(!audioClipDictionary.ContainsKey(audioClipObject.key))
            {
                audioClipDictionary.Add(audioClipObject.key, audioClipObject.audioClip);
            }
        }
    }

    public AudioClip GetAudioClip(string key)
    {
        audioClipDictionary.TryGetValue(key, out AudioClip audioClip);
        return audioClip;
    }
}
