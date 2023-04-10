using FPS.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStateType { None, Idle, Alerted, Patrol, Attack, Pursuit, Dead }
public enum AITargetType { None, Waypoint, Visual_Player, Visual_Light, Audio }
public enum AITriggerEventType { Enter, Stay, Exit}


public struct AITarget
{    
    private AITargetType type;
    private Collider collider;
    private Vector3 position;
    private float distance;
    private float _time;

    public AITargetType Type { get => type; }
    public Collider Collider { get => collider; }
    public Vector3 Position { get => position; }
    public float Distance { get => distance; set => distance = value; }  
    public float _Time { get => _time; }

    public void Set(AITargetType t, Collider c, Vector3 p, float d)
    {
        type = t;
        collider = c;  
        position = p;
        distance = d;
        _time = Time.time;
    }

    public void Clear()
    {
        type = AITargetType.None;
        collider = null;
        position = Vector3.zero;
        _time = 0.0f;
        distance = Mathf.Infinity;
    }
}


namespace FPS.AI
{
    public class AIStateMachine : MonoBehaviour
    {

        //Zombie properties
        [SerializeField] [Range(10.0f, 360.0f)] private float _fov = 50.0f;
        [SerializeField][Range(0.0f, 1.0f)] private float _sight = 1.0f;
        [SerializeField][Range(0.0f, 1.0f)] private float _hearing = 1.0f;
        [SerializeField][Range(0.0f, 1.0f)] private float _aggression = 1.0f;        
        [SerializeField][Range(0.0f, 1.0f)] private float _intelligence = 0.5f;
        [SerializeField][Range(0, 100)] private int _health = 100;


        //Public
        public AITarget VisualThreat = new AITarget();
        public AITarget AudioThreat = new AITarget();


        //Private
        private Dictionary<AIStateType, AIState> _states = new Dictionary<AIStateType, AIState>();
        private AITarget _target = new AITarget();
        private AIState _currentState = null;
        
        private bool _alerted = false;
        private int _attackType = 0;
        private int _speed = 0; 

        [SerializeField] private AIStateType _currentStateType = AIStateType.Idle;
        [SerializeField] private SphereCollider _sensorTrigger = null;

        //Component cache
        private NavMeshAgent _navAgent = null;
        private Animator _animator = null;
        private Collider _collider = null;


        // Hashes
        private int _attackHash = Animator.StringToHash("Attack");
        private int _speedHash = Animator.StringToHash("Speed");
        private int _alertedHash = Animator.StringToHash("Alerted");
        


        //Public properties
        public Animator animator  { get => _animator; }
        public NavMeshAgent navAgent  { get => _navAgent; }

        public Vector3 sensorPosition
        {
            get
            {
                if (_sensorTrigger == null) return Vector3.zero;
                Vector3 point = _sensorTrigger.transform.position;
                point.x += _sensorTrigger.center.x * _sensorTrigger.transform.lossyScale.x;
                point.y += _sensorTrigger.center.y * _sensorTrigger.transform.lossyScale.y;
                point.z += _sensorTrigger.center.z * _sensorTrigger.transform.lossyScale.z;

                return point;   
            }
        }

        public float sensorRadius
        {
            get
            {
                if(_sensorTrigger == null) return 0.0f;
                float radius = Mathf.Max(_sensorTrigger.radius * _sensorTrigger.transform.lossyScale.x,
                  _sensorTrigger.radius * _sensorTrigger.transform.lossyScale.y);

                return Mathf.Max(radius, _sensorTrigger.radius * _sensorTrigger.transform.lossyScale.z);
            }
        }
       
        public bool alerted { get => _alerted; set => _alerted = value; }
        public int attackType { get => _attackType; set => _attackType = value; }
        public int speed { get => _speed; set => _speed = value; }

        public float fov { get => _fov; }
        public float hearing { get => _hearing; }
        public float sight { get => _sight; }      
        public float intelligence { get => _intelligence; }        
        public float aggression { get => _aggression; set => _aggression = value; }
        public int health { get => _health; set => _health = value; }
      
       

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            _navAgent = GetComponent<NavMeshAgent>();

            if (GameSceneManager.Instance!=null)
            {
                if (_collider)
                {
                    GameSceneManager.Instance.RegisterAIStatesMachine(_collider.GetInstanceID(), this);
                }
                if (_sensorTrigger)
                {
                    GameSceneManager.Instance.RegisterAIStatesMachine(_sensorTrigger.GetInstanceID(), this);
                }
            }
        }

        private void Start()
        {
            if (_sensorTrigger != null)
            {
                AISensor script = _sensorTrigger.GetComponent<AISensor>();
                script.parentStateMachine = this;
            }

            AIState[] aiStates = GetComponents<AIState>();
            foreach (AIState aiState in aiStates) 
            {
                if (aiState != null && !_states.ContainsKey(aiState.GetStateType()))
                {
                    _states[aiState.GetStateType()] = aiState;
                    aiState.SetStateMachine(this);
                }
            }

            if (_states.ContainsKey(_currentStateType))
            {
                _currentState = _states[_currentStateType];
                _currentState.onEnterState();
            }
        }

        
        void Update()
        {
            if (_currentState == null) return;

            AIStateType newStateType = _currentState.onUpdate();

            if (newStateType != _currentStateType)
            {
                AIState newState= null;
                if (_states.TryGetValue(newStateType, out newState))
                {
                    _currentState.onExitState();
                    newState.onEnterState();
                    _currentState = newState;
                }
                _currentStateType = newStateType;
            }

            if (_animator != null)
            {              
                _animator.SetInteger(_attackHash, _attackType);
                _animator.SetInteger(_speedHash, _speed);
                _animator.SetBool(_alertedHash, _alerted);
                
            }
        }

        private void FixedUpdate()
        {
            VisualThreat.Clear();
            AudioThreat.Clear();
            if (_target.Type != AITargetType.None)
            {
                _target.Distance = Vector3.Distance(transform.position, _target.Position);
            }
        }


        public void SetTarget(AITargetType t, Collider c, Vector3 p, float d)
        {
            _target.Set(t, c, p, d);
        }

        public void SetTarget(AITarget target)
        {
            _target = target;
        }

        public void ClearTarget()
        {
            _target.Clear();

        }

        public void onTriggerEvent(AITriggerEventType type, Collider other)
        {
            if (_currentState != null)
            {
                _currentState.onTriggerEvent(type, other);
            }
        }

        public AITargetType GetTargetType()
        {
            return _target.Type;
        }
    }
}


    
    

