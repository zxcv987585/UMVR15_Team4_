using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEgg : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    private ParticleSystem bombParticle;

    private bool isStartBomb = false;

    private void Start()
    {
        bombParticle = GetComponent<ParticleSystem>();
    }
    
    private void Update()
    {
        if(isStartBomb) return;
    
        if(enemyController.GetEnemyNowState() == EnemyState.Attack)
        {
            isStartBomb = true;
            
            StartCoroutine(StartBomb());
        }
    }
    
    private IEnumerator StartBomb()
    {
        yield return new WaitForSeconds(2);
        
        enemyController.EnableAttackCollider(true);
        bombParticle.Play();
    }
}
