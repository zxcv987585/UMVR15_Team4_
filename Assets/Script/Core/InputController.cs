using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    bool canInput = true;
    private void Awake()
    {
        GameInput.Instance.OnItemMenu += ItemMenu;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ItemMenu()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        CheckCursorstate();
    }
    //確認滑鼠狀態是否為隱藏，確保每次輸入不會因為其他狀態而出現異常
    public bool CanProcessInput() => Cursor.lockState == CursorLockMode.Locked && canInput;
    //獲得奔跑輸入
    public bool GetSprintInput() => Input.GetKey(KeyCode.LeftShift) ;
    //獲得滑鼠水平輸入
    public float GetMouseXAxis() => CanProcessInput() ? Input.GetAxis("Mouse X") : 0;
    //獲得滑鼠垂直輸入
    public float GetMouseYAxis() => CanProcessInput() ? Input.GetAxis("Mouse Y") : 0;
    //獲得滑鼠滾輪輸入
    public float GetMouseScrollAxis() => CanProcessInput() ? Input.GetAxis("Mouse ScrollWheel") : 0;
    //是否按下滑鼠左鍵
    public bool GetMouseLeftInputHold() => CanProcessInput() ? Input.GetMouseButton(0) : false;
    //是否放開滑鼠左鍵
    public bool GetMouseLeftInputUp() => CanProcessInput() ? Input.GetMouseButtonUp(0) : false;
    //獲得滑鼠右鍵輸入
    public bool GetAimInputDown() => CanProcessInput() ? Input.GetMouseButtonDown(1) : false;
    //獲得滑鼠滾輪按鍵輸入
    public bool GetMouseScrollInput() => CanProcessInput() ? Input.GetMouseButtonUp(2) : false;
    //獲得空白鍵輸入
    public bool GetSpaceInput() => CanProcessInput() ? Input.GetKeyDown(KeyCode.Space) : false;
    //確認滑鼠當前狀態（如果按下ESC就顯示或隱藏滑鼠）
    private void CheckCursorstate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    //獲得移動輸入
    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }
}
