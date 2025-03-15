using System.IO;
using UnityEngine;

public class MapScreenshot : MonoBehaviour
{
    public Camera mapCamera; // 指定你的正投影攝影機
    public int imageWidth = 2048; // 設定圖片寬度
    public int imageHeight = 2048; // 設定圖片高度
    public string fileName = "MapScreenshot.png"; // 輸出的圖片名稱

    public void CaptureMap()
    {
        if (mapCamera == null)
        {
            Debug.LogError("請指定一個攝影機！");
            return;
        }

        // **建立 RenderTexture**，讓攝影機渲染到這個貼圖上
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 24);
        mapCamera.targetTexture = rt;
        mapCamera.Render();

        // **將 RenderTexture 轉換成 Texture2D**
        Texture2D screenShot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenShot.Apply();

        // **儲存成 PNG**
        byte[] bytes = screenShot.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        // **清除 RenderTexture**
        mapCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        Debug.Log($"地圖截圖已儲存：{filePath}");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CaptureMap(); // 按下 P 鍵時截圖
        }
    }
}
