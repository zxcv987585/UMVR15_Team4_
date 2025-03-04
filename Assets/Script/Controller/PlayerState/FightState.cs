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
    public float attackResetTime = 0.6f;
    //預估動畫播放所需的時間
    private float attackAnimationTime = 0.4f;
    //確認是否可以攻擊
    public bool CanAttack = true;
    //傳送目前攻擊Trigger給動畫控制器
    public Action<string> AttackCombo;
    //傳送重置Trigger的指令給動畫控制器
    public Action<bool> isAttacking;
    //傳送劍氣生成指令給WeaponManager
    public event Action SwordSlash;


    public FightState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player) {}

    public override void Enter()
    {
        currentComboStep = 0;
        isAttacking.Invoke(true);
    }

    public override void Update()
    {
        if (QuitState())
        {
            ResetCombo();
            StateMachine.ChangeState(player.idleState);
            return;
        }

        if (CanAttack && player.isAttack)
        {
            Attack();
            return;
        }
        if (player.isDash)
        {
            StateMachine.ChangeState(player.dashState);
            ResetCombo();
            return;
        }
        if (player.isAiming)
        {
            StateMachine.ChangeState(player.aimState);
            ResetCombo();
            return;
        }
        if (player.IsDie)
        {
            StateMachine.ChangeState(player.deadState);
            return;
        }
        if (player.isHit)
        {
            ResetCombo();
            return;
        }
    }

    public override void Move() { }

    private void Attack()
    {
        if (!CanAttack) return;
        if (player.LockTarget != null) 
        {
            Vector3 direction = (player.LockTarget.position - player.transform.position).normalized;
            direction.y = 0;
            player.transform.rotation = Quaternion.LookRotation(direction);
        }

        attackTimer = 0;
        CanAttack = false;

        float distanceToEnemy = player.LockTarget != null ? Vector3.Distance(player.transform.position, player.LockTarget.position) : 0;
        float MinDashDistance = 3f;

        if (player.LockTarget != null && distanceToEnemy > MinDashDistance) 
        {
            player.StartPlayerCoroutine(DashAttack());
        }
        else
        {
            PerformAttack();
        }
    }

    private IEnumerator DashAttack()
    {
        //突進攻擊持續時間
        float dashAttackDuration = 0.2f;
        //突進攻擊瞬間速度
        float dashspeed = 30f;

        Vector3 startPos = player.transform.position;
        Vector3 TargetPos = player.LockTarget.position;

        AttackCombo?.Invoke("DashAttack");

        yield return new WaitForSeconds(0.25f);

        float elapsedTime = 0f;
        while (elapsedTime < dashAttackDuration) 
        {
            player.controller.Move((TargetPos - startPos).normalized * dashspeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.controller.Move((TargetPos - player.transform.position).normalized * 0.1f);
        yield return new WaitForSeconds(0.3f);

        PerformAttack();
    }

    private void PerformAttack()
    {
        // 播放對應的攻擊動畫
        switch (currentComboStep)
        {
            case 0:
                AttackCombo.Invoke("Attack1");
                //SwordSlash.Invoke();
                player.isAttack = false;
                break;
            case 1:
                AttackCombo.Invoke("Attack2");
                //SwordSlash.Invoke();
                player.isAttack = false;
                break;
            case 2:
                AttackCombo.Invoke("Attack3");
                //SwordSlash.Invoke();
                player.isAttack = false;
                break;
            case 3:
                AttackCombo.Invoke("Attack4");
                //SwordSlash.Invoke();
                player.isAttack = false;
                break;
            case 4:
                ResetCombo();
                AttackCombo.Invoke("Attack1");
                //SwordSlash.Invoke();
                player.isAttack = false;
                break;
        }

        player.StartPlayerCoroutine(AttackCoolDown());
    }

    private void ResetCombo()
    {
        currentComboStep = 0;
        isAttacking.Invoke(false);
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