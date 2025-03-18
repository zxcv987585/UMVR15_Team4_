using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
	[SerializeField] private Button startButton;
	[SerializeField] private Button settingButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private OptionUI optionUI;

	private const string BUTTON_SELECT = "ButtonSelect";
	private const float SELECT_SCALE_Number = 1.2f;
	private GameObject selectButton;

	private void Start()
	{
		//幫按鈕追加選取及取消選取的事件
		ButtonAddSelectEvent(startButton, ButtonUISelect, ButtonUIDeSelect);
		ButtonAddSelectEvent(settingButton, ButtonUISelect, ButtonUIDeSelect);
		ButtonAddSelectEvent(exitButton, ButtonUISelect, ButtonUIDeSelect);
		
		//讓預設選取的按鈕為開始遊戲按鍵
		selectButton = startButton.gameObject;
		EventSystem.current.SetSelectedGameObject(selectButton);

		//加上各按鈕被點擊後的事件
		startButton.onClick.AddListener(() =>
		{
			LoadManager.Load(LoadManager.Scene.Battle01);
		});
		settingButton.onClick.AddListener(() => 
		{
			optionUI.Show();
		});
		exitButton.onClick.AddListener(() => 
		{
			Application.Quit();
		});
	}

	private void Update()
	{
		//如果遇上取消選取按鈕的事件, 則會選擇最後一個選取按鈕, 好保留最少有一個選取按鈕
		if(EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(selectButton);
		}
	}

	/// <summary>
	/// UI.Button 自定義函式, 用來添加 Button 被選取及取消選取事件
	/// </summary>
	/// <param name="button"></param>
	/// <param name="selectAction">選取時要新增的事件</param>
	/// <param name="deSelectAction">取消選取時要新增的事件</param>
	private void ButtonAddSelectEvent(Button button, Action<Button> selectAction, Action<Button> deSelectAction)
	{
		EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();

		// Select 事件
		EventTrigger.Entry onSelectEntry = new EventTrigger.Entry{
			eventID = EventTriggerType.Select
		};
		onSelectEntry.callback.AddListener((eventDate) =>
		{
			selectAction(button);
		});

		// Deselect 事件
		EventTrigger.Entry onDeselectEntry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.Deselect
		};
		onDeselectEntry.callback.AddListener((eventData) =>
		{
			deSelectAction(button);
		});

		// PointEnter 事件
		EventTrigger.Entry onPointEnter = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerEnter
		};
		onPointEnter.callback.AddListener((eventData) => 
		{
			selectAction(button);
		});

		// PointExit 事件
		EventTrigger.Entry onPointerExit = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerExit
		};
		onPointerExit.callback.AddListener((eventData) =>
		{
			deSelectAction(button);
		});

		// 添加事件到 EventTrigger
		eventTrigger.triggers.Add(onSelectEntry);
		eventTrigger.triggers.Add(onDeselectEntry);
		eventTrigger.triggers.Add(onPointEnter);
		eventTrigger.triggers.Add(onPointerExit);
	}

	private void ButtonUISelect(Button button)
	{
		//當新選取的按鈕與原本不同則撥放切換音效
		if(selectButton != button.gameObject)
		{
			AudioManager.Instance.PlaySound(BUTTON_SELECT,Vector3.zero);
		}

		//將選取的按鈕放大
		selectButton = button.gameObject;
		button.transform.localScale = Vector3.one * SELECT_SCALE_Number;
	}

	private void ButtonUIDeSelect(Button button)
	{
		button.transform.localScale = Vector3.one;
	}
}
