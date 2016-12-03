/******************************************************************************/
/*!
@file   Movement.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections;

namespace Prototype
{
  public enum Direction { Forward, Backward, Up, Down, Left, Right }

  /// <summary>
  /// Base component that drives movement for all agents.
  /// </summary>
  [RequireComponent(typeof(Rigidbody))]
  public partial class Movement : StratusBehaviour
  {
    public enum Distance { Extra, Long, Short }

    //--------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------/
    public bool Enabled = true;
    public LocomotionMode Type = LocomotionMode.Rigidbody;
    [Tooltip("Relative Movement Speed Cap")]
    public float DragFactor = 10.0f;
    public float SlopeDetectionAngle = 5;
    public float PushBackEpsilon = .1f;
    [HideInInspector]
    Mode CurrentMode = Mode.Forces;
    public bool Debugging = false;
    //--------------------------------------------------------------------------/    
    protected Rigidbody RigidBody;
    protected Vector3 CurrentMovementDirection;
    float TurnTimeCounter;
    bool IsJumping = false;
    // Path
    IEnumerator PathRoutine;
    Vector3 CurrentDestination;
    Path CurrentPath;
    LineRenderer LineRenderer;
    CapsuleCollider Collider;
    // Cooldowns
    float Cooldown = 0.0f; // Time before the next action
    float SidestepCooldown = 1.0f;
    static float PathEpsilon = 0.1f;
    //--------------------------------------------------------------------------/
    // Virtual
    //--------------------------------------------------------------------------/
    protected virtual void OnMove(Vector3 dir, float speed, float acceleration) {}
    protected virtual void OnUpdate() {}
    //--------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the component
    /// </summary>
    void Start()
    {
      this.RigidBody = GetComponent<Rigidbody>();
      this.Subscribe();
      Collider = GetComponent<CapsuleCollider>();

      if (Debugging)
      {
        this.LineRenderer = this.gameObject.AddComponent<LineRenderer>();
        this.LineRenderer.SetColors(Color.red, Color.blue);
      }
    }

    /// <summary>
    /// Updates movement
    /// </summary>
    void Update()
    {
      //transform.position = Vector3.MoveTowards(transform.position, new Vector3(-25, 0, -25), this.MoveSpeed * Time.deltaTime);

      if (Debugging)
      {
        if (CurrentMode == Mode.Path && this.CurrentPath != null)
        {
          Trace.Script("Drawing path!", this);
          this.LineRenderer.SetVertexCount(CurrentPath.All.Count);
          this.LineRenderer.SetPositions(CurrentPath.All.ToArray());
        }
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
      }

      // Called on children
      this.OnUpdate();

      // Every frame, the cooldown timer ticks...
      Cooldown -= Time.deltaTime;
      if (Cooldown < 0.0f) Cooldown = 0.0f;
    }

    /// <summary>
    ///  Checks if we have collided with the ground, so that the GameObject
    /// may jump again.
    /// </summary>
    /// <param name="hit"></param>
    protected void OnCollisionEnter(Collision hit)
    {
      // If we hit the ground...
      if (hit.collider.tag.Contains("Ground"))
        IsJumping = false;
    }
    /// <summary>
    /// Moves this GameObject along the specified direction vector using its rigidbody.
    /// </summary>
    /// <param name="vec">The movement direction</param>
    /// <param name="speed">What speed to use</param>
    protected void Move(Vector3 dir, float speed, float acceleration)
    {
      CurrentMode = Mode.Forces;

      if (Type == LocomotionMode.Translation)
      {
        var startPos = this.gameObject.transform.position;
        var endPos = transform.position + dir;
        this.transform.position = Vector3.Lerp(startPos, endPos, 1.0f);
      }
      else if (Type == LocomotionMode.Rigidbody)
      {

        CurrentMovementDirection = dir;
        RigidBody.AddForce(CurrentMovementDirection.normalized * acceleration, ForceMode.Impulse);        
      }

      this.OnMove(dir, speed, acceleration);
    }

    /// <summary>
    /// Quickly dashes alongside a given direction
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="impulse"></param>
    protected void Dash(Vector3 dir, float impulse)
    {
      // Will you complete me right mao
    }

    /// <summary>
    /// Will attempt to get within the speicfied range of the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="range"></param>
    /// <param name="config"></param>
    void ApproachTarget(Transform target, Path.Configuration config)
    {
      // Stop following the current path
      StopFollowingPath();

      // Now follow it
      CurrentMode = Mode.Path;
      PathRoutine = ApproachTargetRoutine(target, config);
      StartCoroutine(PathRoutine);

    }

    void DrawCurrentPath()
    {

    }

    /// <summary>
    /// Attempts to approach the target until it gets within range
    /// </summary>
    IEnumerator ApproachTargetRoutine(Transform target, Path.Configuration config)
    {
      //Trace.Script("Will now approach the target '" + target.name + "'", this);

      // While we are not within range of the target, keep making paths to it
      while (Vector3.Distance(transform.position, target.position) > config.Range)
      {
        // Calculate the path
        var path = CurvedPath.CalculatePath(transform.position, transform.forward, target.position, config.Speed, config.ApproachAngle, config.Weight);
        if (Debugging)
        {
          Trace.Script("Starting from '" + transform.position + "', will now follow the following path with '" + path.All.Count + "' points", this);
          //foreach (var point in path.All)
          //{
          //Trace.Script(point, this);
          //}

        }
        foreach (var point in path.All)
        {
          //Trace.Script("Moving to next point: " + point, this);
          float step = config.Speed * Time.deltaTime;
          Vector3 newForward = point - transform.position;
          newForward.y = 0;
          transform.forward = newForward;
          while (Vector3.Distance(transform.position, point) > 0.2f)
          {
            for(int i = 1; i < path.All.Count; ++i)
            {
              Debug.DrawLine(path.All[i - 1], path.All[i], Color.cyan);
            }
            //float dist = Vector3.Distance(transform.position, point);
            //Trace.Script("Not yet in range! Distance to point = " + dist + " > Epsilon = " + 0.2f + ", From " + transform.position + " to " + point, this);
            //Vector3 velocity;
            //if(RigidBody.velocity.magnitude == 0)
            //{
            //	velocity = transform.forward.normalized * config.Speed;
            //	RigidBody.AddForce(velocity, ForceMode.VelocityChange);
            //}
            //else
            //{
            //	Vector3 vf = (point - transform.position) - velocity;
            //	RigidBody.AddForce(vf * 2, ForceMode.Acceleration);
            //	Trace.Script(vf.magnitude);
            //}
            //Vector3 vel = (point - transform.position).normalized * config.Speed;
            //RigidBody.AddForce(vel, ForceMode.VelocityChange);
            transform.position = Vector3.MoveTowards(transform.position, point, step);
            
            yield return new WaitForFixedUpdate();
          }
        }

        //Trace.Script("Finished traversing the path!", this);
      }

      //Trace.Script("Target has been reached!", this);
      this.CurrentMode = Mode.Idle;
      // We have finally reached the target
      this.gameObject.Dispatch<ReachedTargetEvent>(new ReachedTargetEvent());
    }


    /// <summary>
    /// Moves the agent to the target destination
    /// </summary>
    /// <param name="config"></param>
    void MoveToTarget(Vector3 target, Path.Configuration config)
    {
      // Do not attempt to recompute the path if they are not equal       
      if (this.CurrentDestination != null &&
          target == this.CurrentDestination)
        return;

      // Stop following the current path
      StopFollowingPath();

      // Calculate the path
      var path = CurvedPath.CalculatePath(transform.position, transform.forward, target,
                                          config.Speed, config.ApproachAngle, config.Weight);

      // Now follow it
      FollowPath(path, config.Speed);
    }

    /// <summary>
    /// Follows the given path through the use of a coroutine.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="speed"></param>
    void FollowPath(Path path, float speed)
    {
      //Trace.Script("Starting from '" + transform.position + "', will now follow the following path:", this);

      if (Debugging)
      {
        Trace.Script("Starting from '" + transform.position + "', will now follow the following path at speed = '" + speed + "' :", this);
        foreach (var point in path.All)
        {
          Trace.Script(point, this);
        }

      }

      CurrentMode = Mode.Path;
      PathRoutine = FollowPathRoutine(path, speed);
      StartCoroutine(PathRoutine);
    }

    /// <summary>
    /// Stops following the current path.
    /// </summary>
    void StopFollowingPath()
    {
      if (PathRoutine != null)
        StopCoroutine(PathRoutine);
    }

    /// <summary>
    /// Moves this agent through a list of points.
    /// </summary>
    IEnumerator FollowPathRoutine(Path waypoints, float speed)
    {
      foreach (var point in waypoints.All)
      {
        //Trace.Script("Moving to next point: " + point, this);
        float step = speed * Time.deltaTime;
        while (Vector3.Distance(transform.position, point) > PathEpsilon)
        {
          float dist = Vector3.Distance(transform.position, point);
          //Trace.Script("Not yet in range! Distance to point = " + dist + " > Epsilon = " + Epsilon + ", From " + transform.position + " to " + point, this);
          transform.position = Vector3.MoveTowards(transform.position, point, step);
          yield return new WaitForFixedUpdate();
        }
        //Trace.Script("Reached the point!", this);
      }

      //Trace.Script("Arrived at the destination!");
      this.CurrentMode = Mode.Idle;
      this.gameObject.Dispatch<ReachedEndOfPathEvent>(new ReachedEndOfPathEvent(transform.position));
    }

    /// <summary>
    /// Rotates this agent alongside a given direction.
    /// </summary>
    /// <param name="dir">The direction of rotation</param>
    protected void Rotate(Direction dir, float speed)
    {
      if (dir == Direction.Left)
        this.transform.Rotate(0, -speed, 0, UnityEngine.Space.World);
      else if (dir == Direction.Right)
        this.transform.Rotate(0, speed, 0, UnityEngine.Space.World);
    }

    void LookAt(Vector3 target)
    {
      this.transform.LookAt(target);
    }

    /// <summary>
    /// Moves this GameObject along its vertical axis, opposite gravity
    /// </summary>
    protected void Jump(float speed)
    {
      if (IsJumping)
        return;

      GetComponent<Rigidbody>().AddForce(transform.up * speed / 4, ForceMode.Impulse);
      IsJumping = true;
    }

    /// <summary>
    /// Moves this GameObject along a specified direction with a quick impulse
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="dist"></param>
    protected void Sidestep(Direction dir, Distance dist)
    {
      if (OnCooldown()) return;

      if (Debugging) Trace.Script("Sidestepping in the direction: " + dir.ToString(), this);
      var directionVector = Utilities.ToVector(dir, this.transform);

      float modifier = 0.0f;
      switch (dist)
      {
        case Distance.Short:
          modifier = 1.0f;
          break;
        case Distance.Long:
          modifier = 2.0f;
          break;
        case Distance.Extra:
          modifier = 3.0f;
          break;
      }

      //this.RigidBody.AddForce(directionVector * this.JumpSpeed * modifier, ForceMode.VelocityChange);
      // Rotate around Z because why not
      //var seq = Actions.Sequence(this);
      //Actions.Rotate(seq, transform, new Vector3(transform.rotation.eulerAngles.x, 
      //                                           transform.rotation.eulerAngles.y, 359.0f), 
      //                                           this.SidestepCooldown - 0.01f, Ease.Linear);
      //Actions.Trace(seq, "Done sidestepping!");
      //Actions.Property(seq, ()=>transform.rotation, new Vector3(0,0,359f), this.SidestepCooldown, Ease.Linear);

      //this.Jump();

      this.ApplyCooldown(SidestepCooldown);
    }

    void OnCollisionStay(Collision col)
    {
      if (col.gameObject.layer == 0)
      {
        if (col.impulse.y == 0)
        {
          RaycastHit hit;
          Physics.Raycast(transform.position, transform.forward, out hit);
          Vector3 dir = transform.forward + hit.normal * Vector3.Dot(transform.forward, hit.normal);
          this.RigidBody.AddForce(dir);
        }
      }
    }

    public void AlignForward(Transform transform, float speed)
    {
      TurnTimeCounter += Time.deltaTime;
      Mathf.Clamp(TurnTimeCounter, 0, speed);
      CurrentMovementDirection = Vector3.Slerp(transform.forward, CurrentMovementDirection, TurnTimeCounter / speed);
      this.transform.LookAt(this.transform.position + CurrentMovementDirection);
    
    }

    public void ApplyCooldown(float amount)
    {
      Cooldown += amount;
    }

    public bool OnCooldown()
    {
      if (this.Cooldown > 0.0f)
      {
        if (Debugging) Trace.Script("My movement is on cooldown!", this);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Disables for a specified duration.
    /// </summary>
    /// <param name="duration">How long should input be disabled for.</param>
    public void Disable(float duration = 0.0f)
    {
      //if (Tracing)
      Trace.Script("Disabling movement for " + duration + " seconds!", this);
      this.Enabled = false;
      // Enable input back after a moment
      if (duration <= 0.0f) return;
      var seq = Actions.Sequence(this);
      Actions.Property(seq, () => Enabled, true, duration, Ease.Linear);
    }


  }

}