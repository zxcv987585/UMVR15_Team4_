using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShootParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem shootParticle; // 指派你的粒子特效
    [SerializeField] private GameObject acitSplashPrefab; // 酸液的 Prefab
    [SerializeField] private float flightTime = 1.5f;       // 粒子飛行時間
    //[SerializeField] private float acitSplashLifeTime = 4f;

    private Transform playerTransform;     // 玩家目標位置

    private void OnEnable()
    {
        playerTransform = FindObjectOfType<PlayerController>()?.transform;
        PalabolaParticleToTarget(shootParticle, transform.position, playerTransform.position, 2.0f);
    }

    /// <summary>
    /// 將粒子特效以拋射物的方式扔到指定地點
    /// </summary>
    /// <param name="particleSystem"> 要被拋射的粒子特效 </param>
    /// <param name="startPosition"> 拋射的起點 </param>
    /// <param name="targetPosition"> 拋射的終點 </param>
    /// <param name="flightTime"> 從起點到終點需要飛行的時間 </param>
    private void PalabolaParticleToTarget(ParticleSystem particleSystem, Vector3 startPosition, Vector3 targetPosition, float flightTime)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = flightTime + 0.5f; // 壽命多留一點
        main.startSpeed = 0f; // 交給 VelocityOverLifetime 控制速度
        main.gravityModifier = 1f; // 看需求調整重力倍數

        Vector3 displacement = targetPosition - startPosition;
        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
        float horizontalDistance = horizontalDisplacement.magnitude;

        float gravity = Mathf.Abs(Physics.gravity.y); // 一般是 9.81
        float verticalDisplacement = displacement.y;

        // 計算水平速度
        Vector3 horizontalDirection = horizontalDisplacement.normalized;
        float horizontalSpeed = horizontalDistance / flightTime;

        // 計算垂直速度
        // 等加速度運動公式：v = (Δy / t) + (0.5 * g * t)
        float verticalSpeed = (verticalDisplacement / flightTime) + (0.5f * gravity * flightTime);

        // 最終速度合併
        Vector3 velocity = horizontalDirection * horizontalSpeed + Vector3.up * verticalSpeed;

        // 套用速度到 VelocityOverLifetime
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(velocity.x);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(velocity.y);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(velocity.z);

        // 放置位置
        particleSystem.transform.position = startPosition;

        // 播放特效
        particleSystem.Play();

        // 設置酸液地板的位置並啟用
        StartCoroutine(InstantiateAcitSplash(targetPosition));
    }

    // 顯示飛彈落地的酸液地板
    private IEnumerator InstantiateAcitSplash(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(flightTime);

        //targetPosition.y = 0.2f;
        Instantiate(acitSplashPrefab, targetPosition, Quaternion.identity);
    }
}
