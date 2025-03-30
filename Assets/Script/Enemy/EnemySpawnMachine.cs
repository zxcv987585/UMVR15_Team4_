using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnMachine : MonoBehaviour
{
    [SerializeField][Header("最多只會生幾隻怪")] private int _canSpawnAmount;
    [SerializeField][Header("場上最大怪物數量")] private int _allowSceneEnemyAmount;
    [SerializeField][Header("每幾秒生一隻怪")] private float _spawnTimer;

    [SerializeField] private EnemyDataSO _enemyDataSO;
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private GameObject _lockWallPrefab;
    [SerializeField] private Transform[] _spawnTransformArray;
    [SerializeField] private ParticleSystem _bombParticleSystem;
    [SerializeField] private ParticleSystem _bombParticleSystem2;
    [SerializeField] private EnemySpawnRaycast _enemySpawnRaycast;

    [SerializeField] private Collider _triggerCollider;
    private Coroutine _spawnCoroutine;
    private Health _health;
    private EnemyController _enemyController;

    private void Start()
    {
        _bombParticleSystem.Stop();
        _bombParticleSystem2.gameObject.SetActive(false);
        
        // 設定血量
        StartCoroutine(CheckTriggerIsEnter());
    }

    private IEnumerator CheckTriggerIsEnter()
    {
        while(_triggerCollider.enabled)
        {
            yield return null;
        }

        _spawnCoroutine = StartCoroutine(SpawnEnemyCoroutine());
        
        _health = GetComponent<Health>();
        _health.SetMaxHealth(_enemyDataSO.maxHP);
        _health.OnDamage += DamageHandle;
        _health.OnDead += DeadHandle;
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        Transform spawnTransform;

        int spawnCount = 0;

        while(true)
        {
            // 在場上數量少於一定數量才繼續生怪
            yield return new WaitUntil(() => EnemyManager.Instance.GetEnemyControllerCount() < _allowSceneEnemyAmount);

            spawnTransform =  _spawnTransformArray[Random.Range(0, _spawnTransformArray.Length)];
            _enemySpawnRaycast.SetEndPoint(spawnTransform, _spawnTimer);
            _enemyController = EnemyManager.Instance.SpawnEnemy(_spawnPrefab, spawnTransform, _spawnTimer).GetComponent<EnemyController>();

            // 當該物件生超過一定次數, 就不在繼續生成怪物
            spawnCount++;
            if(spawnCount > _canSpawnAmount) break;

            yield return new WaitForSeconds(_spawnTimer);
        }
    }

    private void DamageHandle()
    {
        this.PlaySound("SpawnMachineDamage");

        BattleUIManager.Instance.ShowDamageText(transform.position + Vector3.up * 2f, _health.LastDamage);
    }

    private void DeadHandle()
    {
        this.PlaySound("EggyDead");

        if(_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
        
        _lockWallPrefab?.SetActive(false);
        
        _enemySpawnRaycast.StopRaycast();

        StartCoroutine(DeadCoroutine());
    }

    private IEnumerator DeadCoroutine()
    {
        Transform childTransform = transform.Find("Ammo_Shocks");
        GameObject childObject = childTransform != null ? childTransform.gameObject : null;

        if (_enemyController != null && _enemyController.gameObject.activeSelf)
            _enemyController.StopRaycastSpawnCoroutine();
    
        _bombParticleSystem.Play();
        _bombParticleSystem2.gameObject.SetActive(true);
        _bombParticleSystem2.Play();
        
        yield return new WaitForSeconds(0.7f);
        if(childObject != null) childObject.SetActive(false);
        
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
