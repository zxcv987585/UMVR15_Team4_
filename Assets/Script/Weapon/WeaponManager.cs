using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public WeaponManager.WeaponType type;
    public GameObject prefab;
}

public class WeaponManager : MonoBehaviour
{
    public enum WeaponType { None, Katana, Gun };

    private PlayerController player;
    private PlayerHealth health;
    private CameraController Maincamera;

    public Transform weaponHolder;
    private GameObject currentWeapon;
    private Transform attackPoint;
    [Tooltip("玩家揮劍時的特效 1")]
    public GameObject SwordSlash1;
    [Tooltip("玩家揮劍時的特效 2")]
    public GameObject SwordSlash2;
    [Tooltip("玩家揮劍時的特效 3")]
    public GameObject SwordSlash3;
    [Tooltip("玩家揮劍時的特效 4")]
    public GameObject SwordSlash4;
    [Tooltip("玩家揮劍時的特效 突進")]
    public GameObject SwordSlashForword;
    [Tooltip("Slash生成點")]
    public Transform SlashPoint;

    public List<WeaponData> WeaponPrefabs;
    public WeaponType defualtWeapon = WeaponType.Katana;
    // 紀錄切換前的武器
    private WeaponType previousWeaponType;
    // 紀錄當前的武器
    private WeaponType currentWeaponType;

    // 攻擊視窗開啟旗標
    private bool isAttackWindowActive = false;
    // 記錄在一次攻擊視窗內已經擊中的敵人
    private HashSet<Collider> attackedEnemies = new HashSet<Collider>();

    private void Awake()
    {
        EquipWeapon(defualtWeapon);
        player = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();
        Maincamera = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        // 取得攻擊點
        if (currentWeapon != null)
        {
            attackPoint = currentWeapon.transform.Find("AttackPoint");
            if (attackPoint == null)
            {
                Debug.Log("這裡是WeaponManager，找不到AttackPoint");
            }
        }

        // 在攻擊視窗內持續偵測攻擊
        if (isAttackWindowActive)
        {
            Attack();
        }

        // 如果玩家被攻擊就停止傷害判定
        if (player.IsHit || player.IsCriticalHit || player.IsDash || player.IsSkilling)
        {
            isAttackWindowActive = false;
        }
    }

    // 在動畫事件中呼叫：開始攻擊視窗
    public void StartAttackWindow()
    {
        isAttackWindowActive = true;
        // 每次攻擊開始時清空之前記錄
        attackedEnemies.Clear();
    }

    // 在動畫事件中呼叫：結束攻擊視窗
    public void EndAttackWindow()
    {
        isAttackWindowActive = false;
    }

    // 攻擊檢查：在攻擊視窗內持續檢查並對新進入的敵人施加傷害
    public void Attack()
    {
        if (attackPoint == null) return;

        int combinedLayer = player.playerData.BossLayer | player.playerData.EnemyLayer | player.playerData.BoxLayer;

        Collider[] hitTarget = Physics.OverlapSphere(attackPoint.position, player.playerData.attackRadius, combinedLayer);
        foreach (Collider Target in hitTarget)
        {
            if (!attackedEnemies.Contains(Target))
            {
                attackedEnemies.Add(Target);
                Debug.Log($"攻擊到: {Target.name}");
                Health TargetHealth = Target.GetComponent<Health>();
                if (TargetHealth != null)
                {
                    TargetHealth.TakeDamage(player.playerData.attackDamage);
                    health.AttackHealPP(1.5f);
                }
                if (player.HitEffect != null)
                {
                    Instantiate(player.HitEffect, attackPoint.position, attackPoint.rotation);
                }
            }
        }
    }


    //劍氣特效
    public void SpawnSlash1()
    {
        if (SlashPoint != null && SwordSlash1 != null)
        {
            Vector3 CurrentEuler = SwordSlash1.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash1, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlash2()
    {
        if (SlashPoint != null && SwordSlash2 != null)
        {
            Vector3 CurrentEuler = SwordSlash2.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash2, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlash3()
    {
        if (SlashPoint != null && SwordSlash3 != null)
        {
            Vector3 CurrentEuler = SwordSlash3.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash3, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlash4()
    {
        if (SlashPoint != null && SwordSlash4 != null)
        {
            Vector3 CurrentEuler = SwordSlash4.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash4, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlashForword()
    {
        if (SlashPoint != null && SwordSlashForword != null)
        {
            Vector3 CurrentEuler = SwordSlashForword.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlashForword, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }

    public void EquipWeapon(WeaponType weaponType)
    {
        if (weaponType == WeaponType.None) return;
        if (currentWeaponType == weaponType) return;

        WeaponData weaponData = WeaponPrefabs.Find(w => w.type == weaponType);
        if (weaponData == null || weaponData.prefab == null)
        {
            Debug.Log($"武器 {weaponType} 沒有正確設定！");
            return;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponData.prefab, weaponHolder);
        currentWeaponType = weaponType;
    }

    public void EquipPreviousWeapon()
    {
        EquipWeapon(previousWeaponType);
    }

    public void SwitchWeapon(WeaponType weaponType)
    {
        previousWeaponType = currentWeaponType;
        EquipWeapon(weaponType);
    }
}
