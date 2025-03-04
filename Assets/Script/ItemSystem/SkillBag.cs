using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/SkillBag")]
public class SkillBag : ScriptableObject
{
    public List<SkillDataSO> skillList = new List<SkillDataSO>();
}
