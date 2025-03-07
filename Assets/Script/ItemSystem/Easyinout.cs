using System;
using System.Collections;
using UnityEngine;

public class EasyInOut : MonoBehaviour
{
    //緩動函數
    public static float EaseIn(float t) => t * t; //慢到快
    public static float EaseOut(float t) => 1 - (1 - t) * (1 - t); //快到慢
    public static float EaseInOut(float t) => t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2; //慢到快到慢

    //泛型變數的緩動變化
    public IEnumerator ChangeValue<T>(T start, T end, float duration, Action<T> onUpdate, Func<float, float> easingFunction)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            //選擇的緩動方法
            float easedT = easingFunction(t);

            if (typeof(T) == typeof(float))
            {
                onUpdate((T)(object)Mathf.Lerp((float)(object)start, (float)(object)end, easedT));
            }
            else if (typeof(T) == typeof(Vector3))
            {
                onUpdate((T)(object)Vector3.Lerp((Vector3)(object)start, (Vector3)(object)end, easedT));
            }
            yield return null;
        }
        onUpdate(end);
    }
}