using System;
using System.Collections;
using UnityEngine;

public class FightState : PlayerState
{
    //追蹤連擊次數
    private int currentComboStep = 0;
    //追蹤可以執行連擊的時間
    public float attackTimer = 0;
    //連擊重置的最久等候時間
    public float attackResetTime = 0.8f;
    //預估動畫播放所需的時間
    private float attackAnimationTime = 0.45f;
    //確認是否可以攻擊
    public bool CanAttack = true;
    //傳送目前攻擊Trigger給動畫控制器
    public Action<string> AttackCombo;
    //傳送重置Trigger的指令給動畫控制器
    public Action<bool> isAttacking;
    //追蹤是否處在動畫播放狀態
    private bool isInAttackAnimation = false;


    public FightState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player) {}

    public override void Enter()
    {
        currentComboStep = 0;
        
        isInAttackAnimation = true;
        isAttacking.Invoke(true);
    }

    public override void Update()
    {
        if (QuitState() && !isInAttackAnimation)
        {
            ResetCombo();
            StateMachine.ChangeState(player.idleState);
        }

        if (CanAttack && player.isAttack)
        {
            Attack();
        }
        else if (player.isDash)
        {
            StateMachine.ChangeState(player.dashState);
            ResetCombo();
        }
        else if (player.isAiming)
        {
            StateMachine.ChangeState(player.aimState);
            ResetCombo();
        }
    }

    public override void Move() { }

    private void Attack()
    {
        if (!CanAttack) return;

        attackTimer = 0;
        CanAttack = false;

        // 播放對應的攻擊動畫
        switch (currentComboStep)
        {
            case 0:
                AttackCombo.Invoke("Attack1");
                player.isAttack = false;
                break;
            case 1:
                AttackCombo.Invoke("Attack2");
                player.isAttack = false;
                break;
            case 2:
                AttackCombo.Invoke("Attack3");
                player.isAttack = false;
                break;
            case 3:
                AttackCombo.Invoke("Attack4");
                player.isAttack = false;
                break;
            case 4:
                ResetCombo();
                AttackCombo.Invoke("Attack1");
                player.isAttack = false;
                break;
        }

        player.StartCoroutine(AttackCoolDown());
    }

    private void ResetCombo()
    {
        currentComboStep = 0;
        isAttacking.Invoke(false);
        isInAttackAnimation = false;
    }

    IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(attackAnimationTime);

        CanAttack = true;
        player.isAttack = false;
        currentComboStep++;
    }

    bool QuitState()
    {
        if (CanAttack && !player.isAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackResetTime)
            {
                ResetCombo();
                return true;
            }
        }
        return false;
    }
}