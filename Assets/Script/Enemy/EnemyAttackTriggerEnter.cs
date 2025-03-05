using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacTriggerEnter : MonoBehaviour, IEnemyAttack
{
	public event Action OnAttackHit;
	private bool hasAttack = false;

	[SerializeField] private Collider attackCollider;

    public void StartAttack()
    {
        attackCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
	{
		if(!hasAttack && other.TryGetComponent(out PlayerHealth health))
		{
			hasAttack = true;
			
			OnAttackHit?.Invoke();
		}
	}

    public void ResetHasAttack()
    {
        hasAttack = false;
        attackCollider.enabled = false;
    }
}
