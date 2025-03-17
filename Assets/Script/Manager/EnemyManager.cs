using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance {get; private set;}

    [SerializeField] private List<GameObject> enemyPrefabList;
    private Dictionary<GameObject, Queue<GameObject>> enemyObjectPool;

    private List<IEnemy> enemyList;

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
        enemyList = new List<IEnemy>();

        // 初始化怪物的物件池
        enemyObjectPool = new Dictionary<GameObject, Queue<GameObject>>();
        foreach(GameObject enemyPrefab in enemyPrefabList)
        {
            enemyObjectPool.Add(enemyPrefab, new Queue<GameObject>());
        }
    }

    // 由 EnemyManager 統一管控所有怪物的 Update
    private void Update()
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].EnemyUpdate();
        }
    }
    
    /// <summary>
    /// 讓所有怪物暫停活動
    /// </summary>
    /// <param name="pauseTime"> 讓怪物暫停的時間 </param>
    public void SetEnemyIsPause(float pauseTime)
    {
        StartCoroutine(PauseCoroutine(pauseTime));
    }
    
    // 暫停用的協成, 根據 PauseTime 來決定暫停幾秒
    private IEnumerator PauseCoroutine(float pauseTime)
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].SetIsPause(true);
        }
        
        yield return new WaitForSeconds(pauseTime);
        
        for(int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].SetIsPause(false);
        }
    }
    
    /// <summary>
    /// 給予目前所有活著的怪物傷害
    /// </summary>
    /// <param name="damage"> 傷害值 </param>
    public void TakeAllEnemyDamage(float damage)
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].Health.TakeDamage(damage);
        }
    }

    /// <summary>
    /// 將 IEnemy 類別, 加入 EnemyManger 管理的更新列表
    /// </summary>
    /// <param name="enemyController"></param>
    public void AddToUpdateList(IEnemy enemy)
    {
        enemyList.Add(enemy);
    }
    
    /// <summary>
    /// 將 IEnemy 類別, 移除 EnemyManger 管理的更新列表
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveToUpdateList(IEnemy enemy)
    {
        enemyList.Remove(enemy);
    }

    // 取得目前活動中的怪物數量
    public int GetEnemyControllerCount()
    {
        return enemyList.Count;
    }

    /// <summary>
    /// 生成 Enemy 物件
    /// </summary>
    /// <param name="spawnPrefab"> 生成的物件 Prefab </param>
    /// <param name="spawnTransform"> 生成的物件 Transform 位置 </param>
    public void SpawnEnemy(GameObject spawnPrefab, Transform spawnTransform, float spawnDissolveTime = 1f)
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
        enemyGameObject.GetComponent<EnemyController>()?.SetDissolveTime(spawnDissolveTime);
        enemyGameObject.SetActive(true);
    }

    /// <summary>
    /// 回收怪物物件
    /// </summary>
    /// <param name="enemyPrefab"> 物件的 GameObject </param>
    public void RecycleEnemy(GameObject enemyPrefab)
    {
        enemyPrefab.SetActive(false);
        RemoveToUpdateList(enemyPrefab.GetComponent<IEnemy>());

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
