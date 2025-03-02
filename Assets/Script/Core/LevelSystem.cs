using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public PlayerDataSO playerData;

    //通知全域管理器玩家已升級（如果有需要的話）
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
            PlayerLevelup?.Invoke();
            Debug.Log("玩家已升級!");
        }
    }
}
