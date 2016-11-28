/******************************************************************************/
/*!
@file   CameraControllerUtilities.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype
{

  public partial class CameraController : StratusBehaviour
  {    
    /// <summary>
    /// Calculates the next position of the camera around the target.
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="height"></param>
    /// <param name="angle"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    Vector3 CalculatePositionAround(float radius, float height, float angle, Transform target)
    {
      var targetForwardAngle = target.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
      //Trace.Script("Angle = " + angle + ", Target angle = " + targetForwardAngle);
      var newPosition = new Vector3(Mathf.Sin(angle + targetForwardAngle) * radius,
                                height,
                                Mathf.Cos(angle + targetForwardAngle) * radius)
                                + target.transform.position;

      return newPosition;
    }    

    /// <summary>
    /// Updates the current angle.
    /// </summary>
    void UpdateCurrentAngle()
    {
      this.CurrentAngle = Mathf.Lerp(this.CurrentAngle,
                               this.Angle,
                               Time.deltaTime * this.Movement.Damping);
    }

  }

}