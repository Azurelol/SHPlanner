using UnityEngine;
using System.Collections;

public class AttackShortRange : BTNode
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
        nodeData.MyTree.MyGameObject.GetComponent<CombatController>().ShortAttack();

        return NodeStatus.Running;
    }
}