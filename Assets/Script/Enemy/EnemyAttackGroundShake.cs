using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackGroundShake : MonoBehaviour
{
    /*
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float expandDuration = 2f;
    [SerializeField] private float ringThickness = 0.5f; // ¶êÀô«p«×
    [SerializeField] private float damage = 20;

    private float currentRadius = 0f;
    private float timer = 0f;

    private void Update()
    {
        if (timer < expandDuration)
        {
            timer += Time.deltaTime;
            currentRadius = Mathf.Lerp(0f, maxRadius, timer / expandDuration);

            Collider[] colliders = Physics.OverlapSphere(transform.position, currentRadius, LayerMask.GetMask("Player"));
            foreach (Collider collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                
                if (distance >= currentRadius - ringThickness && distance <= currentRadius + ringThickness)
                {
                    if (collider.TryGetComponent(out PlayerHealth playerHealth))
                    {
                        playerHealth.TakeDamage(damage);
                    }
                }
            }
        }
    }
    */
    
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
