using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackOverlapSphere : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private ParticleSystem bombParticle;
    [SerializeField] private Renderer bombRenderer;
    [SerializeField] private float damage;
    [SerializeField] private float bombTimer = 3f;
    public event Action OnAttackHit;

    private bool _hasInit = false;
    private bool _hasAttack;
    private const string PLAYER = "Player";
    private const string EMISSION_COLOR = "_EmissionColor";

    private void OnEnable()
    {
        if(!_hasInit)
        {
            Init();
        }

        _hasAttack = false;
        StartAttack();
    }

    private void Init()
    {
        enemyController.Health.OnDead += BombAttack;
        _hasInit = true;
    }

    public void ResetHasAttack()
    {
        
    }

    public void StartAttack()
    {
        if(!_hasAttack)
        {
            this.PlaySound("EggyAttack");
            StartCoroutine(DelayBombAttack(bombTimer/2));
            _hasAttack = true;
        }
    }
    
    private IEnumerator DelayBombAttack(float delayBombTime)
    {
        Material material = bombRenderer.material;
        float timer = 0f;
    
        while(timer < delayBombTime)
        {
            yield return new WaitUntil(() => !enemyController.IsPause);
            
            if(enemyController.Health.IsDead)
            {
                this.StopSound("EggyAttack");
                break;
            }
            
            // 計算進度 (0 ~ 1)
            float progress = timer / delayBombTime;

            // 頻率隨著時間增加（例如初始 1Hz，到最後 10Hz）
            float frequency = Mathf.Lerp(1f, 10f, progress);

            // 計算閃爍 (sin 波動)
            float emissionStrength = (Mathf.Sin(Time.time * frequency * Mathf.PI * 2) + 1f) / 2f;
            
            material.SetColor(EMISSION_COLOR, Color.red * emissionStrength);
            
            timer += Time.deltaTime;
        
            yield return null;
        }
        
        if(!enemyController.Health.IsDead)
        {
            material.SetColor(EMISSION_COLOR, Color.red);
            enemyController.Health.TakeDamage(999);
        }
    }
    
    private void BombAttack()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask(PLAYER));
        foreach(Collider collider in colliderArray)
        {
            if(collider.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.CriticalDamage(damage);
            }
        }
        bombParticle.Play();
    }
}
