/******************************************************************************/
/*!
@file   CameraController.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// The main driver for the event-driven camera in the project, 
  /// implementing all camera functionality.
  /// </summary>
  [RequireComponent(typeof(Camera))]
  public partial class CameraController : StratusBehaviour
  {
    // The current state of the camera
    public enum MovementMode { Fixed, Follow, Orbit, Free };
    public enum LookMode { Fixed, Track, Free }
    public enum LookDirection { TargetPosition, TargetForward }

    /// <summary>
    /// The configuration for the camera's movement behaviour.
    /// </summary>
    [Serializable]
    public class MovementConfiguration
    {
      //----------------------------------------------------------------------/
      /// <summary>
      /// Horizontal distance from target. 
      /// </summary>
      public float Radius = 10.0f;
      /// <summary>
      /// Vertical distance from target. 
      /// </summary>
      public float Height = 15.0f;
      /// <summary>
      /// Counterclock-wise angle around target. At 0, camera is directly behind target. 
      /// </summary>
      [Range(0.0f, 360.0f)]
      public float Angle = 90.0f;
      /// <summary>
      /// How long the camera it takes the camera to follow the target as it moves 
      /// </summary>
      public float Damping = 1.0f;
      /// <summary>
      /// Copies this configuration.
      /// </summary>
      /// <param name="rhs"></param>
      public void Copy(MovementConfiguration rhs)
      {
        Radius = rhs.Radius;
        Height = rhs.Height;
        Angle = rhs.Angle;
        Damping = rhs.Damping;
      }
      /// <summary>
      /// Clones this MovementConfiguration.
      /// </summary>
      /// <returns></returns>
      public MovementConfiguration Clone()
      {
        var view = new MovementConfiguration();
        view.Copy(this);
        return view;
      }
    }

    /// <summary>
    /// The configuration for the camera's orientation and view
    /// </summary>
    [Serializable]
    public class OrientationConfiguration
    {
      /// <summary>
      /// How long the camera it takes the camera to follow the target as it moves. 
      /// </summary>
      [Range(0.0f, 1.0f)] public float Damping = 1.0f;
      /// <summary>
      /// How much to zoom the camera.
      /// </summary>
      [Range(0.0f, 100.0f)]
      public float Zoom = 20.0f;
      /// <summary>
      /// What direction vector the camera is using on its look target.
      /// </summary>
      public LookDirection Direction = LookDirection.TargetPosition;
      /// <summary>
      /// A position offset from the target.
      /// </summary>
      public Vector3 Offset = new Vector3();
      /// <summary>
      /// Default constructor.
      /// </summary>
      public OrientationConfiguration() { }
      /// <summary>
      /// Copies another ViewConfiguration.
      /// </summary>
      /// <param name="rhs"></param>
      public void Copy(OrientationConfiguration rhs)
      {
        Damping = rhs.Damping;
        Zoom = rhs.Zoom;
        Direction = rhs.Direction;
        Offset = rhs.Offset;
      }
      /// <summary>
      /// Clones this ViewConfiguration.
      /// </summary>
      /// <returns></returns>
      public OrientationConfiguration Clone()
      {
        var view = new OrientationConfiguration();
        view.Copy(this);
        return view;
      }
    }

    /// <summary>
    /// A collection of settings related to transitioning the camera from
    /// one state to another.
    /// </summary>
    [Serializable]
    public class Transition
    {
      public float Duration = 0.0f;
      public Ease Easing = Ease.Linear;
      public bool Return = false;
      public Transition() { }
      public Transition(float duration, Ease ease, bool returning)
      {
        Duration = duration; Easing = ease; Return = returning;
      }
    }

    //--------------------------------------------------------------------------/
    // Properties
    //--------------------------------------------------------------------------/
    Camera Camera;
    public static CameraController Current;
    //------------------------------------------------------------------------/
    // State
    //------------------------------------------------------------------------/
    [ReadOnly]
    public MovementMode CurrentMovementMode = MovementMode.Fixed;
    private MovementMode PreviousMovementMode = MovementMode.Fixed;
    private MovementMode NextMovementMode = MovementMode.Fixed;
    [ReadOnly]
    public LookMode CurrentViewMode = LookMode.Fixed;
    private LookMode PreviousViewMode = LookMode.Fixed;
    private LookMode NextViewMode = LookMode.Fixed;
    //------------------------------------------------------------------------/
    // Camera Settings
    //------------------------------------------------------------------------/
    public MovementConfiguration Movement = new MovementConfiguration();
    MovementConfiguration PreviousMovement = new MovementConfiguration();
    public OrientationConfiguration View = new OrientationConfiguration();
    OrientationConfiguration PreviousView = new OrientationConfiguration();
    //------------------------------------------------------------------------/
    // Stored settings
    //------------------------------------------------------------------------/
    /// <summary>
    /// The target being currently followed.
    /// </summary>
    [ReadOnly] public Transform FollowTarget;
    Transform PreviousFollowTarget;
    /// <summary>
    /// The target being currently being viewed.
    /// </summary>
    [ReadOnly] public Transform ViewTarget;
    Transform PreviousViewTarget;
    //------------------------------------------------------------------------/
    float CurrentAngle;
    float OrbitAngle;
    Vector3 LastPosition;
    float Size;
    //float FieldOfView;
    //------------------------------------------------------------------------/
    /// <summary>
    /// Returns the specified angle, in radians.
    /// </summary>
    float Angle { get { return Movement.Angle * Mathf.Deg2Rad; } }
    bool Tracing = false;

    //--------------------------------------------------------------------------/
    // Methods
    //--------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the CameraController. Subscribes to Space-level events.
    /// </summary>
    void Awake()
    {
      Camera = this.gameObject.GetComponent<Camera>();
      CameraController.Current = this;
      // Subscribe to events  
      this.Subscribe();
      // Save default settings
      this.Size = this.Camera.orthographicSize;
      //this.FieldOfView = this.Camera.fieldOfView;
    }

    /// <summary>
    /// Called upon when the CameraController is destroyed.
    /// </summary>
    protected override void OnDestroyed()
    {
      CameraController.Current = null;
    }

    /// <summary>
    /// Sets the camera's movement behaviour.
    /// </summary>
    /// <param name="nextMode"></param>
    /// <param name="movement"></param>
    /// <param name="transition"></param>
    void SetMovement(Transform followTarget, MovementMode nextMode, MovementConfiguration movement, Transition transition)
    {
      // Save the configuration
      this.PreviousMovement.Copy(this.Movement);
      this.Movement.Copy(movement);
      this.PreviousMovementMode = this.CurrentMovementMode;
      this.PreviousFollowTarget = this.FollowTarget;
      this.FollowTarget = followTarget;
      this.NextMovementMode = nextMode;
      // Set the current angle
      this.CurrentAngle = this.Angle;      
      // Transition to the next mode
      if (transition.Duration > 0.0f)
      {
        var seq = Actions.Sequence(this);
        Actions.Delay(seq, transition.Duration + 0.1f);
        Actions.Call(seq, this.SetNextMovementMode);
        if (transition.Return)
        {
          Actions.Delay(seq, transition.Duration);
          Actions.Call(seq, this.RevertMovement);
        }
      }
      else
      {
        this.SetNextMovementMode();
      }
    }

    /// <summary>
    /// Sets the camera's viewing behaviour.
    /// </summary>
    /// <param name="nextMode"></param>
    /// <param name="view"></param>
    /// <param name="transition"></param>
    void SetView(Transform viewTarget, LookMode nextMode, OrientationConfiguration view, Transition transition)
    {
      //Trace.Script("Setting view to " + nextMode, this);
      // Save the configuration
      this.PreviousView.Copy(this.View);
      this.View.Copy(view);
      this.PreviousViewMode = this.CurrentViewMode;
      this.PreviousViewTarget = this.ViewTarget;
      this.ViewTarget = viewTarget;
      this.NextViewMode = nextMode;

      // Transition to the next mode
      if (transition.Duration > 0.0f)
      {
        var seq = Actions.Sequence(this);
        Actions.Delay(seq, transition.Duration + 0.1f);
        Actions.Call(seq, this.SetNextViewMode);
        if (transition.Return)
        {
          Actions.Delay(seq, transition.Duration);
          Actions.Call(seq, this.RevertView);
        }
      }
      else
      {
        this.SetNextViewMode();
      }
    }

    /// <summary>
    ///  Sets the next mode and look mode.
    /// </summary>
    void SetNextMovementMode()
    {
      this.CurrentMovementMode = this.NextMovementMode;
    }

    void SetNextViewMode()
    {
      this.CurrentViewMode = this.NextViewMode;
    }

    /// <summary>
    /// Returns to the previous movement configuration.
    /// </summary>
    void RevertMovement()
    {
      // Revert to the previous configuration
      this.Movement.Copy(this.PreviousMovement);
      // Change the mode back
      this.SetMovement(this.PreviousFollowTarget, this.PreviousMovementMode, this.Movement, new Transition(0.0f, Ease.Linear, false));
    }

    /// <summary>
    /// Returns to the previous viewing configuration.
    /// </summary>
    void RevertView()
    {
      Trace.Script("Reverting view", this);
      // Revert to the previous configuration
      this.View.Copy(this.PreviousView);
      // Change the mode back
      this.SetView(this.PreviousViewTarget, this.PreviousViewMode, this.View, new Transition(0.0f, Ease.Linear, false));
    }

    /// <summary>
    /// Updates the camera controller. This is where its operation branches
    /// depending on what the current modes are set to.
    /// </summary>
    void LateUpdate()
    {
      // Move
      if (this.CurrentMovementMode == MovementMode.Follow) this.Follow();
      else if (this.CurrentMovementMode == MovementMode.Orbit) this.Orbit();
      else if (this.CurrentMovementMode == MovementMode.Free) this.Move();
      // View
      if (this.CurrentViewMode == LookMode.Track) this.Track();
    }



  }

}