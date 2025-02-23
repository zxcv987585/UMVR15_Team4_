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

    public Transform weaponHolder;
    private GameObject currentWeapon;

    public List<WeaponData> WeaponPrefabs;
    public WeaponType defualtWeapon = WeaponType.Katana;
    //紀錄切換前的武器
    private WeaponType previousWeaponType;
    //紀錄當前的武器
    private WeaponType currentWeaponType;

    private void Awake()
    {
        EquipWeapon(defualtWeapon);
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