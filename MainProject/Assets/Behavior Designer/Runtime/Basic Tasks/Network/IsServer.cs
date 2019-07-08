using UnityEngine.Networking;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNetwork
{
    public class IsServer : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}