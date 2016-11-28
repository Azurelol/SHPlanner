/******************************************************************************/
/*!
@file   CameraControllerBehaviors.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype 
{
  public partial class CameraController : StratusBehaviour
  {
    /// <summary>
    /// Follows the current target, if one is set.
    /// </summary>
    void Follow()
    {
      if (!this.FollowTarget)
        return;

      //Trace.Script("FOLLOW");
      // Update the current angle
      this.UpdateCurrentAngle();

      // Calculate the new position of the camera around the target
      var newPosition = this.CalculatePositionAround(this.Movement.Radius,
                                                this.Movement.Height,
                                                this.CurrentAngle,
                                                FollowTarget);

      // Apply the new translation
      this.transform.position = Vector3.Lerp(this.transform.localPosition, newPosition, Time.deltaTime * this.Movement.Damping);

    }

    /// <summary>
    /// Follows the target in an orbital motion, updating the angle around the target
    /// every frame (by a specified speed)
    /// </summary>
    void Orbit()
    {
      //Trace.Script("Orbiting!");
      // Change the angle every frame
      this.Movement.Angle -= 0.5f;
      // Is there a better way?
      //if (this.Movement.Angle >= 360.0f) this.Movement.Angle = 0.0f;
      // Now apply follow behavior
      this.Follow();
    }

    /// <summary>wwww
    /// Tracks the current target.
    /// </summary>
    void Track()
    {
      if (!this.ViewTarget)
        return;

      // Depending on what look direction the camera is to use..
      var targetCenter = new Vector3(0.0f, this.ViewTarget.transform.localScale.y / 2, 0.0f);
      //Trace.Script("Offset = " + this.View.Offset);
      var lookPosition = new Vector3();
      switch (View.Direction)
      {
        case LookDirection.TargetPosition:
          lookPosition = this.ViewTarget.transform.position + this.View.Offset + targetCenter;
          break;
        case LookDirection.TargetForward:
          lookPosition = this.ViewTarget.transform.position + this.ViewTarget.transform.forward + this.View.Offset + targetCenter;
          break;
      }
      // Calculate the new rotation
      lookPosition = lookPosition - transform.position;
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPosition), this.View.Damping);
      //this.transform.LookAt(lookPosition, Vector3.up);
    }

    /// <summary>
    /// Reposition the camera around the current target.
    /// </summary>
    /// <param name="transition"></param>
    void Reposition(Transition transition)
    {
      // Calculate the new position
      var newPos = this.CalculatePositionAround(Movement.Radius, Movement.Height,
                                          Movement.Angle, FollowTarget.transform);
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Translate(seq, this.transform, newPos, transition.Duration, transition.Easing);
    }

    /// <summary>
    /// Reposition the camera around a given target.
    /// </summary>
    /// <param name="transition"></param>
    void Reposition(Transform target, MovementConfiguration movement, Transition transition)
    {
      //Trace.Script("REPOSITION!");
      // Switch to fixed immediately
      this.SetMovement(target, MovementMode.Fixed, movement, new Transition());
      // Calculate the new position
      var newPosition = this.CalculatePositionAround(Movement.Radius, Movement.Height,
                                          Movement.Angle, FollowTarget.transform);
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Translate(seq, this.transform, newPosition, transition.Duration, transition.Easing);
    }


    /// <summary>
    /// Has the camera look at a given target over a period of time.
    /// </summary>
    /// <param name="transition"></param>
    void LookAt(Transform target, OrientationConfiguration view, Transition transition)
    {
      // Switch to fixed immediately
      this.SetView(target, LookMode.Fixed, view, new Transition());


      //var lookVec = Vector3.Normalize(this.transform.position - this.ViewTarget.transform.position + this.View.Offset);
      //var lookRot = Quaternion.LookRotation(lookVec, Vector3.up);


      //Trace.Script("Now looking at " + this.ViewTarget, this);

      // @WILLIAM: These methods don't work. 
      //var seq = Actions.Sequence(this.gameObject.Actions());
      //Actions.Property(seq, () => this.transform.rotation.eulerAngles, lookVec, transition.Duration, transition.Easing);
      //Actions.Rotate(seq, this.transform, lookVec, transition.Duration, transition.Easing);

      this.transform.LookAt(this.ViewTarget.position + this.View.Offset, Vector3.up);
    }

    /// <summary>
    /// Zooms the camera towards the target. If there is no target, will 
    /// zoom ahead.
    /// </summary>
    /// <param name="target">The target to zoom towards.</param>
    /// <param name="amount">The amount of the zoom. </param>
    /// <param name="transition">The transition which to use.</param>
    void Zoom(float amount, Transition transition)
    {
      var seq = Actions.Sequence(this.gameObject.Actions());

      // Orthographic 
      if (Camera.orthographic)
      {
        Actions.Property(seq, () => Camera.orthographicSize, amount, transition.Duration, transition.Easing);
      }
      // Perspective
      else
      {
        Actions.Property(seq, () => Camera.fieldOfView, amount, transition.Duration, Ease.Linear);
      }
    }

    /// <summary>
    /// Zooms the camera back to its previous zoom level.
    /// </summary>
    /// <param name="transition">The transition which to use.</param>
    void ZoomBack(Transition transition)
    {
      var seq = Actions.Sequence(this.gameObject.Actions());
      // Orthographic
      if (Camera.orthographic)
      {
        Actions.Property(seq, () => Camera.orthographicSize, this.Size, transition.Duration, transition.Easing);
      }
      // Perspective
      else
      {
        Actions.Delay(seq, transition.Duration);
        Actions.Call(seq, Camera.ResetFieldOfView);
      }
    }

    /// <summary>
    /// Zooms the camera back, using a default transition.
    /// </summary>
    void ZoomBack()
    {
      var transition = new Transition();
      transition.Duration = 1.0f;
      transition.Easing = Ease.QuadOut;
      this.ZoomBack(transition);
    }

    /// <summary>
    /// Gives player control of the camera.
    /// </summary>
    void Move(ref Vector2 axis, ref float sensitivity)
    {
      // Calculate the offset
      Vector3 offset = new Vector3(axis.x * sensitivity, axis.y * sensitivity, 0.0f);
      //Vector3 offsetY = new Vector3(0.0f, axis.y * sensitivity, 0.0f);
      offset = transform.rotation * offset;
      // Update the camera position
      transform.position = offset;
    }

    Countdown FreeLookTimer = new Countdown(1f);

    /// <summary>
    /// Move along the last player input.
    /// </summary>
    void Move()
    {
      // If the timer...
      if (FreeLookTimer.Update(Time.fixedDeltaTime))
      {
        Trace.Script("Reverting!");
        this.RevertMovement();
        return;
      }

      //Trace.Script("Moving!");
      //this.Move(ref CurrentFreeLook.Axis, ref CurrentFreeLook.Sensitivity);

    }


  }  
}
