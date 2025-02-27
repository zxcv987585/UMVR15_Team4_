using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
    [Tooltip("玩家鎖定視角的最大距離")]
    public float LockRange = 10f;
    [Tooltip("可以被鎖定的Layer層")]
    public LayerMask EnemyLayer;
    [Tooltip("玩家的位移Vector3")]
    public Vector3 Velocity;
    [Tooltip("玩家的傷害量")]
    public float attackDamage = 25f;
    [Tooltip("玩家的攻擊範圍")]
    public float attackRadius = 0.5f;

    [Header("鎖定時的攝影機邏輯")]
    [Tooltip("鎖定狀態下的攝影機中心點")]
    [SerializeField] Transform cameraPivot;
    [SerializeField] float CameraSpeed = 5f;

    //取得距離玩家最近的敵方單位
    public Transform LockTarget;
    //用來記錄最後一次Dash的時間
    private float lastDashTime = -Mathf.Infinity;

    public bool isRun { get; private set; } = false;
    public bool isAiming { get; private set; } = false;
    public bool isHit { get; private set; } = false;
    public bool isAttack { get; set; } = false;
    public bool isRolling { get; set; } = false;
    public bool isDash { get; set; } = false;
    public bool IsDie { get; set; } = false;

    //玩家受傷與死亡的Delegate事件
    public Action<string> OnHit;
    public Action<bool> isDead;

    private void Awake()
    {
        //初始化玩家身上掛載的CC、Health、WeaponManager
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        weaponManager = GetComponent<WeaponManager>();
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
        health.SetMaxHealth(MaxHealth);
    }

    private void Start()
    {
        //從GameInput取得玩家輸入
        GameInput.Instance.OnSprintAction += SetIsRun;
        GameInput.Instance.OnAimAction += SetIsAiming;
        GameInput.Instance.OnAttackAction += SetIsAttack;
        GameInput.Instance.OnDashkAction += Dash;
        //Delegate訂閱事件
        health.OnDamage += GetHit;
        health.OnDead += Died;
    }

    void Update()
    {
        //重力運作邏輯
        ApplyGravity();
        //檢測狀態機更新邏輯
        stateMachine.Update();
        //鎖定時控制攝影機
        if(LockTarget != null)
        {
            Vector3 direction = LockTarget.position - cameraPivot.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, targetRotation, Time.deltaTime * CameraSpeed);

            //if(LockTarget.GetComponent<>)
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LockOnTarget();
        }
    }

    //共用重力邏輯
    private void ApplyGravity()
    {
        //Dash期間不要生效重力，以防重力影響速度
        if (isDash) return;
        //玩家如果在地面就計算Y軸不超過一定數值，如果不在地面加上重力數值讓玩家採地板
        if (controller.isGrounded)
        {
            Velocity.y = -0.05f;
        }
        else
        {
            Velocity.y -= Gravity * Time.deltaTime;
        }
        //計算完後丟回controller進行Y軸移動
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
        //死亡就不要移動
        if (IsDie) return;
        if (isDash) return;
        //將MoveState計算完的數值傳入Controller進行移動
        controller.Move(targetDirection * currentSpeed * Time.deltaTime);
        //玩家如果轉向人物也必須跟著旋轉
        SmoothRotation(targetDirection);
    }
    private void SetIsRun(bool isRun)
    {
        //死亡就不要跑步
        if (IsDie) return;
        //透過涵式更改PlayerController的跑步bool狀態
        this.isRun = isRun;
    }

    //攻擊模式的核心邏輯
    private void SetIsAttack(bool Attack)
    {
        //死亡就不要砍人
        if (IsDie) return;
        //如果是瞄準跟Dash狀態就不要執行攻擊，否則會邏輯打架
        if (stateMachine.GetState<AimState>() != null) return;
        if (stateMachine.GetState<DashState>() != null) return;
        //透過涵式更改PlayerController的攻擊bool狀態
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
        //死亡就不要舉槍
        if (IsDie) return;
        //透過涵式更改PlayerController的瞄準bool狀態
        isAiming = isAim;
    }
    //瞄準時讓角色時刻透過滑鼠旋轉瞄準
    public void MoveWithOutRotation(Vector3 direction, float speed)
    {
        controller.Move(direction * speed * Time.deltaTime);
    }

    //玩家受傷邏輯（無狀態機，屬於隨時都可能進入狀態
    public void GetHit()
    {
        //死亡就不會受傷
        if (IsDie) return;
        //通知已經訂閱的系統玩家受到傷害
        OnHit?.Invoke("Hit");
        //透過涵式更改PlayerController的受傷bool狀態
        isHit = true;
        //啟動協程，進入玩家受傷的硬質時間
        StartCoroutine(HitCoolDown());
    }
    //計算玩家受傷時的硬質協程
    IEnumerator HitCoolDown()
    {
        //讓玩家必須等待硬質時間結束才能接著移動
        yield return new WaitForSeconds(HitCoolTime);
        //透過涵式更改PlayerController的受傷bool狀態
        isHit = false;
        //硬質結束直接回到idle狀態
        stateMachine.ChangeState(idleState);
    }
    
    //玩家死亡邏輯
    public void Died()
    {
        //透過涵式更改PlayerController的死亡bool狀態
        IsDie = true;
    }

    //Dash狀態機的核心邏輯
    private void Dash()
    {
        //死亡就不會Dash
        if (IsDie) return;
        //如果玩家在Idle跟瞄準狀態就不能Dash
        if (stateMachine.GetState<AimState>() != null)
        {
            return;
        }
        //冷卻時間結束就准許Dash
        if (Time.time >= lastDashTime + DashCoolTime)
        {
            //透過涵式更改PlayerController的Dash bool狀態
            isDash = true;
            //紀錄最後一次Dash的時間，以確保冷卻時間生效
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
    private void LockOnTarget()
    {
        if(LockTarget == null)
        {
            LockTarget = GetClosestEnemy();
        }
        else
        {
            LockTarget = null;
        }

        if (LockTarget != null) 
        { 
            Vector3 directionToTarget = LockTarget.position - transform.position;
            directionToTarget.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), Time.deltaTime * RotateSpeed);
        }
    }
    //偵測距離玩家最近的敵方單位
    private Transform GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, LockRange, EnemyLayer);
        Transform Target = null;
        float CloseDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies) 
        { 
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance < CloseDistance)
            {
                CloseDistance = distance;
                Target = enemy.transform;
            }
        }
        //回傳Target給攝影機
        return Target;
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