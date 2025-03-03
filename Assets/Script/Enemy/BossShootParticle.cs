using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShootParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem shootParticle; // 指派你的粒子特效
    [SerializeField] private Transform playerTransform;     // 玩家目標位置
    [SerializeField] private float flightTime = 1.5f;       // 粒子飛行時間

    private float gravity = 9.81f;

    private void OnEnable()
    {
        //ShootAtPlayer();

        LaunchParticleToTarget(
        shootParticle,      // 特效
        transform.position,      // 發射點
        playerTransform.position, // 目標位置
        2.0f);                    // 飛行時間 (秒)
    }

    public void ShootAtPlayer()
    {
        Vector3 startPosition = shootParticle.transform.position;
        Vector3 targetPosition = playerTransform.position;

        Debug.Log("startPosition = " + startPosition);
        Debug.Log("targetPosition = " + targetPosition);

        float gravity = Mathf.Abs(Physics.gravity.y);
        

        Vector3 horizontalDisplacement = new Vector3(
            targetPosition.x - startPosition.x,
            0f,
            targetPosition.z - startPosition.z
        );

        Vector3 horizontalVelocity = horizontalDisplacement / flightTime;

        float verticalDisplacement = targetPosition.y - startPosition.y;
        if (verticalDisplacement < -100f) verticalDisplacement = -100f; // 避免太誇張的高度差
        //float verticalVelocity = (verticalDisplacement - 0.5f * gravity * flightTime * flightTime) / flightTime;
        float verticalVelocity = (verticalDisplacement / flightTime) + (0.5f * gravity * flightTime);


        var main = shootParticle.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = flightTime + 0.5f;
        main.gravityModifier = 1f; // 使用場景重力

        var velocityOverLifetime = shootParticle.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(horizontalVelocity.x);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(verticalVelocity);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(horizontalVelocity.z);

        shootParticle.Play();
    }

    public void LaunchParticleToTarget(ParticleSystem particleSystem, Vector3 startPosition, Vector3 targetPosition, float flightTime)
    {
        var main = particleSystem.main;
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
        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(velocity.x);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(velocity.y);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(velocity.z);

        // 放置位置
        particleSystem.transform.position = startPosition;

        // 播放特效
        particleSystem.Play();
    }
}
