using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackGroundShake : MonoBehaviour
{
    [SerializeField] private float _damage = 10;
    private float _radius;
    private float _duration;
    private float _thickness = 3f;
    private ParticleSystem _particleSystem;
    private LayerMask _layerMask;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _radius = _particleSystem.main.startSize.constant / 4f;
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
            float currentRadius = Mathf.Lerp(0, _radius, timer/_duration);
            float innerRadius = Mathf.Max(0, currentRadius - _thickness);
            
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, currentRadius, _layerMask);
            foreach(Collider collider in colliderArray)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                if(distance >= innerRadius && distance <= currentRadius)
                {
                    if(collider.TryGetComponent(out PlayerHealth playerHealth))
                    {
                        playerHealth.CriticalDamage(_damage);
                        yield break;
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
