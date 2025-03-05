using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Spikes : BaseSkill
{
    private float radius = 5f;
    private float hitFlyPower = 5f;
    public AnimatorOverrideController overrideController;
    
    public override void SkillAbility()
    {
        transform.position = FindObjectOfType<PlayerController>()?.transform.position ?? Vector3.zero;
        skillParticleSystem.Play();

        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask(ENEMY));
        foreach (Collider hit in hitColliderArray)
        {
            if (hit.TryGetComponent(out EnemyController enemy))
            {
                enemy.HitFly(hitFlyPower);
                enemy.gameObject.GetComponent<Health>().TakeDamage(GetSkillDataSO().damage);
            }
        }
    }
}
