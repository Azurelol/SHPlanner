using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PointList
{
    
    private Dictionary<int, WayPoint> _nodes = new Dictionary<int, WayPoint>();

    public PointList()
    {
        
    }
    public WayPoint Remove()
    {
        var minNode = _nodes.First();
        int minkey = minNode.Key;
        foreach (var curNode in _nodes)
        {
            if (curNode.Value.Cost < minNode.Value.Cost)
            {
                minNode = curNode;
                minkey = curNode.Key;
            }
        }
        
        _nodes.Remove(minkey);

        return minNode.Value;
    }

    public void Add(WayPoint node)
    {
        _nodes[hashify(node)] = node;
    }

    public bool Count(WayPoint node)
    {
        return _nodes.ContainsKey(hashify(node));
    }
    public void Reset()
    {
        _nodes.Clear();
    }

    public bool Empty()
    {
        return _nodes.Count <= 0;
    }

    public WayPoint Get(WayPoint node)
    {
        return _nodes[hashify(node)];
    }

    private int hashify(WayPoint node)
    {
        return node.ID;
    }
}
