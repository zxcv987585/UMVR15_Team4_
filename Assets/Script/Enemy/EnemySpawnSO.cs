using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
// public class EnemySpawnInfo
// {
//     //public int spawnCount;
//     public GameObject enemyPrefab;
//     public Transform enemySpawnPosition;
// }

[CreateAssetMenu(menuName = "ScriptableObject/EnemySpawnSO")]
public class EnemySpawnSO : ScriptableObject
{
    // public Vector3 spawnPosition;
    // public GameObject spawnGameobject;
    public EnemySpawnInfo[] enemySpawnInfoList;
}
