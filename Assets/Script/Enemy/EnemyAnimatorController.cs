using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	
	private EnemyState _currentState;
	private bool _isIdle;
	private AnimatorStateInfo _animatorStateInfo;
	
	public Action<bool> OnIdleChange;
	public Action<bool> OnAttackChange;
	public Action<bool> OnDamageChange;
	public Action<bool> OnStartAttackCheck;
	public Action OnDead;
	
	private const string IDLE = "Idle";
	
	private void OnEnable()
	{
		_currentState = EnemyState.Idle;
		_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		CheckAnimationIsIdle();
	}
	
	// 檢查當前動畫是否是 Animation Idle
	private void CheckAnimationIsIdle()
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayIdle = _animatorStateInfo.IsName(IDLE);
		
		if(_isIdle == isPlayIdle) return;
		
		_isIdle = isPlayIdle;
		OnIdleChange?.Invoke(_isIdle);
	}
	
	public void SetEnemyState(EnemyState newState)
	{
		if(_currentState == EnemyState.Dead || _currentState == newState) return;
		
		_currentState = newState;
		//Debug.Log(gameObject.name + "_currentState" + _currentState.ToString());
		_animator.CrossFade(_currentState.ToString(), 0.2f);
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
