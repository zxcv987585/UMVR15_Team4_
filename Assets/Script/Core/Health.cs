using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    
    [SerializeField][Header("最大血量")] float _maxHealth;
    [SerializeField][Header("當前血量")] float _currentHealth;

    //受到攻擊時要觸發的委派事件
    public event Action OnDamage;

    //玩家死亡時要觸發的委派事件
    public event Action OnDead;

    //敵人死亡時要觸發的委派事件
    public event Action<Transform> EnemyDead;

    public float LastDamage {get; private set;}
    
    private bool _isdead = false;
    private bool _isInvincibility = false;  // 無敵狀態

    // 設置怪物的血量上限
    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
    }

    public void SetIsInvincibility(bool isInvincibility) => _isInvincibility = isInvincibility;

    public void Init()
    {
        _isdead = false;
    }

    //取得當前血量
    public float GetCurrentHealth() => _currentHealth;
    //取得最大血量，主要用於定義當前血量最大值
    public float GetMaxHealth() => _maxHealth;
    //取得當前血量以及最大血量的比例，主要用於UI血條，可用可不用
    public float GetHealthRatio() => _currentHealth / _maxHealth;

    //確認玩家是否為死亡狀態
    public bool IsDead() => _isdead;

    //受傷函式，用於傳入傷害
    public void TakeDamage(float damage)
    {
        if (_isdead) return;

        if(_isInvincibility) return;

        //Debug.Log($"受到共{damage}傷害！剩餘血量：{_currentHealth}");
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        LastDamage = damage;

        if (_currentHealth > 0)
        {
            OnDamage?.Invoke();
        }

        if (_currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    //死亡後傳送訂閱給各大系統
    private void HandleDeath()
    {
        if (_isdead) return;

        if (_currentHealth <= 0)
        {
            _isdead = true;
            OnDead?.Invoke();
            EnemyDead?.Invoke(transform);
        }
    }

    //回血系統，呼叫後傳入數值來加血
    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
    }
}
