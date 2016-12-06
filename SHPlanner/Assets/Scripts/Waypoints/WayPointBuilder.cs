using UnityEngine;
using System.Collections;

public class WayPointBuilder : MonoBehaviour
{
  public Material mat;
  // Use this for initialization

  static int AgentLayer = 8;
  static int InteractiveObjectLayer = 9;

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
