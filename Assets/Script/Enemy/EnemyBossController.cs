using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossController : MonoBehaviour, IEnemy
{
    //
    [SerializeField] private Material bossMaterial;
    private float dissolveDuration = 3.0f; // 漸變時間

    [SerializeField] private GameObject _bossUIfinPrefab;
    private BossUIfin _bossUIfin;
    //

    [SerializeField] private GameObject _bossUIPrefab;
    [SerializeField] private EnemyDataSO _enemyDataSO;
	[SerializeField] private Animator _animator;
	[SerializeField] private Transform _bodyTransform;
	[SerializeField] private GameObject _shootAttackPrefab;
	[SerializeField] private GameObject _fogPrefab;

	[SerializeField] private float _attackCooldownTime;

	private bool _isIdle = true;
	//private bool _isAttackCooldown = false;
	private float _originalAnimatorSpeed;
	private Transform _playerTransform;
	private PlayerHealth _playerHealth;
	//private Collider _collider;
	private AnimatorStateInfo _animatorStateInfo;
	private BossUI _bossUI;
    private EnemySpawnTirgger _enemySpawnTirgger;
	private NavMeshAgent _navMeshAgent;

	private bool _hpLessTrigger70 = false;
	private bool _hpLessTrigger35 = false;
	
	public Health Health{get; private set;}
	public bool IsPause{get; private set;}

	private BossState _state;
	private BossState _lastAttackState;

	private enum BossState
	{
		Idle,
		Walk,
		RunAttack,
		CallEnemy,
		ShootAttack,
		FloorAttack,
		Dead
	}
	
	private Dictionary<BossState, int> _attackWeights;
	
	private void Start()
	{
        //
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            bossMaterial = renderer.material;
        }
        bossMaterial.SetFloat("_DissolveAmount", 0f);
        //
        //_collider = GetComponent<Collider>();
	
		// 設定血量及相關事件
		Health = GetComponent<Health>();
		Health.SetMaxHealth(_enemyDataSO.maxHP);
		Health.OnDamage += TakeDamage;
		Health.OnDead += DeadHandler;

		// 設定 NavMeshAgent 不移動, 該元件僅用來抓取路徑
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_navMeshAgent.updatePosition = false;
		_navMeshAgent.updateRotation = false;
		_navMeshAgent.speed = _enemyDataSO.moveSpeed;
		_navMeshAgent.stoppingDistance = 6f;
		_navMeshAgent.enabled = true;

		// 抓取玩家資料
		_playerTransform = FindObjectOfType<PlayerController>()?.transform;
		_playerHealth = _playerTransform.GetComponent<PlayerHealth>();

		// Boss 本身自帶的 UI 效果
		GameObject go = Instantiate(_bossUIPrefab, FindObjectOfType<BattleUIManager>().transform);
        _bossUI = go.GetComponent<BossUI>();
        _bossUI.SetHealth(Health);

        // 預設 Boss 狀態為 Idle
        ChangeEnemyState(BossState.Idle);
		
		// 用字串去抓, 很蠢, 但先這樣
		_enemySpawnTirgger = GameObject.Find("EnemyBossSpawnTrigger").GetComponent<EnemySpawnTirgger>();
		
		// 將 Boss 的 Update 也交給 EnemyManger 來管理
		EnemyManager.Instance.AddToUpdateList(this);

		// 切換 Boss 的 BGM
		AudioManager.Instance.PlayBGM("BattleBackGoundMusic");
	}
	
	public void EnemyUpdate()
    {
		// 如果怪物處於暫停狀態
		if(IsPause) return;
    
		// 如果玩家或怪物已經死亡則不在更新, 並將狀態改為 Idle
        if(_playerHealth.IsDead() || _state == BossState.Dead)
        {
            if(_state != BossState.Idle) ChangeEnemyState(BossState.Idle);
            return;
        }

		// 如果怪物在 Idle 狀態, 則切換為 Walk 狀態
		CheckAnimationIsIdle();
		if(_isIdle) ChangeEnemyState(BossState.Walk);
    }

	// 準備攻擊期間, 往玩家移動
	private IEnumerator ReadToAttackCoroutine()
	{
		_navMeshAgent.transform.position = transform.position;
		_navMeshAgent.transform.rotation = transform.rotation;

		_navMeshAgent.isStopped = false;

		float timer = 0f;

		while(timer < _attackCooldownTime)
		{
			yield return new WaitUntil(() => !IsPause);

			HandleMove();
			
			timer += Time.deltaTime;
			yield return null;
		}
		
		_navMeshAgent.isStopped = true;
		AdjustAttackWeight();
		ChangeEnemyState(GetNextBossState());
	}

	// 移動及旋轉至玩家方向
	private void HandleMove()
	{
		_navMeshAgent.SetDestination(_playerTransform.position);

		Vector3 nextPosition = _navMeshAgent.nextPosition;
		Vector3 direction = _navMeshAgent.desiredVelocity.normalized;
        direction.y = 0; // 確保不會影響 Y 軸 (防止怪物漂浮)

		if (direction != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _navMeshAgent.angularSpeed * Time.deltaTime);
			//transform.rotation = _navMeshAgent.transform.rotation;
		}

		if(Vector3.Distance(transform.position, _playerTransform.position) > 6f)
		{
		    transform.position += _enemyDataSO.moveSpeed * Time.deltaTime * direction;
			_navMeshAgent.nextPosition = transform.position;
		}
	}

	// 檢查目前是否為 Idle 狀態
	private void CheckAnimationIsIdle()
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		_isIdle = _animatorStateInfo.IsName(BossState.Idle.ToString());
	}
	
	private void InitAttackWeight()
	{
	    _attackWeights = new Dictionary<BossState, int>()
		{
			{ BossState.RunAttack, 45 },
			{ BossState.CallEnemy, 10 },
			{ BossState.ShootAttack, 45 },
			{ BossState.FloorAttack, 0 }
		};
	}
	
	// 根據目前情況, 調整攻擊種類的權重
	private void AdjustAttackWeight()
	{
		InitAttackWeight();
	
		float healthRatio = Health.GetHealthRatio();
		float distance = Vector3.Distance(transform.position, _playerTransform.position);

		// 調整召喚小怪機率
		if(!_hpLessTrigger70 && healthRatio < 0.7f)
		{
		    _hpLessTrigger70 = true;
		    
		    _attackWeights = new Dictionary<BossState, int>()
			{
				{ BossState.RunAttack, 0 },
				{ BossState.CallEnemy, 100 },
				{ BossState.ShootAttack, 0 },
				{ BossState.FloorAttack, 0 }
			};
			
			return;
		}
		
		if(!_hpLessTrigger35 && healthRatio < 0.35f)
		{
		    _hpLessTrigger35 = true;
		    
		    _attackWeights = new Dictionary<BossState, int>()
			{
				{ BossState.RunAttack, 0 },
				{ BossState.CallEnemy, 100 },
				{ BossState.ShootAttack, 0 },
				{ BossState.FloorAttack, 0 }
			};
			
			return;
		}

		// 根據玩家距離, 調整攻擊方式
		if (distance < _enemyDataSO.attackRange)
		{
			_attackWeights[BossState.RunAttack] = 60;
			_attackWeights[BossState.ShootAttack] = 30;
		}
		else
		{
			_attackWeights[BossState.RunAttack] = 30;
			_attackWeights[BossState.ShootAttack] = 60;
		}

		if(_attackWeights.TryGetValue(_lastAttackState, out int value))
		{
			_attackWeights[_lastAttackState] -= 35;
		}
	}
	
	// 根據攻擊的分配權重, 取得要用哪招
	private BossState GetNextBossState()
	{
		int totalWeight = 0;
		foreach (int weight in _attackWeights.Values)
		{
			totalWeight += weight;
		}

		int randomValue = UnityEngine.Random.Range(0, totalWeight);
		int sumRandomValue = 0;

		foreach (var data in _attackWeights)
		{
			sumRandomValue += data.Value;
			if (randomValue < sumRandomValue)
			{
				_lastAttackState = data.Key;
				return data.Key;
			}
		}
	
		Debug.Log(" EnemyBossController/GetNextBossState 不應該回傳該參數才對");
	    return BossState.RunAttack;
	}
	
	private void ChangeEnemyState(BossState newState)
	{
		if(_state == BossState.Dead || _state == newState) return;
		
		_state = newState;
		_animator.CrossFade(_state.ToString(), 0.2f);

        switch (_state)
        {
            case BossState.Idle:
				_isIdle = true;
                break;
            case BossState.Walk:
				AudioManager.Instance.PlaySound("BossWalk", transform.position, true, _attackCooldownTime*2);
				StartCoroutine(ReadToAttackCoroutine());
				break;
            case BossState.RunAttack:
				AudioManager.Instance.PlaySound("BossAttackGround", transform.position);
				StartCoroutine(RunAttackCoroutine());
                break;
			case BossState.CallEnemy:
				AudioManager.Instance.PlaySound("BossAttackCall", transform.position);
				StartCoroutine(DelayCallEnemyCoroutine());
                break;
            case BossState.ShootAttack:
				AudioManager.Instance.PlaySound("BossAttackShoot", transform.position);
				StartCoroutine(DelayShootAttackCoroutine());
                break;
            case BossState.FloorAttack:
                break;
            case BossState.Dead:
                break;
        }
    }
    
    private IEnumerator RunAttackCoroutine()
    {
		GameObject go = Instantiate(_fogPrefab);
		go.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

		//-----鑽地板
		float timer = 0f;
		
		Vector3 originalVector3 = transform.position;
		Vector3 targetVector3 = transform.position + Vector3.down * 4f;
		
		yield return new WaitForSeconds(0.8f);
		
		// 鑽到地板的動畫
		while(timer < 1.3f)
		{
			yield return new WaitUntil(() => !IsPause);
			
			transform.position = Vector3.Slerp(originalVector3, targetVector3, timer/1.3f);
			
			timer += Time.deltaTime;
		    yield return null;
		}
		
		transform.position = targetVector3;
		//-----鑽地板結束
		
		
		//-----旋轉衝刺
		// 取得向玩家的方向
		Vector3 direct = (_playerTransform.position - transform.position).normalized;
		direct.y = 0;
		
		_navMeshAgent.isStopped = false;
		_navMeshAgent.speed = 35f;
		_navMeshAgent.nextPosition = transform.position;
		_navMeshAgent.SetDestination(transform.position + direct * 100f);
		
		bool hasCollider = false;
		
		timer = 0f;

		AudioManager.Instance.PlaySound("GoGoGo", transform.position);
		
		while(true)
		{
		    yield return new WaitUntil(() => !IsPause);
		    
		    Vector3 nextPosition = _navMeshAgent.nextPosition;
		    nextPosition.y = -4f;
		
			transform.position = nextPosition;
			go.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
			
			if(_navMeshAgent.velocity.magnitude < 0.1f)
			{
				timer += Time.deltaTime;
			}
			
			if(timer > 0.5f) break;
		    
		    Collider[] colliderArray = Physics.OverlapSphere(transform.position + Vector3.up * 4f, 4f);
		    _bodyTransform.Rotate(Vector3.up * 20f);
		    
		    foreach(Collider collider in colliderArray)
		    {
		        if(collider.TryGetComponent(out PlayerHealth playerHealth))
		        {
		            //Debug.Log("123");
		            playerHealth.CriticalDamage(50);
		            hasCollider = true;
		        }
		    }
		    
		    if(hasCollider) break;
		    
		    yield return null;
		}
		
		_navMeshAgent.isStopped = true;
		_navMeshAgent.speed = _enemyDataSO.moveSpeed;
		//-----旋轉衝刺結束
		
		//-----爬起來
		AudioManager.Instance.PlaySound("BossAttackGround", transform.position);
		timer = 0f;
		
		_animator.Play(_state.ToString(), 0, 0f);
		Quaternion startRotation = _bodyTransform.localRotation; // 記錄起始旋轉
		Quaternion targetRotation = Quaternion.identity; // 目標旋轉
		
		while(timer < 0.3f)
		{
			float rotateSpeed = Mathf.Lerp(540f, 0f, timer/0.3f);
			_bodyTransform.localRotation = Quaternion.RotateTowards(_bodyTransform.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
		    
		    timer += Time.deltaTime;
		    yield return null;
		}
		
		_bodyTransform.localRotation = Quaternion.identity;
		
		timer = 0f;
		originalVector3 = transform.position;
		targetVector3 = transform.position + Vector3.up * 4f;
	
		// 鑽到地板的動畫
		while(timer < 1.3f)
		{
			yield return new WaitUntil(() => !IsPause);
			
			transform.position = Vector3.Slerp(originalVector3, targetVector3, timer/1.3f);
			
			timer += Time.deltaTime;
		    yield return null;
		}
		
		transform.position = targetVector3;

		Destroy(go);
		
		ChangeEnemyState(BossState.Idle);
		//-----爬起來完成
    }

	private IEnumerator DelayShootAttackCoroutine()
	{
		_shootAttackPrefab.SetActive(false);

		yield return new WaitForSeconds(1.7f);

		_shootAttackPrefab.SetActive(true);
	}

	// private IEnumerator DelayFloorAttackCoroutine()
	// {
	// 	_floorAttackPrefab.SetActive(false);

	// 	yield return new WaitForSeconds(0.8f);

	// 	_floorAttackPrefab.SetActive(true);
	// }
	
	// 招喚小怪, 用 Animation Event 來觸發
	private IEnumerator DelayCallEnemyCoroutine()
	{
		yield return new WaitForSeconds(1.0f);
		
	    _enemySpawnTirgger.StartSpawnEnemy();
	}

	private void DeadHandler()
	{
		ChangeEnemyState(BossState.Dead);
		AudioManager.Instance.PlaySound("BossDead", transform.position);

		EnemyManager.Instance.TakeAllEnemyDamage(999);
		
		_playerTransform.GetComponent<LevelSystem>().AddExperience(_enemyDataSO.exp);

        // 觸發 Shader Dissolve 效果
        if (bossMaterial != null)
        {
            StartCoroutine(FadeOutBoss());
        }
    }

    // Coroutine 讓材質變化 和 Boss死亡
    private IEnumerator FadeOutBoss()
    {
        yield return new WaitForSeconds(2f);
        float timer = 0f;
        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0, 0.3f, timer / dissolveDuration);
            bossMaterial.SetFloat("_DissolveAmount", dissolveValue);
            yield return null;
        }
        bossMaterial.SetFloat("_DissolveAmount", 0.3f); // 確保最後完全 Dissolve

        // 最後跳出Boss 死亡UI
        GameObject go2 = Instantiate(_bossUIfinPrefab, FindObjectOfType<BattleUIManager>().transform);
        _bossUIfin = go2.GetComponent<BossUIfin>();
    }

    //如果需要 Enemy 受傷, 呼叫該函數
    public void TakeDamage()
	{
		if(_state == BossState.Dead) return;

		//AudioManager.Instance.PlaySound(enemyDataSO.SfxDamageKey, transform.position);

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up * 6, Health.LastDamage);
	}
	
	// 暫停怪物
    public void SetIsPause(bool isPause)
    {
        IsPause = isPause;
        
        if(isPause) _originalAnimatorSpeed = _animator.speed;
        _animator.speed = isPause ? 0f : _originalAnimatorSpeed;
    }
}
