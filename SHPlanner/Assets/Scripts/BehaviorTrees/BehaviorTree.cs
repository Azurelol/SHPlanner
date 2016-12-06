using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class BehaviorTree
{
    public ArrayList myData = new ArrayList();
    private List<BTAgentData> tempArray = new List<BTAgentData>();
    private int lastIndex = 0;
    private int lastDepth = 0;
    private int curIndex = 0;
    public GameObject MyGameObject;
    public Stack<KeyValuePair<int, int>> excecutionStack = new Stack<KeyValuePair<int, int>>();
    public Stack<BTAgentData> nodeStack = new Stack<BTAgentData>();
    public BehaviorTree(GameObject obj)
    {
        MyGameObject = obj;
    }
    public BehaviorTree AddNode(int nodeIndex, int depth, BTNodeData data = null)
    {
        BTAgentData newNode = new BTAgentData
        {
            MyType = nodeIndex,
            MyTree = this,
            MyIndex = curIndex++
        };

        if (nodeStack.Count == 0)
        {
            newNode.ParentIndex = -1;
            newNode.Depth = 0;
            newNode.CurStatus = NodeStatus.Ready;
            nodeStack.Push(newNode);
        }
        else
        {
            //if the node is a child
            while (nodeStack.Count != 0)
            {
                if (depth > nodeStack.Peek().Depth)
                {
                    newNode.ParentIndex = nodeStack.Peek().MyIndex;
                    nodeStack.Peek().ChildIndecies.Add(newNode.MyIndex);
                    newNode.Depth = depth;
                    nodeStack.Push(newNode);


                    break;
                }
                else
                {
                    nodeStack.Pop();
                }
            }

        }

        BehaviorTreeSystem.nodes[newNode.MyType].Initialize(ref newNode, data);

        myData.Add(newNode);

        return this;
    }

    public NodeStatus AddToExcecutionList(int type, int index)
    {
        excecutionStack.Push(new KeyValuePair<int, int>(type, index));

        return NodeStatus.Ready;
    }

    public void Initialize()
    {
        AddToExcecutionList(((BTAgentData)myData[0]).MyType, ((BTAgentData)myData[0]).MyIndex);
    }
}