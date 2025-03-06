using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAcitSplash : MonoBehaviour, IEnemyAttack
{
    public event Action OnAttackHit;

    [SerializeField] private float radius = 4f;
    [SerializeField] private ParticleSystem acitParticle;
    [SerializeField] private float attackCooldown = 0.5f;

    private void Start()
    {
        
    }

    public void ResetHasAttack()
    {
        
    }

    public void StartAttack()
    {

    }
    
    private IEnumerator IntervalAttack()
    {
        yield return new WaitForSeconds(1f);
        
        float timer = 0f;
        
        while(timer < 3.5f)
        {
            timer += Time.deltaTime;
        }
    }
}
