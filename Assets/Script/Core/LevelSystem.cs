using System;
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
            playerData.MaxPP += 10;
            playerData.MaxHealth += 10;
            playerData.GunDamage += 3;
            PlayerLevelup.Invoke();
        }
    }
}
