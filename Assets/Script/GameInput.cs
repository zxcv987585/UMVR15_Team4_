using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
		Skill1,
		Skill2,
		Skill3,
		Skill4
	}

	public event Action<bool> OnAttackAction;
	public event Action<bool> OnAimAction;
	public event Action OnDashkAction;
	public event Action<bool> OnSprintAction;
	public event Action<Bind> OnSkillAction;
	
	public bool isAttackClick = false;

	private PlayerInputAction playerInputAction;
	private const string REBIND = "ReBind";
	private bool canInput = true;

	private void Awake()
	{
		Instance = this;

		playerInputAction = new PlayerInputAction();
		LoadRebind();
		playerInputAction.Player.Enable();

		playerInputAction.Player.Attack.performed += AttackPress_Performed;
		playerInputAction.Player.Attack.canceled += AttackRelease_Performed;
		//playerInputAction.Player.Attack.performed += ctx => OnAttackAction?.Invoke(this, EventArgs.Empty);
		playerInputAction.Player.Aim.performed += AimPress_performed;
		playerInputAction.Player.Aim.canceled += AimRelease_Performed;
		playerInputAction.Player.Dash.performed += Dash_performed;
		playerInputAction.Player.Sprint.performed += SprintPress_performed;
		playerInputAction.Player.Sprint.canceled += SprintRelease_performed;
		playerInputAction.Player.Skill1.performed += Skill1_performed;
		playerInputAction.Player.Skill2.performed += Skill2_performed;
		playerInputAction.Player.Skill3.performed += Skill3_performed;
		playerInputAction.Player.Skill4.performed += Skill4_performed;
	}

    private void AttackRelease_Performed(InputAction.CallbackContext context)
    {
        if(canInput)
		{
			OnAttackAction?.Invoke(true);
		}
    }

    private void AttackPress_Performed(InputAction.CallbackContext context)
	{
		if(canInput)
		{
			OnAttackAction?.Invoke(false);
		}
	}

	private void AimPress_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnAimAction?.Invoke(true);
	}
	
	private void AimRelease_Performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnAimAction?.Invoke(false);
	}

	private void Dash_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnDashkAction?.Invoke();
	}
	
	private void SprintPress_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnSprintAction?.Invoke(true);
	}
	
	private void SprintRelease_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnSprintAction?.Invoke(false);
	}

	private void Skill1_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnSkillAction?.Invoke(Bind.Skill1);
	}

	private void Skill2_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnSkillAction?.Invoke(Bind.Skill2);
	}

	private void Skill3_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnSkillAction?.Invoke(Bind.Skill3);
	}

	private void Skill4_performed(InputAction.CallbackContext context)
	{
		if(canInput)
			OnSkillAction?.Invoke(Bind.Skill4);
	}

	private void SaveRebind()
	{
		string rebind = playerInputAction.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString(REBIND, rebind);
		PlayerPrefs.Save();

		Debug.Log("SaveRebind Success");
	}

	private void LoadRebind()
	{
		if(PlayerPrefs.HasKey(REBIND))
		{
			string rebind = PlayerPrefs.GetString(REBIND);
			playerInputAction.LoadBindingOverridesFromJson(rebind);
			
			Debug.Log("LoadRebind Success");
		}
	}

	/// <summary>
	/// 回傳玩家移動的方向 Vector2 類型, 方向已歸一化(Normalized)
	/// </summary>
	/// <returns></returns>
	public Vector2 GetMoveVector2()
	{
		Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;

		return inputVector;
	}
	
	public Vector3 GetMoveVector3()
	{
		Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;
		

		return new Vector3(inputVector.x, 0f, inputVector.y);
	}

	public Vector2 GetMouseMoveVector2()
	{
		Vector2 mouseMoveVector = playerInputAction.Player.MouseMove.ReadValue<Vector2>();

		return mouseMoveVector;
	}

	public string GetBindText(Bind bind)
	{
		switch (bind)
		{
			default:
				return "";
			case Bind.MoveUp:
				return playerInputAction.Player.Move.bindings[1].ToDisplayString();
			case Bind.MoveDown:
				return playerInputAction.Player.Move.bindings[2].ToDisplayString();
			case Bind.MoveLeft:
				return playerInputAction.Player.Move.bindings[3].ToDisplayString();
			case Bind.MoveRight:
				return playerInputAction.Player.Move.bindings[4].ToDisplayString();
			case Bind.Attack:
				return playerInputAction.Player.Attack.bindings[0].ToDisplayString();
			case Bind.Aim:
				return playerInputAction.Player.Aim.bindings[0].ToDisplayString();
			case Bind.Dash:
				return playerInputAction.Player.Dash.bindings[0].ToDisplayString();;
			case Bind.Sprint:
				return playerInputAction.Player.Sprint.bindings[0].ToDisplayString();;
			case Bind.Skill1:
				return playerInputAction.Player.Skill1.bindings[0].ToDisplayString();;
			case Bind.Skill2:
				return playerInputAction.Player.Skill2.bindings[0].ToDisplayString();;
			case Bind.Skill3:
				return playerInputAction.Player.Skill3.bindings[0].ToDisplayString();;
			case Bind.Skill4:
				return playerInputAction.Player.Skill4.bindings[0].ToDisplayString();;
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
		playerInputAction.Player.Disable();

		InputAction inputAction;
		int bindIndex;

		switch (bind)
		{
			case Bind.MoveUp:
				inputAction = playerInputAction.Player.Move;
				bindIndex = 1;
				break;
			case Bind.MoveDown:
				inputAction = playerInputAction.Player.Move;
				bindIndex = 2;
				break;
			case Bind.MoveLeft:
				inputAction = playerInputAction.Player.Move;
				bindIndex = 3;
				break;
			case Bind.MoveRight:
				inputAction = playerInputAction.Player.Move;
				bindIndex = 4;
				break;
			case Bind.Attack:
				inputAction = playerInputAction.Player.Attack;
				bindIndex = 0;
				break;
			case Bind.Aim:
				inputAction = playerInputAction.Player.Aim;
				bindIndex = 0;
				break;
			case Bind.Dash:
				inputAction = playerInputAction.Player.Dash;
				bindIndex = 0;
				break;
			case Bind.Sprint:
				inputAction = playerInputAction.Player.Sprint;
				bindIndex = 0;
				break;
			case Bind.Skill1:
				inputAction = playerInputAction.Player.Skill1;
				bindIndex = 0;
				break;
			case Bind.Skill2:
				inputAction = playerInputAction.Player.Skill2;
				bindIndex = 0;
				break;
			case Bind.Skill3:
				inputAction = playerInputAction.Player.Skill3;
				bindIndex = 0;
				break;
			case Bind.Skill4:
				inputAction = playerInputAction.Player.Skill4;
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
				playerInputAction.Player.Enable();
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
		InputActionMap inputActionMap = playerInputAction.Player;

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
