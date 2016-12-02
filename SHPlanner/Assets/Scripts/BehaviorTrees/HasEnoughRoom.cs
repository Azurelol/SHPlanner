using UnityEngine;
using System.Collections;

public class RoomData : BTNodeData
{
    public RoomData(int weight_)
    {
        weight = weight_;
        Id = _curId++;
    }
    public int weight = 0;
    public int Id = 0;
    private static int _curId = 1;
}

public class HasEnoughRoom : BTDecoratorNode
{

    public override void Initialize(ref BTAgentData nodeData, BTNodeData data)
    {
        if (data == null)
        {
            nodeData.MyData = new RoomData(1);
        }
        else
        {
            nodeData.MyData = data;
        }
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
        var data = GetChild(ref nodeData);
        var result = WorldState.QueryAttackPosition(((RoomData) nodeData.MyData));

        //if the distance is too far
        if (result > 0)
        {
            NodeStatus stat = (BehaviorTreeSystem.nodes[data.MyType]).Tick(ref data);
            data.CurStatus = stat;
            return stat;
        }
        else
        {
            return NodeStatus.Failure;
        }
    }
}
