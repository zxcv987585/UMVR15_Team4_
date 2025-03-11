using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Judgement_Cut : BaseSkill
{
    private float radius = 12f;

    public override void SkillAbility()
    {
        StartCoroutine(Spatial_Section(1f));
    }
    
    private IEnumerator Spatial_Section(float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = FindObjectOfType<PlayerController>()?.transform.position ?? Vector3.zero;
        skillParticleSystem.Play();

        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask(ENEMY));
        foreach (Collider hit in hitColliderArray)
        {
            if (hit.TryGetComponent(out EnemyController enemy))
            {
                EnemyManager.Instance.SetEnemyIsPause(2f);
                StartCoroutine(WaitToAttack());
            }
        }
    }
    private IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(2f);
        EnemyManager.Instance.TakeAllEnemyDamage(skillDataSO.damage);
    }
}
