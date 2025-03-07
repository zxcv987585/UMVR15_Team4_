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
    [Header("最大PP量")]
    [SerializeField] float MaxPP;
    [Header("當前PP量")]
    [SerializeField] float CurrentPP;
    [Header("玩家特效")]
    [Tooltip("玩家治療時的特效")]
    [SerializeField] GameObject HealEffect;
    [Tooltip("玩家突擊時的特效")]
    [SerializeField] GameObject Spike;
    [SerializeField] LevelSystem levelSystem;

    private PlayerController player;

    //受到攻擊時要觸發的委派事件
    public event Action OnDamage;

    //玩家死亡時要觸發的委派事件
    public event Action OnDead;

    //敵人死亡時要觸發的委派事件
    public event Action<Transform> EnemyDead;

    //計算短時間內受到多少次傷害，如果短時間內受到多次傷害就給予無敵緩衝時間
    private float DamageCount;
    //紀錄上一次受到傷害的時間
    private float LastDamageTime;
    //多久沒受到傷害就重新計算受傷無敵
    private float ResetDamageTime = 1.5f;

    private bool Isdead = false;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        levelSystem = GetComponent<LevelSystem>();
        levelSystem.PlayerLevelup += NewMaxHealth;
    }

    private void LateUpdate()
    {
        if(DamageCount >= 3)
        {
            StartCoroutine(ResetDamageCount());
        }
        else if(Time.time - LastDamageTime >= ResetDamageTime)
        {
            DamageCount = 0;
        }
    }

    //升級時提昇最大血量
    private void NewMaxHealth()
    {
        this.MaxHealth += 10;
    }

    //初始化最大血量
    public void SetMaxHealth(float maxHealth)
    {
        this.MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    //初始化最大PP值
    public void SetMaxPP(float maxPP)
    {
        this.MaxPP = maxPP;
        CurrentPP = MaxPP;
    }

    //取得當前血量
    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }

    //取得當前PP值
    public float GetCurrentPP()
    {
        return CurrentPP;
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
        if (DamageCount == 3 || Isdead || player.Invincible) return;

        Debug.Log($"受到共{damage}傷害！剩餘血量：{CurrentHealth}");
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        DamageCount++;
        LastDamageTime += Time.time;

        if (CurrentHealth > 0)
        {
            OnDamage?.Invoke();
        }

        if (CurrentHealth <= 0)
        {
            HeadleDeath();
        }
    }

    private IEnumerator ResetDamageCount()
    {
        yield return new WaitForSeconds(1f);
        DamageCount = 0;
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

    //重生系統，用於玩家使用復活道具時使用
    public void Rivive()
    {
        Debug.Log("玩家已復活！");

        CurrentHealth = MaxHealth;
        Isdead = false;
    }
}
