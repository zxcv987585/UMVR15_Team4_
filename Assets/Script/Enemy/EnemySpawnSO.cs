using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int spawnCount;
}

public class EnemySpawnSO : ScriptableObject
{
    public Vector3 spawnPosition;
    public EnemySpawnInfo[] enemySpawnInfoList;
}
