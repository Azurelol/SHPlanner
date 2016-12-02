using UnityEngine;
using System.Collections;

public abstract class BTControlFlowNode : BTNode
{
    public override void Initialize(ref BTAgentData nodeData, BTNodeData data)
    {
        throw new System.NotImplementedException();
    }

    public  override NodeStatus Enter(ref BTAgentData nodeData)
    {
        ResetChildren(ref nodeData);
        return NodeStatus.Running;
    }

    public override void Exit(ref BTAgentData nodeData)
    {
        
    }

    public override NodeStatus Update(ref BTAgentData nodeData)
    {
        return NodeStatus.Running;
    }

    public NodeStatus RunChild(int child, ref BTAgentData nodeData)
    {
        return nodeData.MyTree.AddToExcecutionList(GetChild(child, ref nodeData).MyType, GetChild(child, ref nodeData).MyIndex);
    }

    public void ResetChildren(ref BTAgentData nodeData)
    {
        foreach (var index in nodeData.ChildIndecies)
        {
            ((BTAgentData) nodeData.MyTree.myData[(int) index]).CurStatus = NodeStatus.Ready;
        }
        
    }
    public BTAgentData GetChild(int child, ref BTAgentData nodeData)
    {
        return (BTAgentData)nodeData.MyTree.myData[(int)nodeData.ChildIndecies[child]];
    }
}
