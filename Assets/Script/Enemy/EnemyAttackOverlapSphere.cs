using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackOverlapSphere : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private ParticleSystem bombParticle;
    public event Action OnAttackHit;

    private bool hasAttack = false;
    private const string PLAYER = "Player";

    public void ResetHasAttack()
    {
        
    }

    public void StartAttack()
    {
        if(hasAttack)
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask(PLAYER));
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out PlayerHealth playerHealth))
                {
                    OnAttackHit?.Invoke();
                    bombParticle.Play();
                }
            }

            hasAttack = true;
        }
    }
}
