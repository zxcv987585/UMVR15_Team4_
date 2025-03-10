﻿using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //宣告玩家狀態機
    public PlayerStateMachine stateMachine { get; private set; }
    //宣告玩家所有狀態
    public FightState fightState;
    public IdleState idleState;
    public MoveState moveState;
    public DashState dashState;
    public AimState aimState;
    public DeadState deadState;
    //宣告CC
    public CharacterController controller;
    //宣告血量系統
    private PlayerHealth health;
    //取得動畫控制器（用來開關RootMotion）
    private Animator animator;
    //取得武器管理系統
    private WeaponManager weaponManager;

    [Header("玩家Data")]
    public PlayerDataSO playerData;

    [Tooltip("玩家的位移Vector3")]
    public Vector3 Velocity;
    
    [Tooltip("玩家奔跑時的特效")]
    [SerializeField] GameObject SprintEffect;
    [Tooltip("玩家擊中時的特效")]
    public GameObject HitEffect;

    [Header("鎖定邏輯")]
    [Tooltip("動態存放鎖定的敵方單位")]
    public Transform LockTarget;
    //紀錄每次檢查周遭敵人的時間
    private float NextCheckTime = 0f;
    //多久檢查一次附近敵人
    private float CheckInterval = 0.2f;
    //檢查玩家與敵人的距離
    [SerializeField] float stopRootMotionDistance = 1f;

    //用來記錄最後一次Dash的時間
    private float lastDashTime = -Mathf.Infinity;

    public bool isRun { get; private set; } = false;
    public bool isAiming { get; private set; } = false;
    public bool isHit { get; private set; } = false;
    public bool isAttack { get; set; } = false;
    public bool isRolling { get; set; } = false;
    public bool isDash { get; set; } = false;
    public bool IsDie { get; set; } = false;
    public bool CloseEnemy { get; set; } = false;
    public bool Invincible { get; set; } = false;
    public bool InItemMenu { get; set; } = false;
    public bool isSkilling { get; private set; } = false;

    //玩家受傷與死亡的Delegate事件
    public event Action OnHit;
    public Action<bool> isDead;

    private void Awake()
    {
        //初始化玩家身上掛載的CC、Health、WeaponManager、Animator
        controller = GetComponent<CharacterController>();
        health = GetComponent<PlayerHealth>();
        weaponManager = GetComponent<WeaponManager>();
        animator = GetComponent<Animator>();
        //初始化時建立玩家狀態機
        stateMachine = gameObject.AddComponent<PlayerStateMachine>();
        //初始化所有狀態，讓狀態成為單例
        fightState = new FightState(stateMachine, this);
        idleState = new IdleState(stateMachine, this);
        moveState = new MoveState(stateMachine, this);
        dashState = new DashState(stateMachine, this);
        aimState = new AimState(stateMachine, this);
        deadState = new DeadState(stateMachine, this);
        //初始化後預設進入Idle模式
        stateMachine.Initialize(idleState);
        //從Data資料庫初始化玩家最大血量
        health.SetMaxHealth(playerData.MaxHealth);
        //從Data資料庫初始化玩家最大PP值
        health.SetMaxPP(playerData.MaxPP);
        //設置玩家不會因為切場景而被破壞
        if (FindObjectsOfType<PlayerController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //從GameInput取得玩家輸入
        GameInput.Instance.OnSprintAction += SetIsRun;
        GameInput.Instance.OnAimAction += SetIsAiming;
        GameInput.Instance.OnAttackAction += SetIsAttack;
        GameInput.Instance.OnDashkAction += Dash;
        GameInput.Instance.OnLockAction += LockOn;
        GameInput.Instance.OnItemMenu += ItemMenu;
        //Delegate訂閱事件
        health.OnDamage += GetHit;
        health.OnDead += Died;
    }

    void Update()
    {
        //重力運作邏輯
        ApplyGravity();
        //檢測狀態機更新邏輯，僅限沒有技能施放時使用
        if (!isSkilling || !isHit)
        {
            stateMachine.Update();
        }
        //時刻檢查周遭是否存在敵人
        if (Time.time >= NextCheckTime)
        {
            NextCheckTime = Time.time + CheckInterval;
            GetClosestEnemy();
        }
        //鎖定敵人邏輯，如果有鎖定敵人並且相當靠近就停止RootMotion
        if (LockTarget != null && CloseEnemy)
        {
            DisableRootMotion();
        }
        else
        {
            EnableRootMotion();
        }
        //如果沒有鎖定敵人就動態抓取離玩家最近的敵方單位，如果有鎖定敵人就解除鎖定
        if (LockTarget != null)
        {
            float Targetdistance = Vector3.Distance(transform.position, LockTarget.transform.position);
            if (Targetdistance > playerData.LockRange)
            {
                AutoUnlockEnemy();
            }
        }
    }

    //技能系統
    public void CastSkill(string skillName, float SkillDuration)
    {
        if (isSkilling || isAiming) return;

        if (LockTarget != null)
        {
            Vector3 direction = (LockTarget.position - transform.position).normalized;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        isSkilling = true;
        animator.Play(skillName);
        StartCoroutine(SkillCastingRoutine(SkillDuration));
    }
    private IEnumerator SkillCastingRoutine(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        isSkilling = false;
    }

    //共用重力邏輯
    private void ApplyGravity()
    {
        if (isDash) return;

        if (controller.isGrounded)
        {
            Velocity.y = -0.05f;
        }
        else
        {
            Velocity.y -= playerData.Gravity * Time.deltaTime;
        }

        controller.Move(Velocity * Time.deltaTime);
    }

    //取得玩家移動按鍵輸入
    public Vector3 GetMoveInput()
    {
        return GameInput.Instance.GetMoveVector3();
    }

    //Walk、Run狀態機的核心邏輯
    public void MoveCharacter(Vector3 targetDirection, float currentSpeed)
    {
        if (isHit || IsDie || isDash || isSkilling) return;

        controller.Move(targetDirection * currentSpeed * Time.deltaTime);
        SmoothRotation(targetDirection);
    }
    private void SetIsRun(bool isRun)
    {
        if (isHit || isSkilling || IsDie) return;

        this.isRun = isRun;
    }

    //攻擊模式的核心邏輯
    public void SetIsAttack(bool Attack)
    {
        if (isHit || isSkilling || IsDie || InItemMenu || stateMachine.GetState<AimState>() != null || stateMachine.GetState<DashState>() != null) return;

        isAttack = Attack;
    }

    //狀態機不繼承MonoBehaviour，無法使用StartCoroutine，因此由這邊提供給各大狀態機啟動協程
    public void StartPlayerCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    //瞄準模式的核心邏輯
    private void SetIsAiming(bool isAim)
    {
        if (isSkilling || IsDie || InItemMenu || stateMachine.GetState<DashState>() != null) return;

        isAiming = isAim;

        if (LockTarget != null)
        {
            LockTarget = null;
        }
    }
    //瞄準時讓角色時刻透過滑鼠旋轉瞄準
    public void MoveWithOutRotation(Vector3 direction, float speed)
    {
        controller.Move(direction * speed * Time.deltaTime);
    }

    //玩家受傷邏輯（無狀態機，屬於隨時都可能進入狀態
    public void GetHit()
    {
        if (IsDie) return;
  
        OnHit?.Invoke();
        isHit = true;

        StartCoroutine(HitCoolDown());
    }
    //計算玩家受傷時的硬質協程
    IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(playerData.HitCoolTime);

        isHit = false;
    }
    IEnumerator GunHitCoolDown()
    {
        yield return new WaitForSeconds(playerData.HitCoolTime);
        isHit = false;
        stateMachine.ChangeState(aimState);
    }
    
    //玩家死亡邏輯
    public void Died()
    {
        IsDie = true;
    }

    //Dash狀態機的核心邏輯
    private void Dash()
    {
        if (isHit || IsDie || stateMachine.GetState<AimState>() != null) return;

        if (Time.time >= lastDashTime + playerData.DashCoolTime)
        {
            isDash = true;
            lastDashTime = Time.time;
        }
    }
    //紀錄Dash時玩家輸入的方向
    public void SetRotation(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    //玩家鎖定敵人
    private void LockOn()
    {
        LockOnTarget();
    }
    private void LockOnTarget()
    {
        if (isAiming || IsDie) return;

        if (LockTarget == null)
        {
            LockTarget = GetClosestEnemy();
            if (LockTarget != null)
            {
                Health enemyHealth = LockTarget.GetComponent<Health>();
                if (enemyHealth != null) 
                {
                    enemyHealth.EnemyDead += EnemyDead;
                }
            }
        }
        else
        {
            LockTarget = null;
        }


        if (LockTarget != null)
        {
            Debug.Log($"正在鎖定敵人");
        }
        else
        {
            Debug.Log("解除鎖定敵人");
        }
    }
    //檢查敵人是否死亡
    private void EnemyDead(Transform Enemytransform)
    {
        if(LockTarget == Enemytransform)
        {
            AutoUnlockEnemy();
        }
    }
    //敵人死亡自動解除鎖定
    private void AutoUnlockEnemy()
    {
        if (LockTarget != null)
        {
            Health enemyHealth = LockTarget.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.EnemyDead -= EnemyDead;
                CloseEnemy = false; 
            }
        }
        LockTarget = null;
        Debug.Log("敵人已解除鎖定！");
    }
    //偵測距離玩家最近的敵方單位
    private Transform GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, playerData.LockRange, playerData.EnemyLayer);
        Transform Target = null;
        float CloseDistance = Mathf.Infinity;

        bool foundCloseEnemy = false;

        foreach (Collider enemy in enemies) 
        { 
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance < CloseDistance)
            {
                CloseDistance = distance;
                Target = enemy.transform;
            }
            if (distance <= stopRootMotionDistance)
            {
                foundCloseEnemy = true;
            }
        }
        CloseEnemy = foundCloseEnemy;

        return Target;
    }
    //檢查敵人與玩家距離是否需要開關RootMotion
    private void EnableRootMotion()
    {
        animator.applyRootMotion = true;
    }
    private void DisableRootMotion()
    {
        animator.applyRootMotion = false;
    }

    //進入道具系統的邏輯
    private void ItemMenu()
    {
        if (InItemMenu == false)
        {
            InItemMenu = true;
        }
        else
        {
            InItemMenu = false;
        }
    }

    //平滑旋轉角度
    private void SmoothRotation(Vector3 targetMovement)
    {
        if (targetMovement.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetMovement, Vector3.up), playerData.RotateSpeed * Time.deltaTime);
    }

    //取得相機正方方向
    public Vector3 GetCurrentCameraForward()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        return cameraForward;
    }

    //取得相機右方方向
    public Vector3 GetCurrentCameraRight()
    {
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        return cameraRight;
    }
}