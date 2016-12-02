using UnityEngine;
using System.Collections.Generic;
using System;

public enum BTNodeTypes : int
{
    Test,
    Sequencer,
    Idle,
    ApproachTarget,
    WithinInRange,
    AttackShort,
    Selector,
    EnoughRoom,
    Count
}
public static class BehaviorTreeSystem
{

    public static BTNode [] nodes = new BTNode[(int)BTNodeTypes.Count];
	// Use this for initialization
    static BehaviorTreeSystem()
    {
        nodes[(int)BTNodeTypes.Test] = new TestNode();
        nodes[(int)BTNodeTypes.Sequencer] = new BTSequencer();
        nodes[(int)BTNodeTypes.Idle] = new BTIdle();
        nodes[(int)BTNodeTypes.ApproachTarget] = new ApproachTarget();
        nodes[(int)BTNodeTypes.WithinInRange] = new WithinInRange();
        nodes[(int)BTNodeTypes.AttackShort] = new AttackShortRange();
        nodes[(int)BTNodeTypes.Selector] = new BTSelector();
        nodes[(int)BTNodeTypes.EnoughRoom] = new HasEnoughRoom();
    }

    static void Register(BTAgentData nodeData)
    {
        
    }

    public static void Update(BehaviorTree tree)
    {
        //update the root
        //update its children
        if (tree.excecutionStack.Count > 0)
        {
            var node = tree.excecutionStack.Peek();
            var dat = (BTAgentData)tree.myData[node.Value];
            ((BTAgentData)tree.myData[node.Value]).CurStatus = nodes[node.Key].Tick(ref dat);
            if (((BTAgentData)tree.myData[node.Value]).CurStatus == NodeStatus.Failure ||
                ((BTAgentData)tree.myData[node.Value]).CurStatus == NodeStatus.Success)
            {
                if (tree.excecutionStack.Count == 1)
                {
                    ((BTAgentData) tree.myData[node.Value]).CurStatus = NodeStatus.Ready;
                }
                else
                {
                    tree.excecutionStack.Pop();
                }
            }

            tree.myData[node.Value] = dat;

        }
    }
}
