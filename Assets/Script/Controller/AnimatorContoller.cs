using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorContoller : MonoBehaviour
{
    Animator animator;
    PlayerController playerController;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.fightState.AttackCombo += AttackCombo;
            playerController.fightState.isAttacking += AttackReset;
            playerController.idleState.IsIdle += Idle;
            playerController.moveState.IsMoving += Walk;
            playerController.moveState.IsRun += Sprint;
            playerController.dashState.Dash += Dash;
            playerController.dashState.DashReset += Dashreset;
            playerController.aimState.OnAim += Aim;
            playerController.aimState.OnAimMove += AimMove;
            playerController.OnHit += Hit;
        }
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
        }
    }

    private void AttackReset(bool isAttack)
    {
        if (!isAttack)
        {
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
        }
    }

    private void AttackCombo(string attack)
    {
        animator.SetTrigger(attack);
    }
}