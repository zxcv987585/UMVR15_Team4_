using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public PlayerDataSO playerData;

    public event Action PlayerLevelup;

    private void LateUpdate()
    {
        LevelUp();
    }

    public void LevelUp()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            playerData.attackDamage += 3;
            playerData.MaxHealth += 10;
            playerData.GunDamage += 2;
            PlayerLevelup.Invoke();
            Debug.Log("äﬂâ∆õﬂè°ãâ!");
        }
    }
}
