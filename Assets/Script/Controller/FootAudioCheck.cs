using UnityEngine;

public class FootAudioCheck : MonoBehaviour
{
    public enum FootAudioName
    {
        FootAudio,
        FootAudio2,
    }
    
    private FootAudioName _footAudioName;

    public void FootAudio()
    {
        _footAudioName = (FootAudioName)Random.Range(0, 2);

        AudioManager.Instance.PlaySound(_footAudioName.ToString(), transform.position);
    }
}
