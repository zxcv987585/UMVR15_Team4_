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

    public void SpawnEnemy(GameObject spawnGameObject, Transform spawnTransform)
    {
        Instantiate(spawnGameObject,spawnTransform.position, Quaternion.identity);

        // if(enemyObjectPool.TryGetValue(spawnGameObject, out Queue<GameObject> enemyprefabQueue) != null)
        // {

        // }
    }
}
