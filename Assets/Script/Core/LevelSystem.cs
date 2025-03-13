using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public PlayerDataSO playerData;

    public event Action PlayerLevelup;

    public void AddExperience(float xp)
    {
        playerData.CurrentExp += xp;
        Debug.Log("經驗值增加：" + xp + " | 當前經驗值：" + playerData.CurrentExp + " / " + playerData.XPForNextLevel);

        while (playerData.CurrentExp >= playerData.XPForNextLevel)
        {
            playerData.CurrentExp -= playerData.XPForNextLevel;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        playerData.CurrentLevel++;

        playerData.attackDamage += 3;
        playerData.MaxPP += 10;
        playerData.MaxHealth += 10;
        playerData.GunDamage += 3;

        playerData.XPForNextLevel = Mathf.RoundToInt(playerData.XPForNextLevel * 1.2f);
        Debug.Log("升級了！當前等級：" + playerData.CurrentLevel + " | 下一級所需經驗值：" + playerData.XPForNextLevel);
        PlayerLevelup?.Invoke();
    }
}
