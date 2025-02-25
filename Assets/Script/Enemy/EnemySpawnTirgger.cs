using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public Transform enemySpawnPosition;
}

public class EnemySpawnTirgger : MonoBehaviour
{
    //[SerializeField] private EnemySpawnSO enemySpawnSO;
    [SerializeField] private List<EnemySpawnInfo> enemySpawnInfoList;
    private bool hasSpawn = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!hasSpawn && other.GetComponent<PlayerController>() != null)
        {
            //生成怪物
            foreach(EnemySpawnInfo enemySpawnInfo in enemySpawnInfoList)
            {
                EnemyManager.Instance.SpawnEnemy(enemySpawnInfo.enemyPrefab, enemySpawnInfo.enemySpawnPosition);
            }

            hasSpawn = true;
        }
    }
}
