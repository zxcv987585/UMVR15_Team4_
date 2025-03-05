using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManagerSingleton
{
    private GameObject gameObject;

    private static GameManagerSingleton m_Instance;
    public static GameManagerSingleton Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameManagerSingleton();
                m_Instance.gameObject = new GameObject("Gamemanager");
                m_Instance.gameObject.AddComponent<InputController>();
                m_Instance.m_Inputcontrol = m_Instance.gameObject.GetComponent<InputController>();
            }
            return m_Instance;
        }
    }
    private InputController m_Inputcontrol;
    public InputController InputControl
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
