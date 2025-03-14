using System.Collections;
using UnityEngine;

public class Skill_Spikes : BaseSkill
{
    private float radius = 5f;
    private float hitFlyPower = 5f;
    
    public override void SkillAbility()
    {
        StartCoroutine(DelaySkillEffect(0.7f));
    }

    private IEnumerator DelaySkillEffect(float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = FindObjectOfType<PlayerController>()?.transform.position ?? Vector3.zero;

        skillParticleSystem.Play();

        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliderArray)
        {
            if (hit.TryGetComponent(out EnemyController enemy))
            {
                enemy.HitFly(hitFlyPower);
            }
            
            if(hit.TryGetComponent(out Health health))
            {
                health.TakeDamage(GetSkillDataSO().damage);
            }
        }
    }
}
