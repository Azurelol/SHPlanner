using UnityEngine;
using System.Collections;

public class WayPointBuilder : MonoBehaviour
{
    public Material mat;
	// Use this for initialization
	void Awake ()
    {
        WaypointGraph.Initialize();
        WaypointGraph.EdgeMat = mat;
        WaypointGraph.DebugDraw();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
