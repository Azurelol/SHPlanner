/******************************************************************************/
/*!
@file   CurvePath.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype
{
  public class CurvedPath
  {
    Vector3[] Values = new Vector3[4];

    public static Vector3[] CreateDividedDifferenceValues(Vector3 startPos, Vector3 startVel, Vector3 endPos, Vector3 endVel)
    {
      Vector3[] values = new Vector3[4];
      Vector3[][] dividedDifferenceTable = new Vector3[3][];
      dividedDifferenceTable[0] = new Vector3[] { startVel, endPos - startPos, endVel };

      for (int i = 1; i < dividedDifferenceTable.Length; ++i)
      {
        Vector3[] currentColumn = dividedDifferenceTable[i - 1];
        Vector3[] temp = new Vector3[currentColumn.Length - 1];
        for (int j = 0; j < temp.Length; ++j)
        {
          temp[j] = currentColumn[j + 1] - currentColumn[j];
        }
        dividedDifferenceTable[i] = temp;
      }

      values[0] = startPos;
      values[1] = startVel;
      values[2] = dividedDifferenceTable[1][0];
      values[3] = dividedDifferenceTable[2][0];
      return values;
    }

    public static Vector3 GetPointAlongPath(Vector3[] values, float t)
    {
      return values[0] + values[1] * t + values[2] * t * t + values[3] * t * t * (t - 1);
    }

    public static Movement.Path CalculatePath(Vector3 startPos, Vector3 startDir, Vector3 endPos, float speed, float approachAngle, float weight)
    {
      var distToTarget = Vector3.Distance(startPos, endPos);
      var approachAngleRads = Mathf.Deg2Rad * approachAngle;
      // Fixes the end velocity
      var angle = Mathf.Acos(Vector3.Dot(Vector3.forward, (startPos - endPos).normalized));
      Vector3 approachVector = new Vector3(Mathf.Cos(approachAngleRads), 0f, Mathf.Sin((approachAngleRads))) * speed;
      
      // The velocity used by the agent when starting on the path. This includes the direction.
      Vector3 startVel = startDir * speed * weight;
      // The vector which the agent will use to make its approach to the target, using the approach angle
      Vector3 endVel = Quaternion.AngleAxis(angle, Vector3.up) * approachVector * weight;
      
      // Now let's compute the divided difference values..
      var values = CreateDividedDifferenceValues(startPos, startVel, endPos, endVel);
      // Using these values, let's get points along the path
      // Get all the way points to the target
      var path = new Movement.Path();
      float numberOfPoints = (int)distToTarget * 5;
      //numberOfPoints = 20.0f;
      //Trace.Script("Starting position = " + source.position + ", Target position = " + target.position);
      //Trace.Script("Number of points = " + numberOfPoints + ", startVel = " + startVel + ", endVel = " + endVel);
      for (int i = 0; i < numberOfPoints; ++i)
      {
        float t = i / numberOfPoints;
        Vector3 waypoint = GetPointAlongPath(values, t);
        //Trace.Script("t = " + t + ", waypoint = " + waypoint);
        path.Add(waypoint);
      }

      // Need to return the next position
      return path;
    }

    public void CreateDividedDifference(Vector3 startPos, Vector3 startVel, Vector3 endPos, Vector3 endVel)
    {
      Values = CreateDividedDifferenceValues(startPos, startVel, endPos, endVel);
    }

    /// <summary>
    /// Gets a point along the path with value t.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPoint(float t)
    {
      return GetPointAlongPath(Values, t);
    }

    /// <summary>
    /// Calculates a static path. This is followed by calling 'GetStaticPath()' every frame.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="startVel"></param>
    /// <param name="endPos"></param>
    /// <param name="endVel"></param>
    //public void CalculatePath(Vector3 startPos, Vector3 startVel, Vector3 endPos, Vector3 endVel)
    //{
    //  this.CreateDividedDifference(startPos, startVel, endPos, endVel);
    //}

    public Vector3 GetPointAlongPath(float t)
    {
      return Values[0] + Values[1] * t + Values[2] * t * t + Values[3] * t * t * (t - 1);
    }

		public Vector3 GetVelocityAlongPath(float t)
		{
			return Values[1] + Values[2] * 2 * t + Values[3] * (3 * t * t - 2 * t);
		}

		public Vector3 GetAccelerationAlongPath(float t)
		{
			return Values[2] * 2 + Values[3] * (6 * t - 2); 
		}

    public Movement.Path CalculateWaypoints(Transform source, Transform target, float speed, float approachAngle, float weight)
    {
      return CalculateWaypoints(source.transform.position, target.transform.position, speed, approachAngle, weight);
    }


		public Movement.Path CalculateWaypoints(Vector3 source, Vector3 target, float speed, float approachAngle, float weight)
    {
      var distToTarget = Vector3.Distance(source, target);
      var approachAngleRads = Mathf.Deg2Rad * approachAngle;
      // Fixes the end velocity
      var angle = Mathf.Acos(Vector3.Dot(Vector3.forward, (source - target).normalized));    
      Vector3 approachVector = new Vector3(Mathf.Cos(approachAngleRads), 0f, Mathf.Sin((approachAngleRads))) * speed;

      // The vector which the agent will use to start the path
      Vector3 startVel = new Vector3(Mathf.Cos(30.0f), 0f, Mathf.Sin(30.0f)) * speed;
      // The vector which the agent will use to make its approach to the target, using
      // the approach angle
      Vector3 endVel = Quaternion.AngleAxis(angle, Vector3.up) * approachVector * weight;
      // Now compute the path
      this.CreateDividedDifference(source, startVel, target, endVel);


      // Get all the way points to the target
      var waypoints = new Movement.Path();
      float numberOfPoints = (int)distToTarget;
      //numberOfPoints = 20.0f;
      //Trace.Script("Starting position = " + source.position + ", Target position = " + target.position);
      //Trace.Script("Number of points = " + numberOfPoints + ", startVel = " + startVel + ", endVel = " + endVel);
      for (int i = 0; i < numberOfPoints; ++i)
      {
        float t = i / numberOfPoints;
        Vector3 waypoint = GetPointAlongPath(t);
        //Trace.Script("t = " + t + ", waypoint = " + waypoint);
        waypoints.Add(waypoint);
      }

      // Need to return the next position
      return waypoints;
    }
  }

	//public class CurvedPathInterpolator
	//{
	//	private Vector3[] Values = new Vector3[3];
	//	CurvedPathInterpolator(Vector3 startPos, Vector3 startVel, Vector3 endPos)
	//	{
	//		Values[0] = startPos; Values[1] = startVel;
	//		Values[2] = (endPos - startPos) - startVel;
	//	}
	//	public Vector3 GetAcceleration()
	//	{
	//
	//	}
	//}

}
