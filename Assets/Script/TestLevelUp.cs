using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelUp : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            FindObjectOfType<LevelSystem>().AddExperience(500);
            FindAnyObjectByType<PlayerHealth>().HealPP(200);
        }
    }
}
