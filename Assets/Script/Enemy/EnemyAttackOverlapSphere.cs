using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackOverlapSphere : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private Health health;
    [SerializeField] private ParticleSystem bombParticle;
    [SerializeField] private Renderer bombRenderer;
    [SerializeField] private float damage;
    public event Action OnAttackHit;

    private bool hasAttack = false;
    private const string PLAYER = "Player";

    private void Start()
    {
        health.OnDead += BombAttack;
    }

    public void ResetHasAttack()
    {
        
    }

    public void StartAttack()
    {
        if(!hasAttack)
        {
            StartCoroutine(DelayBombAttack(3f));
            hasAttack = true;
        }
    }
    
    private IEnumerator DelayBombAttack(float delayBombTime)
    {
        Material material = bombRenderer.material;
        float timer = 0f;
    
        while(timer < delayBombTime)
        {
            // 計算進度 (0 ~ 1)
            float progress = timer / delayBombTime;

            // 頻率隨著時間增加（例如初始 1Hz，到最後 10Hz）
            float frequency = Mathf.Lerp(1f, 10f, progress);

            // 計算閃爍 (sin 波動)
            float emissionStrength = (Mathf.Sin(Time.time * frequency * Mathf.PI * 2) + 1f) / 2f;
            
            material.SetColor("_EmissionColor", Color.red * emissionStrength);
            
            timer += Time.deltaTime;
        
            yield return null;
        }
        
        material.SetColor("_EmissionColor", Color.red);
        
        health.TakeDamage(999);
    }
    
    private void BombAttack()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask(PLAYER));
        foreach(Collider collider in colliderArray)
        {
            if(collider.TryGetComponent(out PlayerHealth playerHealth))
            {
                //OnAttackHit?.Invoke();
                playerHealth.TakeDamage(damage);
                bombParticle.Play();
            }
        }
    }
}
