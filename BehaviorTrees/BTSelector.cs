using UnityEngine;
using System.Collections;

public class BTSelector : BTControlFlowNode
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
        if (stat == NodeStatus.Failure)
        {
            ++nodeData.CurChild;
        }
        else if (stat == NodeStatus.Success)
        {
            return NodeStatus.Success;
        }
        if (nodeData.CurChild == nodeData.ChildIndecies.Count)
        {
            nodeData.CurChild = 0;
            return NodeStatus.Failure;
        }

        RunChild(nodeData.CurChild, ref nodeData);
        return NodeStatus.Running;
    }
}