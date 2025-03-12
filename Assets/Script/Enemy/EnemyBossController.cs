using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossController : MonoBehaviour, IEnemy
{
	[SerializeField] private GameObject _bossUIPrefab;
    [SerializeField] private EnemyDataSO _enemyDataSO;
	[SerializeField] private Animator _animator;
	[SerializeField] private GameObject _shootAttackPrefab;
	[SerializeField] private GameObject _floorAttackPrefab;

	[SerializeField] private float _attackCooldownTime;

	private bool _isIdle = true;
	private bool _isAttackCooldown = false;
	private bool _isPause = false;
	private Transform _playerTransform;
	private PlayerHealth _playerHealth;
	private AnimatorStateInfo _animatorStateInfo;
	private BossUI _bossUI;
	private EnemySpawnTirgger _enemySpawnTirgger;

	private bool _hpLessTrigger70 = false;
	private bool _hpLessTrigger35 = false;
	
	public Health Health{get; private set;}

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
		// 設定血量及相關事件
		Health = GetComponent<Health>();
		Health.SetMaxHealth(_enemyDataSO.maxHP);
		Health.OnDamage += TakeDamage;
		Health.OnDead += DeadHandler;

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
	}
	
	public void EnemyUpdate()
    {
		if(_isPause) return;
    
        if(_playerHealth.IsDead() || state == BossState.Dead) return;

		CheckAnimationIsIdle();
		LookAtPlayer();

		if(_isIdle) CheckPlayerDistance();
    }

	private void LookAtPlayer()
	{
		Vector3 direction = (_playerTransform.position - transform.position).normalized;
		direction.y = 0;
		Quaternion targetRotation = Quaternion.LookRotation(direction);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 15f * Time.deltaTime);
	}

	private void CheckAnimationIsIdle()
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayIdle = _animatorStateInfo.IsName(BossState.Idle.ToString());
		
		if(_isIdle == isPlayIdle) return;
		
		if(!isPlayIdle)
		{
			_isIdle = false;
		}
		else
		{
			if(!_isAttackCooldown)
			{
				StartCoroutine(StartAttackCoolDown());
			}
		}
	}

	private IEnumerator StartAttackCoolDown()
	{
		_isAttackCooldown = true;

		yield return new WaitForSeconds(_attackCooldownTime);

		//isIdle = true;
		ChangeEnemyState(BossState.Idle);
		_isAttackCooldown = false;
	}
	
	private void CheckPlayerDistance()
	{
		if(_playerTransform != null)
		{
			float distance = Vector3.Distance(transform.position, _playerTransform.position);

			if(CheckHealthEvent())
			{
				ChangeEnemyState(BossState.CallEnemy);
				return;
			}

			if (distance <= _enemyDataSO.attackRange)
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
				_isIdle = true;
                break;
            case BossState.RunAttack:
				_animator.SetTrigger(state.ToString());
                break;
			case BossState.CallEnemy:
				_animator.SetTrigger(state.ToString());
				StartCoroutine(DelayCallEnemyCoroutine());
                break;
            case BossState.ShootAttack:
				_animator.SetTrigger(state.ToString());
				StartCoroutine(DelayShootAttackCoroutine());
                break;
            case BossState.FloorAttack:
				_animator.SetTrigger(state.ToString());
				StartCoroutine(DelayFloorAttackCoroutine());
                break;
            case BossState.Dead:
				_animator.SetTrigger("isDead");
                break;
        }
    }

	private IEnumerator DelayShootAttackCoroutine()
	{
		_shootAttackPrefab.SetActive(false);

		yield return new WaitForSeconds(1.7f);

		_shootAttackPrefab.SetActive(true);
	}

	private IEnumerator DelayFloorAttackCoroutine()
	{
		_floorAttackPrefab.SetActive(false);

		yield return new WaitForSeconds(0.8f);

		_floorAttackPrefab.SetActive(true);
	}
	
	// 招喚小怪, 用 Animation Event 來觸發
	private IEnumerator DelayCallEnemyCoroutine()
	{
		yield return new WaitForSeconds(1.0f);
		
	    _enemySpawnTirgger.StartSpawnEnemy();
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

		BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up * 6, Health.LastDamage);
	}

	// 檢查血量是否低於特定條件, 來觸發事件
	private bool CheckHealthEvent()
	{
		if(!_hpLessTrigger70 && Health.GetHealthRatio() < 0.7f)
		{
			_hpLessTrigger70 = true;
			
			return true;
		}

		if(!_hpLessTrigger35 && Health.GetHealthRatio() < 0.35f)
		{
			_hpLessTrigger35 = true;
			
			return true;
		}

		return false;
	}
	
	public void DestroySelf()
	{
		Destroy(gameObject);
	}

    public void SetIsPause(bool isPause)
    {
        _isPause = isPause;
        
        _animator.speed = isPause ? 0f : 1f;
    }
}
