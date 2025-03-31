using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    [SerializeField] private Canvas battleCanvas;
    //
    [SerializeField] private TextMeshProUGUI playerName;
    //

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

    [SerializeField] private Image onDotRedImage;
    private Coroutine onDotEffectCoroutine;
    private bool isDotEffectPlaying = false;

    [SerializeField] private Slider EXPBar;
    [SerializeField] private TextMeshProUGUI currentEXPText;
    [SerializeField] private TextMeshProUGUI maxEXPText;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName");

        damageTextQuene = new Queue<DamageText>();
        health.OnDamage += ChangeHPStatus;
        health.OnGunDamage += ChangeHPStatus;
        health.OnCriticalDamage += ChangeHPStatus;
        health.OnDot += ChangeHPStatus;
        health.HaveReviveItemDead += ChangeHPStatus;
        health.NoReviveItemDead += ChangeHPStatus;
        health.OnHeal += ChangeHPStatus;
        health.OnPPChanged += playerMaxPPupdate;
        health.OnHealPP += ChangePPStatus;
        health.PlayerRivive += playerMaxHealthupdate;
        levelSystem.PlayerLevelup += playerMaxHealthupdate;

        currentHPText.text = health.GetMaxHealth().ToString();
        maxHPText.text = health.GetMaxHealth().ToString();
        currentPPText.text = health.GetMaxPP().ToString();
        maxPPText.text = health.GetMaxPP().ToString();

        levelSystem.PlayerLevelup += UpdateEXPUI;
        UpdateEXPUI();

        health.OnDot += OnDotUI;
    }

    public void UpdateEXPUI()
    {
        float currentExp = levelSystem.playerData.CurrentExp;
        float maxExp = levelSystem.playerData.XPForNextLevel;

        currentEXPText.text = Mathf.FloorToInt(currentExp).ToString();
        maxEXPText.text = Mathf.FloorToInt(maxExp).ToString();
        EXPBar.value = currentExp / maxExp;
    }

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            Destroy(gameObject);
            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        currentPPText.text = health.GetCurrentPP().ToString();
        PPSlider.value = health.GetPPRatio();
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

    private void OnDotUI()
    {
        if (isDotEffectPlaying == false)
        {
            StartCoroutine(OnDotUIAnimation());
        }
    }
    private IEnumerator OnDotUIAnimation()
    {
        isDotEffectPlaying = true;
        onDotRedImage.gameObject.SetActive(true);

        float duration = 0.5f;
        float timer = 0f;
        CanvasGroup canvasGroup = onDotRedImage.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        //Color color = onDotRedImage.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            //onDotRedImage.color = color;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.125f);

        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            //onDotRedImage.color = color;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        onDotRedImage.gameObject.SetActive(false);
        isDotEffectPlaying = false;
    }
}
