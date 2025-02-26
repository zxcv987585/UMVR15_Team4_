using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private EnemyDataSO enemyDataSO;
	[SerializeField] private EnemyAnimatorController enemyAnimatorController;
	[SerializeField] private EnemyAttackHandler enemyAttackHandler;

	[SerializeField] Renderer dissolveRenderer;
	private Material material;
	private const string DISSOLVE_AMOUNT = "_DissolveAmount";

	//private int hp;
	private bool isAttack = false;
	private bool isDamage = false;
	private EnemyState enemyState;
	private Rigidbody rb;
	private Health health;
	private NavMeshAgent navMeshAgent;
	private Transform playerTransform;
	
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		playerTransform = FindObjectOfType<PlayerController>()?.transform;
		material = dissolveRenderer.material;

		health = GetComponent<Health>();
		health.SetMaxHealth(enemyDataSO.maxHP);
		health.OnDamage += DamageEvent;
		health.OnDead += DeadEvent;

		navMeshAgent.stoppingDistance = enemyDataSO.attackRange;
		navMeshAgent.speed = enemyDataSO.moveSpeed;
		
		enemyAnimatorController.OnAttackChange += SetIsAttack;
		enemyAnimatorController.OnDamageChange += SetIsDamage;
		enemyAnimatorController.OnStartAttackCheck += EnableAttackCollider;
		enemyAnimatorController.OnDead += StartDestory;
		enemyAttackHandler.OnAttackHit += Attack;
		
		ChangeEnemyState(EnemyState.Idle);
		StartCoroutine(DelayEnableNavMeshAgent());
	}

	public void Init()
	{
		gameObject.SetActive(true);
		enemyState = EnemyState.Idle;
		health.SetMaxHealth(enemyDataSO.maxHP);
		navMeshAgent.isStopped = true;
		ShowDissolve();
	}

	private IEnumerator DelayEnableNavMeshAgent()
	{
		yield return null;
		navMeshAgent.enabled = true;
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
		if(playerTransform != null && navMeshAgent.isOnNavMesh)
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
		if(enemyState == EnemyState.Dead || !navMeshAgent.enabled) return;
		
		enemyState = newState;

		switch (enemyState)
		{
			case EnemyState.Idle:
				navMeshAgent.isStopped = true;
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
			case EnemyState.Walk:
				navMeshAgent.isStopped = false;
				//navMeshAgent.SetDestination(GetRandomPositionAroundPlayer());
				navMeshAgent.SetDestination(playerTransform.position);
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
			case EnemyState.Attack:
				navMeshAgent.isStopped = true;
				StartCoroutine(TryAttackAfterTurn(playerTransform.position));
				break;
			case EnemyState.Damage:
				navMeshAgent.isStopped = true;
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
			case EnemyState.Dead:
				navMeshAgent.isStopped = true;
				enemyAnimatorController?.SetEnemyState(enemyState);
				break;
		}
	}

	private IEnumerator TryAttackAfterTurn(Vector3 targetPosition)
	{
		while (true)
		{
			if(enemyState == EnemyState.Dead) break;

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

	private void DamageEvent()
	{
		AudioManager.Instance.PlaySound(enemyDataSO.SfxDamageKey, transform.position);
		ChangeEnemyState(EnemyState.Damage);

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, health.LastDamage);
	}

	private void DeadEvent()
	{
		AudioManager.Instance.PlaySound(enemyDataSO.SfxDeadKey, transform.position);
		ChangeEnemyState(EnemyState.Dead);

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, health.LastDamage);
	}

	/// <summary>
	/// 擊飛該怪物
	/// </summary>
	/// <param name="flyPower">擊飛的高度參數</param>
	public void HitFly(float flyPower)
	{
		if (enemyDataSO.isBoss || enemyState == EnemyState.Dead) return;

		navMeshAgent.enabled = false;
		rb.isKinematic = false;

		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(Vector3.up * flyPower, ForceMode.Impulse);
		StartCoroutine(EnableNavMeshDelay(1f));
	}

	private IEnumerator EnableNavMeshDelay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		while(Mathf.Abs(rb.velocity.y) > 0.1f)
		{
			yield return null;
		}

		navMeshAgent.enabled = true;
		rb.isKinematic = true;
	}

	public void Attack()
	{
		if(isAttack)
			playerTransform.GetComponent<Health>().TakeDamage(enemyDataSO.attackPower);
	}
	
	public void SetIsAttack(bool isAttack)
	{
		this.isAttack = isAttack;
		
		if(!isAttack)
		{
			enemyAttackHandler.ResetAttackHandler();
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

	private void StartDestory()
	{
		StartCoroutine(DeadDissolveCoroutine());
	}
	
	public void DestroySelf()
	{
		//Destroy(gameObject);
		gameObject.SetActive(false);
		EnemyManager.Instance.RecycleEnemy(gameObject);
	}

	private IEnumerator DeadDissolveCoroutine()
	{
		float dissolveAmount = 0f;
		
		while(dissolveAmount < 1f)
		{
			dissolveAmount += Time.deltaTime * 0.5f;
			material.SetFloat(DISSOLVE_AMOUNT, dissolveAmount);
			
			yield return null;
		}

		DestroySelf();
	}

	public void ShowDissolve()
	{
		material.SetFloat(DISSOLVE_AMOUNT, 0f);
	}

	private void EnableAttackCollider(bool isEnable)
	{
		enemyAttackHandler.enabled = isEnable;
	}
}
