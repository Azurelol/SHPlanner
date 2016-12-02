using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PointList
{
    
    private Dictionary<int, AStarNode> _nodes = new Dictionary<int, AStarNode>();

    public PointList()
    {
        
    }
    public AStarNode Remove()
    {
        var minNode = _nodes.First();
        int minkey = 0;
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

    public void Add(AStarNode node)
    {
        _nodes[hashify(node)] = node;
    }

    public bool Count(AStarNode node)
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

    public AStarNode Get(AStarNode node)
    {
        return _nodes[hashify(node)];
    }

    private int hashify(AStarNode node)
    {
        return (int) (((int) node.MyPoint.Location.x) ^ ((int)node.MyPoint.Location.y) ^ ((int)node.MyPoint.Location.z) + 0x9e3779b9 + (((int)node.MyPoint.Location.x) << 6) + (((int)node.MyPoint.Location.x) >> 2) + (((int)node.MyPoint.Location.z) >> 2));
    }
}
