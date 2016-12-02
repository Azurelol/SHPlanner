using UnityEngine;
using System.Collections;

public class TestNodeData : BTNodeData
{
    public float timer = 0.0f;
}

public class TestNode : BTNode
{
    public override void Initialize(ref BTAgentData nodeData, BTNodeData data)
    {
        if (data == null)
        {
            nodeData.MyData = new TestNodeData();
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

    // Update is called once per frame
    public override NodeStatus Update(ref BTAgentData nodeData)
    {
        nodeData.MyTree.MyGameObject.GetComponent<KinematicController>().Move(Vector3.back);
        ((TestNodeData)nodeData.MyData).timer += Time.deltaTime;
        if (((TestNodeData) nodeData.MyData).timer >= 0.5f)
        {
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }
}
