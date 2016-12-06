using UnityEngine;
using System.Collections;
using System.Linq;

public class WayPoint
{
    public Vector3 Location;
    public ArrayList Neighbors = new ArrayList();
    public bool OnClosedList = false;
    public float GivenCost = 0;
    public float Cost = 0;
    public WayPoint parent;
    public int ID;

    public WayPoint(Vector3 location, int ID)
    {
        Location = location;
        this.ID = ID;
    }
}
