using FPS.AI;
using UnityEngine;

public class IdleState : AIState
{

    [SerializeField] private Vector2 _idleTimeRange = new Vector2(10.0f, 60.0f);

    //private
    float _idleTime = 0.0f;
    float _timer = 0.0f;
   

    public override void onEnterState()
    {
        print("Enterring idle State");
        base.onEnterState();

        if (_stateMachine == null) return;

        _idleTime = Random.Range(_idleTimeRange.x, _idleTimeRange.y);
        _timer = 0.0f;

        _stateMachine.speed = 0;
        _stateMachine.attackType = 0;       
        _stateMachine.alerted = false;

        _stateMachine.ClearTarget();
    }


    public override AIStateType GetStateType()
    {
        return AIStateType.Idle;
    }

    public override AIStateType onUpdate()
    {
        if (_stateMachine == null) return AIStateType.Idle;

        if (_stateMachine.VisualThreat.Type == AITargetType.Visual_Player)
        {
            _stateMachine.SetTarget(_stateMachine.VisualThreat);
            return AIStateType.Pursuit;
        }

        if (_stateMachine.VisualThreat.Type == AITargetType.Visual_Light)
        {
            _stateMachine.SetTarget(_stateMachine.VisualThreat);
            return AIStateType.Alerted;
        }

        if (_stateMachine.VisualThreat.Type == AITargetType.Audio)
        {
            _stateMachine.SetTarget(_stateMachine.AudioThreat);
            return AIStateType.Alerted;
        }

        _timer += Time.deltaTime;
        if (_timer > _idleTime)
        {           
            return AIStateType.Patrol;
        }

        return AIStateType.Idle;
    }
}


