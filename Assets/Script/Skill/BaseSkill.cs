using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
	[SerializeField] protected SkillDataSO skillDataSO;
	[SerializeField] protected ParticleSystem skillParticleSystem;
	
	public bool canUse = true;
	public const string ENEMY = "Enemy";
	public string AnimationName;
	public float CastDurtion;
	public float PPCost;

    public virtual void Use()
	{
		SkillAbility();
		StartCooldown();
	}

	public abstract void SkillAbility();
	
	public virtual SkillDataSO GetSkillDataSO() => skillDataSO;

	public void StartCooldown() => StartCoroutine(CooldownCoroutine());

	public IEnumerator CooldownCoroutine()
	{
		canUse = false;

		yield return new WaitForSeconds(skillDataSO.cooldownTime);

		canUse = true;
	}
}
