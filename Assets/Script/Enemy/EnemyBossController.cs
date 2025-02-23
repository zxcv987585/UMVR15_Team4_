using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossController : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyDataSO;
	[SerializeField] private EnemyAnimatorController enemyAnimatorController;

	private int hp;
	private bool isAttack = false;
	private bool isDamage = false;
	private EnemyState enemyState;
	private Rigidbody rb;
	private Transform playerTransform;
	
	private void Start()
	{
		hp = enemyDataSO.maxHP;
		rb = GetComponent<Rigidbody>();
		playerTransform = FindObjectOfType<PlayerController>()?.transform;
		
		enemyAnimatorController.OnAttackChange += SetIsAttack;
		enemyAnimatorController.OnDamageChange += SetIsDamage;
		enemyAnimatorController.OnDead += DestroySelf;
		//enemyAttackHandler.OnAttackHit += Attack;
		
		ChangeEnemyState(EnemyState.Idle);
	}

	private void Update()
	{
		if(!isAttack && !isDamage)
		{
			CheckPlayerDistance();
		}
	}
	
	private void CheckPlayerDistance()
	{
		if(playerTransform != null)
		{
			float distance = Vector3.Distance(transform.position, playerTransform.position);

			if (distance <= enemyDataSO.attackRange)
			{
				ChangeEnemyState(EnemyState.Attack);
			}
			else
			{
				ChangeEnemyState(EnemyState.Walk);
			}
		}
	}
	
	private void ChangeEnemyState(EnemyState newState)
	{
		if(enemyState == EnemyState.Dead) return;
		
		enemyState = newState;

		switch (enemyState)
		{
			case EnemyState.Idle:
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
			case EnemyState.Walk:
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
			case EnemyState.Attack:
				StartCoroutine(TryAttackAfterTurn(playerTransform.position));
				break;
			case EnemyState.Damage:
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
			case EnemyState.Dead:
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
		}
	}

	private IEnumerator TryAttackAfterTurn(Vector3 targetPosition)
	{
		while (true)
		{
			// 檢查玩家是否仍在攻擊範圍內
			float distance = Vector3.Distance(transform.position, targetPosition);
			if (distance > enemyDataSO.attackRange)
			{
				ChangeEnemyState(EnemyState.Walk);
				yield break;
			}

			// 計算旋轉方向
			Vector3 direction = (targetPosition - transform.position).normalized;
			direction.y = 0;
			Quaternion targetRotation = Quaternion.LookRotation(direction);

			// 平滑旋轉
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2f * Time.deltaTime);

			// 檢查是否轉向完成
			if (Quaternion.Angle(transform.rotation, targetRotation) < 5f)
			{
				break;
			}

			yield return null;
		}

		// 確保敵人仍在攻擊狀態
		if (enemyState == EnemyState.Attack)
		{
			enemyAnimatorController.SetEnemyState(EnemyState.Attack);
		}
	}

	//抓取玩家周圍 360度的隨機座標
	private Vector3 GetRandomPositionAroundPlayer()
	{
		float randomAngle = UnityEngine.Random.Range(0f, MathF.PI * 2f);
		Vector3 offset = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * enemyDataSO.attackRange;

		return playerTransform.position + offset;
	}

	//如果需要 Enemy 受傷, 呼叫該函數
	public void TakeDamage(int damage)
	{
		if(enemyState == EnemyState.Dead) return;

		hp -= damage;
		if(hp <= 0)
		{
			hp = 0;
			AudioManager.Instance.PlaySound(enemyDataSO.SfxDeadKey, transform.position);
			ChangeEnemyState(EnemyState.Dead);
		}

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, damage);
	}
	
	public void SetIsAttack(bool isAttack)
	{
		this.isAttack = isAttack;
		
		if(!isAttack)
		{
			//enemyAttackHandler.ResetAttackHandler();
		}
		else
		{
			AudioManager.Instance.PlaySound(enemyDataSO.SfxAttackKey, transform.position);
		}
	}
	
	public void SetIsDamage(bool isDamage)
	{
		this.isDamage = isDamage;
	}
	
	public void DestroySelf()
	{
		Destroy(gameObject);
	}
}
