using FPS.AI;
using UnityEngine;

public class AISensor : MonoBehaviour
{ 
    
    private AIStateMachine _parentStateMachine = null;
    public AIStateMachine parentStateMachine { set { _parentStateMachine = value; } }

    private void OnTriggerEnter(Collider other)
    {        
        if (_parentStateMachine != null)
        {
            _parentStateMachine.onTriggerEvent(AITriggerEventType.Enter, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_parentStateMachine != null)
        {
            _parentStateMachine.onTriggerEvent(AITriggerEventType.Stay, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_parentStateMachine != null)
        {
            _parentStateMachine.onTriggerEvent(AITriggerEventType.Exit, other);
        }
    }
}
