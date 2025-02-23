using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
	public Action OnAttackHit;
	private bool hasAttack = false;

	private void OnTriggerEnter(Collider other)
	{
		if(!hasAttack && other.TryGetComponent(out Health health))
		{
			hasAttack = true;
			OnAttackHit?.Invoke();
		}
	}
	
	public void ResetAttackHandler()
	{
		hasAttack = false;
	}
}
