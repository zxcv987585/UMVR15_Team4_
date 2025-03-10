using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance {get; private set;}

    [SerializeField] private List<GameObject> enemyPrefabList;
    private Dictionary<GameObject, Queue<GameObject>> enemyObjectPool;

    private List<EnemyController> enemyControllerList;

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
        enemyControllerList = new List<EnemyController>();

        // 初始化怪物的物件池
        enemyObjectPool = new Dictionary<GameObject, Queue<GameObject>>();

        foreach(GameObject enemyPrefab in enemyPrefabList)
        {
            GameObject go = Instantiate(enemyPrefab);
            go.SetActive(false);
            //Debug.Log(" EnemyManager/Start Prefab key = " + enemyPrefabKey);
            enemyObjectPool.Add(go, new Queue<GameObject>());
        }
    }

    // 由 EnemyManager 統一管控所有怪物的 Update
    private void Update()
    {
        for(int i = 0; i < enemyControllerList.Count; i++)
        {
            enemyControllerList[i].EnemyUpdate();
        }
    }

    /// <summary>
    /// 將 EnemyController 類別, 加入 EnemyManger 管理的更新列表
    /// </summary>
    /// <param name="enemyController"></param>
    public void AddToUpdateList(EnemyController enemyController)
    {
        enemyControllerList.Add(enemyController);
    }

    public int GetEnemyControllerCount()
    {
        return enemyControllerList.Count;
    }

    /// <summary>
    /// 生成 Enemy 物件
    /// </summary>
    /// <param name="spawnPrefab"> 生成的物件 Prefab </param>
    /// <param name="spawnTransform"> 生成的物件 Transform 位置 </param>
    public void SpawnEnemy(GameObject spawnPrefab, Transform spawnTransform)
    {
        //Debug.Log(" EnemyManager/SpawnEnemy Prefab key = " + spawnPrefabKey);

        Debug.Log("SpawnPrefab = " + spawnPrefab);
        foreach(var data in enemyObjectPool)
        {
            Debug.Log(data.Key);
        }

        if(enemyObjectPool.TryGetValue(spawnPrefab, out Queue<GameObject> enemyprefabQueue))
        {
            if(enemyprefabQueue.Count > 0)
            {
                GameObject enemyGameObject = enemyprefabQueue.Dequeue();
                enemyGameObject.transform.position = spawnTransform.position;
                enemyGameObject.transform.rotation = spawnTransform.rotation;
                enemyGameObject.GetComponent<EnemyController>().Init();

                Debug.Log("對應 Enemy 物件池足夠, 取出並放置");
            }
            else
            {
                GameObject enemyGameObject = Instantiate(spawnPrefab);
                enemyGameObject.transform.position = spawnTransform.position;
                enemyGameObject.transform.rotation = spawnTransform.rotation;

                Debug.Log("對應 Enemy 物件池不足, 產生新物件");
            }
        }
        else
        {
            Debug.Log(" EnemyManager/SpawnEnemy 該物件沒有對應的物件池");
            
            GameObject enemyGameObject = Instantiate(spawnPrefab);
            enemyGameObject.transform.position = spawnTransform.position;
            enemyGameObject.transform.rotation = spawnTransform.rotation;

            enemyObjectPool.Add(enemyGameObject, new Queue<GameObject>());
        }
    }

    /// <summary>
    /// 回收怪物物件
    /// </summary>
    /// <param name="enemyPrefab"> 物件的 GameObject </param>
    public void RecycleEnemy(GameObject enemyPrefab)
    {
        enemyControllerList.Remove(enemyPrefab.GetComponent<EnemyController>());

        //Debug.Log(" EnemyManager/RecycleEnemy Prefab key = " + enemyPrefabKey);

        if(enemyObjectPool.TryGetValue(enemyPrefab, out Queue<GameObject> enemyPrefabQueue))
        {
            enemyPrefabQueue.Enqueue(enemyPrefab);
        }
        else
        {
            Debug.Log(" EnemyManager/RecycleEnemy 找不到對應的物件池, 生成新的對應物件池");
            
            enemyObjectPool.Add(enemyPrefab, new Queue<GameObject>());
        }
    }
}
