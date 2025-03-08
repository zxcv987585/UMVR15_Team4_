using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentstate;
    private List<PlayerState> States = new List<PlayerState>();


    public void Initialize(PlayerState initialState)
    {
        currentstate = initialState;
        currentstate.Enter();
    }

    public void Update()
    {
        if (currentstate != null)
        {
            currentstate.Update();
        }
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentstate != null)
        {
            currentstate.Exit();
        }
        currentstate = newState;
        currentstate.Enter();
    }

    public T GetState<T>() where T : PlayerState
    {
        return currentstate as T;
    }
}