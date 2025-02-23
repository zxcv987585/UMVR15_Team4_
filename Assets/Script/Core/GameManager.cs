using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManagerSingleton
{
    private GameObject gameObject;

    //單例
    private static GameManagerSingleton m_Instance;
    //接口
    public static GameManagerSingleton Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameManagerSingleton();
                m_Instance.gameObject = new GameObject("Gamemanager");
                m_Instance.gameObject.AddComponent<InputController>();
            }
            return m_Instance;
        }
    }
    //單例
    private InputController m_Inputcontrol;
    //接口
    public InputController inputControl
    {
        get
        {
            if (m_Inputcontrol == null)
            {
                m_Inputcontrol = gameObject.GetComponent<InputController>();
            }
            return m_Inputcontrol;
        }
    }
}
