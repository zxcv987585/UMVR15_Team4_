using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIState { None, Menu, Pause, Rebirth, MissionFail }
public static class UIManager
{
    public static UIState CurrentState = UIState.None;
}
public class InputController : MonoBehaviour
{
    bool canInput = true;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "TitleScene")
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnItemMenu += ItemMenu;
            GameInput.Instance.OnEscape += PressESCUI;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "TitleScene")
        {
            return;
        }
        if (scene.name == "TitleScene")
        {
            GameInput.Instance.OnItemMenu -= ItemMenu;
            GameInput.Instance.OnEscape -= PressESCUI;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ItemMenu()
    {
        if (UIManager.CurrentState == UIState.Rebirth || UIManager.CurrentState == UIState.MissionFail)
        {
            return;
        }

        if (UIManager.CurrentState != UIState.None && UIManager.CurrentState != UIState.Menu)
        {
            return;
        }

        if (UIManager.CurrentState == UIState.None)
        {
            UIManager.CurrentState = UIState.Menu;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (UIManager.CurrentState == UIState.Menu)
        {
            UIManager.CurrentState = UIState.None;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        PauseUI pauseUI = FindAnyObjectByType<PauseUI>();
        StartCoroutine(pauseUI.RunPauseUI());
    }

    private void PressESCUI()
    {
        if (UIManager.CurrentState == UIState.Rebirth || UIManager.CurrentState == UIState.MissionFail)
        {
            return;
        }
        if (UIManager.CurrentState != UIState.None && UIManager.CurrentState != UIState.Pause)
        {
            return;
        }

        if (UIManager.CurrentState == UIState.None)
        {
            UIManager.CurrentState = UIState.Pause;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (UIManager.CurrentState == UIState.Pause)
        {
            UIManager.CurrentState = UIState.None;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        StopUI stopUI = FindAnyObjectByType<StopUI>();
        stopUI.PressESCUI();
    }

    //確認滑鼠狀態是否為隱藏，確保每次輸入不會因為其他狀態而出現異常
    public bool CanProcessInput() => Cursor.lockState == CursorLockMode.Locked && canInput;
    public float GetMouseXAxis() => CanProcessInput() ? Input.GetAxis("Mouse X") : 0;
    //獲得滑鼠垂直輸入
    public float GetMouseYAxis() => CanProcessInput() ? Input.GetAxis("Mouse Y") : 0;
    //獲得滑鼠滾輪輸入
    public float GetMouseScrollAxis() => CanProcessInput() ? Input.GetAxis("Mouse ScrollWheel") : 0;
    //獲得空白鍵輸入
    public bool GetSpaceInput() => CanProcessInput() ? Input.GetKeyDown(KeyCode.Space) : false;

    //獲得移動輸入
    public Vector3 GetMoveInput()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }
}
