using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnLockSystem : MonoBehaviour
{
    private LevelSystem level;
    [SerializeField] private int unlockLevel;
    [SerializeField] private GameObject unlockMessagePrefab;
    [SerializeField] private Transform messageParent;

    private string skillName = "";

    void Start()
    {
        level = FindAnyObjectByType<LevelSystem>();
        if (level == null)
        {
            Debug.LogError("UnLockSystem: 找不到 LevelSystem！");
            return;
        }

        // **嘗試在同一層 (兄弟物件) 尋找 SkillSlot**
        SkillSlot skillSlot = transform.parent != null ? transform.parent.GetComponentInChildren<SkillSlot>() : null;

        if (skillSlot != null && skillSlot.skillData != null)
        {
            skillName = skillSlot.skillData.skillName; // **取得技能名稱**
            Debug.Log($"UnLockSystem: 找到技能 {skillName}");
        }
    }
    void Update()
    {
        if (level.playerData.CurrentLevel >= unlockLevel)
        {
            ShowUnlockMessage();
            gameObject.SetActive(false); // **當等級達標時，停用自身**
        }
    }
    private void ShowUnlockMessage()
    {
        if (unlockMessagePrefab != null && messageParent != null)
        {
            // **生成訊息物件**
            GameObject messageObj = Instantiate(unlockMessagePrefab, messageParent);
            TextMeshProUGUI messageText = messageObj.GetComponentInChildren<TextMeshProUGUI>();
            CanvasGroup canvasGroup = messageObj.GetComponent<CanvasGroup>();

            if (messageText != null)
                messageText.text = $"取得技能：{skillName}";
        }
    }
}
