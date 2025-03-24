using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameInput : MonoBehaviour
{
	public static GameInput Instance{get; private set;}

	public enum Bind
	{
		MoveUp,
		MoveDown,
		MoveLeft,
		MoveRight,
		Attack,
		Aim,
		Dash,
		Sprint,
		LockOn,
		Skill1,
		Skill2,
		ItemMenu,
		Interaction,
		Escape,
		UseItem1,
		UseItem2,
		UseItem3,
		UseItem4,
		UseItem5,
		UseItem6,
	}
	
	public readonly Dictionary<Bind, string> BindChinese = new Dictionary<Bind, string>
	{
	    { Bind.MoveUp, "前進" },
        { Bind.MoveDown, "後退" },
        { Bind.MoveLeft, "左移" },
        { Bind.MoveRight, "右移" },
        { Bind.Attack, "攻擊" },
        { Bind.Aim, "瞄準" },
        { Bind.Dash, "衝刺" },
        { Bind.Sprint, "疾跑" },
        { Bind.LockOn, "鎖定" },
        { Bind.Skill1, "技能 1" },
        { Bind.Skill2, "技能 2" },
        { Bind.ItemMenu, "道具選單" },
        { Bind.Interaction, "互動" },
        { Bind.Escape, "暫停" },
        { Bind.UseItem1, "使用道具 1" },
        { Bind.UseItem2, "使用道具 2" },
        { Bind.UseItem3, "使用道具 3" },
        { Bind.UseItem4, "使用道具 4" },
        { Bind.UseItem5, "使用道具 5" },
        { Bind.UseItem6, "使用道具 6" },
	};

	public event Action<bool> OnAttackAction;
	public event Action<bool> OnAimAction;
	public event Action OnDashAction;
    public event Action OnLockAction;
    public event Action OnItemMenu;
    public event Action<bool> OnSprintAction;
	public event Action<Bind> OnSkillAction;
	public event Action OnInteraction;
	public event Action OnEscape;
	public event Action<Bind> OnUseItem;
	
	public bool isAttackClick = false;

	private PlayerInputAction _playerInputAction;
	private const string REBIND = "ReBind";
	//private bool _canInput = true;

	private void Awake()
	{
        //確保GameInput在TitleScene會自我銷毀
        SceneManager.sceneLoaded += OnSceneLoaded;
		
		//確保GameInput在任何場景都是唯一單例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInputAction = new PlayerInputAction();
		LoadRebind();
		_playerInputAction.Player.Enable();

		BindInputActionEvent();
	}

	// 綁定 InputAction, 對應的 Event
	private void BindInputActionEvent()
	{
		var player = _playerInputAction.Player;

		player.Attack.performed += _ => OnAttackAction?.Invoke(true);
        player.Attack.canceled += _ => OnAttackAction?.Invoke(false);
        player.Aim.performed += _ => OnAimAction?.Invoke(true);
        player.Aim.canceled += _ => OnAimAction?.Invoke(false);
        player.Dash.performed += _ => OnDashAction?.Invoke();
        player.Sprint.performed += _ => OnSprintAction?.Invoke(true);
        player.Sprint.canceled += _ => OnSprintAction?.Invoke(false);
        player.LockOn.performed += _ => OnLockAction?.Invoke();
        player.Skill1.performed += _ => OnSkillAction?.Invoke(Bind.Skill1);
        player.Skill2.performed += _ => OnSkillAction?.Invoke(Bind.Skill2);
        player.ItemMenu.performed += _ => OnItemMenu?.Invoke();
        player.Interaction.performed += _ => OnInteraction?.Invoke();
        player.Escape.performed += _ => OnEscape?.Invoke();
		player.UseItem1.performed += _ => OnUseItem?.Invoke(Bind.UseItem1);
		player.UseItem2.performed += _ => OnUseItem?.Invoke(Bind.UseItem2);
		player.UseItem3.performed += _ => OnUseItem?.Invoke(Bind.UseItem3);
		player.UseItem4.performed += _ => OnUseItem?.Invoke(Bind.UseItem4);
		player.UseItem5.performed += _ => OnUseItem?.Invoke(Bind.UseItem5);
		player.UseItem6.performed += _ => OnUseItem?.Invoke(Bind.UseItem6);
	}
	
	// 取得 Bind 對應的 中文名稱
	public string GetBindChinese(Bind bind)
	{
	    return BindChinese.TryGetValue(bind, out string name) ? name : bind.ToString();
	}

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            Destroy(gameObject);
            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

	// 存檔按鍵的綁定鍵位
    private void SaveRebind()
	{
		string rebind = _playerInputAction.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString(REBIND, rebind);
		PlayerPrefs.Save();

		Debug.Log("SaveRebind Success");
	}

	// 讀取按鍵的綁定鍵位
	private void LoadRebind()
	{
		if(PlayerPrefs.HasKey(REBIND))
		{
			string rebind = PlayerPrefs.GetString(REBIND);
			_playerInputAction.LoadBindingOverridesFromJson(rebind);
			
			Debug.Log("LoadRebind Success");
		}
	}

	/// <summary>
	/// 回傳玩家移動的方向 Vector2 類型, 方向已歸一化(Normalized)
	/// </summary>
	/// <returns></returns>
	public Vector2 GetMoveVector2()
	{
		Vector2 inputVector = _playerInputAction.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;

		return inputVector;
	}
	
	public Vector3 GetMoveVector3()
	{
		Vector2 inputVector = _playerInputAction.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;
		

		return new Vector3(inputVector.x, 0f, inputVector.y);
	}

	public Vector2 GetMouseMoveVector2()
	{
		Vector2 mouseMoveVector = _playerInputAction.Player.MouseMove.ReadValue<Vector2>();

		return mouseMoveVector;
	}

	/// <summary>
	/// 取得 Bind 對應的文字名稱
	/// </summary>
	/// <param name="bind"></param>
	/// <returns></returns>
	public string GetBindText(Bind bind)
	{
		Dictionary<Bind, InputAction> bindMap = new Dictionary<Bind, InputAction>
        {
            { Bind.MoveUp, _playerInputAction.Player.Move },
            { Bind.MoveDown, _playerInputAction.Player.Move },
            { Bind.MoveLeft, _playerInputAction.Player.Move },
            { Bind.MoveRight, _playerInputAction.Player.Move },
            { Bind.Attack, _playerInputAction.Player.Attack },
            { Bind.Aim, _playerInputAction.Player.Aim },
            { Bind.Dash, _playerInputAction.Player.Dash },
            { Bind.Sprint, _playerInputAction.Player.Sprint },
			{ Bind.LockOn, _playerInputAction.Player.LockOn},
            { Bind.Skill1, _playerInputAction.Player.Skill1 },
            { Bind.Skill2, _playerInputAction.Player.Skill2 },
			{ Bind.ItemMenu, _playerInputAction.Player.ItemMenu},
            { Bind.Interaction, _playerInputAction.Player.Interaction },
            { Bind.Escape, _playerInputAction.Player.Escape },
			{ Bind.UseItem1, _playerInputAction.Player.UseItem1 },
			{ Bind.UseItem2, _playerInputAction.Player.UseItem2 },
			{ Bind.UseItem3, _playerInputAction.Player.UseItem3 },
			{ Bind.UseItem4, _playerInputAction.Player.UseItem4 },
			{ Bind.UseItem5, _playerInputAction.Player.UseItem5 },
			{ Bind.UseItem6, _playerInputAction.Player.UseItem6 }
        };

        if (bindMap.TryGetValue(bind, out InputAction action))
        {
            return action.bindings[0].ToDisplayString();
        }

        return "";
	}

	/// <summary>
	/// 重新綁定功能對應按鍵
	/// </summary>
	/// <param name="bind"></param>
	/// <param name="onActionRebound"></param>
	/// <param name="OnActionFailRebound"></param>
	public void ReBindKeyboard(Bind bind, Action onActionRebound, Action OnActionFailRebound)
	{
		_playerInputAction.Player.Disable();

		if (!TryGetActionAndIndex(bind, out InputAction inputAction, out int bindIndex))
        {
            Debug.LogError("GameInput ReBindKeyboard 輸入的資料有誤");
            return;
        }

		inputAction.PerformInteractiveRebinding(bindIndex)
			.OnComplete(callback =>
			{
				//取得新綁定的按鍵為何
				string newBind = callback.action.bindings[bindIndex].effectivePath;

				if(CheckNewBindRepeat(inputAction, newBind, bindIndex))
				{
					Debug.Log("按鍵有衝突");
					inputAction.RemoveBindingOverride(bindIndex);
					OnActionFailRebound();
				}
				else
				{
					onActionRebound();
					SaveRebind();
				}

				callback.Dispose();
				_playerInputAction.Player.Enable();
			})
			.Start();
	}

	// 取得 Bind 對應的 InputAction 以及 bindIndex
	private bool TryGetActionAndIndex(Bind bind, out InputAction inputAction, out int bindIndex)
    {
        var player = _playerInputAction.Player;

        inputAction = bind switch
        {
            Bind.MoveUp or Bind.MoveDown or Bind.MoveLeft or Bind.MoveRight => player.Move,
            Bind.Attack => player.Attack,
            Bind.Aim => player.Aim,
            Bind.Dash => player.Dash,
            Bind.Sprint => player.Sprint,
			Bind.LockOn => player.LockOn,
            Bind.Skill1 => player.Skill1,
            Bind.Skill2 => player.Skill2,
			Bind.ItemMenu => player.ItemMenu,
			Bind.Interaction => player.Interaction,
			Bind.Escape => player.Escape,
			Bind.UseItem1 => player.UseItem1,
			Bind.UseItem2 => player.UseItem2,
			Bind.UseItem3 => player.UseItem3,
			Bind.UseItem4 => player.UseItem4,
			Bind.UseItem5 => player.UseItem5,
			Bind.UseItem6 => player.UseItem6,
            _ => null
        };

        bindIndex = bind switch
        {
            Bind.MoveUp => 1,
            Bind.MoveDown => 2,
            Bind.MoveLeft => 3,
            Bind.MoveRight => 4,
            _ => 0
        };

        return inputAction != null;
    }

	/// <summary>
	/// 確認新綁定的按鍵是否有跟原本的按鍵列表重複, True 為有重複
	/// </summary>
	/// <param name="inputAction"></param>
	/// <param name="newBind"></param>
	/// <returns></returns>
	private bool CheckNewBindRepeat(InputAction inputAction, string newBind, int bindIndex)
	{
		InputActionMap inputActionMap = _playerInputAction.Player;

		foreach(InputAction action in inputActionMap)
		{
			for(int i = 0; i < action.bindings.Count; i++)
			{
				if(action == inputAction && i == bindIndex) continue;
				if(action.bindings[i].effectivePath == newBind) return true;
			}
		}

		return false;
	}
}
