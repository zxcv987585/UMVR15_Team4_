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
	[SerializeField] private MonoBehaviour enemyAttackMonoBehaviour;
	private IEnemyAttack enemyAttack;
	private EnemyState enemyState;

	[SerializeField] private Renderer dissolveRenderer;
	[SerializeField] private ParticleSystem deadParticle;
	[SerializeField] private float dissolveTime = 1f;
	private Material material;
	private const string DISSOLVE_AMOUNT = "_DissolveAmount";
	private const string EMISSION_COLOR= "_EmissionColor";
	private const string RIM_COLOR = "_RimColor";

	private bool isAttack = false;
	private bool isDamage = false;
	
	private Rigidbody rb;
	private NavMeshAgent navMeshAgent;
	private Collider bodyCollider;
	private Health health;
	private PlayerHealth playerHealth;
	private Transform playerTransform;
	
	private void Start()
	{
		//抓取起始元件
		rb = GetComponent<Rigidbody>();
		bodyCollider = GetComponent<Collider>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		playerTransform = FindObjectOfType<PlayerController>()?.transform;
		playerHealth = playerTransform.GetComponent<PlayerHealth>();
		material = dissolveRenderer.material;
		enemyAttack = (IEnemyAttack)enemyAttackMonoBehaviour;

		//設定 血量
		health = GetComponent<Health>();
		health.SetMaxHealth(enemyDataSO.maxHP);
		health.OnDamage += DamageEvent;
		health.OnDead += DeadEvent;

		//設定 NavMeshAgent
		navMeshAgent.stoppingDistance = enemyDataSO.attackRange;
		navMeshAgent.speed = enemyDataSO.moveSpeed;
		
		//訂閱其他 Enemy 相關事件
		enemyAnimatorController.OnAttackChange += SetIsAttack;
		enemyAnimatorController.OnDamageChange += SetIsDamage;
		enemyAnimatorController.OnStartAttackCheck += AttackIsColliderCheck;
		enemyAnimatorController.OnDead += StartDestory;
		enemyAttack.OnAttackHit += Attack;
		
		//預設怪物狀態機以 Idle 開始
		ChangeEnemyState(EnemyState.Idle);
		
		//如果 NavMeshAgent 預設啟用, 會因為 NavMeshAgent 只允許物件放置在 NavMeshPath 上, 導致物件無法正常設定 transform.position, 因此延遲啟動
		StartCoroutine(DelayEnableNavMeshAgent());
		StartCoroutine(StartDissolveCoroutine(dissolveTime));

	}


    //從物件池抓出時的初始化
    public void Init()
	{
		gameObject.SetActive(true);
		bodyCollider.enabled = true;
		enemyState = EnemyState.Idle;
		health.SetMaxHealth(enemyDataSO.maxHP);
		navMeshAgent.isStopped = true;
		
		material.SetFloat(DISSOLVE_AMOUNT, 0f);
	}

	//延遲啟用 NavMeshAgent
	private IEnumerator DelayEnableNavMeshAgent()
	{
		yield return null;

		navMeshAgent.updatePosition = false;
		navMeshAgent.updateRotation = false;
		navMeshAgent.speed = enemyDataSO.moveSpeed;

		navMeshAgent.enabled = true;
	}

	public void EnemyUpdate()
	{
		if(playerHealth.IsDead())
		{
			if(enemyState != EnemyState.Idle)
			{
				ChangeEnemyState(EnemyState.Idle);
			}
			return;
		}

		//如果目前怪物狀態不是在攻擊或被打中, 則檢查玩家位置
		if(!isAttack && !isDamage)
		{
			CheckPlayerDistance();

			if(enemyState == EnemyState.Walk)
			{
				MoveHandler();
			}
		}
	}

	private void MoveHandler()
	{
		Vector3 nextPosition = navMeshAgent.nextPosition;
        Vector3 direction = (nextPosition - transform.position).normalized;
        direction.y = 0; // 確保不會影響 Y 軸 (防止怪物漂浮)

		if (direction != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
		}

        transform.position += direction * enemyDataSO.moveSpeed * Time.deltaTime;
	}
	
	// 檢測玩家位置來判斷要進行 攻擊或是追擊
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
	
	// 切換怪物狀態機狀態
	private void ChangeEnemyState(EnemyState newState)
	{
		// 如果怪物狀態為死去 或是 不在地上, 則不允許變更狀態
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

	// 將怪物轉向玩家位置後, 在進行攻擊行為
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

	// 怪物受傷時的事件, 訂閱在 <Health> 的 OnDamage
	private void DamageEvent()
	{
		AudioManager.Instance.PlaySound(enemyDataSO.SfxDamageKey, transform.position);
		ChangeEnemyState(EnemyState.Damage);

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, health.LastDamage);
	}

	// 怪物死亡時呼叫該事件, 訂閱在 <Health> 的 OnDead
	public void DeadEvent()
	{
		bodyCollider.enabled = false;

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
		if (enemyState == EnemyState.Dead) return;

		//navMeshAgent.enabled = false;
		rb.isKinematic = false;

		float originalHeight = transform.position.y;
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(Vector3.up * flyPower, ForceMode.Impulse);
		StartCoroutine(EnableNavMeshDelay(1f, originalHeight));

		//StartCoroutine(HitFlyCoroutine(flyPower, 1f));
	}

	// 判斷怪物是否已落地
	private IEnumerator EnableNavMeshDelay(float delayTime, float originalHeight)
	{
		yield return new WaitForSeconds(delayTime);

		while(Mathf.Abs(rb.velocity.y) > 0.1f)
		{
			if(transform.position.y <= originalHeight) break;
			yield return null;
		}

		//navMeshAgent.enabled = true;
		rb.isKinematic = true;
	}

	private IEnumerator HitFlyCoroutine(float flyPower, float duration)
	{
		//isFly = true;
		navMeshAgent.isStopped = true; // 禁用 NavMeshAgent 的自動導航
		isAttack = false;             // 禁止攻擊
		isDamage = false;             // 禁止受擊
		enemyAnimatorController?.SetEnemyState(EnemyState.Damage);

		Vector3 velocity = Vector3.up * flyPower; // 初始上升速度
		float timer = 0f;

		while (timer < duration)
		{
			timer += Time.deltaTime;

			// 更新速度（模擬重力影響）
			velocity += Physics.gravity * Time.deltaTime;

			// 使用 NavMeshAgent.Move() 來位移
			navMeshAgent.Move(velocity * Time.deltaTime);

			// 確保不會穿透地面
			if (transform.position.y <= 0.16f) break;

			yield return null;
		}

		// 保證敵人落地
		transform.position = new Vector3(transform.position.x, 0.16f, transform.position.z);

		yield return new WaitForSeconds(0.2f); // 短暫停頓，避免瞬間恢復

		navMeshAgent.isStopped = false; // 恢復導航
		//isFly = false;
		ChangeEnemyState(EnemyState.Idle); // 切回待機狀態
	}
	
	// 如果 EnemyAnimatorController 有回傳 開關攻擊事件, 則去呼叫 EnemyAttack 對應的 攻擊/結束 函式
	private void AttackIsColliderCheck(bool isStartAttack)
    {
        if(isStartAttack)
        {
			enemyAttack.StartAttack();
        }
        else
        {
            enemyAttack.ResetHasAttack();
        }
    }

	//攻擊玩家
	public void Attack()
	{
		if(isAttack)
		{
			playerHealth.TakeDamage(enemyDataSO.attackPower);
		}
	}
	
	// 設定當前是否正在攻擊
	public void SetIsAttack(bool isAttack)
	{
		this.isAttack = isAttack;
		
		if(!isAttack)
		{
			enemyAttack.ResetHasAttack();
		}
		else
		{
			AudioManager.Instance.PlaySound(enemyDataSO.SfxAttackKey, transform.position);
		}
	}
	
	// 設定當前是否正被攻擊
	public void SetIsDamage(bool isDamage)
	{
		this.isDamage = isDamage;
	}

	// 啟動播放死亡動畫的協程
	private void StartDestory()
	{
		StartCoroutine(DeadDissolveCoroutine(1f));
	}

	// 隱藏該物件, 及物件池回收	
	public void DestroySelf()
	{
		gameObject.SetActive(false);
		EnemyManager.Instance.RecycleEnemy(gameObject);
	}

	//死亡時的消融動畫效果
	private IEnumerator DeadDissolveCoroutine(float showTimer)
	{
		material.SetColor(EMISSION_COLOR, Color.cyan);
		material.SetColor(RIM_COLOR, Color.cyan);

		deadParticle.gameObject.SetActive(true);
		deadParticle.Play();

		float timer = 0f;
		while(timer < showTimer)
		{
			timer += Time.deltaTime;
			material.SetFloat(DISSOLVE_AMOUNT, timer/showTimer);
			
			yield return null;
		}

		DestroySelf();
	}

	// 生成時的消融效果, 並於消融完成後, 將物件加入 EnemyManager 的更新列表中
	private IEnumerator StartDissolveCoroutine(float showTimer)
	{
		material.SetColor(EMISSION_COLOR, Color.cyan);
		material.SetColor(RIM_COLOR, Color.cyan);

		float timer = 0f;
		while(timer < showTimer)
		{
			timer += Time.deltaTime;
			material.SetFloat(DISSOLVE_AMOUNT, 1 - timer/showTimer);

			yield return null;
		}

		material.SetFloat(DISSOLVE_AMOUNT, 0f);
		material.SetColor(EMISSION_COLOR, Color.black);
		material.SetColor(RIM_COLOR, Color.black);

		EnemyManager.Instance.AddToUpdateList(this);
	}
}
