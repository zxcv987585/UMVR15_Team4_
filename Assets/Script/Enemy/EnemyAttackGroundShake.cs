using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackGroundShake : MonoBehaviour
{
    [SerializeField] private float radius = 9f;
    [SerializeField] private float damage = 10;
    [SerializeField] private float attackCooldown = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(IntervalAttack());
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
        float damageTimer = 3f;
        
        while(timer < damageTimer)
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player"));
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                }
            }
            
            yield return new WaitForSeconds(attackCooldown);
            timer += attackCooldown;
        }
    }
}
