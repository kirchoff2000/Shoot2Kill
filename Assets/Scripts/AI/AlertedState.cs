using FPS.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertedState : AIState
{
    //Inspector assigned
    [SerializeField] [Range(1.0f, 60.0f)] float _maxduration = 10.0f;


    //Private
    private float _timer = 0.0f;



    public override void onEnterState()
    {
        print("Enterring idle Alerted state");
        base.onEnterState();

        if (_stateMachine == null) return;
       
        _timer = _maxduration;

        _stateMachine.speed = 0;
        _stateMachine.attackType = 0;
        _stateMachine.alerted = true;       
    }


    public override AIStateType onUpdate()
    {
        _timer -= Time.deltaTime;

        if(_timer <= 0.0f)
        {
            return AIStateType.Patrol;
        }

        if (_stateMachine.VisualThreat.Type == AITargetType.Visual_Player)
        {
            _stateMachine.SetTarget(_stateMachine.VisualThreat);
            return AIStateType.Pursuit;
        }

        if (_stateMachine.VisualThreat.Type == AITargetType.Audio)
        {
            _stateMachine.SetTarget(_stateMachine.AudioThreat);
            _timer = _maxduration;
        }

        if (_stateMachine.VisualThreat.Type == AITargetType.Visual_Light)
        {
            _stateMachine.SetTarget(_stateMachine.VisualThreat);
            _timer = _maxduration;
        }


        return AIStateType.Alerted;
    }


    public override AIStateType GetStateType()
    {
        return AIStateType.Alerted;
    }
}
