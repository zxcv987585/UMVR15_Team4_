using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour
{
    [SerializeField] private EnemyDrop _enemyDrop;
    [SerializeField] private Transform ImpactEffect;
    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        _health.OnDead += DropItem;
    }
    
    private void DropItem()
    {
        Instantiate(ImpactEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        Instantiate(_enemyDrop, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
