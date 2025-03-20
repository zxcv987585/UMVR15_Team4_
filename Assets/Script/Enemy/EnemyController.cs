using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IEnemy
{
	[SerializeField] private EnemyDataSO _enemyDataSO;
	[SerializeField] private EnemyAnimatorController _enemyAnimatorController;
	[SerializeField] private MonoBehaviour _enemyAttackMonoBehaviour;
	[SerializeField] private GameObject _dropPrefab;
	private IEnemyAttack _enemyAttack;
	private EnemyState _enemyState;

	//-----消融效果-----
	[SerializeField] private Renderer _dissolveRenderer;
	[SerializeField] private ParticleSystem _deadParticle;
	private float _dissolveTime = 1f;
	private Material _material;
	private const string DISSOLVE_AMOUNT = "_DissolveAmount";
	private const string EMISSION_COLOR= "_EmissionColor";
	private const string RIM_COLOR = "_RimColor";
	//-----消融效果-----

	private bool _isInit = true;	// 用來等待生成動畫結束後, 才跑 Update 的旗標
	private bool _hasInit = false; //用來檢查該物件是否為第一次被生成
	private bool _isAttack;
	private bool _isDamage;
	
	private Coroutine _rotateCoroutine;
	private Rigidbody _rb;
	private NavMeshAgent _navMeshAgent;
	private Collider _bodyCollider;
	private PlayerHealth _playerHealth;
	private Transform _playerTransform;
	
	public Health Health {get; private set;}
	public bool IsPause{get; private set;}
	
	private void OnEnable()
	{
		if(!_hasInit) Init();
	
		IsPause = false;

		Health.Init();
		Health.SetMaxHealth(_enemyDataSO.maxHP);

		_isAttack = false;
		_isDamage = false;
		_bodyCollider.enabled = true;

		//預設怪物狀態機以 Idle 開始
		_enemyState = EnemyState.Idle;
		ChangeEnemyState(EnemyState.Idle);
		
		//如果 NavMeshAgent 預設啟用, 會因為 NavMeshAgent 只允許物件放置在 NavMeshPath 上, 導致物件無法正常設定 transform.position, 因此延遲啟動
		StartCoroutine(DelayEnableNavMeshAgent());
		
		EnemyManager.Instance.AddToUpdateList(this);

		// 生成動畫, 我也不曉得為啥要 /2 才會正常
		if(_dissolveTime == 1f)
		{
			// 如果是預設消融時間, 則正常生成
		    StartCoroutine(StartDissolveCoroutine(_dissolveTime/2));
		}
		else
		{
			// 如果不是預設時間, 視為生怪機生的
			StartCoroutine(RaycastStartDissolveCoroutine(_dissolveTime/2));
		}
	}


    // 第一次被實例化時執行
    public void Init()
	{
		_hasInit = true;

		//抓取起始元件
		_rb = GetComponent<Rigidbody>();
		_bodyCollider = GetComponent<Collider>();
		_playerTransform = FindObjectOfType<PlayerController>()?.transform;
		_playerHealth = _playerTransform.GetComponent<PlayerHealth>();
		_material = _dissolveRenderer.material;
		_enemyAttack = (IEnemyAttack)_enemyAttackMonoBehaviour;

		//設定 血量
		Health = GetComponent<Health>();
		Health.OnDamage += DamageEvent;
		Health.OnDead += DeadEvent;

		//設定 NavMeshAgent
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_navMeshAgent.stoppingDistance = _enemyDataSO.attackRange;
		_navMeshAgent.speed = _enemyDataSO.moveSpeed;

		//訂閱其他 Enemy 相關事件
		_enemyAnimatorController.OnAttackChange += SetIsAttack;
		_enemyAnimatorController.OnDamageChange += SetIsDamage;
		_enemyAnimatorController.OnStartAttackCheck += AttackIsColliderCheck;
		_enemyAnimatorController.OnDead += StartDestory;
		_enemyAttack.OnAttackHit += Attack;
	}

	public void SetDissolveTime(float dissolveTime)
	{
		_dissolveTime = dissolveTime;
	}

	//延遲啟用 NavMeshAgent
	private IEnumerator DelayEnableNavMeshAgent()
	{
		yield return null;

		_navMeshAgent.updatePosition = false;
		_navMeshAgent.updateRotation = false;
		_navMeshAgent.speed = _enemyDataSO.moveSpeed;

		_navMeshAgent.enabled = true;
		_navMeshAgent.SetDestination(transform.position);
	}
	
	
	public void SetIsPause(bool isPause)
	{
		IsPause = isPause;
		
		_enemyAnimatorController.SetIsPause(isPause);
	}

	public void EnemyUpdate()
	{
		// 如果還在播放 初始動畫則直接離開
		if(_isInit) return;
		
		if(IsPause) return;

		// 如果玩家死了就先把狀態換成 Idle, 之後就直接離開
		if(_playerHealth.IsDead())
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
				HandlerMove();
			}
		}
	}

	// 當狀態為 Walk, 轉向及追蹤玩家
	private void HandlerMove()
	{
		Vector3 nextPosition = _navMeshAgent.nextPosition;
        Vector3 direction = (nextPosition - transform.position).normalized;
        direction.y = 0; // 確保不會影響 Y 軸 (防止怪物漂浮)

		if (direction != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _navMeshAgent.angularSpeed * Time.deltaTime);
		}

        transform.position += _enemyDataSO.moveSpeed * Time.deltaTime * direction;
	}
	
	// 檢測玩家位置來判斷要進行 攻擊或是追擊
	private void CheckPlayerDistance()
	{
		if(_playerTransform != null && _navMeshAgent.isOnNavMesh)
		{
			float distance = Vector3.Distance(transform.position, _playerTransform.position);

			if (distance <= _enemyDataSO.attackRange)
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
		if(_enemyState == EnemyState.Dead || !_navMeshAgent.enabled) return;
		
		_enemyState = newState;

		switch (_enemyState)
		{
			case EnemyState.Idle:
				_navMeshAgent.isStopped = true;
				_enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
			case EnemyState.Walk:
				_navMeshAgent.isStopped = false;
				//navMeshAgent.SetDestination(GetRandomPositionAroundPlayer());
				_navMeshAgent.SetDestination(_playerTransform.position);
				_enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
			case EnemyState.Attack:
				_navMeshAgent.isStopped = true;
				_rotateCoroutine = StartCoroutine(TryAttackAfterTurn(_playerTransform.position));
				break;
			case EnemyState.Damage:
				_navMeshAgent.isStopped = true;
				_enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
			case EnemyState.Dead:
				_navMeshAgent.isStopped = true;
				_enemyAnimatorController?.SetEnemyState(_enemyState);
				break;
		}
	}

	// 將怪物轉向玩家位置後, 在進行攻擊行為
	private IEnumerator TryAttackAfterTurn(Vector3 targetPosition)
	{
		Debug.Log("TrytoRotate");
		
		while (true)
		{
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !IsPause);
		
			if(_enemyState == EnemyState.Dead) break;

			// 檢查玩家是否仍在攻擊範圍內
			float distance = Vector3.Distance(transform.position, targetPosition);
			if (distance > _enemyDataSO.attackRange)
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
			_enemyAnimatorController.SetEnemyState(EnemyState.Attack);
		}
	}
	
	public Health GetHealth()
	{
		return Health;
	}

	// 怪物受傷時的事件, 訂閱在 <Health> 的 OnDamage
	private void DamageEvent()
	{
		AudioManager.Instance.PlaySound(_enemyDataSO.SfxDamageKey, transform.position);
		ChangeEnemyState(EnemyState.Damage);

		if(Health.LastDamage != 999)
		{
		    BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, Health.LastDamage);
		}
	}

	// 怪物死亡時呼叫該事件, 訂閱在 <Health> 的 OnDead
	public void DeadEvent()
	{
		_bodyCollider.enabled = false;

		AudioManager.Instance.PlaySound(_enemyDataSO.SfxDeadKey, transform.position);
		ChangeEnemyState(EnemyState.Dead);

		if(Health.LastDamage != 999)
		{
		    BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up, Health.LastDamage);
		}
		
		_playerTransform.GetComponent<LevelSystem>().AddExperience(_enemyDataSO.exp);
	}

	/// <summary>
	/// 擊飛該怪物
	/// </summary>
	/// <param name="flyPower">擊飛的高度參數</param>
	public void HitFly(float flyPower)
	{
		if (_enemyState == EnemyState.Dead) return;

		//navMeshAgent.enabled = false;
		_rb.isKinematic = false;

		float originalHeight = transform.position.y;
		_rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
		_rb.AddForce(Vector3.up * flyPower, ForceMode.Impulse);
		StartCoroutine(EnableNavMeshDelay(1f, originalHeight));

		//StartCoroutine(HitFlyCoroutine(flyPower, 1f));
	}

	// 判斷怪物是否已落地
	private IEnumerator EnableNavMeshDelay(float delayTime, float originalHeight)
	{
		yield return new WaitForSeconds(delayTime);

		while(Mathf.Abs(_rb.velocity.y) > 0.1f)
		{
			if(transform.position.y <= originalHeight) break;
			yield return null;
		}

		_rb.isKinematic = true;
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
			_playerHealth.TakeDamage(_enemyDataSO.attackPower);
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
			AudioManager.Instance.PlaySound(_enemyDataSO.SfxAttackKey, transform.position);
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
		_material.SetColor(EMISSION_COLOR, Color.cyan);
		_material.SetColor(RIM_COLOR, Color.cyan);

		_deadParticle.gameObject.SetActive(true);
		_deadParticle.Play();
			
		float dropTimer = showTimer / 2;

		float timer = 0f;
		while(timer < showTimer)
		{
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !IsPause);
			
			timer += Time.deltaTime;
			_material.SetFloat(DISSOLVE_AMOUNT, timer/showTimer);
			
			if(timer > dropTimer && _dropPrefab != null)
			{
				Instantiate(_dropPrefab, transform.position, Quaternion.identity);
				dropTimer = 999;
			}
			
			yield return null;
		}

		_dissolveTime = 1f;
		_navMeshAgent.enabled = false;
		_isInit = true;

		// 待死亡動畫結束後, 讓物件池回收自己
		EnemyManager.Instance.RecycleEnemy(gameObject);
	}

	// 生成時的消融效果, 並於消融完成後, 將物件加入 EnemyManager 的更新列表中
	private IEnumerator StartDissolveCoroutine(float showTimer)
	{
		_material.SetColor(EMISSION_COLOR, Color.cyan);
		_material.SetColor(RIM_COLOR, Color.cyan);

		float timer = 0f;

		while(timer < showTimer)
		{
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !IsPause);
		
			timer += Time.deltaTime;
			_material.SetFloat(DISSOLVE_AMOUNT, 1 - timer/showTimer);

			yield return null;
		}

		_material.SetFloat(DISSOLVE_AMOUNT, 0f);
		_material.SetColor(EMISSION_COLOR, Color.black);
		_material.SetColor(RIM_COLOR, Color.black);

		_isInit = false;
	}
	
	private IEnumerator  RaycastStartDissolveCoroutine(float showTimer)
	{
	    _material.SetColor(EMISSION_COLOR, Color.red);
		_material.SetColor(RIM_COLOR, Color.red);

		float timer = 0f;

		while(timer < showTimer)
		{
			// 如果目前 isPause 為 true, 則暫停更新 Coroutine
			yield return new WaitUntil(() => !IsPause);
		
			timer += Time.deltaTime;
			_material.SetFloat(DISSOLVE_AMOUNT, 1 - timer/showTimer);

			yield return null;
		}

		_material.SetFloat(DISSOLVE_AMOUNT, 0f);
		_material.SetColor(EMISSION_COLOR, Color.black);
		_material.SetColor(RIM_COLOR, Color.black);

		_isInit = false;
	}
	
	public void StopRaycastSpawnCoroutine()
	{
	    StopAllCoroutines();
	    StartCoroutine(CancelDissolveCoroutine(1f));
	}
	
	private IEnumerator CancelDissolveCoroutine(float showTimer)
	{
		float dissolveRatio = _material.GetFloat(DISSOLVE_AMOUNT);
		float timer = 0f;
		
		while(timer < showTimer)
		{
			timer += Time.deltaTime;
		
			_material.SetFloat(DISSOLVE_AMOUNT, Mathf.Lerp(dissolveRatio, 1, timer/showTimer));
		
		    yield return null;
		}
		
		_material.SetFloat(DISSOLVE_AMOUNT, 1f);
		
		EnemyManager.Instance.RecycleEnemy(gameObject);
	}
}
