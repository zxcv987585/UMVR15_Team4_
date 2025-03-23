using System;
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
		Escape
	}

	public event Action<bool> OnAttackAction;
	public event Action<bool> OnAimAction;
	public event Action OnDashAction;
    public event Action OnLockAction;
    public event Action OnItemMenu;
    public event Action<bool> OnSprintAction;
	public event Action<Bind> OnSkillAction;
	public event Action OnInteraction;
	public event Action OnEscape;
	
	public bool isAttackClick = false;

	private PlayerInputAction _playerInputAction;
	private const string REBIND = "ReBind";
	private bool _canInput = true;

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

		player.Attack.performed += ctx => OnAttackAction?.Invoke(true);
        player.Attack.canceled += ctx => OnAttackAction?.Invoke(false);
        player.Aim.performed += ctx => OnAimAction?.Invoke(true);
        player.Aim.canceled += ctx => OnAimAction?.Invoke(false);
        player.Dash.performed += ctx => OnDashAction?.Invoke();
        player.Sprint.performed += ctx => OnSprintAction?.Invoke(true);
        player.Sprint.canceled += ctx => OnSprintAction?.Invoke(false);
        player.LockOn.performed += ctx => OnLockAction?.Invoke();
        player.Skill1.performed += ctx => OnSkillAction?.Invoke(Bind.Skill1);
        player.Skill2.performed += ctx => OnSkillAction?.Invoke(Bind.Skill2);
        player.ItemMenu.performed += ctx => OnItemMenu?.Invoke();
        player.Interaction.performed += ctx => OnInteraction?.Invoke();
        player.Escape.performed += ctx => OnEscape?.Invoke();
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

	public string GetBindText(Bind bind)
	{
		switch (bind)
		{
			default:
				return "";
			case Bind.MoveUp:
				return _playerInputAction.Player.Move.bindings[1].ToDisplayString();
			case Bind.MoveDown:
				return _playerInputAction.Player.Move.bindings[2].ToDisplayString();
			case Bind.MoveLeft:
				return _playerInputAction.Player.Move.bindings[3].ToDisplayString();
			case Bind.MoveRight:
				return _playerInputAction.Player.Move.bindings[4].ToDisplayString();
			case Bind.Attack:
				return _playerInputAction.Player.Attack.bindings[0].ToDisplayString();
			case Bind.Aim:
				return _playerInputAction.Player.Aim.bindings[0].ToDisplayString();
			case Bind.Dash:
				return _playerInputAction.Player.Dash.bindings[0].ToDisplayString();;
			case Bind.Sprint:
				return _playerInputAction.Player.Sprint.bindings[0].ToDisplayString();;
			case Bind.Skill1:
				return _playerInputAction.Player.Skill1.bindings[0].ToDisplayString();;
			case Bind.Skill2:
				return _playerInputAction.Player.Skill2.bindings[0].ToDisplayString();;
		}
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

		InputAction inputAction;
		int bindIndex;

		switch (bind)
		{
			case Bind.MoveUp:
				inputAction = _playerInputAction.Player.Move;
				bindIndex = 1;
				break;
			case Bind.MoveDown:
				inputAction = _playerInputAction.Player.Move;
				bindIndex = 2;
				break;
			case Bind.MoveLeft:
				inputAction = _playerInputAction.Player.Move;
				bindIndex = 3;
				break;
			case Bind.MoveRight:
				inputAction = _playerInputAction.Player.Move;
				bindIndex = 4;
				break;
			case Bind.Attack:
				inputAction = _playerInputAction.Player.Attack;
				bindIndex = 0;
				break;
			case Bind.Aim:
				inputAction = _playerInputAction.Player.Aim;
				bindIndex = 0;
				break;
			case Bind.Dash:
				inputAction = _playerInputAction.Player.Dash;
				bindIndex = 0;
				break;
			case Bind.Sprint:
				inputAction = _playerInputAction.Player.Sprint;
				bindIndex = 0;
				break;
			case Bind.Skill1:
				inputAction = _playerInputAction.Player.Skill1;
				bindIndex = 0;
				break;
			case Bind.Skill2:
				inputAction = _playerInputAction.Player.Skill2;
				bindIndex = 0;
				break;
			default:
				inputAction = null;
				bindIndex = 0;
				Debug.LogError("GameInput ReBindKeyboard 輸入的資料有誤");
				break;
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

				callback.Dispose();
				_playerInputAction.Player.Enable();
				onActionRebound();
				SaveRebind();
			})
			.Start();

		
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

				if(action.bindings[i].effectivePath == newBind)
				{
					return true;
				}
			}
		}

		return false;
	}
}
