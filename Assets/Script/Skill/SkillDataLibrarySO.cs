using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/SkillDataLibrarySO")]
public class SkillDataLibrarySO : ScriptableObject
{
    [System.Serializable]
    public class SkillData
    {
        public SkillDataSO skillDataSO;
        public BaseSkill skillPrefab;
    }
    
    [SerializeField] private List<SkillData> skillDataList;
    
    // 傳入 SkillDataSO, 取得對應的 Prefab
    public BaseSkill GetSkillPrefab(SkillDataSO skillDataSO)
    {
        foreach(SkillData skillData in skillDataList)
        {
            if(skillData.skillDataSO == skillDataSO)
            {
                return skillData.skillPrefab;
            }
        }
        
        Debug.Log(" SkillDataLibrarySO/GetSkillPrefab 目前傳入的 SkillDataSO 沒有對應的 Prefab 物件");
        return null;
    }
}
