using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (BattleUIManager.Instance != null)
        {
            BattleUIManager.Instance.ShowUI();
        }
    }
}
