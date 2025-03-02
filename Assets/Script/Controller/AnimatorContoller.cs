using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorContoller : MonoBehaviour
{
    Animator animator;
    PlayerController player;
    Health health;

    private bool isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        health = GetComponent<Health>();

        if (player != null)
        {
            player.fightState.AttackCombo += AttackCombo;
            player.fightState.isAttacking += AttackReset;
            player.idleState.IsIdle += Idle;
            player.moveState.IsMoving += Walk;
            player.moveState.IsRun += Sprint;
            player.dashState.Dash += Dash;
            player.dashState.ForceIdle += Idle;
            player.dashState.DashReset += Dashreset;
            player.aimState.OnAim += Aim;
            player.aimState.OnAimMove += AimMove;
            player.OnHit += Hit;
            health.OnDead += Dead;
        }
    }

    private void Dead()
    {
        if (isDead) return;
        isDead = true;
        animator.SetBool("Dead", true);
        Debug.Log("���⦺�`");
        //animator.speed = 0;

        health.OnDead -= Dead;
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

    private void Hit(string Hit)
    {
        animator.ResetTrigger(Hit);
        animator.SetTrigger(Hit);
    }

    private void Dashreset(bool resetDash)
    {
        if (resetDash)
        {
            animator.ResetTrigger("Dash");
        }
    }

    private void Dash(string isDash)
    {
        animator.SetTrigger(isDash);
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