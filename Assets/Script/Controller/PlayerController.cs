using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Animator animator;
    //取得攝影機
    public Transform MainCamera;
    //取得等級系統
    public LevelSystem levelSystem;
    //取得暫停UI的介面
    public StopUI stopUI;

    [Header("玩家Data")]
    public PlayerDataSO playerData;

    [Header("玩家Vector3")]
    [Tooltip("玩家的位移Vector3")]
    public Vector3 Velocity;

    [Header("鎖定邏輯")]
    [Tooltip("動態存放鎖定的敵方單位")]
    public Transform LockTarget;
    //紀錄每次檢查周遭敵人的時間
    private float NextCheckTime = 0f;
    //多久檢查一次附近敵人
    private float CheckInterval = 0.2f;
    //檢查玩家與敵人的距離
    [SerializeField] float stopRootMotionDistance = 1f;

    [Header("玩家特效")]
    [Tooltip("玩家奔跑時的特效")]
    [SerializeField] GameObject SprintEffect;
    [Tooltip("玩家擊中時的特效")]
    public GameObject HitEffect;
    [Tooltip("玩家大招時的特效")]
    public GameObject Judgement_Cut_Effect;
    [Tooltip("玩家升級時的特效")]
    public GameObject LevelUp_Effect;
    [Tooltip("玩家復活時的特效")]
    public GameObject Rivive_Effect;
    [Tooltip("玩家攻擊上升時的特效")]
    public GameObject AttackUP_Effect;
    [Tooltip("玩家防禦上升時的特效")]
    public GameObject DefenseUP_Effect;

    //用來記錄最後一次Dash的時間
    private float lastDashTime = -Mathf.Infinity;

    //防呆用受傷紀錄
    private Coroutine hitCoolDownCoroutine;

    //防呆用槍傷紀錄
    private Coroutine AimhitCoolCoroutine;

    //所有狀態旗標
    public bool IsRun { get; private set; } = false;
    public bool IsAiming { get; private set; } = false;
    public bool IsHit { get; private set; } = false;
    public bool IsAttack { get; set; } = false;
    public bool IsDash { get; set; } = false;
    public bool IsDie { get; set; } = false;
    public bool IsCloseEnemy { get; set; } = false;
    public bool Invincible { get; set; } = false;
    public bool IsSkilling { get; private set; } = false;
    public bool IsCriticalHit { get; set; } = false;
    public bool IsAttackBuff { get; private set; } = false;
    public bool IsDefenseBuff { get; private set; } = false;
    public bool IsRivive { get; set; } = false;
    public bool IsGunHit { get; set; } = false;
    public bool IsDashAttack { get; set; } = false;
    public bool IsTeleporting { get; set; } = false;
    public bool IsRightKeyDown { get; private set; } = false;

    private bool isInCriticalDamageCooldown = false;

    //玩家受傷與死亡的Delegate事件
    public event Action OnHit;
    public event Action OnGunHit;
    public event Action CriticalGunHit;
    public Action<bool> isDead;

    private void Awake()
    {
        //初始化玩家身上掛載的CC、Health、WeaponManager、Animator
        controller = GetComponent<CharacterController>();
        health = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        levelSystem = GetComponent<LevelSystem>();
        stopUI = FindObjectOfType<StopUI>();
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
        //確保玩家在TitleScene會自我銷毀
        SceneManager.sceneLoaded += OnSceneLoaded;
        //設置玩家不會因為切場景而被破壞
        if (FindObjectsOfType<PlayerController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //初始化數據
        playerData.CurrentExp = 0;
        playerData.XPForNextLevel = 100;
        playerData.MaxHealth = 100;
        playerData.MaxPP = 100;
        playerData.CurrentLevel = 1;
        playerData.attackDamage = 15;
        playerData.GunDamage = 8;
        playerData.Defense = 0f;
    }

    private void Start()
    {
        //從GameInput取得玩家輸入
        GameInput.Instance.OnSprintAction += SetIsRun;
        GameInput.Instance.OnAimAction += SetIsAiming;
        GameInput.Instance.OnAimAction += SetRightKey;
        GameInput.Instance.OnAttackAction += SetIsAttack;
        GameInput.Instance.OnDashAction += Dash;
        GameInput.Instance.OnLockAction += LockOn;
        //Delegate訂閱事件
        health.OnDamage += GetHit;
        health.HaveReviveItemDead += Died;
        health.NoReviveItemDead += Died;
        health.OnCriticalDamage += OnCriticalDamage;
        health.OnGunDamage += TakeAimHit;
        health.PlayerRivive += Rivive;
        levelSystem.PlayerLevelup += LevelUp;
    }

    void Update()
    {
        //重力運作邏輯
        ApplyGravity();
        //檢測狀態機更新邏輯，僅限沒有技能施放時使用
        if (!IsSkilling)
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
        if (LockTarget != null && IsCloseEnemy)
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

        if (IsDie && hitCoolDownCoroutine != null) 
        {
            StopCoroutine(hitCoolDownCoroutine);
            animator.CrossFade("Die", 0f, 0);
        }

        if(UIManager.CurrentState == UIState.Menu)
        {
            IsAiming = false;
        }
    }

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            GameInput.Instance.OnSprintAction -= SetIsRun;
            GameInput.Instance.OnAimAction -= SetIsAiming;
            GameInput.Instance.OnAttackAction -= SetIsAttack;
            GameInput.Instance.OnDashAction -= Dash;
            GameInput.Instance.OnLockAction -= LockOn;
            Destroy(gameObject);
            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //技能系統
    public void CastSkill(string skillName, float SkillDuration)
    {
        if (IsCriticalHit || IsHit || IsDie || IsRivive || IsTeleporting || IsSkilling || IsAiming) return;

        if (LockTarget != null)
        {
            Vector3 direction = (LockTarget.position - transform.position).normalized;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        IsSkilling = true;
        animator.CrossFade(skillName, 0f, 0);
        StartCoroutine(SkillCastingRoutine(SkillDuration));
    }
    private IEnumerator SkillCastingRoutine(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        IsSkilling = false;
    }

    //共用重力邏輯
    private void ApplyGravity()
    {
        if (IsDash) return;

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
        if (IsCriticalHit || IsHit || IsDie || IsRivive || IsTeleporting) return Vector3.zero;

        return GameInput.Instance.GetMoveVector3();
    }

    //Walk、Run狀態機的核心邏輯
    public void MoveCharacter(Vector3 targetDirection, float currentSpeed)
    {
        if (IsCriticalHit || IsHit || IsDie || IsRivive || IsTeleporting || IsDash || IsSkilling) return;

        controller.Move(targetDirection * currentSpeed * Time.deltaTime);
        SmoothRotation(targetDirection);
    }
    private void SetIsRun(bool isRun)
    {
        if (IsCriticalHit || IsHit || IsDie || IsRivive || IsTeleporting || IsSkilling) return;

        this.IsRun = isRun;
    }
    //攻擊模式的核心邏輯
    public void SetIsAttack(bool Attack)
    {
        if (IsCriticalHit || IsHit || IsSkilling || IsDie || UIManager.CurrentState == UIState.Menu || UIManager.CurrentState == UIState.Pause || stateMachine.GetState<AimState>() != null || stateMachine.GetState<DashState>() != null) return;

        IsAttack = Attack;
    }

    //狀態機不繼承MonoBehaviour，無法使用StartCoroutine，因此由這邊提供給各大狀態機啟動協程
    public void StartPlayerCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    public void PlayAttackVoice()
    {
        this.PlaySound("PlayerAttack");
    }

    //瞄準模式的核心邏輯
    private void SetIsAiming(bool isAim)
    {
        if (IsCriticalHit || IsHit || IsDie || IsRivive || IsTeleporting || IsSkilling || IsDashAttack || UIManager.CurrentState == UIState.Menu || UIManager.CurrentState == UIState.Pause || stateMachine.GetState<DashState>() != null) return;

        this.IsAiming = isAim;

        if (LockTarget != null)
        {
            LockTarget = null;
        }
    }
    private void SetRightKey(bool isAim)
    {
        if (isInCriticalDamageCooldown)
            return;

        IsRightKeyDown = isAim;

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
        OnHit?.Invoke();
        IsHit = true;

        if (hitCoolDownCoroutine != null)
        {
            StopCoroutine(hitCoolDownCoroutine);
        }

        hitCoolDownCoroutine = StartCoroutine(HitCoolDown());
    }
    private void OnCriticalDamage()
    {
        if (IsAiming)
        {
            CriticalGunHit?.Invoke();
            IsCriticalHit = true;
            isInCriticalDamageCooldown = true;
            StartCoroutine(CriticalDamageCoolDown());
        }
        else
        {
            IsCriticalHit = true;
            isInCriticalDamageCooldown = true;
            StartCoroutine(CriticalDamageCoolDown());
        }

        if (hitCoolDownCoroutine != null)
        {
            StopCoroutine(hitCoolDownCoroutine);
        }
    }
    private void TakeAimHit()
    {
        OnGunHit?.Invoke();
        IsGunHit = true;

        if (AimhitCoolCoroutine != null)
        {
            StopCoroutine(AimhitCoolCoroutine);
        }

        AimhitCoolCoroutine = StartCoroutine(GunHitCoolDown());
    }
    //計算玩家受傷時的硬質協程
    IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(playerData.HitCoolTime);

        if (IsCriticalHit || IsDash)
            yield break;

        IsHit = false;
        Vector3 inputDirection = GetMoveInput().normalized;
        if (inputDirection == Vector3.zero)
        {
            stateMachine.ChangeState(idleState);
        }
        else if (inputDirection != Vector3.zero)
        {
            stateMachine.ChangeState(moveState);
        }
    }
    IEnumerator GunHitCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.CrossFade("AimLocoMotion", 0f, 1);
        IsGunHit = false;
    }
    IEnumerator CriticalDamageCoolDown()
    {
        yield return new WaitForSeconds(1.5f);
        IsHit = false;
        IsCriticalHit = false;
        // 等待前先取得移動輸入
        Vector3 inputDirection = GetMoveInput().normalized;
        yield return new WaitForSeconds(0.1f);

        // 直接輪詢右鍵狀態，確保獲取的是最新狀態
        bool rightKeyCurrentlyDown = Input.GetMouseButton(1);

        if (inputDirection == Vector3.zero && !rightKeyCurrentlyDown)
        {
            IsAiming = false;
            stateMachine.ChangeState(idleState);
        }
        else if (inputDirection != Vector3.zero && !rightKeyCurrentlyDown)
        {
            IsAiming = false;
            stateMachine.ChangeState(moveState);
        }
        else if (rightKeyCurrentlyDown)
        {
            IsAiming = true;
            animator.CrossFade("AimLocoMotion", 0f, 1);
            stateMachine.ChangeState(aimState);
        }
    }


    //玩家死亡邏輯
    public void Died()
    {
        IsDie = true;
        IsAiming = false;
    }

    //Dash狀態機的核心邏輯
    private void Dash()
    {
        if (IsCriticalHit || IsDie || IsRivive || IsTeleporting || stateMachine.GetState<AimState>() != null || IsSkilling || IsDashAttack) return;

        if (Time.time >= lastDashTime + playerData.DashCoolTime)
        {
            if (hitCoolDownCoroutine != null) 
            {
                IsHit = false;
                StopCoroutine(HitCoolDown());
            }
            IsDash = true;
            IsAttack = false;
            lastDashTime = Time.time;
        }
    }
    //播放Dash音效
    public void DashSound()
    {
        this.PlaySound("Dash");
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
        if (IsAiming || IsDie) return;

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
                IsCloseEnemy = false; 
            }
        }
        LockTarget = null;
    }
    //偵測距離玩家最近的敵方單位
    private Transform GetClosestEnemy()
    {
        Collider[] bossTargets = Physics.OverlapSphere(transform.position, playerData.LockRange, playerData.BossLayer);
        if (bossTargets.Length > 0)
        {
            Transform bossTarget = null;
            float closestBossDistance = Mathf.Infinity;
            foreach (Collider boss in bossTargets)
            {
                float distance = Vector3.Distance(transform.position, boss.transform.position);
                if (distance < closestBossDistance)
                {
                    closestBossDistance = distance;
                    bossTarget = boss.transform;
                }
            }
            IsCloseEnemy = (closestBossDistance <= stopRootMotionDistance);
            return bossTarget;
        }

        // 若未檢測到 Boss，則以原邏輯檢查一般敵人
        Collider[] enemies = Physics.OverlapSphere(transform.position, playerData.LockRange, playerData.EnemyLayer);
        Transform closestEnemy = null;
        float closestEnemyDistance = Mathf.Infinity;
        bool foundCloseEnemy = false;
        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemy = enemy.transform;
            }
            if (distance <= stopRootMotionDistance)
            {
                foundCloseEnemy = true;
            }
        }
        IsCloseEnemy = foundCloseEnemy;
        return closestEnemy;
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

    //平滑旋轉角度
    private void SmoothRotation(Vector3 targetMovement)
    {
        if (targetMovement.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetMovement, Vector3.up), playerData.RotateSpeed * Time.deltaTime);
    }

    //取得相機正方方向
    public Vector3 GetCurrentCameraForward()
    {
        Vector3 cameraForward = MainCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        return cameraForward;
    }

    //取得相機右方方向
    public Vector3 GetCurrentCameraRight()
    {
        Vector3 cameraRight = MainCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        return cameraRight;
    }

    //隱藏玩家
    public void HidePlayer()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = false;
        }
    }

    // 顯示玩家
    public void ShowPlayer()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = true;
        }
    }

    //放慢時間
    public void TriggerSlowTime()
    {
        StartCoroutine(SlowTimeCoroutine(1f, 0.1f));
    }

    private IEnumerator SlowTimeCoroutine(float duration, float slowFactor)
    {
        float originalTimeScale = Time.timeScale;
        float originalFixedDeltaTime = Time.fixedDeltaTime;

        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = originalFixedDeltaTime * slowFactor;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

    //大招前搖效果
    public void JudgementCut()
    {
        Instantiate(Judgement_Cut_Effect, transform.position, Quaternion.identity);
        CameraController camera = Camera.main.GetComponent<CameraController>();
        if (camera != null)
        {
            camera.StartCoroutine(camera.ShakeCamera(1f, 0.1f));
            Debug.Log("找到攝影機！開始抖動");
        }
    }
    //重擊時攝影機的晃動
    public void SpikeShake()
    {
        CameraController camera = Camera.main.GetComponent<CameraController>();
        if (camera != null)
        {
            camera.StartCoroutine(camera.ShakeCamera(0.2f, 0.1f));
            Debug.Log("找到攝影機！開始抖動");
        }
    }

    //升級時的特效
    public void LevelUp()
    {
        Instantiate(LevelUp_Effect, transform.position, LevelUp_Effect.transform.rotation);
    }

    //攻擊Buff邏輯
    public IEnumerator AttackUP(float Amout, float Duration)
    {
        Debug.Log("開始增加傷害！");

        float OriginAttackDamage = playerData.attackDamage;

        GameObject AttackUPEffect = Instantiate(AttackUP_Effect, transform.position, AttackUP_Effect.transform.rotation);
        AttackUPEffect.transform.SetParent(transform);

        float elapsed = 0f;
        playerData.attackDamage += Amout;

        while (elapsed < Duration) 
        {
            elapsed += Time.deltaTime;
            IsAttackBuff = true;
            yield return null;
        }

        Debug.Log("增傷結束！");
        playerData.attackDamage = OriginAttackDamage;
        IsAttackBuff = false;

        Destroy(AttackUPEffect);
    }
    //防禦Buff邏輯
    public IEnumerator DefenseUP(float Amout, float Duration)
    {
        Debug.Log("開始增加防禦！");

        GameObject DefenseUPEffect = Instantiate(DefenseUP_Effect, transform.position, DefenseUP_Effect.transform.rotation);
        DefenseUPEffect.transform.SetParent(transform);

        float elapsed = 0f;
        playerData.Defense += Amout;

        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;
            IsDefenseBuff = true;
            yield return null;
        }

        Debug.Log("增防結束！");
        playerData.Defense = 0;
        IsDefenseBuff = false;

        Destroy(DefenseUPEffect);
    }

    //玩家重生
    private void Rivive()
    {
        IsRivive = true;
        Instantiate(Rivive_Effect, transform.position, Rivive_Effect.transform.rotation);
        StartCoroutine(RiviveCoolDown());
    }
    private IEnumerator RiviveCoolDown()
    {
        yield return new WaitForSeconds(0.8f);
        IsRivive = false;
        IsDie = false;

        Vector3 inputDirection = GetMoveInput().normalized;
        if (inputDirection == Vector3.zero)
        {
            stateMachine.ChangeState(idleState);
        }
        else if (inputDirection != Vector3.zero)
        {
            stateMachine.ChangeState(moveState);
        }

        UIManager.CurrentState = UIState.None;
    }

    public void GetIntoPortal()
    {
        if (IsAiming)
        {
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
            animator.Play("TakeRifle", 1, 0f);
            animator.SetBool("IsAim", false);
            animator.SetTrigger("Take");
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
            stateMachine.ChangeState(idleState);
            IsAiming = false;
        }
        else
        {
            stateMachine.ChangeState(idleState);
        }
    }
}