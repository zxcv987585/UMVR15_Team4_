using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSingleUI : MonoBehaviour
{
	[SerializeField] private Image skillImage;
	[SerializeField] private Image cooldownImage;
	[SerializeField] private TextMeshProUGUI useButtonText;
	
	private SkillDataSO skillDataSO;
	private Coroutine cooldownCoroutine;
	
	private void Start()
	{
	}
	
	public void UpdateVisual(SkillDataSO skillDataSO, GameInput.Bind bind)
	{
		this.skillDataSO = skillDataSO;
		
		skillImage.sprite = skillDataSO.skillIcon;
		skillImage.enabled = true;

		useButtonText.text = GameInput.Instance.GetBindText(bind);
	}
	
	/// <summary>
	/// 跑技能格子的冷卻動畫
	/// </summary>
	/// <param name="cooldownTime">冷卻時間</param>
	public void StarCoolDown(float cooldownTime)
	{
		if(cooldownCoroutine != null)
		{
			StopCoroutine(cooldownCoroutine);
		}
		cooldownCoroutine = StartCoroutine(CoolDownCoroutine(cooldownTime));
	}

	private IEnumerator CoolDownCoroutine(float cooldownTime)
	{
		cooldownImage.fillAmount = 1f;
		float timer = 0f;
		
		while(timer < cooldownTime)
		{
			timer += Time.deltaTime;
			cooldownImage.fillAmount = 1 - (timer / cooldownTime);
			
			yield return null;
		}
		
		cooldownImage.fillAmount = 0f;
	}
}
