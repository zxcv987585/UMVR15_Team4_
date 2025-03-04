using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance {get; private set;}

    [SerializeField] private List<GameObject> enemyPrefabList;
    private Dictionary<int, Queue<GameObject>> enemyObjectPool;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        enemyObjectPool = new Dictionary<int, Queue<GameObject>>();

        foreach(GameObject enemyPrefab in enemyPrefabList)
        {
            int enemyPrefabKey = enemyPrefab.GetInstanceID();
            Debug.Log(" EnemyManager/Start Prefab key = " + enemyPrefabKey);
            enemyObjectPool.Add(enemyPrefabKey, new Queue<GameObject>());
        }
    }

    /// <summary>
    /// 生成 Enemy 物件
    /// </summary>
    /// <param name="spawnPrefab"> 生成的物件 Prefab </param>
    /// <param name="spawnTransform"> 生成的物件 Transform 位置 </param>
    public void SpawnEnemy(GameObject spawnPrefab, Transform spawnTransform)
    {
        int spawnPrefabKey = spawnPrefab.GetInstanceID();
        Debug.Log(" EnemyManager/SpawnEnemy Prefab key = " + spawnPrefabKey);

        if(enemyObjectPool.TryGetValue(spawnPrefabKey, out Queue<GameObject> enemyprefabQueue))
        {
            if(enemyprefabQueue.Count > 0)
            {
                GameObject enemy = enemyprefabQueue.Dequeue();
                enemy.transform.position = spawnTransform.position;
                enemy.GetComponent<EnemyController>().Init();

                Debug.Log("對應 Enemy 物件池足夠, 取出並放置");
            }
            else
            {
                GameObject enemy = Instantiate(spawnPrefab);
                enemy.transform.position = spawnTransform.position;

                Debug.Log("對應 Enemy 物件池不足, 產生新物件");
            }
        }
        else
        {
            Debug.Log(" EnemyManager/SpawnEnemy 該物件沒有對應的物件池");
        }
    }

    /// <summary>
    /// 回收怪物物件
    /// </summary>
    /// <param name="enemyPrefab"> 物件的 GameObject </param>
    public void RecycleEnemy(GameObject enemyPrefab)
    {
        int enemyPrefabKey = enemyPrefab.GetInstanceID();
        Debug.Log(" EnemyManager/RecycleEnemy Prefab key = " + enemyPrefabKey);

        if(enemyObjectPool.TryGetValue(enemyPrefabKey, out Queue<GameObject> enemyPrefabQueue))
        {
            enemyPrefabQueue.Enqueue(enemyPrefab);
        }
        else
        {
            Debug.Log(" EnemyManager/RecycleEnemy 找不到對應的物件池, 生成新的對應物件池");
            
            enemyObjectPool.Add(enemyPrefabKey, new Queue<GameObject>());
        }
    }
}
