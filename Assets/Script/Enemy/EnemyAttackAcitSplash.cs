using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAcitSplash : MonoBehaviour, IEnemyAttack
{
    public event Action OnAttackHit;
    [SerializeField] private ParticleSystem acitParticle;
    
    [SerializeField] private float radius = 4f;
    [SerializeField] private float damage = 5;
    [SerializeField] private float attackCooldown = 0.5f;

    private void Start()
    {
        StartCoroutine(IntervalAttack());
        AudioManager.Instance.PlaySound("AcitSplash", transform.position);
    }

    public void ResetHasAttack()
    {
        
    }

    public void StartAttack()
    {

    }
    
    private IEnumerator IntervalAttack()
    {
        yield return new WaitForSeconds(0.5f);
        
        float timer = 0f;
        float damageTimer = 9.5f;
        
        while(timer < damageTimer)
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player"));
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDot(damage);
                }
            }
            
            yield return new WaitForSeconds(attackCooldown);
            timer += attackCooldown;
        }

        Destroy(gameObject);
    }
}
