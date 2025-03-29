using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    bool canInput = true;
    bool menu = false;
    bool press = false;

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
        if (Cursor.lockState == CursorLockMode.None && !menu)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            menu = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            menu = false;
        }

        PauseUI pauseUI = FindAnyObjectByType<PauseUI>();
        StartCoroutine(pauseUI.RunPauseUI());
    }

    private void PressESCUI()
    {
        if (Cursor.lockState == CursorLockMode.None && !press)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            press = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            press = false;
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
