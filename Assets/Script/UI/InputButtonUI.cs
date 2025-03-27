using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject _singleInputButtonUIPrefab;

    private void Start()
    {
        foreach(GameInput.Bind bind in Enum.GetValues(typeof(GameInput.Bind)))
        {
            GameObject go = Instantiate(_singleInputButtonUIPrefab, transform);
            go.GetComponent<SingleInputButtonUI>().SetBind(bind);
        }
    }
}
