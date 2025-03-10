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
            // Queue<GameObject> gameObjectQueue = new Queue<GameObject>();
            // GameObject go = Instantiate(enemyPrefab);
            // go.SetActive(false);
            // gameObjectQueue.Enqueue(go);
            enemyObjectPool.Add(enemyPrefab, new Queue<GameObject>());
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

    // 取得目前活動中的怪物數量
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
        // 如果回收的物件沒有放在物件池類, 則創建新的池
        if (!enemyObjectPool.TryGetValue(spawnPrefab, out Queue<GameObject> objectQueue))
        {
            //Debug.LogWarning("SpawnEnemy: 該物件沒有對應的物件池，正在建立新的池子...");
            enemyObjectPool[spawnPrefab] = new Queue<GameObject>();
        }

        GameObject enemyGameObject;

        if(enemyObjectPool[spawnPrefab].Count > 0)
        {
            enemyGameObject = enemyObjectPool[spawnPrefab].Dequeue();
            Debug.Log("對應 Enemy 物件池足夠, 取出並放置");
        }
        else
        {
            enemyGameObject = Instantiate(spawnPrefab);
            Debug.Log("對應 Enemy 物件池不足, 產生新物件");
        }

        enemyGameObject.transform.position = spawnTransform.position;
        enemyGameObject.transform.rotation = spawnTransform.rotation;
        enemyGameObject.SetActive(true);
        //enemyGameObject.GetComponent<EnemyController>().Init();
    }

    /// <summary>
    /// 回收怪物物件
    /// </summary>
    /// <param name="enemyPrefab"> 物件的 GameObject </param>
    public void RecycleEnemy(GameObject enemyPrefab)
    {
        enemyPrefab.SetActive(false);
        enemyControllerList.Remove(enemyPrefab.GetComponent<EnemyController>());

        foreach(var data in enemyObjectPool)
        {
            if(enemyPrefab.name.StartsWith(data.Key.name))
            {
                data.Value.Enqueue(enemyPrefab);
                return;
            }
        }

        Debug.Log(" EnemyManager/RecycleEnemy 沒找到對應的池子");
    }
}
