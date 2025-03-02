using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance{get; private set;}

    //--------------------------傷害數字-------------------------------
    [SerializeField] private Queue<DamageText> damageTextQuene;
    [SerializeField] private Transform damageUI;
    [SerializeField] private DamageText damageTextPrefab;
    //-----------------------------------------------------------------

    [SerializeField] private Health health;
    [SerializeField] private TextMeshProUGUI currentHPText;
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private Slider HPSlider;
    [SerializeField] private Image costHPImage;
    [SerializeField] private LevelSystem levelSystem;

    private float fadeTime = 1f;
    private Coroutine costAnimationCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        damageTextQuene = new Queue<DamageText>();
        health.OnDamage += ChangeHPStatus;
        health.OnDead += ChangeHPStatus;
        levelSystem.PlayerLevelup += playerMaxHealthupdate;

        currentHPText.text = health.GetMaxHealth().ToString();
        maxHPText.text = health.GetMaxHealth().ToString();
    }

    /// <summary>
    /// 顯示傷害數字
    /// </summary>
    /// <param name="showPosition">顯示的位置</param>
    /// <param name="damage">傷害的數字</param>
    public void ShowDamageText(Vector3 showPosition, float damage)
    {
        if(damageTextQuene.Count > 0)
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

    //當玩家的 HP 變動, 也跟著變動 UI 的 HP
    private void ChangeHPStatus()
    {
        currentHPText.text = health.GetCurrentHealth().ToString();
        
        //costHPImage.fillAmount = HPImage.fillAmount;
        // if(costAnimationCoroutine != null)
        // {
        //     StopCoroutine(costAnimationCoroutine);
        // }
        // costAnimationCoroutine = StartCoroutine(CostAnimationCoroutine());

        HPSlider.value = health.GetHealthRatio();
    }
    private void playerMaxHealthupdate()
    {
        float newMaxHealth = health.GetMaxHealth();
        Debug.Log($"玩家升級！新最大血量: {newMaxHealth}");
        maxHPText.text = newMaxHealth.ToString();
    }

    //HP 傷害紅條淡出動畫
    private IEnumerator CostAnimationCoroutine()
    {
        //costHPImage.fillAmount = HPImage.fillAmount;

        float timer = 0f;
        Color color = costHPImage.color;

        while(timer < fadeTime)
        {
            timer += Time.deltaTime;
            
            color.a = Mathf.Lerp(1f, 0f, timer/fadeTime);
            costHPImage.color = color;

            yield return null;
        }
    }
}
