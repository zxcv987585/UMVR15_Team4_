using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	[SerializeField] private List<SkillSingleUI> skillSingleUIList;
	[SerializeField] private List<BaseSkill> baseSkillList;
	
	private Dictionary<GameInput.Bind, (BaseSkill skill, SkillSingleUI ui)> skillBind;
	
	private void Start()
	{
		skillBind = new Dictionary<GameInput.Bind, (BaseSkill skill, SkillSingleUI ui)>();
		
		GameInput.Instance.OnSkillAction += UseSkill;
		InitSkillBind();

		SetSkillBind(GameInput.Bind.Skill1, baseSkillList[0]);
		SetSkillBind(GameInput.Bind.Skill2, baseSkillList[1]);
	}

	private void UseSkill(GameInput.Bind bind)
	{
		if(skillBind.TryGetValue(bind, out var data))
		{
			if(data.skill != null)
			{
				if(data.skill.canUse)
				{
					data.skill.Use();

            		data.ui.StarCoolDown(data.skill.GetSkillDataSO().cooldownTime);
				}
			}
        }
    }

	private void InitSkillBind()
	{
		skillBind.Add(GameInput.Bind.Skill1, (default , skillSingleUIList[0]));
		skillBind.Add(GameInput.Bind.Skill2, (default , skillSingleUIList[1]));
		skillBind.Add(GameInput.Bind.Skill3, (default , skillSingleUIList[2]));
		skillBind.Add(GameInput.Bind.Skill4, (default , skillSingleUIList[3]));
	}

    public void SetSkillBind(GameInput.Bind bind, BaseSkill baseSkill)
	{
		BaseSkill newSkill = Instantiate(baseSkill,transform);
		SkillSingleUI skillSingleUI = skillBind[bind].ui;
		skillSingleUI.UpdateVisual(newSkill.GetSkillDataSO(), bind);

		skillBind[bind] = (newSkill, skillSingleUI);
	}
}
