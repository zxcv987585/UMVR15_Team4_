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
	private bool _isIdle = false;
	private bool _isAttack;
	
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
		_navMeshAgent.stoppingDistance = 0.01f;
		_navMeshAgent.speed = _enemyDataSO.moveSpeed;
		_navMeshAgent.updatePosition = false;
		_navMeshAgent.updateRotation = false;

		//訂閱其他 Enemy 相關事件
		_enemyAnimatorController.OnAttackChange += SetIsAttack;
		_enemyAnimatorController.OnIdleChange += SetIsIdle;
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
		
		// 如果目前怪物被暫停中, 就不更新
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
		
		if(_isIdle) ChangeEnemyState(EnemyState.Walk);
		HandlerMove();
	}

	// 當狀態為 Walk, 轉向及追蹤玩家
	private void HandlerMove()
	{
		if(_enemyState != EnemyState.Walk) return;
	
		// 重置 NavMeshAgent 位置為 物件位置
		if (Vector3.Distance(transform.position, _navMeshAgent.nextPosition) > 0.5f)
		{
			_navMeshAgent.Warp(transform.position);
		}
		_navMeshAgent.transform.rotation = transform.rotation;
		_navMeshAgent.SetDestination(_playerTransform.position);
	
		// 計算下個位置的地點及方向
        Vector3 direction = (_navMeshAgent.nextPosition - transform.position).normalized;
        direction.y = 0; // 確保不會影響 Y 軸 (防止怪物漂浮)

		Vector3 rotateDirection = (_playerTransform.position - transform.position).normalized;
		rotateDirection.y = 0f;

		// 取得玩家方向的四元數
		Quaternion targetRotation = transform.rotation;
		if (rotateDirection.magnitude > 0.01f)
		{
			targetRotation = Quaternion.LookRotation(rotateDirection);
		}
		
		// 計算旋轉角度
		float rotationAngle = Quaternion.Angle(transform.rotation, targetRotation);
		float distance = Vector3.Distance(transform.position, _playerTransform.position);
		
		// 檢查移動後, 是不是剛好到攻擊範圍
        if(distance <= _enemyDataSO.attackRange && rotationAngle < 0.1f)
        {
			ChangeEnemyState(EnemyState.Attack);
			return;
        }
		
		// 旋轉物件
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _navMeshAgent.angularSpeed * Time.deltaTime);
		
		// 判斷玩家距離決定是否要繼續前進
		if(distance > _enemyDataSO.attackRange)
		{
		    transform.position += _enemyDataSO.moveSpeed * Time.deltaTime * direction;
		}
	}
	
	private void SetIsIdle(bool isIdle) => _isIdle = isIdle;
	
	// 切換怪物狀態機狀態
	private void ChangeEnemyState(EnemyState newState)
	{
		// 如果怪物狀態為死去 或是 不在地上, 則不允許變更狀態
		if(_enemyState == EnemyState.Dead || !_navMeshAgent.enabled || _enemyState == newState) return;
		
		_enemyState = newState;
		_enemyAnimatorController?.SetEnemyState(_enemyState);

		_navMeshAgent.isStopped = !(_enemyState == EnemyState.Walk);
		_isIdle = _enemyState == EnemyState.Idle;
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
		if(_isInit) return;	// 如果還在生成中, 無法被擊飛

		//navMeshAgent.enabled = false;
		_rb.isKinematic = false;

		float originalHeight = transform.position.y;
		_rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
		_rb.AddForce(Vector3.up * flyPower, ForceMode.Impulse);
		//StartCoroutine(EnableNavMeshDelay(1f, originalHeight));
		StartCoroutine(EnableNavMeshDelay());
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

	private IEnumerator EnableNavMeshDelay(float checkInterval = 0.1f)
	{
		NavMeshHit hit;
		int enemyAreaMask = 1 << NavMesh.GetAreaFromName("Enemy");

		yield return new WaitForSeconds(0.1f);
		
		// 等待敵人落地
		while (true)
		{
			// 嘗試在當前位置附近尋找 NavMesh 上的點
			if (NavMesh.SamplePosition(transform.position, out hit, 1f, enemyAreaMask))
			{
				// 當 y 軸低於 NavMesh 點 或 靠近該點，視為已落地
				if (transform.position.y <= hit.position.y + 0.1f)
					break;
			}

			yield return new WaitForSeconds(checkInterval);
		}

		Debug.Log("So Quit");

		// 確保 NavMeshAgent 可用
		_rb.isKinematic = true;
		//_navMeshAgent.enabled = true;
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
		if(_enemyState == EnemyState.Attack)
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
		Health.SetIsInvincibility(true);

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

		Health.SetIsInvincibility(false);
		_isInit = false;
	}
	
	public void StopRaycastSpawnCoroutine()
	{
		if(!_isInit) return;

	    StopAllCoroutines();
		Health.SetIsInvincibility(false);

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
