using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour
{
    [SerializeField] private EnemyDrop _enemyDrop;
    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        _health.OnDead += DropItem;
    }
    
    private void DropItem()
    {
        Instantiate(_enemyDrop, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
    
    
}
