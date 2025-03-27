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
            Debug.LogError("UnLockSystem: –³–@Žæ“¾ELevelSystem¡I");
            return;
        }
    }
    void Update()
    {
        if (level.playerData.CurrentLevel >= unlockLevel)
        {
            ShowUnlockMessage();
            gameObject.SetActive(false); // **·úÑ¥¯Å¹F¼Ð®É¡A°±¥Î¦Û¨­**
        }
    }
    private void ShowUnlockMessage()
    {
        if (unlockMessagePrefab != null && messageParent != null)
        {
            SkillSlot skillSlot = transform.parent != null ? transform.parent.GetComponentInChildren<SkillSlot>() : null;

            if (skillSlot != null && skillSlot.skillData != null)
            {
                skillName = skillSlot.skillData.skillName; // **¨ú±o§Þ¯à¦WºÙ**
                Debug.Log($"UnLockSystem: §Þ¯à{skillName}");
            }

            // **¥Í¦¨°T®§ª«?E*
            GameObject messageObj = Instantiate(unlockMessagePrefab, messageParent);
            TextMeshProUGUI messageText = messageObj.GetComponentInChildren<TextMeshProUGUI>();
            CanvasGroup canvasGroup = messageObj.GetComponent<CanvasGroup>();

            if (messageText != null)
                messageText.text = $"¨ú±o§Þ¯à:{skillName}";
        }
    }
}
