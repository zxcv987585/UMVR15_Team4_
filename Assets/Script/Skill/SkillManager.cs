using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	public static SkillManager Instance {get; private set;}
	
	[SerializeField] private SkillDataLibrarySO skillDataLibrarySO;
	
	private Dictionary<GameInput.Bind, BaseSkill> skillBind;
	private PlayerController player;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
	{
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        skillBind = new Dictionary<GameInput.Bind, BaseSkill>();
		
		GameInput.Instance.OnSkillAction += UseSkill;
	}

	// 當點擊按鍵時, 呼叫該按鍵綁定的技能
	private void UseSkill(GameInput.Bind bind)
	{
		if (player.isSkilling || player.isAiming) return;

		if(skillBind.TryGetValue(bind, out BaseSkill baseSkill))
		{
			if(baseSkill.canUse)
			{
				player.CastSkill(baseSkill.AnimationName, baseSkill.CastDurtion);
				baseSkill.Use();
			}
		}
    }

	// 綁定 Skill 對應的技能
    public void SetSkillBind(GameInput.Bind bind, SkillDataSO skillDataSO)
	{
		// 如果目前該按鍵已有綁定, 則先清除元件
		if(skillBind.TryGetValue(bind, out BaseSkill baseSkill))
		{
			Destroy(baseSkill.gameObject);
		}

		GameObject baseSkillPrefab = Instantiate(skillDataLibrarySO.GetSkillPrefab(skillDataSO), transform);
		skillBind[bind] = baseSkillPrefab.GetComponent<BaseSkill>();
	}
	
	public void RemoveSkillBind(GameInput.Bind bind)
	{
		skillBind.Remove(bind);
	}
}