/******************************************************************************/
/*!
@file   MovementEvents.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Prototype
{
  public partial class Movement : StratusBehaviour
  {
    //--------------------------------------------------------------------------/
    // Event Definitions
    //--------------------------------------------------------------------------/
    public abstract class MovementEvent : Stratus.Event
    {
      public Vector3 Vector;
      public float Speed = 1.0f;
    }

    /// <summary>
    /// Moves by applying a velocity to the RigidBody. If the speed is not set (left at 0f)
    /// it will use its default.
    /// </summary>
    public class MoveEvent : MovementEvent
    {
      public Direction Direction;
      public float Acceleration = 2.0f;
      public float MaximumVelocity = 10.0f;
      public float Drag = 20.0f;
      public MoveEvent(Vector3 vec) { Vector = vec; }
      public MoveEvent(Direction dir) { Direction = dir; }
    }
    /// <summary>
    /// Jumps by applying a force opposite gravity.
    /// </summary>
    public class JumpEvent : MovementEvent
    {

    }

    /// <summary>
    /// Applies an impulse along a given direction.
    /// </summary>
    public class DashEvent : MovementEvent
    {

    }

    /// <summary>
    /// Rotates the agent along a given direction.
    /// </summary>
    public class RotateEvent : MovementEvent
    {
      public Direction Direction;
      public RotateEvent(Direction dir) { Direction = dir; }
    }

    /// <summary>
    /// When movement has started.
    /// </summary>
    public class StartedEvent : Stratus.Event { }
    /// <summary>
    /// When movement has ended.
    /// </summary>
    public class EndedEvent : Stratus.Event { }
    /// <summary>
    /// Moves to the agent to the target destination. It will compute a path to it.
    /// </summary>
    public class MoveToTargetEvent : Stratus.Event
    {
      public Vector3 Target;
      public Path.Configuration Configuration = new Path.Configuration();
    }
    /// <summary>
    /// Asks the agent to follow the list of waypoints at the given speed
    /// </summary>
    public class FollowPathEvent : Stratus.Event
    {
      public Path Waypoints;
      public float Speed;
      public FollowPathEvent(Path path, float speed) { Waypoints = path; Speed = speed; }
    }
    /// <summary>
    /// Signals that this agent has finished moving along the recent path of waypoints.
    /// </summary>
    public class ReachedEndOfPathEvent : Stratus.Event
    {
      public Vector3 Position;
      public ReachedEndOfPathEvent(Vector3 position) { Position = position; }
    }
    /// <summary>
    /// Cancels the current path.
    /// </summary>
    public class StopFollowingPathEvent : Stratus.Event
    {
    }
    /// <summary>
    /// Tries to approach the given target. Computes a path to it. If the target
    /// is not within a specified range when it arrives to the destination,
    /// it will attempt again.
    /// </summary>
    public class ApproachTargetEvent : Stratus.Event
    {
      public Transform Target;
      public Path.Configuration Configuration = new Path.Configuration();
    }
    /// <summary>
    /// Dispatched when this agent has entered the range of the target
    /// </summary>
    public class ReachedTargetEvent : Stratus.Event
    {
    }

    /// <summary>
    /// Sidesteps along a cardinal direction based on the agent's forward
    /// </summary>
    public class SidestepEvent : Stratus.Event
    {
      public Direction Direction;
      public Distance Distance;
      public SidestepEvent(Direction dir, Distance dist) { Direction = dir; Distance = dist; }
    }
    /// <summary>
    /// Looks at the specified target.
    /// </summary>
    public class LookAtEvent : Stratus.Event
    {
      public Vector3 Target;
      public LookAtEvent(Vector3 target) { Target = target; }
    }

    public class SidestepStartedEvent
    {
      public float Duration;
      public SidestepStartedEvent(float duration) { Duration = duration; }
    }
    /// <summary>
    /// Disables this component's behavior temporarily
    /// </summary>
    public class DisableEvent : Stratus.Event
    {
      public float Duration = 0.5f;
      public DisableEvent(float duration) { Duration = duration; }
    }

    //--------------------------------------------------------------------------/
    // Event Callbacks
    //--------------------------------------------------------------------------/
    /// <summary>
    /// Subscribe to movement events
    /// </summary>
    void Subscribe()
    {
      this.gameObject.Connect<MoveEvent>(this.OnMoveEvent);
      this.gameObject.Connect<LookAtEvent>(this.OnLookAtEvent);
      this.gameObject.Connect<JumpEvent>(this.OnJumpEvent);
      this.gameObject.Connect<SidestepEvent>(this.OnSidestepEvent);
      this.gameObject.Connect<DisableEvent>(this.OnDisableEvent);
      this.gameObject.Connect<FollowPathEvent>(this.OnFollowPathEvent);
      this.gameObject.Connect<MoveToTargetEvent>(this.OnMoveToTargetEvent);
      this.gameObject.Connect<StopFollowingPathEvent>(this.OnStopFollowingPathEvent);
      this.gameObject.Connect<ApproachTargetEvent>(this.OnApproachTargetEvent);
    }

    void OnDisableEvent(DisableEvent e)
    {
      this.Disable(e.Duration);
    }

    /// <summary>
    /// Invoked when a move event is received
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMoveEvent(MoveEvent e)
    {
      if (!Enabled)
        return;

      if (OnCooldown())
        return;

      // Record the current movement direction vector
      CurrentMovementDirection = e.Vector;

      if (e.Vector != Vector3.zero)
      {
        if (e.Speed > 0.0f)
          this.Move(e.Vector, e.Speed, e.Acceleration);
      }
      else
      {
        if (e.Speed > 0.0f)
          this.Move(Utilities.ToVector(e.Direction, transform), e.Speed, e.Acceleration);
        //else
        //  this.Move(Utilities.ToVector(e.Direction, transform), this.MoveSpeed);
      }
    }

    void OnApproachTargetEvent(ApproachTargetEvent e)
    {
      this.ApproachTarget(e.Target, e.Configuration);
    }

    void OnMoveToTargetEvent(MoveToTargetEvent e)
    {
      this.MoveToTarget(e.Target, e.Configuration);
    }

    void OnFollowPathEvent(FollowPathEvent e)
    {
      Trace.Script("hey!");
      if (e.Speed > 0.0f)
        FollowPath(e.Waypoints, e.Speed);
      //else
      //  FollowPath(e.Waypoints, this.MoveSpeed,);
    }

    void OnStopFollowingPathEvent(StopFollowingPathEvent e)
    {
      this.StopFollowingPath();
    }


    protected void OnRotateEvent(RotateEvent e)
    {
      this.Rotate(e.Direction, e.Speed);
    }

    protected void OnJumpEvent(JumpEvent e)
    {
      this.Jump(e.Speed);
    }

    protected void OnSidestepEvent(SidestepEvent e)
    {
      this.Sidestep(e.Direction, e.Distance);
    }

    protected void OnLookAtEvent(LookAtEvent e)
    {
      LookAt(e.Target);
    }


    
    //--------------------------------------------------------------------------/
    // Custom types
    //--------------------------------------------------------------------------/
    public enum LocomotionMode { Translation, Rigidbody }
    public enum Mode { Forces, Velocities, Path, Idle }

    public class Path
    {
      [Serializable]
      public class Configuration
      {        
        public float ApproachAngle = 90.0f;
        public float Speed = 0.0f;
        public float Weight = 1.0f;
        public float Range = 0.1f;
        public Configuration(float approachAngle, float speed, float weight)
        {
          ApproachAngle = approachAngle;
          Speed = speed;
          Weight = weight;
        }
        public Configuration() { }

        //public override 
        //
        //public static bool operator !=(Configuration lhs, Configuration rhs)
        //{
        //  if (lhs.Destination == rhs.Destination
        //      && lhs.ApproachAngle == rhs.ApproachAngle
        //      && lhs.Speed == rhs.Speed
        //      && lhs.Weight == rhs.Weight)
        //    return false;
        //
        //  return true;
        //}
        //public static bool operator==(Configuration lhs, Configuration rhs)
        //{
        //  if (lhs.Destination != rhs.Destination
        //      && lhs.ApproachAngle != rhs.ApproachAngle
        //      && lhs.Speed != rhs.Speed
        //      && lhs.Weight != rhs.Weight)
        //    return false;
        //
        //  return true;
        //}
      }

      List<Vector3> Waypoints = new List<Vector3>();
      public void Add(Vector3 waypoint) { Waypoints.Add(waypoint); }
      public List<Vector3> All { get { return Waypoints; } }
    }



  }




  // Movement direction

  public static class Utilities
  {
    public static Vector3 ToVector(Direction dir, Transform transform)
    {
      Vector3 directionVector = new Vector3();
      switch (dir)
      {
        case Direction.Forward:
          directionVector = transform.forward;
          break;
        case Direction.Backward:
          directionVector = -transform.forward;
          break;
        case Direction.Left:
          directionVector = -transform.right;
          break;
        case Direction.Right:
          directionVector = transform.right;
          break;
      }

      return directionVector;
    }
  }



}