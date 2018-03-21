using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Pursue the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=5")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PursueIcon.png")]
    public class Pursue : NavMeshMovement
    {
        [Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
        public SharedFloat targetDistPrediction = 20;
        [Tooltip("Multiplier for predicting the look ahead distance")]
        public SharedFloat targetDistPredictionMult = 20;
        [Tooltip("The GameObject that the agent is pursuing")]
        public SharedGameObject target;

        // The position of the target at the last frame
        private Vector3 targetPosition;

        //Custom
        private GameObject IkTarget;
        private Animator anim;
        //private Transform look;

        public override void OnStart()
        {
            //Custom
//            look = transform.Find("lookClone");
//
//            look.transform.SetParent(transform, false);
            //

            base.OnStart();

            targetPosition = target.Value.transform.position;
            SetDestination(Target());

            //Custom
            anim = GetComponent<Animator>();
            IkTarget = GameObject.Find("LookTarget");
            anim.SetBool("isRunning", true);
            anim.SetBool("isLooking", false);
            anim.SetBool("isPatrolling", false);
            anim.SetBool("isCharging", false);
        }

        // Pursue the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {

            if (HasArrived()) 
            {

                //Custom
                anim.SetBool("isRunning", false);
                anim.SetBool("isPatrolling", false);
                anim.SetBool("isShooting", true);
                //IkTarget.transform.position = GameObject.Find("Player").transform.position;

                return TaskStatus.Success;
            }
            else
            {
                anim.SetBool("isShooting", false);
            }

            // Target will return the predicated position
            SetDestination(Target());


            //Custom
            anim.SetBool("isLooking", false);
            anim.SetBool("isPatrolling", false);
            anim.SetBool("isCharging", false);
            anim.SetBool("isRunning", true);
            return TaskStatus.Running;
        }

        // Predict the position of the target
        private Vector3 Target()
        {
            // Calculate the current distance to the target and the current speed
            var distance = (target.Value.transform.position - transform.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            if (speed <= distance / targetDistPrediction.Value) {
                futurePrediction = targetDistPrediction.Value;
            } else {
                futurePrediction = (distance / speed) * targetDistPredictionMult.Value; // the prediction should be accurate enough
            }

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = targetPosition;
            targetPosition = target.Value.transform.position;
            return targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            targetDistPrediction = 20;
            targetDistPredictionMult = 20;
            target = null;
        }
    }
}