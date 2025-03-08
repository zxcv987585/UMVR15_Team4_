using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public WeaponManager.WeaponType type;
    public GameObject prefab;
}

public class WeaponManager : MonoBehaviour
{
    public enum WeaponType { None, Katana, Gun };

    private PlayerController player;
    private CameraController camera;

    public Transform weaponHolder;
    private GameObject currentWeapon;
    private Transform attackPoint;
    [Tooltip("玩家揮劍時的特效 1")]
    public GameObject SwordSlash1;
    [Tooltip("玩家揮劍時的特效 2")]
    public GameObject SwordSlash2;
    [Tooltip("玩家揮劍時的特效 3")]
    public GameObject SwordSlash3;
    [Tooltip("玩家揮劍時的特效 4")]
    public GameObject SwordSlash4;
    [Tooltip("玩家揮劍時的特效 突進")]
    public GameObject SwordSlashForword;
    [Tooltip("Slash生成點")]
    public Transform SlashPoint;

    public List<WeaponData> WeaponPrefabs;
    public WeaponType defualtWeapon = WeaponType.Katana;
    // 紀錄切換前的武器
    private WeaponType previousWeaponType;
    // 紀錄當前的武器
    private WeaponType currentWeaponType;

    // 攻擊視窗開啟旗標
    private bool isAttackWindowActive = false;
    // 記錄在一次攻擊視窗內已經擊中的敵人
    private HashSet<Collider> attackedEnemies = new HashSet<Collider>();
    //擊中慢動作的防呆旗標
    private bool isHitPauseActive = false;

    private void Awake()
    {
        EquipWeapon(defualtWeapon);
        player = GetComponent<PlayerController>();
        camera = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        // 取得攻擊點
        if (currentWeapon != null)
        {
            attackPoint = currentWeapon.transform.Find("AttackPoint");
            if (attackPoint == null)
            {
                Debug.Log("這裡是WeaponManager，找不到AttackPoint");
            }
        }

        // 在攻擊視窗內持續偵測攻擊
        if (isAttackWindowActive)
        {
            Attack();
        }
    }

    // 在動畫事件中呼叫：開始攻擊視窗
    public void StartAttackWindow()
    {
        isAttackWindowActive = true;
        // 每次攻擊開始時清空之前記錄
        attackedEnemies.Clear();
    }

    // 在動畫事件中呼叫：結束攻擊視窗
    public void EndAttackWindow()
    {
        isAttackWindowActive = false;
    }

    // 攻擊檢查：在攻擊視窗內持續檢查並對新進入的敵人施加傷害
    public void Attack()
    {
        if (attackPoint == null) return;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, player.playerData.attackRadius, player.playerData.EnemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            // 如果這次攻擊視窗內還沒擊中該敵人，就處理傷害
            if (!attackedEnemies.Contains(enemy))
            {
                attackedEnemies.Add(enemy);
                Debug.Log($"擊中 {enemy.name}");
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(player.playerData.attackDamage);
                }
                if (player.HitEffect != null)
                {
                    Instantiate(player.HitEffect, attackPoint.position, attackPoint.rotation);
                }
                if (!isHitPauseActive)
                {
                    StartCoroutine(HitPauseCoroutine(0.03f, 0.03f));
                }
                if(camera != null)
                {
                    camera.StartCoroutine(camera.ShakeCamera(0.3f, 2f));
                }
            }
        }
    }


    //劍氣特效
    public void SpawnSlash1()
    {
        if (SlashPoint != null && SwordSlash1 != null)
        {
            Vector3 CurrentEuler = SwordSlash1.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash1, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlash2()
    {
        if (SlashPoint != null && SwordSlash2 != null)
        {
            Vector3 CurrentEuler = SwordSlash2.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash2, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlash3()
    {
        if (SlashPoint != null && SwordSlash3 != null)
        {
            Vector3 CurrentEuler = SwordSlash3.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash3, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlash4()
    {
        if (SlashPoint != null && SwordSlash4 != null)
        {
            Vector3 CurrentEuler = SwordSlash4.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlash4, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }
    public void SpawnSlashForword()
    {
        if (SlashPoint != null && SwordSlashForword != null)
        {
            Vector3 CurrentEuler = SwordSlashForword.transform.eulerAngles;
            Quaternion offset = Quaternion.Euler(CurrentEuler);
            Instantiate(SwordSlashForword, SlashPoint.position, SlashPoint.rotation * offset);
        }
    }

    public void EquipWeapon(WeaponType weaponType)
    {
        if (weaponType == WeaponType.None) return;
        if (currentWeaponType == weaponType) return;

        WeaponData weaponData = WeaponPrefabs.Find(w => w.type == weaponType);
        if (weaponData == null || weaponData.prefab == null)
        {
            Debug.Log($"武器 {weaponType} 沒有正確設定！");
            return;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponData.prefab, weaponHolder);
        currentWeaponType = weaponType;
    }

    public void EquipPreviousWeapon()
    {
        EquipWeapon(previousWeaponType);
    }

    public void SwitchWeapon(WeaponType weaponType)
    {
        previousWeaponType = currentWeaponType;
        EquipWeapon(weaponType);
    }

    //打擊停頓效果
    private IEnumerator HitPauseCoroutine(float duration, float pauseTimeScale)
    {
        isHitPauseActive = true;

        float originalTimeScale = Time.timeScale;
        float originalFixedDeltaTime = Time.fixedDeltaTime;

        Time.timeScale = pauseTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * pauseTimeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        isHitPauseActive = false;
    }
}
