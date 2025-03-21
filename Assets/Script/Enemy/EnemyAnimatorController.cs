using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
	[SerializeField] Animator _animator;
	
	private EnemyState _currentState;
	private bool _isAttack;
	private bool _isDamage;
	private bool _isIdle;
	private AnimatorStateInfo _animatorStateInfo;
	
	public Action<bool> OnIdleChange;
	public Action<bool> OnAttackChange;
	public Action<bool> OnDamageChange;
	public Action<bool> OnStartAttackCheck;
	public Action OnDead;
	
	private const string IDLE = "Idle";
	private const string ATTACK = "Attack";
	private const string DAMAGE = "Damage";
	private const string IS_WALK = "isWalk";
	private const string IS_ATTACK = "isAttack";
	private const string IS_DEAD = "isDead";
	private const string IS_DAMAGE = "isDamage";
	
	private void OnEnable()
	{
		_currentState = EnemyState.Idle;
		_animator = GetComponent<Animator>();

		_isAttack = false;
		_isDamage = false;
	}

	private void Update()
	{
		CheckAnimationIsIdle();
		CheckAnimationIsAttack();
		CheckAnimationIsDamage();
	}
	
	// 檢查當前動畫是否是 Animation Idle
	private void CheckAnimationIsIdle()
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayIdle = _animatorStateInfo.IsName(IDLE) && _animatorStateInfo.normalizedTime < 1;
		
		if(_isIdle == isPlayIdle) return;
		
		_isIdle = isPlayIdle;
		OnIdleChange?.Invoke(_isIdle);
	}
	
	// 檢查當前動畫是否是 Animation Attack
	private void CheckAnimationIsAttack()
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayAttack = _animatorStateInfo.IsName(ATTACK) && _animatorStateInfo.normalizedTime < 1;
		
		if(_isAttack == isPlayAttack) return;
		
		_isAttack = isPlayAttack;
		_animator.SetBool(IS_ATTACK, _isAttack);
		OnAttackChange?.Invoke(_isAttack);
	}
	
	// 檢查當前動畫是否是 Animation Damage
	private void CheckAnimationIsDamage()
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayDamage = _animatorStateInfo.IsName(DAMAGE) && _animatorStateInfo.normalizedTime < 1;
		
		if(_isDamage == isPlayDamage) return;
		
		_isDamage = isPlayDamage;
		OnDamageChange?.Invoke(_isDamage);
	}
	
	public void SetEnemyState(EnemyState newState)
	{
		if(_currentState == EnemyState.Dead) return;
		
		_currentState = newState;
		_animator.CrossFade(_currentState.ToString(), 0.2f);
		
		// switch (_currentState)
		// {
		// 	case EnemyState.Idle:
		// 		_animator.SetBool(IS_WALK, false);
		// 		_animator.SetBool(IS_ATTACK, false);
		// 		break;
		// 	case EnemyState.Walk:
		// 		_animator.SetBool(IS_WALK, true);
		// 		_animator.SetBool(IS_ATTACK, false);
		// 		break;
		// 	case EnemyState.Attack:
		// 		_animator.SetBool(IS_WALK, false);
		// 		_animator.SetBool(IS_ATTACK, true);
		// 		break;
		// 	case EnemyState.Damage:
		// 		_animator.SetTrigger(IS_DAMAGE);
		// 		break;
		// 	case EnemyState.Dead:
		// 		_animator.SetTrigger(IS_DEAD);
		// 		break;
		// }
	}
	
	public void OnDeadTrigger()
	{
		OnDead?.Invoke();
	}

	public void StartAttackCheckTrigger()
	{
		OnStartAttackCheck?.Invoke(true);
	}

	public void EndAttackCheckTrigger()
	{
		OnStartAttackCheck?.Invoke(false);
	}
	
	public void SetIsPause(bool isPause)
	{
		if(isPause)
		{
			_animator.speed = 0f;
		}
		else
		{
			_animator.speed = 1f;
		}
	}
}
