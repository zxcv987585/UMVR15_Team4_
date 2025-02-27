using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine stateMachine { get; private set; }
    public FightState fightState;
    public IdleState idleState;
    public MoveState moveState;
    public DashState dashState;
    public AimState aimState;
    public DeadState deadState;
    public CharacterController controller;
    private Health health;

    //取得武器管理系統
    private WeaponManager weaponManager;


    [Header("移動參數")]
    [Tooltip("移動速度")]
    public float MoveSpeed = 5;
    [Tooltip("奔跑加速")]
    [Range(1, 3)]
    public float SprintSpeedModifier = 2;
    [Tooltip("角色旋轉速度")]
    public float RotateSpeed = 5f;
    [Tooltip("賦予角色重力")]
    public float Gravity = 40f;
    [Tooltip("Dash的瞬間速度")]
    public float DashSpeed = 100f;
    [Tooltip("Dash持續時間")]
    public float DashDuration = 1f;
    [Tooltip("Dash冷卻時間")]
    public float DashCoolTime = 1.5f;
    [Tooltip("受傷狀態的持續時間")]
    public float HitCoolTime = 1f;
    [Tooltip("玩家最大血量")]
    public float MaxHealth = 150f;
    //角色死亡狀態
    public bool IsDie = false;

    private float lastDashTime = -Mathf.Infinity;

    public Vector3 Velocity;
    public bool isRun { get; private set; } = false;
    public bool isAiming { get; private set; } = false;
    public bool isHit { get; private set; } = false;
    public bool isAttack { get; set; } = false;
    public bool isRolling { get; set; } = false;
    public bool isDash { get; set; } = false;

    public Action<string> OnHit;
    public Action<bool> isDead;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        stateMachine = gameObject.AddComponent<PlayerStateMachine>();
        fightState = new FightState(stateMachine, this);
        idleState = new IdleState(stateMachine, this);
        moveState = new MoveState(stateMachine, this);
        dashState = new DashState(stateMachine, this);
        aimState = new AimState(stateMachine, this);
        deadState = new DeadState(stateMachine, this);

        stateMachine.Initialize(idleState);
        health.SetMaxHealth(MaxHealth);
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Start()
    {
        stateMachine.Initialize(new IdleState(stateMachine, this));
        health.OnDamage += GetHit;
        health.OnDead += Died;
        GameInput.Instance.OnSprintAction += SetIsRun;
        GameInput.Instance.OnAimAction += SetIsAiming;
        GameInput.Instance.OnAttackAction += SetIsAttack;
        GameInput.Instance.OnDashkAction += Dash;
    }

    void Update()
    {
        ApplyGravity();
        stateMachine.Update();
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
            Velocity.y -= Gravity * Time.deltaTime;
        }
        controller.Move(Velocity * Time.deltaTime);
    }

    //判斷玩家是否有按鍵輸入
    public Vector3 GetMoveInput()
    {
        return GameInput.Instance.GetMoveVector3();
    }

    //Walk、Run狀態機的核心邏輯
    public void MoveCharacter(Vector3 targetDirection, float currentSpeed)
    {
        if (IsDie) return;
        controller.Move(targetDirection * currentSpeed * Time.deltaTime);
        SmoothRotation(targetDirection);
    }
    private void SetIsRun(bool isRun)
    {
        if (IsDie) return;
        this.isRun = isRun;
    }

    //攻擊模式的核心邏輯
    private void SetIsAttack(bool Attack)
    {
        if (IsDie) return;
        if (stateMachine.GetState<AimState>() != null) return;
        if (stateMachine.GetState<DashState>() != null) return;
        isAttack = Attack;
    }
    public void StartPlayerCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    //瞄準模式的核心邏輯
    private void SetIsAiming(bool isAim)
    {
        if (IsDie) return;
        isAiming = isAim;
    }
    public void MoveWithOutRotation(Vector3 direction, float speed)
    {
        controller.Move(direction * speed * Time.deltaTime);
    }

    //玩家受傷邏輯（無狀態機，屬於隨時都可能進入狀態
    public void GetHit()
    {
        if (IsDie) return;
        OnHit?.Invoke("Hit");
        isHit = true;
        StartCoroutine(HitCoolDown());
    }
    IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(HitCoolTime);
        isHit = false;
        stateMachine.ChangeState(idleState);
    }
    
    //玩家死亡邏輯
    public void Died()
    {
        IsDie = true;
    }

    //Dash狀態機的核心邏輯
    private void Dash()
    {
        if (IsDie) return;
        if (stateMachine.GetState<IdleState>() != null || stateMachine.GetState<AimState>() != null)
        {
            return;
        }
        if (Time.time >= lastDashTime + DashCoolTime)
        {
            isDash = true;
            lastDashTime = Time.time;
        }
    }
    public void SetRotation(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    //平滑旋轉角度
    private void SmoothRotation(Vector3 targetMovement)
    {
        if (targetMovement.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetMovement, Vector3.up), RotateSpeed * Time.deltaTime);
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