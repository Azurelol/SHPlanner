using UnityEngine;
using System.Collections;

public class Finder : MonoBehaviour
{
    private WayPoint[] path;
    private int curPoint = 0;
    private Vector3 target = new Vector3();
	// Use this for initialization
	void Start ()
    {
      path = AStar.FindPath(transform.position, GameObject.Find("Sphere").transform.position).ToArray();
	    target = path[curPoint].Location;
	}
	
	// Update is called once per frame
	void Update () {
        if (path != null && curPoint == path.Length - 1)
        {
            //TODO:call the callback
            print("yo");
        }
        Vector3 move = Vector3.zero;
        float dist = 2;
	    var loc = path[curPoint].Location;
        dist = (path[curPoint].Location - transform.position).sqrMagnitude;
        if (dist < 1 && curPoint != path.Length - 1)
        {
            ++curPoint;
            target = path[curPoint].Location;
        }

        if (true)
        {
            move = target - transform.position;
            move = move.normalized * Time.deltaTime;
        }
        //controller.Move(move);
	    transform.position += move;
	}
}
