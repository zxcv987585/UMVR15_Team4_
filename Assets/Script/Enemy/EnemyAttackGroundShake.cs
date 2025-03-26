using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackGroundShake : MonoBehaviour
{
    [SerializeField] private float _damage = 10;
    private float _radius;
    private float _duration;
    private ParticleSystem _particleSystem;
    private LayerMask _layerMask;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _radius = _particleSystem.main.startSize.constant / 2f;
        _duration = _particleSystem.main.startLifetime.constant;
        _layerMask = LayerMask.GetMask("Player");
    }

    public void StartAttack()
    {
        StartCoroutine(GroundAttack());
    }

    private IEnumerator GroundAttack()
    {
        _particleSystem.Play();
    
        float timer = 0f;
        
        while(timer < _duration)
        {
            timer += Time.deltaTime;
            
            float currentRadius = Mathf.Lerp(0, _radius, timer/_duration);
            
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, currentRadius, _layerMask);
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(_damage);
                    yield break;
                }
            }
            
            yield return null;
        }
    }
}
