using UnityEngine;
using System.Collections;

public class BTSequencer : BTControlFlowNode
{
    public override void Initialize(ref BTAgentData nodeData, BTNodeData data)
    {

    }
    public override NodeStatus Enter(ref BTAgentData nodeData)
    {
        return base.Enter(ref nodeData);
    }

    public override void Exit(ref BTAgentData nodeData)
    {
        base.Exit(ref nodeData);
    }

    public override NodeStatus Update(ref BTAgentData nodeData)
    {
        NodeStatus stat = GetChild(nodeData.CurChild, ref nodeData).CurStatus;
        if (stat == NodeStatus.Success)
        {
            ++nodeData.CurChild;
        }
        else if (stat == NodeStatus.Failure)
        {
            return NodeStatus.Failure;
        }
        if (nodeData.CurChild == nodeData.ChildIndecies.Count)
        {
            return NodeStatus.Success;
        }
        RunChild(nodeData.CurChild, ref nodeData);
        return NodeStatus.Running;
    }
}
