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
	private IEnemyAttack _enemyAttack;
	private EnemyState _enemyState;

	[SerializeField] private Renderer dissolveRenderer;
	[SerializeField] private ParticleSystem deadParticle;
	[SerializeField] private float dissolveTime = 1f;
	private Material material;
	private const string DISSOLVE_AMOUNT = "_DissolveAmount";
	private const string EMISSION_COLOR= "_EmissionColor";
	private const string RIM_COLOR = "_RimColor";

	private bool _isInit = true;	// 用來等待生成動畫結束後, 才跑 Update 的旗標
	private bool _hasInit = false; //用來檢查該物件是否為第一次被生成
	private bool _isAttack;
	private bool _isDamage;
	private bool _isPause;
	
	private Rigidbody rb;
	private NavMeshAgent navMeshAgent;
	private Collider bodyCollider;
	private Health health;
	private PlayerHealth playerHealth;
	private Transform playerTransform;
	
	private void OnEnable()
	{
		if(!_hasInit) Init();

		_isPause = false;

		health.Init();
		health.SetMaxHealth(enemyDataSO.maxHP);

		_isAttack = false;
		_isDamage = false;
		bodyCollider.enabled = true;

		//預設怪物狀態機以 Idle 開始
		_enemyState = EnemyState.Idle;
		ChangeEnemyState(EnemyState.Idle);
		
		//如果 NavMeshAgent 預設啟用, 會因為 NavMeshAgent 只允許物件放置在 NavMeshPath 上, 導致物件無法正常設定 transform.position, 因此延遲啟動
		StartCoroutine(DelayEnableNavMeshAgent());
		
		EnemyManager.Instance.AddToUpdateList(this);
		StartCoroutine(StartDissolveCoroutine(dissolveTime));
	}


    // 第一次被實例化時執行
    public void Init()
	{
		_hasInit = true;

		//抓取起始元件
		rb = GetComponent<Rigidbody>();
		bodyCollider = GetComponent<Collider>();
		playerTransform = FindObjectOfType<PlayerController>()?.transform;
		playerHealth = playerTransform.GetComponent<PlayerHealth>();
		material = dissolveRenderer.material;
		_enemyAttack = (IEnemyAttack)enemyAttackMonoBehaviour;

		//設定 血量
		health = GetComponent<Health>();
		health.OnDamage += DamageEvent;
		health.OnDead += DeadEvent;

		//設定 NavMeshAgent
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.stoppingDistance = enemyDataSO.attackRange;
		navMeshAgent.speed = enemyDataSO.moveSpeed;

		//訂閱其他 Enemy 相關事件
		enemyAnimatorController.OnAttackChange += SetIsAttack;
		enemyAnimatorController.OnDamageChange += SetIsDamage;
		enemyAnimatorController.OnStartAttackCheck += AttackIsColliderCheck;
		enemyAnimatorController.OnDead += StartDestory;
		_enemyAttack.OnAttackHit += Attack;
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
	
	
	public void SetIsPause(bool isPause)
	{
		_isPause = isPause;
		
		enemyAnimatorController.SetIsPause(isPause);
	}

	public void EnemyUpdate()
	{
		// 如果還在播放 初始動畫則直接離開
		if(_isInit) return;
		
		if(_isPause) return;

		// 如果玩家死了就先把狀態換成 Idle, 之後就直接離開
		if(playerHealth.IsDead())
		{
			if(_enemyState != EnemyState.Idle)
			{
				ChangeEnemyState(EnemyState.Idle);
			}
			return;
		}

		//如果目前怪物狀態不是在攻擊或被打中, 則檢查玩家位置
		if(!_isAttack && !_isDamage)
		{
			CheckPlayerDistance();

			if(_enemyState == EnemyState.Walk)
			{
				MoveHandler();
			}
		}
	}

	// 當狀態為 Walk, 轉向及追蹤玩家
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
		if(_enemyState == EnemyState.Dead || !navMeshAgent.enabled) return;
		
		_enemyState = newState;

		switch (_enemyState)
		{
			case EnemyState.Idle:
				navMeshAgent.isStopped = true;
				enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
			case EnemyState.Walk:
				navMeshAgent.isStopped = false;
				//navMeshAgent.SetDestination(GetRandomPositionAroundPlayer());
				navMeshAgent.SetDestination(playerTransform.position);
				enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
			case EnemyState.Attack:
				navMeshAgent.isStopped = true;
				StartCoroutine(TryAttackAfterTurn(playerTransform.position));
				break;
			case EnemyState.Damage:
				navMeshAgent.isStopped = true;
				enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
			case EnemyState.Dead:
				navMeshAgent.isStopped = true;
				enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
		}
	}

	// 將怪物轉向玩家位置後, 在進行攻擊行為
	private IEnumerator TryAttackAfterTurn(Vector3 targetPosition)
	{
		while (true)
		{
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !_isPause);
		
			if(_enemyState == EnemyState.Dead) break;

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
		if (_enemyState == EnemyState.Attack)
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
	
	public Health GetHealth()
	{
		return health;
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
		if (_enemyState == EnemyState.Dead) return;

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

		rb.isKinematic = true;
	}
	
	// 如果 EnemyAnimatorController 有回傳 開關攻擊事件, 則去呼叫 EnemyAttack 對應的 攻擊/結束 函式
	private void AttackIsColliderCheck(bool isStartAttack)
    {
        if(isStartAttack)
        {
			_enemyAttack.StartAttack();
        }
        else
        {
            _enemyAttack.ResetHasAttack();
        }
    }

	//攻擊玩家
	public void Attack()
	{
		if(_isAttack)
		{
			playerHealth.TakeDamage(enemyDataSO.attackPower);
		}
	}
	
	// 設定當前是否正在攻擊
	public void SetIsAttack(bool isAttack)
	{
		_isAttack = isAttack;
		
		if(!isAttack)
		{
			_enemyAttack.ResetHasAttack();
		}
		else
		{
			AudioManager.Instance.PlaySound(enemyDataSO.SfxAttackKey, transform.position);
		}
	}
	
	// 設定當前是否正被攻擊
	public void SetIsDamage(bool isDamage)
	{
		_isDamage = isDamage;
	}

	// 啟動播放死亡動畫的協程
	private void StartDestory()
	{
		StartCoroutine(DeadDissolveCoroutine(1f));
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
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !_isPause);
			
			timer += Time.deltaTime;
			material.SetFloat(DISSOLVE_AMOUNT, timer/showTimer);
			
			yield return null;
		}

		// 待死亡動畫結束後, 讓物件池回收自己
		EnemyManager.Instance.RecycleEnemy(gameObject);
	}

	// 生成時的消融效果, 並於消融完成後, 將物件加入 EnemyManager 的更新列表中
	private IEnumerator StartDissolveCoroutine(float showTimer)
	{
		material.SetColor(EMISSION_COLOR, Color.cyan);
		material.SetColor(RIM_COLOR, Color.cyan);

		float timer = 0f;
		while(timer < showTimer)
		{
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !_isPause);
		
			timer += Time.deltaTime;
			material.SetFloat(DISSOLVE_AMOUNT, 1 - timer/showTimer);

			yield return null;
		}

		material.SetFloat(DISSOLVE_AMOUNT, 0f);
		material.SetColor(EMISSION_COLOR, Color.black);
		material.SetColor(RIM_COLOR, Color.black);

		_isInit = false;
	}
}
