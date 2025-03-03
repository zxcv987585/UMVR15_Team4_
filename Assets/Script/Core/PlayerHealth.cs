using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("最大血量")]
    [SerializeField] float MaxHealth;
    [Header("當前血量")]
    [SerializeField] float CurrentHealth;
    [Header("玩家特效")]
    [Tooltip("玩家治療時的特效")]
    [SerializeField] GameObject HealEffect;
    [SerializeField] LevelSystem levelSystem;

    //受到攻擊時要觸發的委派事件
    public event Action OnDamage;

    //玩家死亡時要觸發的委派事件
    public event Action OnDead;

    //敵人死亡時要觸發的委派事件
    public event Action<Transform> EnemyDead;

    private float lastDamage;
    public float LastDamage
    {
        get { return lastDamage; }
        private set { lastDamage = value; }
    }


    private bool Isdead = false;

    private void Awake()
    {
        levelSystem = GetComponent<LevelSystem>();
        levelSystem.PlayerLevelup += NewMaxHealth;
    }

    private void NewMaxHealth()
    {
        this.MaxHealth += 10;
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    //取得當前血量
    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }

    //取得最大血量，主要用於定義當前血量最大值
    public float GetMaxHealth()
    {
        return MaxHealth;
    }

    //取得當前血量以及最大血量的比例，主要用於UI血條，可用可不用
    public float GetHealthRatio()
    {
        return CurrentHealth / MaxHealth;
    }

    //確認玩家是否為死亡狀態
    public bool IsDead()
    {
        return Isdead;
    }

    //受傷函式，用於傳入傷害
    public void TakeDamage(float damage)
    {
        if (Isdead) return;

        Debug.Log($"受到共{damage}傷害！剩餘血量：{CurrentHealth}");
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        lastDamage = damage;

        if (CurrentHealth > 0)
        {
            OnDamage?.Invoke();
        }

        if (CurrentHealth <= 0)
        {
            HeadleDeath();
        }
    }

    //死亡後傳送訂閱給各大系統
    private void HeadleDeath()
    {
        if (Isdead) return;

        if (CurrentHealth <= 0)
        {
            Isdead = true;
            OnDead?.Invoke();
            EnemyDead?.Invoke(transform);
        }
    }

    //回血系統，呼叫後傳入數值來加血
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);

        if (HealEffect != null)
        {
            GameObject healEffect = Instantiate(HealEffect, transform.position + Vector3.up * 1f, Quaternion.identity);
            healEffect.transform.SetParent(transform);
        }
        else
        {
            Debug.Log("沒有治癒特效可用");
        }
    }
}
