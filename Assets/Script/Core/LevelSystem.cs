using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public PlayerData playerData;

    //通知全域管理器玩家已升級（如果有需要的話）
    public event Action<bool> PlayerLevelup;

    private void Update()
    {
        LevelUp();
    }

    public void LevelUp()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            playerData.attackDamage += 5;
            playerData.MaxHealth += 10;
            Debug.Log("玩家已升級!");
        }
    }
}
