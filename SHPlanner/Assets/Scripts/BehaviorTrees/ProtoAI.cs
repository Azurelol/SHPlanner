using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProtoAI : MonoBehaviour
{
    private BehaviorTree tree;
    private KinematicController controller;
    private AIBehaviors behaviors;
    private GameObject player;
    private WayPoint [] nodeList;
    private int curNode;
    // Use this for initialization
    void Start ()
    {
        controller = GetComponent<KinematicController>();
        behaviors = GetComponent<AIBehaviors>();
        player = GameObject.Find("Player");
        behaviors = GetComponent<AIBehaviors>();
	    tree = new BehaviorTree(gameObject);
        tree.AddNode((int)BTNodeTypes.Selector, 0).
                AddNode((int)BTNodeTypes.WithinInRange, 1, new RangeData(10, 100)).
                    AddNode((int)BTNodeTypes.ApproachTarget, 2).
                AddNode((int)BTNodeTypes.WithinInRange, 1, new RangeData(0, 10)).
                    AddNode((int)BTNodeTypes.EnoughRoom, 2, new RoomData(4)).
                        AddNode((int)BTNodeTypes.AttackShort, 3); ;
        tree.Initialize();

        if (!WaypointGraph.Initialized)
        {
            WaypointGraph.Initialize();
        }

        nodeList = AStar.FindPath(transform.position, player.transform.position).ToArray();
        curNode = 0;
        WaypointGraph.DebugDraw();
        behaviors.MovementTarget = nodeList[curNode].Location;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    //behaviors.Target = player.transform.position;
        //BehaviorTreeSystem.Update(tree);
        Vector3 move = Vector3.zero;
	    float dist = 2;
	    
        dist = (nodeList[curNode].Location - transform.position).sqrMagnitude;
	    if (dist < 1 && curNode != nodeList.Length - 1)
	    {
            ++curNode;
            behaviors.MovementTarget = nodeList[curNode].Location;
        }

        if (true || behaviors.MovementTarget != Vector3.zero)
        {
            move = behaviors.MovementTarget - transform.position;
            move = move.normalized;
        }
        controller.Move(move);
    }
}
