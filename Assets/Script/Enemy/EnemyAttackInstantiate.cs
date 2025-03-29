using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackInstantiate : MonoBehaviour, IEnemyAttack
{
    public event Action OnAttackHit;

    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private GameObject _fireBallPrefab;

    public void ResetHasAttack()
    {
        throw new NotImplementedException();
    }

    public void StartAttack()
    {
        throw new NotImplementedException();
    }

    public void AttackTrigger()
    {
        GameObject go = Instantiate(_fireBallPrefab, transform.position + Vector3.up * 1.3f, Quaternion.identity);
        go.GetComponent<EnemyAttackFireBall>().StartFly(_enemyController);
        
        this.PlaySound("KiwiAttack");
    }
}
