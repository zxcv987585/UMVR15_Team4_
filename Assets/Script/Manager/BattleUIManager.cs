using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    [SerializeField] private Canvas battleCanvas;

    //--------------------------傷害數字-------------------------------
    [SerializeField] private Queue<DamageText> damageTextQuene;
    [SerializeField] private Transform damageUI;
    [SerializeField] private DamageText damageTextPrefab;
    //-----------------------------------------------------------------

    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI currentHPText;
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private Slider HPSlider;
    [SerializeField] private TextMeshProUGUI currentPPText;
    [SerializeField] private TextMeshProUGUI maxPPText;
    [SerializeField] private Slider PPSlider;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private LevelSystem levelSystem;

    private Coroutine costAnimationCoroutine;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        damageTextQuene = new Queue<DamageText>();
        health.OnDamage += ChangeHPStatus;
        health.OnCriticalDamage += ChangeHPStatus;
        health.OnDot += ChangeHPStatus;
        health.OnDead += ChangeHPStatus;
        health.OnHeal += ChangeHPStatus;
        health.OnPPChanged += playerMaxPPupdate;
        health.OnHealPP += ChangePPStatus;
        levelSystem.PlayerLevelup += playerMaxHealthupdate;

        currentHPText.text = health.GetMaxHealth().ToString();
        maxHPText.text = health.GetMaxHealth().ToString();
        currentPPText.text = health.GetMaxPP().ToString();
        maxPPText.text = health.GetMaxPP().ToString();
    }

    private void Health_OnHeal()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        UpdatePPUI();
    }

    /// <summary>
    /// 顯示傷害數字
    /// </summary>
    /// <param name="showPosition">顯示的位置</param>
    /// <param name="damage">傷害的數字</param>
    public void ShowDamageText(Vector3 showPosition, float damage)
    {
        if (damageTextQuene.Count > 0)
        {
            DamageText damageText = damageTextQuene.Dequeue();
            damageText.transform.position = showPosition;
            damageText.Show(damage);
        }
        else
        {
            DamageText damageText = Instantiate(damageTextPrefab, damageUI);
            damageText.transform.position = showPosition;
            damageText.Show(damage);
        }
    }

    //傷害數字物件池, 用來回收物件用的
    public void RecycleDamageText(DamageText damageText)
    {
        damageTextQuene.Enqueue(damageText);
    }

    //PP實時更新至UI介面
    private void UpdatePPUI()
    {
        // 如果想顯示整數
        currentPPText.text = Mathf.FloorToInt(health.GetCurrentPP()).ToString();
        maxPPText.text = Mathf.FloorToInt(health.GetMaxPP()).ToString();
        PPSlider.value = health.GetPPRatio();
        LevelText.text = Mathf.FloorToInt(player.playerData.CurrentLevel).ToString();
    }

    //當玩家的 HP 變動, 也跟著變動 UI 的 HP
    private void ChangeHPStatus()
    {
        currentHPText.text = health.GetCurrentHealth().ToString();

        HPSlider.value = health.GetHealthRatio();
    }
    private void playerMaxHealthupdate()
    {
        float newMaxHealth = health.GetMaxHealth();
        Debug.Log($"玩家升級！新最大血量: {newMaxHealth}");
        maxHPText.text = newMaxHealth.ToString();
        float newCurrentHealth = health.GetCurrentHealth();
        currentHPText.text = newCurrentHealth.ToString();
        LevelText.text = Mathf.FloorToInt(player.playerData.CurrentLevel).ToString();
        PPSlider.value = health.GetPPRatio();
        HPSlider.value = health.GetHealthRatio();
    }

    //當玩家的PP變動，也跟著變動 UI 的 PP
    private void ChangePPStatus()
    {
        currentPPText.text = health.GetCurrentPP().ToString();

        PPSlider.value = health.GetPPRatio();
    }
    private void playerMaxPPupdate()
    {
        PPSlider.value = health.GetPPRatio();
        currentPPText.text = health.GetCurrentPP().ToString();
        maxPPText.text = health.GetMaxPP().ToString();
    }

    //在不對的Scene時要關閉
    public void HideUI()
    {
        battleCanvas.enabled = false;
    }

    public void ShowUI()
    {
        battleCanvas.enabled = true;
    }
}
