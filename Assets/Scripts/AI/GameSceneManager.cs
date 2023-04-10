using FPS.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();

    private static GameSceneManager _instance = null;

    public static GameSceneManager Instance
    {
        get 
        { 
            if (_instance == null)
            {
                _instance = (GameSceneManager)FindObjectOfType<GameSceneManager>();
                return _instance;
            }
            return _instance;

        } 
    }


    public void RegisterAIStatesMachine(int key, AIStateMachine stateMachine)
    {

        if (!_stateMachines.ContainsKey(key))
        {
            _stateMachines[key] = stateMachine;
        }       
    }

    public AIStateMachine GetAIStateMachine(int key)
    {
        AIStateMachine machine = null;
        if (_stateMachines.TryGetValue(key, out machine))
        {
            return machine;
        }
        return null;
    }
}
