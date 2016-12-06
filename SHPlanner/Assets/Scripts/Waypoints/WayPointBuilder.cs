using UnityEngine;
using System.Collections;

public class WayPointBuilder : MonoBehaviour
{
  public Material mat;
  // Use this for initialization

  static int AgentLayer = 8;
  static int InteractiveObjectLayer = 9;

    public void Toggle()
    {
        WaypointGraph.showVerts = !WaypointGraph.showVerts;
        if (WaypointGraph.showVerts)
        {
            foreach (LineRenderer line in WaypointGraph.Lines)
            {
                line.material.color = new Color(0, 0, 0, 0);
            }
        }
        else
        { 
            foreach (LineRenderer line in WaypointGraph.Lines)
            {
                line.material.color = Color.green / WaypointGraph.colorscale;
            }
        }
    }
  void Awake()
  {
    WaypointGraph.Initialize();
    WaypointGraph.EdgeMat = mat;
    WaypointGraph.DebugDraw();

    Physics.IgnoreLayerCollision(AgentLayer, InteractiveObjectLayer);
    Physics.IgnoreLayerCollision(AgentLayer, AgentLayer);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
