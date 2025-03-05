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


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
	{
		skillBind = new Dictionary<GameInput.Bind, BaseSkill>();
		
		GameInput.Instance.OnSkillAction += UseSkill;
	}

	private void UseSkill(GameInput.Bind bind)
	{
		if(skillBind.TryGetValue(bind, out BaseSkill baseSkill))
		{
			if(baseSkill.canUse)
			{
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
}
