using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnRaycast : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;  // 雷射起點
    [SerializeField] private Transform _endPoint;    // 雷射終點
    
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private Vector3 _startVector3;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = _particleSystem.main;

        // 確保啟用 3D Start Size
        _mainModule.startSize3D = true;

        _startVector3 = _startPoint.position + Vector3.up * 2f;
    }

    public void SetEndPoint(Transform endTransform, float closeTimer)
    {
        _endPoint = endTransform;

        StartCoroutine(EndRaycastCoroutine(closeTimer-0.2f));

        if (_startPoint == null || _endPoint == null) return;

        // 計算雷射方向與長度
        Vector3 direction = _endPoint.position - _startVector3;
        float laserLength = direction.magnitude;  // 計算距離

        // 設定粒子系統長度 (改變 Y 軸)
        _mainModule.startSizeX = 0.2f;  // 固定寬度
        _mainModule.startSizeY = laserLength / 2;  // 設定雷射長度
        _mainModule.startSizeZ = 0.2f;  // 固定厚度

        // 設定雷射的中心點
        Vector3 midPoint = (_startVector3 + _endPoint.position) / 2;
        transform.position = midPoint;

        // 修正方向 (確保 Y 軸不會翻轉)
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation *= Quaternion.Euler(90, 0, 0);

        _particleSystem.Play();
    }

    private IEnumerator EndRaycastCoroutine(float closeTimer)
    {
        yield return new WaitForSeconds(closeTimer);

        _particleSystem.Stop();
    }
}
