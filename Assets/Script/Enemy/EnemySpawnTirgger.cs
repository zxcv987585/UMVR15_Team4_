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
    private bool hasSpawn = false;

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }

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
            triggerCollider.enabled = false;
        }

        StartCoroutine(CheckEnemySpawnList());
    }

    private IEnumerator CheckEnemySpawnList()
    {
        yield return null;

        float timer = 0f;

        while(enemySpawnCountInfoList.Count != 0 || enemySpawnTimeInfoList.Count != 0)
        {
            timer += Time.deltaTime;

            enemySpawnTimeInfoList.RemoveAll(enemySpawnTimeInfo => 
            {
                if (enemySpawnTimeInfo.spawnTimer < timer)
                {
                    EnemyManager.Instance.SpawnEnemy(enemySpawnTimeInfo.enemyPrefab, enemySpawnTimeInfo.enemySpawnPosition);
                    return true;
                }
                return false;
            });

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
