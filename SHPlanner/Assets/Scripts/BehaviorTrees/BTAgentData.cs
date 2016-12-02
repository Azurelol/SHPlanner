using UnityEngine;
using System.Collections;

public class BTAgentData
{
    public BehaviorTree MyTree;
    public BTNodeData MyData;
    public ArrayList ChildIndecies = new ArrayList();
    public int MyType;
    public int MyIndex;
    public int ParentIndex;
    public int CurChild;
    public int Depth;
    public NodeStatus CurStatus;
}
