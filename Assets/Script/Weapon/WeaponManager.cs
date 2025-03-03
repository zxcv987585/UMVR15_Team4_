using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Transform weaponHolder;
    private GameObject currentWeapon;
    private Transform attackPoint;

    public List<WeaponData> WeaponPrefabs;
    public WeaponType defualtWeapon = WeaponType.Katana;
    //紀錄切換前的武器
    private WeaponType previousWeaponType;
    //紀錄當前的武器
    private WeaponType currentWeaponType;

    private void Awake()
    {
        EquipWeapon(defualtWeapon);
        player = GetComponent<PlayerController>();
        player.fightState.SwordSlash += SpawnSwordSlash;
    }

    private void Update()
    {
        attackPoint = currentWeapon.transform.Find("AttackPoint");
        if (attackPoint == null)
        {
            Debug.Log("這裡是weaponManager，找不到武器");
        }
    }

    private void SpawnSwordSlash()
    {
        if (player.SwordSlash != null && attackPoint != null)
        {
            StartCoroutine(SwordSlshSpawmTime());
            GameObject SwordEffect = GameObject.Instantiate(player.SwordSlash, attackPoint.position, attackPoint.rotation);
        
            SwordEffect.transform.position = player.transform.position;
        }
    }
    IEnumerator SwordSlshSpawmTime()
    {
        yield return new WaitForSeconds(0.2f);
    }

    public void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, player.playerData.attackRadius, player.playerData.EnemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"擊中 {enemy.name}");
            enemy.GetComponent<Health>().TakeDamage(player.playerData.attackDamage);
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