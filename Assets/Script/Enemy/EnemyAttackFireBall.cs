using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackFireBall : MonoBehaviour, IEnemyAttack
{
    public event Action OnAttackHit;

    [SerializeField] private float _flightTime;
    [SerializeField] private float _height;

    public void ResetHasAttack()
    {
        
    }

    public void StartAttack()
    {
        
    }

    private void Start()
    {
        StartCoroutine(ParabolaFireBall(transform.position, FindAnyObjectByType<PlayerController>().transform.position, _height, _flightTime));
    }

    private IEnumerator ParabolaFireBall(Vector3 start, Vector3 end, float height, float flightTime)
    {
        float timer = 0f;
        while (timer < flightTime)
        {
            timer += Time.deltaTime;
            float t = timer / flightTime;

            // 線性插值計算 XZ 平面
            Vector3 position = Vector3.Lerp(start, end, t);

            // 使用拋物線公式計算 Y 軸高度
            position.y += height * Mathf.Sin(t * Mathf.PI);

            transform.position = position;
            yield return null;
        }

        transform.position = end; // 確保最後位置精準
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHealth playerHealth))
        {
            OnAttackHit?.Invoke();
            Destroy(gameObject);
        }
    }
}
