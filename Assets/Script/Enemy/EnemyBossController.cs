using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossController : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyDataSO;
	[SerializeField] private Animator animator;
	[SerializeField] private GameObject shootAttackPrefab;
	[SerializeField] private GameObject floorAttackPrefab;

	[SerializeField] private float attackCooldownTime;

	private bool isIdle = true;
	private bool isAttackCooldown = false;
	private Health health;
	private Transform playerTransform;
	private AnimatorStateInfo animatorStateInfo;

	private bool hpLessTrigger70 = false;
	private bool hpLessTrigger35 = false;
	

	private BossState state;

	private enum BossState
	{
		Idle,
		RunAttack,
		ShootAttack,
		FloorAttack,
		Dead
	}
	
	private void Start()
	{
		health = GetComponent<Health>();
		health.SetMaxHealth(enemyDataSO.maxHP);
		health.OnDamage += TakeDamage;

		playerTransform = FindObjectOfType<PlayerController>()?.transform;
		
		ChangeEnemyState(BossState.Idle);
	}

	private void Update()
	{
		CheckAnimationIsIdle();
		LookAtPlayer();

		if(isIdle)
		{
			CheckPlayerDistance();
		}
	}

	private void LookAtPlayer()
	{
		Vector3 direction = (transform.position - playerTransform.position).normalized;
		direction.y = 0;
		Quaternion targetRotation = Quaternion.LookRotation(direction);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);
	}

	private void CheckAnimationIsIdle()
	{
		animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayIdle = animatorStateInfo.IsName(BossState.Idle.ToString());
		
		if(isIdle == isPlayIdle) return;
		
		if(!isPlayIdle)
		{
			isIdle = false;
		}
		else
		{
			if(!isAttackCooldown)
			{
				StartCoroutine(StartAttackCoolDown());
			}
		}
	}

	private IEnumerator StartAttackCoolDown()
	{
		isAttackCooldown = true;

		yield return new WaitForSeconds(attackCooldownTime);

		//isIdle = true;
		ChangeEnemyState(BossState.Idle);
		isAttackCooldown = false;
	}
	
	private void CheckPlayerDistance()
	{
		if(playerTransform != null)
		{
			float distance = Vector3.Distance(transform.position, playerTransform.position);

			if (distance <= enemyDataSO.attackRange)
			{
				ChangeEnemyState(BossState.FloorAttack);
			}
			else
			{
				ChangeEnemyState(BossState.ShootAttack);
			}
		}
	}
	
	private void ChangeEnemyState(BossState newState)
	{
		if(state == BossState.Dead || state == newState) return;
		
		state = newState;

        switch (state)
        {
            case BossState.Idle:
				isIdle = true;
                break;
            case BossState.RunAttack:
				animator.SetTrigger(state.ToString());
                break;
            case BossState.ShootAttack:
				animator.SetTrigger(state.ToString());
				StartCoroutine(DelayShootAttackCoroutine());
                break;
            case BossState.FloorAttack:
				animator.SetTrigger(state.ToString());
				StartCoroutine(DelayFloorAttackCoroutine());
                break;
            case BossState.Dead:
                break;
        }
    }

	private IEnumerator DelayShootAttackCoroutine()
	{
		shootAttackPrefab.SetActive(false);

		yield return new WaitForSeconds(1.7f);

		shootAttackPrefab.SetActive(true);
	}

	private IEnumerator DelayFloorAttackCoroutine()
	{
		floorAttackPrefab.SetActive(false);

		yield return new WaitForSeconds(0.8f);

		floorAttackPrefab.SetActive(true);
	}
	
	private void CallEnemy()
	{
	    //Call 小怪出來
	}

	/*
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
	*/

	//如果需要 Enemy 受傷, 呼叫該函數
	public void TakeDamage()
	{
		if(state == BossState.Dead) return;

		AudioManager.Instance.PlaySound(enemyDataSO.SfxDamageKey, transform.position);

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, health.LastDamage);
	}
	
	public void DestroySelf()
	{
		Destroy(gameObject);
	}
}
