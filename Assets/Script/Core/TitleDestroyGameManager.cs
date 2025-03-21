using UnityEngine;

public class TitleDestroyGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameManagerSingleton.Instance != null)
        {
            GameManagerSingleton.Instance.DestoryGameManager();
        }
    }
}
