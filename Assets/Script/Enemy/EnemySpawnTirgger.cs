using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public Transform enemySpawnPosition;
}

[System.Serializable]
public class EnemySpawnTimeInfo
{
    public float spawnTimer;
    public GameObject enemyPrefab;
    public Transform enemySpawnPosition;
}

[System.Serializable]
public class EnemySpawnCountInfo
{
    public float lessCount;
    public GameObject enemyPrefab;
    public Transform enemySpawnPosition;
}

public class EnemySpawnTirgger : MonoBehaviour
{    
    [SerializeField] private List<EnemySpawnInfo> enemySpawnInfoList;
    [SerializeField][Header("進入戰鬥, 時間超過 SpawnTimer 則生成怪物")] private List<EnemySpawnTimeInfo> enemySpawnTimeInfoList;
    [SerializeField][Header("戰鬥時, 怪物少於等於 lessCount 則生成怪物")] private List<EnemySpawnCountInfo> enemySpawnCountInfoList;

    private Collider triggerCollider;

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            StartSpawnEnemy();
        }
    }
    
    // 生成怪物
    public void StartSpawnEnemy()
    {
        //生成怪物
        foreach(EnemySpawnInfo enemySpawnInfo in enemySpawnInfoList)
        {
            EnemyManager.Instance.SpawnEnemy(enemySpawnInfo.enemyPrefab, enemySpawnInfo.enemySpawnPosition);
        }

        triggerCollider.enabled = false;
        
        StartCoroutine(CheckEnemySpawnList());
    }

    // 檢查根據 時間, 場上怪物數量 條件來生成的 怪物
    private IEnumerator CheckEnemySpawnList()
    {
        yield return null;

        float timer = 0f;

        while(enemySpawnCountInfoList.Count != 0 || enemySpawnTimeInfoList.Count != 0)
        {
            timer += Time.deltaTime;

            // 移除符合生成條件(時間)的資料, 並將對應怪物生成
            enemySpawnTimeInfoList.RemoveAll(enemySpawnTimeInfo => 
            {
                if (enemySpawnTimeInfo.spawnTimer < timer)
                {
                    EnemyManager.Instance.SpawnEnemy(enemySpawnTimeInfo.enemyPrefab, enemySpawnTimeInfo.enemySpawnPosition);
                    return true;
                }
                return false;
            });

            // 移除符合生成條件(場上數量)的資料, 並將對應怪物生成
            enemySpawnCountInfoList.RemoveAll(EnemySpawnCountInfo => 
            {
                if (EnemySpawnCountInfo.lessCount >= EnemyManager.Instance.GetEnemyControllerCount())
                {
                    EnemyManager.Instance.SpawnEnemy(EnemySpawnCountInfo.enemyPrefab, EnemySpawnCountInfo.enemySpawnPosition);
                    return true;
                }
                return false;
            });

            yield return null;
        }
    }
}
