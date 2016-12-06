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

    public static List<WayPoint> FindPath(Vector3 start, Vector3 end)
    {
        var startNode = WaypointGraph.GetWaypointAroundVec(start);
        var endNode = WaypointGraph.GetWaypointAroundVec(end);
        //Push Start Node onto the Open List
        PointList openList = new PointList();
        
        openList.Add(startNode);

        //While (Open List is not empty) {
        while (!openList.Empty())
        {
            //Pop cheapest node off Open List (parent node)
            WayPoint node = openList.Remove();
            //If node is the Goal Node, then path found (RETURN “found”)
            if ((node.Location- endNode.Location).sqrMagnitude < 0.5)
            {
                return ConstructPath(node, startNode, end);
            }
            
            //For (all neighboring child nodes) {
            foreach (KeyValuePair<WayPoint, bool> neighbor in node.Neighbors)
            {
                //Compute its cost, f(x) = g(x) + h(x)
                var point = neighbor.Key.Location;
                var endPoint = endNode.Location;
                float dist = (point - node.Location).magnitude;
                float given = node.GivenCost + dist;

                neighbor.Key.Cost = given + EuclidianCost(endPoint, point);

                neighbor.Key.GivenCost = given;
                //If child node isn’t on Open or Closed list, put it on Open List.
                if (!neighbor.Key.OnClosedList && !openList.Count(neighbor.Key))
                {
                    neighbor.Key.parent = node;
                    
                    openList.Add(neighbor.Key);
                }
                //If child node is on Open or Closed List, AND this new one is cheaper,
                else if (openList.Count(neighbor.Key) && openList.Get(neighbor.Key).Cost > neighbor.Key.Cost)
                {
                    //	then take the old expensive one off both lists and put this new 	cheaper one on the Open List.
                   // openList.Add(neighborNode);
                }
                //}
                //If taken too much time this frame (or in single step mode), 
                //	 abort search for now and resume next frame (RETURN “working”)
                //}
            }

            //Place parent node on the Closed List (we’re done with it)
            node.OnClosedList = true;
        }
        //Open List empty, thus no path possible (RETURN “fail”)
        return new List<WayPoint>();
    }

    private static List<WayPoint> ConstructPath(WayPoint endNode, WayPoint startNode, Vector3 actualEnd)
    {
        WayPoint curNode = endNode;
        List<WayPoint> ret = new List<WayPoint>();
        ret.Add(new WayPoint(actualEnd, 0));
        while (curNode != startNode)
        {
            ret.Add(curNode);
            curNode = curNode.parent;
        }

        ret.Add(startNode);

        ret.Reverse();

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

    private static float EuclidianCost(Vector3 p1, Vector3 p2)
    {
        return (p1 - p2).magnitude;

    }
}
