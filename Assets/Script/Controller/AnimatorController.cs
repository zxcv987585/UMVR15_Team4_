using System;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    Animator animator;
    PlayerController player;
    PlayerHealth health;

    private bool isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();

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
            player.OnGunHit += AimHit;
            player.deadState.Dead += Dead;
            player.OnHit += Hit;
            player.CriticalGunHit += CriticalGunHit;
            health.HaveReviveItemDead += Dead;
            health.NoReviveItemDead += Dead;
            health.OnCriticalDamage += CriticalDamage;
            health.PlayerRivive += Rivive;
        }
    }

    private void CriticalGunHit()
    {
        if (isDead) return;
        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);
        animator.SetBool("IsAim", false);
        animator.CrossFade("CriticalDamage", 0f, 0);
        //if (!player.IsRightKeyDown)
        //{

        //}
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
        animator.SetBool("Sprint", false);
        animator.SetBool("Run", false);
        animator.SetBool("Idle", false);
    }

    private void Dead()
    {
        if (isDead) return;
        isDead = true;
        animator.CrossFade("Die", 0f, 0);
        animator.SetBool("Sprint", false);
        animator.SetBool("Run", false);
        animator.SetBool("Idle", false);
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
        animator.ResetTrigger("Attack4");
        animator.ResetTrigger("DashAttack");
    }

    private void AimMove(float moveX, float moveY)
    {
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
    }

    private void Aim(bool isAim)
    {
        if (isAim)
        {
            animator.SetLayerWeight(0, 0);
            animator.Play("Idle", 0, 0f);
            animator.SetLayerWeight(1, 1);
            animator.Play("TakeRifle", 1, 0f);
            animator.SetBool("IsAim", true);
        }
        if (!isAim)
        {
            if (player.IsDie)
            {
                animator.SetLayerWeight(0, 1);
                animator.SetLayerWeight(1, 0);
                animator.CrossFade("Die", 0f, 0);
            }
            else
            {
                animator.SetLayerWeight(0, 1);
                animator.SetLayerWeight(1, 0);
                animator.CrossFade("TakeRifle", 0f, 0);
                animator.SetBool("IsAim", false);
                animator.SetTrigger("Take");
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 0);
            }

        }
    }

    private void AimHit()
    {
        animator.CrossFade("GunHit", 0f, 1);
    }

    private void Hit()
    {
        animator.SetTrigger("Hit");
        animator.SetBool("Sprint", false);
        animator.SetBool("Run", false);
        animator.SetBool("Idle", false);
    }

    private void Dash(string isDash)
    {
        animator.CrossFade("SlideDash", 0f);
        animator.SetBool("Sprint", false);
        animator.SetBool("Run", false);
        animator.SetBool("Idle", false);
    }

    private void Sprint(bool sprint)
    {
        if (player.IsHit || player.IsCriticalHit || player.IsRivive)
        {
            animator.SetBool("Sprint", false);
            return;
        }
        animator.SetBool("Sprint", sprint);
    }

    private void Walk(bool Iswalk)
    {
        if (player.IsHit || player.IsCriticalHit || player.IsRivive)
        {
            animator.SetBool("Run", false);
            return;
        }
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
            animator.ResetTrigger("DashAttack");
        }
    }

    private void AttackCombo(string attack)
    {
        if (player.IsHit || player.IsCriticalHit || player.IsRivive)
        {
            return;
        }
        animator.SetTrigger(attack);
    }
}