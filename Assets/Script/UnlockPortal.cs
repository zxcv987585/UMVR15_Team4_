using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPortal : MonoBehaviour
{
    [SerializeField] private GameObject _enableObject;
    [SerializeField] private EnemySpawnTirgger _enemySpawnTirgger;

    private bool _isEnemyClear = false;
    private bool _isTakeFinish = false;

    private int _hideUnlockCount = 0;

    private void Start()
    {
        if(_enemySpawnTirgger != null)
            _enemySpawnTirgger.OnEnemyClear += OpenIsEnemyClear;
    }

    private void OpenIsEnemyClear()
    {
        _isEnemyClear = true;
        TryOpenPortal();
    }

    public void OpenIsTakeFinish()
    {
        _isTakeFinish = true;
        TryOpenPortal();
    }

    private void TryOpenPortal()
    {
        if(_isEnemyClear && _isTakeFinish)
        {
            _enableObject.SetActive(true);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            _hideUnlockCount++;
            if(_hideUnlockCount >= 10)
            {
                _enableObject.SetActive(true);
            }
        }
    }
}
