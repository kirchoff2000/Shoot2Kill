using UnityEngine;

namespace FPS.AI
{
    public abstract class AIState : MonoBehaviour
    {
        protected int _playerLayerMask = -1;
        protected int _bodyPartLayer = -1;

        protected AIStateMachine _stateMachine;

        public void SetStateMachine(AIStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }      

        public virtual void onEnterState() { }
        public virtual void onExitState() { }

        void Awake()
        {
            // Get a mask for line of sight testing with the player. (+1) is a hack to include the
            // default layer in the current version of unity.
            _playerLayerMask = LayerMask.GetMask("Player", "Bodypart") + 1;

            // Get the layer index of the AI Body Part layer
            _bodyPartLayer = LayerMask.NameToLayer("Bodypart");
        }

        public virtual void OnDestinationReached(bool isReached) { }

        public abstract AIStateType GetStateType();
        public abstract AIStateType onUpdate();


        public void onTriggerEvent(AITriggerEventType evenType, Collider other)
        {
            // If we don't have a parent state machine then bail
            if (_stateMachine == null)
                return;

            // We are not interested in exit events so only step in and process if its an
            // enter or stay.
            if (evenType != AITriggerEventType.Exit)
            {
                // What is the type of the current visual threat we have stored
                AITargetType curType = _stateMachine.VisualThreat.Type;

                // Is the collider that has entered our sensor a player
                if (other.CompareTag("Player"))
                {                   
                    // Get distance from the sensor origin to the collider
                    float distance = Vector3.Distance(_stateMachine.sensorPosition, other.transform.position);

                    // If the currently stored threat is not a player or if this player is closer than a player
                    // previously stored as the visual threat...this could be more important
                    if (curType != AITargetType.Visual_Player ||
                        (curType == AITargetType.Visual_Player && distance < _stateMachine.VisualThreat.Distance))
                    {                      
                        // Is the collider within our view cone and do we have line or sight
                        RaycastHit hitInfo;
                        if (IsVisible(other, out hitInfo, _playerLayerMask))
                        {
                            // Yep...it's close and in our FOV and we have line of sight so store as the current most dangerous threat
                            _stateMachine.VisualThreat.Set(AITargetType.Visual_Player, other, other.transform.position, distance);
                            print("Player entered");
                        }
                    }
                }
                else if (other.CompareTag("Flash Light") && curType != AITargetType.Visual_Player)
                {
                    BoxCollider flashLight = (BoxCollider)other;
                    float distanceToThreat = Vector3.Distance(_stateMachine.sensorPosition, flashLight.transform.position);
                    float zSize = flashLight.size.z * flashLight.transform.lossyScale.z;
                    float aggrFactor = distanceToThreat / zSize;
                    if (aggrFactor <= _stateMachine.sight && aggrFactor <= _stateMachine.intelligence) 
                    {
                        _stateMachine.VisualThreat.Set(AITargetType.Visual_Player, other, other.transform.position, distanceToThreat);
                    }
                }

                else if (other.CompareTag("Sound"))
                {
                    SphereCollider soundTrigger = (SphereCollider)other;
                    if (soundTrigger == null) return;


                    // Get the position of the Agent Sensor 
                    Vector3 agentSensorPosition = _stateMachine.sensorPosition;

                    Vector3 soundPos;
                    float soundRadius;
                    ConvertSphereColliderToWorldSpace(soundTrigger, out soundPos, out soundRadius);

                    // How far inside the sound's radius are we
                    float distanceToThreat = (soundPos - agentSensorPosition).magnitude;

                    // Calculate a distance factor such that it is 1.0 when at sound radius 0 when at center
                    float distanceFactor = (distanceToThreat / soundRadius);

                    // Bias the factor based on hearing ability of Agent.
                    distanceFactor += distanceFactor * (1.0f - _stateMachine.hearing);

                    // Too far away
                    if (distanceFactor > 1.0f)
                        return;

                    // if We can hear it and is it closer then what we previously have stored
                    if (distanceToThreat < _stateMachine.AudioThreat.Distance)
                    {
                        // Most dangerous Audio Threat so far
                        _stateMachine.AudioThreat.Set(AITargetType.Audio, other, soundPos, distanceToThreat);                       
                    }
                }

            }            
        }

        protected virtual bool IsVisible(Collider other, out RaycastHit hitInfo, int layerMask = -1)
        {
            // Let's make sure we have something to return
            hitInfo = new RaycastHit();

            // We need the state machine to be an AIZombieStateMachine
            if (_stateMachine == null) return false;           

            // Calculate the angle between the sensor origin and the direction of the collider
            Vector3 head = _stateMachine.sensorPosition;
            Vector3 direction = other.transform.position - head;
            float angle = Vector3.Angle(direction, transform.forward);

            // If thr angle is greater than half our FOV then it is outside the view cone so
            // return false - no visibility
            if (angle > _stateMachine.fov * 0.5f)
                return false;

            // Now we need to test line of sight. Perform a ray cast from our sensor origin in the direction of the collider for distance
            // of our sensor radius scaled by the zombie's sight ability. This will return ALL hits.
            RaycastHit[] hits = Physics.RaycastAll(head, direction.normalized, _stateMachine.sensorRadius * _stateMachine.sight, layerMask);

            // Find the closest collider that is NOT the AIs own body part. If its not the target then the target is obstructed
            float closestColliderDistance = float.MaxValue;
            Collider closestCollider = null;

            // Examine each hit
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                // Is this hit closer than any we previously have found and stored
                if (hit.distance < closestColliderDistance)
                {
                    // If the hit is on the body part layer
                    if (hit.transform.gameObject.layer == _bodyPartLayer)
                    {
                        // And assuming it is not our own body part
                        if (_stateMachine != GameSceneManager.Instance.GetAIStateMachine(hit.rigidbody.GetInstanceID()))
                        {
                            // Store the collider, distance and hit info.
                            closestColliderDistance = hit.distance;
                            closestCollider = hit.collider;
                            hitInfo = hit;
                        }
                    }
                    else
                    {
                        // Its not a body part so simply store this as the new closest hit we have found
                        closestColliderDistance = hit.distance;
                        closestCollider = hit.collider;
                        hitInfo = hit;
                    }
                }
            }

            // If the closest hit is the collider we are testing against, it means we have line-of-sight
            // so return true.
            if (closestCollider && closestCollider.gameObject == other.gameObject) return true;

            // otherwise, something else is closer to us than the collider so line-of-sight is blocked
            return false;
        }

        public static void ConvertSphereColliderToWorldSpace(SphereCollider col, out Vector3 pos, out float radius)
        {
            // Default Values
            pos = Vector3.zero;
            radius = 0.0f;

            // If no valid sphere collider return
            if (col == null)
                return;

            // Calculate world space position of sphere center
            pos = col.transform.position;
            pos.x += col.center.x * col.transform.lossyScale.x;
            pos.y += col.center.y * col.transform.lossyScale.y;
            pos.z += col.center.z * col.transform.lossyScale.z;

            // Calculate world space radius of sphere
            radius = Mathf.Max(col.radius * col.transform.lossyScale.x,
                                col.radius * col.transform.lossyScale.y);

            radius = Mathf.Max(radius, col.radius * col.transform.lossyScale.z);
        }

    }
}

