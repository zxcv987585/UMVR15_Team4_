using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManagerExtensions
{
    /// <summary>
    /// 呼叫音效
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="key"> 音效的字串, 對應 AudioLiberySO 的 key </param>
    /// <param name="position"> 生成的位置, 預設為呼叫者的 transform.position </param>
    /// <param name="isLoop"> 是否要啟用 loop </param>
    /// <param name="playTimer"> 限制只能播放幾秒, 不限制就是播完 </param>
    public static void PlaySound(this MonoBehaviour caller, string key, Vector3? position = null, bool isLoop = false, float playTimer = 0f)
    {
        if (caller == null)
        {
            Debug.LogError("PlaySound 呼叫者為空！");
            return;
        }

        AudioManager.Instance.PlaySound(key, caller.transform.position, caller.gameObject, isLoop, playTimer);
    }

    public static void StopSound(this MonoBehaviour caller, string key)
    {
        if (caller == null)
        {
            Debug.LogError("StopSound 呼叫者為空！");
            return;
        }
        AudioManager.Instance.StopSound(key, caller.gameObject);
    }
}
