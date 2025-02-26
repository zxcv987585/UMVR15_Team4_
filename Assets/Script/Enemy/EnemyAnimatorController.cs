using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
	[SerializeField] Animator animator;
	
	private EnemyState currentState;
	private bool isAttack = false;
	private bool isDamage = false;
	private AnimatorStateInfo animatorStateInfo;
	
	public Action<bool> OnAttackChange;
	public Action<bool> OnDamageChange;
	public Action<bool> OnStartAttackCheck;
	public Action OnDead;
	
	private const string ATTACK = "Attack";
	private const string DAMAGE = "Damage";
	private const string IS_WALK = "isWalk";
	private const string IS_ATTACK = "isAttack";
	private const string IS_DEAD = "isDead";
	private const string IS_DAMAGE = "isDamage";
	
	private void Start()
	{
		currentState = EnemyState.Idle;
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		CheckAnimationIsAttack();
		CheckAnimationIsDamage();
	}
	
	//檢查當前動畫是否是 Animation Attack
	private void CheckAnimationIsAttack()
	{
		animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayAttack = animatorStateInfo.IsName(ATTACK) && animatorStateInfo.normalizedTime < 1;
		
		if(isAttack == isPlayAttack) return;
		
		isAttack = isPlayAttack;
		animator.SetBool(IS_ATTACK, isAttack);
		OnAttackChange?.Invoke(isAttack);
	}
	
	private void CheckAnimationIsDamage()
	{
		animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		bool isPlayDamage = animatorStateInfo.IsName(DAMAGE) && animatorStateInfo.normalizedTime < 1;
		
		if(isDamage == isPlayDamage) return;
		
		isDamage = isPlayDamage;
		OnDamageChange?.Invoke(isDamage);
	}
	
	public void SetEnemyState(EnemyState newState)
	{
		if(currentState == EnemyState.Dead) return;
		
		currentState = newState;

		switch (currentState)
		{
			case EnemyState.Idle:
				animator.SetBool(IS_WALK, false);
				animator.SetBool(IS_ATTACK, false);
				break;
			case EnemyState.Walk:
				animator.SetBool(IS_WALK, true);
				animator.SetBool(IS_ATTACK, false);
				break;
			case EnemyState.Attack:
				animator.SetBool(IS_WALK, false);
				animator.SetBool(IS_ATTACK, true);
				break;
			case EnemyState.Damage:
				animator.SetTrigger(IS_DAMAGE);
				break;
			case EnemyState.Dead:
				animator.SetTrigger(IS_DEAD);
				break;
		}
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
}
