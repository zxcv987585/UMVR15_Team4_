using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("最大血量")]
    [SerializeField] float MaxHealth = 100f;
    [Header("當前血量")]
    [SerializeField] float CurrentHealth;

    //受到攻擊時要觸發的委派事件
    public event Action PlayerOnDamage;

    //人物死亡時要觸發的委派事件
    public event Action PlayerOnDead;

    private bool Isdead = false;

    void Start()
    {
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

        Debug.Log($"Player受到共{damage}傷害！剩餘血量：{CurrentHealth}");
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (CurrentHealth > 0)
        {
            PlayerOnDamage?.Invoke();
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
            PlayerOnDead?.Invoke();
        }
    }

    //回血系統，呼叫後傳入數值來加血
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
    }
}
