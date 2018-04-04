using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Patrol around the specified waypoints using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=7")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
    public class Patrol : NavMeshMovement
    {
        [Tooltip("Should the agent patrol the waypoints randomly?")]
        public SharedBool randomPatrol = false;
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedFloat waypointPauseDuration = 0;
        [Tooltip("The waypoints to move to")]
        public SharedGameObjectList waypoints;

        // The current index that we are heading towards within the waypoints array
        private int waypointIndex;
        private float waypointReachedTime;

        //Custom variable
        private UnityEngine.AI.NavMeshAgent agent;
        private Animator meleeAnim, rangedAnim;
        private GameObject look;
        private int newWaypointIndex;
        Reference reference;


        public override void OnStart()
        {
            //base.OnStart();

            //Custom
            meleeAnim = GetComponent<Animator>();
            rangedAnim = GetComponent<Animator>();
            reference = GetComponent<Reference>();
            look = GameObject.Find("lookClone" + this.gameObject.name);


            if (look.transform.IsChildOf(transform))
            {
                look.transform.SetParent(null);
            }

            newWaypointIndex = waypointIndex;

            meleeAnim.SetBool("isPatrolling", true);
            meleeAnim.SetBool("isRunning", false);
            meleeAnim.SetBool("isCharging", false);
            meleeAnim.SetBool("isShooting", false);
            waypoints.Value.AddRange(reference.waypoint);
            //
            
            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Value.Count; ++i) 
            {
                if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance) 
                {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }
            waypointReachedTime = -1;
            look.transform.position = new Vector3(waypoints.Value[waypointIndex].transform.position.x, waypoints.Value[waypointIndex].transform.position.y, waypoints.Value[waypointIndex].transform.position.z);
            SetDestination(Target());


        }

        // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
        public override TaskStatus OnUpdate()
        {
            
            //Custom

            LookAtWaypoint();
            //

    
            if (waypoints.Value.Count == 0) 
            {
                return TaskStatus.Failure;
            }
            if (HasArrived()) 
            {

                //Custom
                meleeAnim.SetBool("isPatrolling", false);
                meleeAnim.SetBool("isIdle", true);
                //

                if (waypointReachedTime == -1) 
                {
                    waypointReachedTime = Time.time;
                }
                // wait the required duration before switching waypoints.
                if (waypointReachedTime + waypointPauseDuration.Value <= Time.time) 
                {
                    
                    if (randomPatrol.Value) 
                    {
                        if (waypoints.Value.Count == 1) 
                        {
                            waypointIndex = 0;
                        } 
                        else 
                        {
                            // prevent the same waypoint from being selected
                            //var newWaypointIndex = waypointIndex;
                            while (newWaypointIndex == waypointIndex) 
                            {
                                newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                            }

                            waypointIndex = newWaypointIndex;

                            //Custom

                            if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
                            {
                                look.transform.position = new Vector3(waypoints.Value[waypointIndex].transform.position.x, waypoints.Value[waypointIndex].transform.position.y, waypoints.Value[waypointIndex].transform.position.z);

                            }

                            //

                        }

                    } 
                    else 
                    {
                        waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                    }
                       
                    //SetDestination(Target());
                    waypointReachedTime = -1;

                }
            }

            return TaskStatus.Running;
        }

        //Look at moved empty gameobject
        private void LookAtWaypoint()
        {
          
            Vector3 direction = (look.transform.position - transform.position).normalized; //Need to find position of next waypoint
            direction.y = 0;

            float angle = Vector3.Angle(transform.forward, direction);

            if (angle > 0.01f)
            {
                if (direction != Vector3.zero)
                {
                    meleeAnim.SetBool("isLooking", true);
                    Quaternion lookAtPoint = Quaternion.LookRotation(direction);
                    //transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAtPoint, (Time.deltaTime * 1.4f)); //1.3f for more looking behaviour, 2f is default
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, lookAtPoint, Time.deltaTime * 75f);
                }
           
            } 
            else
            {
                meleeAnim.SetBool("isLooking", false);
                meleeAnim.SetBool("isIdle", false);
                meleeAnim.SetBool("isPatrolling", true);
      
                SetDestination(Target());
            }
         
               

        }

        // Return the current waypoint index position
        private Vector3 Target()
        {
            if (waypointIndex >= waypoints.Value.Count) 
            {
                return transform.position;
            }
            return waypoints.Value[waypointIndex].transform.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            randomPatrol = false;
            waypointPauseDuration = 0;
            waypoints = null;
        }

        // Draw a gizmo indicating a patrol 
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (waypoints == null || waypoints.Value == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Value.Count; ++i) {
                if (waypoints.Value[i] != null) {
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    UnityEditor.Handles.SphereCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1);
#else
                    UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
#endif
                }
            }
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}