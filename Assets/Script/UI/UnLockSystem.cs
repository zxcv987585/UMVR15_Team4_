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
            Debug.LogError("UnLockSystem: 無法取得・LevelSystem｡I");
            return;
        }
    }
    void Update()
    {
        if (level.playerData.CurrentLevel >= unlockLevel)
        {
            ShowUnlockMessage();
            gameObject.SetActive(false); // **ｷ�ﾑ･ｯﾅｹFｼﾐｮﾉ｡Aｰｱ･ﾎｦﾛｨｭ**
        }
    }
    private void ShowUnlockMessage()
    {
        if (unlockMessagePrefab != null && messageParent != null)
        {
            SkillSlot skillSlot = transform.parent != null ? transform.parent.GetComponentInChildren<SkillSlot>() : null;

            if (skillSlot != null && skillSlot.skillData != null)
            {
                skillName = skillSlot.skillData.skillName; // **ｨ�ｱoｧﾞｯ爬Wｺﾙ**
                Debug.Log($"UnLockSystem: ｧﾞｯ炮skillName}");
            }

            // **･ﾍｦｨｰTｮｧｪｫ?E*
            GameObject messageObj = Instantiate(unlockMessagePrefab, messageParent);
            TextMeshProUGUI messageText = messageObj.GetComponentInChildren<TextMeshProUGUI>();
            CanvasGroup canvasGroup = messageObj.GetComponent<CanvasGroup>();

            if (messageText != null)
                messageText.text = $"ｨ�ｱoｧﾞｯ�:{skillName}";
        }
    }
}
