using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillManager : MonoBehaviour
{
	public static SkillManager Instance {get; private set;}
	
	[SerializeField] private SkillDataLibrarySO skillDataLibrarySO;
	
	private Dictionary<GameInput.Bind, BaseSkill> skillBind;
	private PlayerController player;
	private PlayerHealth playerHealth;
	private BossSceneDialogue bossScene;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
	{
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        bossScene = FindAnyObjectByType<BossSceneDialogue>();

        skillBind = new Dictionary<GameInput.Bind, BaseSkill>();
		
		GameInput.Instance.OnSkillAction += UseSkill;
	}

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            GameInput.Instance.OnSkillAction -= UseSkill;
            Destroy(gameObject);
            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 當點擊按鍵時, 呼叫該按鍵綁定的技能
    private void UseSkill(GameInput.Bind bind)
    {
		if (player.IsSkilling || player.IsAiming || player.IsDie || player.IsCriticalHit || player.IsHit || player.IsRivive || player.IsTeleporting || bossScene.IsTalk || UIManager.CurrentState != UIState.None) return;

		if(skillBind.TryGetValue(bind, out BaseSkill baseSkill))
		{
			if(baseSkill.canUse)
			{
				if (!playerHealth.UsePP(baseSkill.ppCost))
				{
					Debug.Log("PP不足!");
					return;
				}
                player.CastSkill(baseSkill.animationName, baseSkill.castDurtion);
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

		AudioManager.Instance.PlaySound("EquipSkill", transform.position);
	}
	
	public void RemoveSkillBind(GameInput.Bind bind)
	{
		skillBind.Remove(bind);
	}
}