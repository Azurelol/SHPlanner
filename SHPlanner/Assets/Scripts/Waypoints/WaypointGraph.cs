using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class WaypointGraph
{
    
    private static ArrayList _waypoints = new ArrayList();
    public static bool Initialized = false;
    private const int _maxJumpHeight = 2;
    private const int _maxJumpLength = 100;
    private const float _marchingBoxHeight = 1.2f;
    private const float _minimumWalkHeight = 0.7f;
    public static AStarNode endNode;
    public static AStarNode startNode;
    private enum MarchResult
    {
        Valid,
        Invalid,
        RequiresJump
    }
    static WaypointGraph()
    {
        
    }
    /// <summary>
    /// initialization
    /// </summary>
    public static void Initialize()
    {
        Initialized = true;

        var objs = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (var obj in objs)
        {
            var way = new WayPoint(obj.transform.position);
            _waypoints.Add(way);
            if (obj.name == "GameObject (28)")
            {
                endNode = new AStarNode(way);
            }
            else if(obj.name == "GameObject (24)")
            {
                startNode = new AStarNode(way);
            }
            GameObject.Destroy(obj);
        }

        for (int i = 0; i < _waypoints.Count; ++i)
        {
            AddNeighbors(i);
        }
    }

    /// <summary>
    /// debug
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="direction"></param>
    /// <param name="color"></param>
    /// <param name="headColor"></param>
    /// <param name="arrowHeadLength"></param>
    /// <param name="arrowHeadAngle"></param>
    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, Color headColor, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
       // Debug.DrawRay(pos, direction, color);
        GameObject obj1 = new GameObject();
        LineRenderer ren = obj1.AddComponent<LineRenderer>();

        ren.SetVertexCount(2);
        ren.SetPosition(0, pos);
        ren.SetPosition(1, pos + direction);
        ren.material.color = color;
        ren.SetColors(color, color);
        ren.SetWidth(0.1f, 0.1f);
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

        GameObject obj2 = new GameObject();
        ren = obj2.AddComponent<LineRenderer>();

        ren.SetVertexCount(2);
        ren.SetPosition(0, pos + direction);
        ren.SetPosition(1, pos + direction + right * arrowHeadLength);
        ren.material.color = headColor;
        ren.SetColors(headColor, headColor);
        ren.SetWidth(0.1f, 0.1f);
        // Debug.DrawRay(pos + direction, right * arrowHeadLength, headColor); GameObject obj = new GameObject();
        GameObject obj3 = new GameObject();
        ren = obj3.AddComponent<LineRenderer>();

        ren.SetVertexCount(2);
        ren.SetPosition(0, pos + direction);
        ren.SetPosition(1, pos + direction + left * arrowHeadLength);
        ren.material.color = headColor;
        ren.SetColors(headColor, headColor);
        ren.SetWidth(0.1f, 0.1f);
        //Debug.DrawRay(pos + direction, left * arrowHeadLength, headColor);
    }
    public static void DebugDraw()
    {
        foreach (var point in _waypoints)
        {
            GameObject obj = new GameObject();
            LineRenderer ren = obj.AddComponent<LineRenderer>();

            ren.SetVertexCount(2);
            ren.SetPosition(0, ((WayPoint) point).Location);
            ren.SetPosition(1, ((WayPoint)point).Location + Vector3.up);
            ren.SetColors(Color.blue, Color.blue);
            ren.SetWidth(0.1f,0.1f);
            foreach (KeyValuePair<WayPoint, bool> neighbor in ((WayPoint)point).Neighbors)
            {
                if (!neighbor.Value)
                {
                    DrawArrow(((WayPoint)point).Location, neighbor.Key.Location - ((WayPoint)point).Location, Color.green, new Color(0.1f,1,0.1f,1), 0.5f);
                }
                else
                {
                    DrawArrow(((WayPoint)point).Location, neighbor.Key.Location - ((WayPoint)point).Location, Color.cyan, new Color(0.1f, 1, 1, 1), 0.5f);
                }
            }
        }
    }

    private static void AddNeighbors(int index)
    {
        for (int i = 0; i < _waypoints.Count; ++i)
        {
            if (i != index)
            {

                var result = MarchingConnectionCheck(((WayPoint)_waypoints[index]), ((WayPoint)_waypoints[i]));
                switch (result)
                {
                    case MarchResult.Valid:
                        ((WayPoint) _waypoints[index]).Neighbors.Add(new KeyValuePair<WayPoint, bool>(((WayPoint)_waypoints[i]), false));
                        break;
                    case MarchResult.Invalid:
                        break;
                    case MarchResult.RequiresJump:
                        ((WayPoint)_waypoints[index]).Neighbors.Add(new KeyValuePair<WayPoint, bool>(((WayPoint)_waypoints[i]), true));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    /// <summary>
    /// connection generation
    /// </summary>
    /// <param name="point1">
    /// yes
    /// </param>
    /// <param name="point2"></param>
    /// <returns>
    /// Whether the connection is invalid, or it needs some sort of special action
    /// </returns>
    private static MarchResult MarchingConnectionCheck(WayPoint point1, WayPoint point2)
    {
        if (Physics.Raycast(new Ray(point1.Location, point2.Location - point1.Location), (point2.Location - point1.Location).magnitude))
        {
            return MarchResult.Invalid;
        }
        Vector3 toVec = point2.Location - point1.Location;
        float density = 2;
        float length = toVec.magnitude * density;
        Ray ray1 = new Ray(point1.Location, Vector3.down);
        RaycastHit hit1 = new RaycastHit();
        Physics.Raycast(ray1, out hit1);
        float lastHeight = hit1.point.y;
        if (toVec.y > _minimumWalkHeight)
        {
            Vector2 horizontalVec = new Vector2(toVec.x,toVec.z);

            if (toVec.y > _maxJumpHeight || horizontalVec.sqrMagnitude > _maxJumpLength)
            {
                return MarchResult.Invalid;
            }
            else
            {
                return MarchResult.RequiresJump;
            }
        }

        for (int i = 0; i < length * density; ++i)
        {
            Ray ray = new Ray(point1.Location + toVec * (i / length) / density, Vector3.down);
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(ray, out hit);
            if (hit.collider.gameObject.tag == "Ramp")
            {
                continue;
            }
            if (hit.point.y > lastHeight + _minimumWalkHeight)
            {
                if (toVec.sqrMagnitude > _maxJumpLength)
                {
                    return MarchResult.Invalid;
                }
                else
                {
                    return MarchResult.RequiresJump;
                }
            }
            if (hit.point.y < lastHeight - _minimumWalkHeight)
            {
                int fo = 2;
            }
            lastHeight = hit.point.y;
        }

        return MarchResult.Valid;
    }
}
