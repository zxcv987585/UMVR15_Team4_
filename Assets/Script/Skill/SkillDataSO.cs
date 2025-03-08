using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
	public string skillName;
	public Sprite skillIcon;
	public int cooldownTime;
	public int damage;
	public bool isUnlocked;
	public string weapon;
	public float PPCost;
}