using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance {get; private set;}

    [SerializeField] private List<GameObject> enemyPrefabList;
    private Dictionary<GameObject, Queue<GameObject>> enemyObjectPool;

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
        enemyObjectPool = new Dictionary<GameObject, Queue<GameObject>>();

        foreach(GameObject enemyPrefab in enemyPrefabList)
        {
            enemyObjectPool.Add(enemyPrefab, new Queue<GameObject>());
        }
    }

    /// <summary>
    /// 生成 Enemy 物件
    /// </summary>
    /// <param name="spawnGameObject"> 生成的物件 Prefab </param>
    /// <param name="spawnTransform"> 生成的物件 Transform 位置 </param>
    public void SpawnEnemy(GameObject spawnGameObject, Transform spawnTransform)
    {
        Instantiate(spawnGameObject,spawnTransform.position, Quaternion.identity);

        if(enemyObjectPool.TryGetValue(spawnGameObject, out Queue<GameObject> enemyprefabQueue))
        {
            if(enemyprefabQueue.Count > 0)
            {
                GameObject enemy = enemyprefabQueue.Dequeue();
                enemy.transform.position = spawnTransform.position;
                enemy.GetComponent<EnemyController>().Init();
            }
            else
            {
                GameObject enemy = Instantiate(spawnGameObject);
                enemy.transform.position = spawnTransform.position;
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
    /// <param name="enemy"> 物件的 GameObject </param>
    public void RecycleEnemy(GameObject enemy)
    {
        if(enemyObjectPool.TryGetValue(enemy, out Queue<GameObject> enemyPrefabQueue))
        {
            enemyPrefabQueue.Enqueue(enemy);
        }
        else
        {
            Debug.Log(" EnemyManager/RecycleEnemy 找不到對應的物件池");
        }
    }
}
