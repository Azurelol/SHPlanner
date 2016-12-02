using UnityEngine;
using System.Collections;
using System.Linq;

public class WayPoint
{
    public Vector3 Location;
    public ArrayList Neighbors = new ArrayList();
    public WayPoint(Vector3 location)
    {
        Location = location;
    }
}
