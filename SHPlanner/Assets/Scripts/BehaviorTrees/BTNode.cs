using System;
using UnityEngine;
using System.Collections;

public enum NodeStatus
{
    Ready,
    Running,
    Failure,
    Success
}

public class BTNodeData
{
}

public abstract class BTNode
{

    // Use this for initialization
    public abstract void Initialize(ref BTAgentData nodeData, BTNodeData data);

    public abstract NodeStatus Enter(ref BTAgentData nodeData);

    public abstract void Exit(ref BTAgentData nodeData);

    // Update is called once per frame
    public abstract NodeStatus Update(ref BTAgentData nodeData);

    public virtual NodeStatus Tick(ref BTAgentData nodeData)
    {
        switch (nodeData.CurStatus)
        {
            case NodeStatus.Ready:
                return Enter(ref nodeData);
                break;
            case NodeStatus.Running:
                return Update(ref nodeData);
                break;
            case NodeStatus.Success:
            case NodeStatus.Failure:
                Exit(ref nodeData);
                return NodeStatus.Failure;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
