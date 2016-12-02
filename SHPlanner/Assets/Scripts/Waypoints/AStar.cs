using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class AStar
{
    
    private static ArrayList _waypoints = new ArrayList();
    public static bool Initialized = false;
    private const int _maxJumpHeight = 2;
    private const int _maxJumpLength = 100;
    private const float _marchingBoxHeight = 1.2f;
    private const float _minimumWalkHeight = 0.7f;
    private const float SQRTWO = 1.4142135623730950488016887242097f;

    private enum MarchResult
    {
        Valid,
        Invalid,
        RequiresJump
    }
    static AStar()
    {
        
    }

    public static List<AStarNode> FindPath(AStarNode startNode, AStarNode endNode)
    {
        //Push Start Node onto the Open List
        PointList openList = new PointList();
        
        openList.Add(startNode);

        //While (Open List is not empty) {
        while (!openList.Empty())
        {
            //Pop cheapest node off Open List (parent node)
            AStarNode node = openList.Remove();
            //If node is the Goal Node, then path found (RETURN “found”)
            if ((node.MyPoint.Location- endNode.MyPoint.Location).sqrMagnitude < 0.5)
            {
                return ConstructPath(node, startNode);
            }
            
            //For (all neighboring child nodes) {
            foreach (KeyValuePair<WayPoint, bool> neighbor in node.MyPoint.Neighbors)
            {
                //Compute its cost, f(x) = g(x) + h(x)
                AStarNode neighborNode = new AStarNode(neighbor.Key);
                var point = neighborNode.MyPoint.Location;
                var endPoint = endNode.MyPoint.Location;
                float dist = (point - node.MyPoint.Location).magnitude;
                float given = node.GivenCost + dist;
                
                neighborNode.Cost = given + OctalCost(Mathf.Abs(endPoint.x - point.x), Mathf.Abs(endPoint.y - point.y), Mathf.Abs(endPoint.z - point.z));

                neighborNode.GivenCost = given;
                //If child node isn’t on Open or Closed list, put it on Open List.
                if (!openList.Count(neighborNode))
                {
                    openList.Add(neighborNode);
                }
                //If child node is on Open or Closed List, AND this new one is cheaper,
                else if (openList.Get(neighborNode).Cost > neighborNode.Cost)
                {
                    //	then take the old expensive one off both lists and put this new 	cheaper one on the Open List.
                    openList.Add(neighborNode);
                }
                //}
                //Place parent node on the Closed List (we’re done with it)
                //If taken too much time this frame (or in single step mode), 
                //	 abort search for now and resume next frame (RETURN “working”)
                //}
            }
        }
        //Open List empty, thus no path possible (RETURN “fail”)
        return new List<AStarNode>();
    }

    private static List<AStarNode> ConstructPath(AStarNode endNode, AStarNode startNode)
    {
        AStarNode curNode = endNode;
        List<AStarNode> ret = new List<AStarNode>();

        while (curNode != startNode)
        {
            ret.Add(curNode);

            curNode = curNode.parent;
        }

        return ret;
    }
    
    private static float OctalCost(float xDiff, float yDiff, float zDiff)
    {
        float min = xDiff < yDiff ? xDiff : yDiff;
        min = min < zDiff ? min : zDiff;
        float max = xDiff > yDiff ? xDiff : yDiff;
        max = max > zDiff ? max : zDiff;

        return min * SQRTWO + max - min;

    }
}
