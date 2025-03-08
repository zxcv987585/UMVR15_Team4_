using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossController : MonoBehaviour
{
	[SerializeField] private GameObject bossUIPrefab;

    [SerializeField] private EnemyDataSO enemyDataSO;
	[SerializeField] private Animator animator;
	[SerializeField] private GameObject shootAttackPrefab;
	[SerializeField] private GameObject floorAttackPrefab;

	[SerializeField] private float attackCooldownTime;

	private bool isIdle = true;
	private bool isAttackCooldown = false;
	private Health health;
	private Transform playerTransform;
	private PlayerHealth playerHealth;
	private AnimatorStateInfo animatorStateInfo;
	private BossUI bossUI;

	private bool hpLessTrigger70 = false;
	private bool hpLessTrigger35 = false;
	

	private BossState state;

	private enum BossState
	{
		Idle,
		RunAttack,
		CallEnemy,
		ShootAttack,
		FloorAttack,
		Dead
	}
	
	private void Start()
	{
		health = GetComponent<Health>();
		health.SetMaxHealth(enemyDataSO.maxHP);
		health.OnDamage += TakeDamage;
		health.OnDead += DeadHandler;

		playerTransform = FindObjectOfType<PlayerController>()?.transform;
		playerHealth = playerTransform.GetComponent<PlayerHealth>();
		
		ChangeEnemyState(BossState.Idle);

		GameObject go = Instantiate(bossUIPrefab, FindObjectOfType<BattleUIManager>().transform);
		bossUI = go.GetComponent<BossUI>();
		bossUI.SetHealth(health);
	}

	private void Update()
	{
		if(playerHealth.IsDead() || state == BossState.Dead) return;

		CheckAnimationIsIdle();
		LookAtPlayer();
		

		if(isIdle)
		{
			if(CheckHealthEvent())
			{
				return;
			}
			CheckPlayerDistance();
		}
	}

	private void LookAtPlayer()
	{
		Vector3 direction = (playerTransform.position - transform.position).normalized;
		direction.y = 0;
		Quaternion targetRotation = Quaternion.LookRotation(direction);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 15f * Time.deltaTime);
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
			case BossState.CallEnemy:
				animator.SetTrigger(state.ToString());
				CallEnemy();
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
				animator.SetTrigger("isDead");
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

	private void DeadHandler()
	{
		ChangeEnemyState(BossState.Dead);
	}

	//如果需要 Enemy 受傷, 呼叫該函數
	public void TakeDamage()
	{
		if(state == BossState.Dead) return;

		//AudioManager.Instance.PlaySound(enemyDataSO.SfxDamageKey, transform.position);

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up * 6, health.LastDamage);
	}

	// 檢查血量是否低於特定條件, 來觸發事件
	private bool CheckHealthEvent()
	{
		if(!hpLessTrigger70 && health.GetHealthRatio() < 0.7f)
		{
			ChangeEnemyState(BossState.CallEnemy);

			animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if(animatorStateInfo.IsName(BossState.Idle.ToString()))
			{
				hpLessTrigger70 = true;
			}

			return true;
		}

		if(!hpLessTrigger35 && health.GetHealthRatio() < 0.35f)
		{
			ChangeEnemyState(BossState.CallEnemy);

			animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if(animatorStateInfo.IsName(BossState.Idle.ToString()))
			{
				hpLessTrigger35 = true;
			}
			return true;
		}

		return false;
	}
	
	public void DestroySelf()
	{
		Destroy(gameObject);
	}
}
