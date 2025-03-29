using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnRaycast : MonoBehaviour
{
    [SerializeField] private GameObject _delayRaycastPrefab;
    [SerializeField] private List<Transform> _spawnTransformList;

    private void OnEnable()
    {
        if(_spawnTransformList == null) return;

        float baseTime = 1.5f;
        float step = 0f;

        _spawnTransformList.ForEach(spawnTransform => {
            GameObject go = Instantiate(_delayRaycastPrefab, spawnTransform.position, spawnTransform.rotation);
            go.GetComponent<EnemyAttackDelayRaycast>().SetDelayTime(baseTime);
            baseTime += step;
        });
    }
}
