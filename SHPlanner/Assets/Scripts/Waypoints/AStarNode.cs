using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AStarNode
{
    public float GivenCost = 0;
    public float Cost = 0;
    public WayPoint MyPoint;
    public AStarNode parent;
    public bool NeedsJump;
    public AStarNode(WayPoint point)
    {
        MyPoint = point;
    }

    
}
