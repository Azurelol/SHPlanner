using UnityEngine;
using System.Collections;

public class ApproachTarget : BTNode
{
    
    public override void Initialize(ref BTAgentData nodeData, BTNodeData data)
    {
        
    }

    public override NodeStatus Enter(ref BTAgentData nodeData)
    {
        return NodeStatus.Running;
    }

    public override void Exit(ref BTAgentData nodeData)
    {
        
    }

    public override NodeStatus Update(ref BTAgentData nodeData)
    {
        var behave = nodeData.MyTree.MyGameObject.GetComponent<AIBehaviors>();

        nodeData.MyTree.MyGameObject.GetComponent<AIBehaviors>().MovementTarget = behave.Target;

        return NodeStatus.Running;
    }
}
