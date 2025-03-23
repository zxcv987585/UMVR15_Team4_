using UnityEngine;

public class BossController : MonoBehaviour
{
    private Material bossMaterial;
    private float dissolveProgress = 0f;
    private bool isBossDead = false;

    void Start()
    {
        bossMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (isBossDead)
        {
            dissolveProgress += Time.deltaTime * 0.5f; //控制浮現速度
            bossMaterial.SetFloat("_DissolveAmount", Mathf.Clamp01(dissolveProgress));
        }
    }

    //這個方法應該在 BOSS 死亡時被呼叫！
    public void OnBossDeath()
    {
        isBossDead = true;
    }
}
