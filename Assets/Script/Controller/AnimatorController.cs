using System;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    Animator animator;
    RuntimeAnimatorController DefaultController;
    PlayerController player;
    PlayerHealth health;

    private bool isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();

        DefaultController = animator.runtimeAnimatorController;

        if (player != null)
        {
            player.fightState.AttackCombo += AttackCombo;
            player.fightState.isAttacking += AttackReset;
            player.idleState.IsIdle += Idle;
            player.moveState.IsMoving += Walk;
            player.moveState.IsRun += Sprint;
            player.dashState.Dash += Dash;
            player.dashState.ForceIdle += Idle;
            player.aimState.OnAim += Aim;
            player.aimState.OnAimMove += AimMove;
            player.OnHit += Hit;
            health.OnDead += Dead;
            health.OnCriticalDamage += CriticalDamage;
            health.PlayerRivive += Rivive;
        }
    }

    private void Rivive()
    {
        isDead = false;
        animator.CrossFade("revive", 0f, 0);
    }

    private void CriticalDamage()
    {
        if (isDead) return;
        animator.CrossFade("CriticalDamage", 0f, 0);
    }

    private void Dead()
    {
        if (isDead) return;
        isDead = true;
        animator.CrossFade("Die", 0f, 0);

        health.OnDead -= Dead;
    }

    private void AimMove(float moveX, float moveY)
    {
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
    }

    private void Aim(bool isAim)
    {
        if (player.IsDie) return;

        if (isAim)
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1);
            animator.Play("TakeRifle", 1, 0f);
            animator.SetBool("IsAim", true);
        }
        if (!isAim)
        {
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
            animator.SetBool("IsAim", false);
            animator.SetTrigger("Take");
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
        }
    }

    private void Hit()
    {
        animator.SetTrigger("Hit");
    }

    private void Dash(string isDash)
    {
        animator.CrossFade("SlideDash", 0f);
    }

    private void Sprint(bool sprint)
    {
        animator.SetBool("Sprint", sprint);
    }

    private void Walk(bool Iswalk)
    {
        animator.SetBool("Run", Iswalk);
    }

    private void Idle(bool IsIdle)
    {
        animator.SetBool("Idle", IsIdle);
        if (IsIdle)
        {
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
            animator.ResetTrigger("Attack4");
        }
    }

    private void AttackReset(bool isAttack)
    {
        if (!isAttack)
        {
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
            animator.ResetTrigger("Attack4");
        }
    }

    private void AttackCombo(string attack)
    {
        animator.SetTrigger(attack);
    }
}