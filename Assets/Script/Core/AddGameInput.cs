using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGameInput : MonoBehaviour
{
    private void Awake()
    {
        if (GameInput.Instance == null)
        {
            GameObject go = new GameObject("GameInput");
            go.AddComponent<GameInput>();
        }
    }
}
