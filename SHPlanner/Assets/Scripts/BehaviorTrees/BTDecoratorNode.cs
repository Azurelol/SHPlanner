using UnityEngine;
using System.Collections;

public abstract class BTDecoratorNode : BTNode
{

    public abstract override void Initialize(ref BTAgentData nodeData, BTNodeData data);

    public abstract override NodeStatus Enter(ref BTAgentData nodeData);

    public abstract override void Exit(ref BTAgentData nodeData);

    public abstract override NodeStatus Update(ref BTAgentData nodeData);

    public BTAgentData GetChild(ref BTAgentData nodeData)
    {
        return (BTAgentData)nodeData.MyTree.myData[(int)nodeData.ChildIndecies[0]];
    }
}
