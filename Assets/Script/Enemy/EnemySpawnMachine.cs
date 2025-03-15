using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnMachine : MonoBehaviour
{
    [SerializeField] private float _spawnTimer;
    [SerializeField] private EnemyDataSO _enemyDataSO;
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private GameObject _lockWallPrefab;
    [SerializeField] private Transform[] _spawnTransformArray;
    [SerializeField] private ParticleSystem _bombParticleSystem;
    [SerializeField] private EnemySpawnRaycast _enemySpawnRaycast;

    [SerializeField] private Collider _triggerCollider;
    private Coroutine _spawnCoroutine;
    private Health _health;

    private void Start()
    {
        // 設定血量
        _health = GetComponent<Health>();
        _health.SetMaxHealth(_enemyDataSO.maxHP);
        _health.OnDamage += DamageHandle;
        _health.OnDead += DeadHandle;

        StartCoroutine(CheckTriggerIsEnter());
    }

    private IEnumerator CheckTriggerIsEnter()
    {
        while(_triggerCollider.enabled)
        {
            yield return null;
        }

        _spawnCoroutine = StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        Transform spawnTransform;

        while(true)
        {
            spawnTransform =  _spawnTransformArray[Random.Range(0, _spawnTransformArray.Length)];
            _enemySpawnRaycast.SetEndPoint(spawnTransform);
            EnemyManager.Instance.SpawnEnemy(_spawnPrefab, spawnTransform, _spawnTimer);

            yield return new WaitForSeconds(_spawnTimer);
        }
    }

    private void DamageHandle()
    {
        BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up * 2f, _health.LastDamage);
    }

    private void DeadHandle()
    {
        StopCoroutine(_spawnCoroutine);
        _lockWallPrefab?.SetActive(false);

        StartCoroutine(DeadCoroutine());
        
    }

    private IEnumerator DeadCoroutine()
    {
        _bombParticleSystem.Play();
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }
}
