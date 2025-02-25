using System;
using System.Collections;
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

    private PlayerController playerController;

    public Transform weaponHolder;
    private GameObject currentWeapon;
    public Transform attackPoint;

    //取得敵人layer層
    public LayerMask Enemy;
    //每次傷害量（可能會換地方傳入傷害）
    public float attackDamage = 25f;
    //攻擊範圍
    private float attackRadius = 1.5f;

    public List<WeaponData> WeaponPrefabs;
    public WeaponType defualtWeapon = WeaponType.Katana;
    //紀錄切換前的武器
    private WeaponType previousWeaponType;
    //紀錄當前的武器
    private WeaponType currentWeaponType;

    private void Awake()
    {
        EquipWeapon(defualtWeapon);
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        attackPoint = currentWeapon.transform.Find("AttackPoint");
        if (attackPoint == null)
        {
            Debug.Log("這裡是weaponManager，找不到武器");
        }
        if (playerController.isAttack)
        {
            Invoke("Attack", 0.45f);
        }
    }

    public void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, Enemy);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"擊中 {enemy.name}");
            enemy.GetComponent<Health>().TakeDamage(attackDamage);
        }
    }

    public void EquipWeapon(WeaponType weaponType)
    {
        if (weaponType == WeaponType.None) return;
        if (currentWeaponType == weaponType) return;

        WeaponData weaponData = WeaponPrefabs.Find(w => w.type == weaponType);
        if (weaponData == null || weaponData.prefab == null)
        {
            Debug.Log($"武器{weaponType}位在列表內設定！");
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