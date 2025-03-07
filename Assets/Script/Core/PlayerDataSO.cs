using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataSO", menuName = "ScriptableObject/PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    [Header("移動參數")]
    [Tooltip("移動速度")]
    public float MoveSpeed = 3;
    [Tooltip("奔跑加速")]
    [Range(1, 3)]
    public float SprintSpeedModifier = 1;
    [Tooltip("角色旋轉速度")]
    public float RotateSpeed = 5f;
    [Tooltip("賦予角色重力")]
    public float Gravity = 40f;
    [Tooltip("Dash的瞬間速度")]
    public float DashSpeed = 20f;
    [Tooltip("Dash持續時間")]
    public float DashDuration = 0.2f;
    [Tooltip("Dash冷卻時間")]
    public float DashCoolTime = 1f;

    [Header("角色屬性")]
    [Tooltip("玩家最大血量")]
    public float MaxHealth = 120f;
    [Tooltip("玩家最大PP值")]
    public float MaxPP = 100f;
    [Tooltip("受傷狀態的持續時間")]
    public float HitCoolTime = 1f;
    [Tooltip("玩家的傷害量")]
    public float attackDamage = 25f;
    [Tooltip("玩家的攻擊範圍")]
    public float attackRadius = 0.5f;
    [Tooltip("玩家的槍械傷害量")]
    public float GunDamage = 12f;

    [Header("鎖定系統")]
    [Tooltip("玩家鎖定視角的最大距離")]
    public float LockRange = 15f;
    [Tooltip("可以被鎖定的Layer層")]
    public LayerMask EnemyLayer;

}
