using FPS.AI;
using UnityEngine;
using UnityEngine.AI;


public class PatrolState : AIState
{

    //Inspector assigne
    [SerializeField] private AIWaypointNetwork _waypointNetwork = null;
    [SerializeField] private bool _randomPatrol = false;
    [SerializeField] private int _currentWaypoint = 0;   


    public override void onEnterState()
    {
        print("Entering patrol state");
        base.onEnterState();

        if (_stateMachine == null) return;

        _stateMachine.attackType = 0;
        _stateMachine.speed = 1;
        _stateMachine.alerted = false;

        if (_stateMachine.GetTargetType() != AITargetType.Waypoint)
        {
            _stateMachine.ClearTarget();
            if (_waypointNetwork != null && _waypointNetwork.Waypoints.Count > 0)
            {
                if (_randomPatrol)
                {
                    _currentWaypoint = Random.Range(0, _waypointNetwork.Waypoints.Count);
                }

                Transform waypoint = _waypointNetwork.Waypoints[_currentWaypoint];
                if (waypoint != null)
                {
                    _stateMachine.SetTarget(AITargetType.Waypoint, null, waypoint.position, Vector3.Distance(_stateMachine.transform.position, waypoint.position));
                    _stateMachine.navAgent.SetDestination(waypoint.position);                   
                }

            }
        }
        _stateMachine.navAgent.isStopped = false;
    }



    public override AIStateType onUpdate()
    {
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

        if (_stateMachine.navAgent.isPathStale || !_stateMachine.navAgent.hasPath || _stateMachine.navAgent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            NextWaypoint();
        }

        return AIStateType.Patrol;
    }


    private void NextWaypoint()
    {
        if (_randomPatrol && _waypointNetwork.Waypoints.Count >1)
        {
            int oldWaypoint = _currentWaypoint;
            while (_currentWaypoint == oldWaypoint)
            {
                _currentWaypoint = Random.Range(0, _waypointNetwork.Waypoints.Count);
            }
        }
        else
        {
            _currentWaypoint = _currentWaypoint == _waypointNetwork.Waypoints.Count - 1 ? 0 : _currentWaypoint + 1;
        }

        if (_waypointNetwork.Waypoints[_currentWaypoint] != null)
        {
            Transform newWaypoint = _waypointNetwork.Waypoints[_currentWaypoint];
            _stateMachine.SetTarget(AITargetType.Waypoint, null, newWaypoint.position, Vector3.Distance(_stateMachine.transform.position, newWaypoint.position));
            _stateMachine.navAgent.SetDestination(newWaypoint.position);
        }
    }








    public override void onExitState()
    {
        base.onExitState();
    }





    public override AIStateType GetStateType()
    {
        return AIStateType.Patrol;
    }

}


