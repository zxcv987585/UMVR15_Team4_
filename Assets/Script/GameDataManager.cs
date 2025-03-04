using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance{get; private set;}

    public GameData gameData;

    private string savePath;

    private void Awake()
    {
        Instance = this;

        savePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, false);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if(File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
        }
    }

    //---------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // 按 ESC 切換模式
        {
            ToggleCursorMode();
        }
    }

    void ToggleCursorMode()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None; // 解除鎖定
            Cursor.visible = true; // 顯示游標
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // 鎖定滑鼠
            Cursor.visible = false; // 隱藏游標
        }
    }
}
