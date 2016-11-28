/******************************************************************************/
/*!
@file   CameraControllerEvents.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class CameraController 
  */
  /**************************************************************************/
  public partial class CameraController : StratusBehaviour
  {
    // Presets
    public class FollowPreset : MovementConfiguration
    {
      public FollowPreset()
      {
      }
    }

    //--------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------/
    /// <summary>
    /// Base camera event class.
    /// </summary>
    public abstract class CameraEvent : Stratus.Event
    {
      public Transform Target;
      public Transition Transition = new Transition();
    }
    /// <summary>
    /// Event that manipulates the camera's position and movement behaviour.
    /// </summary>
    public abstract class MoveEvent : CameraEvent
    {
      public MovementConfiguration Movement = new MovementConfiguration();
    }    
    /// <summary>
    /// Follows a target.
    /// </summary>
    public class FollowEvent : MoveEvent {}
    /// <summary>
    /// Repositions the camera, orbiting around the target every frame.
    /// </summary>
    public class OrbitEvent : MoveEvent { public float Speed; }
    /// <summary>
    /// Repositions the camera around a target.
    /// </summary>
    public class RepositionEvent : MoveEvent { }
    /// <summary>
    /// Gives the player control of the camera.
    /// </summary>
    public class FreeMoveEvent : MoveEvent
    {
      /// <summary>
      /// The x and y axis of movement by the player.
      /// </summary>
      public Vector2 Axis = new Vector2();
      /// <summary>
      /// How sensitive to input the free look is.
      /// </summary>
      public float Sensitivity = 0.05f;
      /// <summary>
      /// How long should the camera remain in free mode until it reverts
      /// </summary>
      public float Duration = 1.5f;
    }
    /// <summary>
    /// Event that manipulates the camera's view behaviour.
    /// </summary>    
    public abstract class ViewEvent : CameraEvent
    {
      public OrientationConfiguration View = new OrientationConfiguration();
    }
    /// <summary>
    /// Looks at a target's position over a given period of time.
    /// </summary>
    public class LookAtEvent : ViewEvent { } 
    /// <summary>
    /// Starts tracking a target.
    /// </summary>
    public class TrackEvent : ViewEvent { }
    /// <summary>
    /// Zooms in the camera.
    /// </summary>
    public class ZoomEvent : ViewEvent {}
    /// <summary>
    /// Zooms out the camera.
    /// </summary>
    public class ZoomBackEvent : CameraEvent {}
    /// <summary>
    /// Returns to the previous movement configuration.
    /// </summary>
    public class RevertMovementEvent : CameraEvent { }
    /// <summary>
    /// Returns to the previous view configuration.
    /// </summary>
    public class RevertViewEvent : CameraEvent { }

    //--------------------------------------------------------------------------/
    // Subscribed events
    //--------------------------------------------------------------------------/
    void Subscribe()
    {
      // Movement
      this.Space().Connect<FollowEvent>(this.OnFollowEvent);
      this.Space().Connect<RepositionEvent>(this.OnRepositionEvent);
      this.Space().Connect<OrbitEvent>(this.OnOrbitEvent);
      this.Space().Connect<RevertMovementEvent>(this.OnRevertMovement);
      // View
      this.Space().Connect<LookAtEvent>(this.OnLookAtEvent);
      this.Space().Connect<TrackEvent>(this.OnTrackEvent);
      this.Space().Connect<ZoomEvent>(this.OnZoomEvent);
      this.Space().Connect<ZoomBackEvent>(this.OnZoomBackEvent);
      this.Space().Connect<RevertViewEvent>(this.OnRevertView);
      // Free look
      this.Space().Connect<FreeMoveEvent>(this.OnFreeLookEvent);
    }
    //------------------------------------------------------------------------/
    // Static Interface
    //------------------------------------------------------------------------/
    /// <summary>
    /// Returns to the previous movement configuration.
    /// </summary>
    /// <param name="space"></param>
    public static void RevertMovement(Stratus.Space space)
    {
      var returnEvent = new RevertMovementEvent();
      space.Dispatch<RevertMovementEvent>(returnEvent);
    }
    /// <summary>
    /// Returns to the previous view configuration.
    /// </summary>
    /// <param name="space"></param>
    public static void RevertView(Stratus.Space space)
    {
      var returnEvent = new RevertViewEvent();
      space.Dispatch<RevertViewEvent>(returnEvent);
    }
    /// <summary>
    /// Repositions the camera around the target.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="configuration"></param>
    /// <param name="transition"></param>
    public static void Reposition(Stratus.Space space, Transform target, MovementConfiguration configuration, Transition transition)
    {
      var repositionEvent = new RepositionEvent();
      repositionEvent.Target = target;
      repositionEvent.Movement = configuration;
      repositionEvent.Transition = transition;
      space.Dispatch<RepositionEvent>(repositionEvent);
    }
    /// <summary>
    /// Zooms in the camera
    /// </summary>
    /// <param name="space"></param>
    /// <param name="config"></param>
    /// <param name="transition"></param>
    public static void Zoom(Stratus.Space space, OrientationConfiguration config, Transition transition)
    {
      var e = new ZoomEvent();
      e.View = config;
      e.Transition = transition;
      space.Dispatch<ZoomEvent>(e);
    }
    /// <summary>
    /// Returns to the previous zoom level.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="transition"></param>
    public static void ZoomBack(Stratus.Space space, Transition transition)
    {
      var e = new ZoomBackEvent();
      e.Transition = transition;
      space.Dispatch<ZoomBackEvent>(e);
    }
    /// <summary>
    /// Follows the target.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="movement"></param>
    /// <param name="transition"></param>
    public static void Follow(Stratus.Space space, Transform target, MovementConfiguration movement, Transition transition)
    {
      //ActionSpace.PrintActiveActions();
      var followEvent = new FollowEvent();
      followEvent.Target = target;
      followEvent.Movement = movement;
      followEvent.Transition = transition;
      space.Dispatch<CameraController.FollowEvent>(followEvent);
    }
    /// <summary>
    /// Looks at the target.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="configuration"></param>
    /// <param name="transition"></param>
    public static void LookAt(Stratus.Space space, Transform target, OrientationConfiguration configuration, Transition transition)
    {
      var lookAtEvent = new LookAtEvent();
      lookAtEvent.Target = target;
      lookAtEvent.View = configuration;
      lookAtEvent.Transition = transition;
      space.Dispatch<LookAtEvent>(lookAtEvent);
    }
    /// <summary>
    /// Has the camera start tracking the target. Its current movement behavior
    /// does not change.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="transition"></param>
    public static void Track(Stratus.Space space, Transform target, OrientationConfiguration view, Transition transition = default(Transition))
    {
      //Trace.Script("Now tracking " + target.name);
      var trackEvent = new TrackEvent();
      trackEvent.Target = target;
      trackEvent.View = view;
      trackEvent.Transition = transition;
      space.Dispatch<TrackEvent>(trackEvent);
    }
    /// <summary>
    /// Orbtis the camera around the target continuously.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="configuration"></param>
    /// <param name="transition"></param>
    public static void Orbit(Stratus.Space space, Transform target, MovementConfiguration configuration, Transition transition)
    {
      var e = new OrbitEvent();
      e.Movement = configuration;
      e.Target = target;
      e.Transition = transition;
      space.Dispatch<OrbitEvent>(e);
    }


    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    void OnRevertMovement(RevertMovementEvent e)
    {
      this.RevertMovement();
    }

    void OnRevertView(RevertViewEvent e)
    {
      this.RevertView();
    }

    void OnZoomEvent(ZoomEvent e)
    {      
      this.Zoom(e.View.Zoom, e.Transition);
      if (e.Transition.Return)
      {
        var seq = Actions.Sequence(gameObject.Actions());
        Actions.Delay(seq, e.Transition.Duration);
        Actions.Call(seq, this.ZoomBack);
      }
    }

    void OnZoomBackEvent(ZoomBackEvent e)
    {
      if (Tracing)  Trace.Script("", this);
      this.ZoomBack(e.Transition);
    }

    void OnFollowEvent(FollowEvent e)
    {
      if (!e.Target)
        throw new System.Exception("No target provided!");
      
      // Before enabling the follow mode
      this.SetMovement(e.Target, MovementMode.Follow, e.Movement, e.Transition);
      // Reposition around the target first
      this.Reposition(e.Transition);
    }   
     
    void OnRepositionEvent(RepositionEvent e)
    {
      if (Tracing) Trace.Script("Now repositioning the camera around '" + e.Target.name + "'", this);            
      //this.SetMovement(e.Target, MovementMode.Fixed, e.Movement, e.Transition);
      //this.Reposition(e.Transition);
      this.Reposition(e.Target, e.Movement, e.Transition);
    }

    void OnOrbitEvent(OrbitEvent e)
    {
      if (!e.Target)
      {
        Trace.Script("No target was provided", this);
      }
      
      // Before enabling the follow mode
      this.SetMovement(e.Target, MovementMode.Orbit, e.Movement, e.Transition);      
      // Reposition around the target first
      this.Reposition(e.Transition);
    }
    
    void OnLookAtEvent(LookAtEvent e)
    {
      if (Tracing) Trace.Script("Now looking at '" + e.Target.name + "'", this);
      //this.SetView(e.Target, LookMode.Fixed, e.View, e.Transition);
      //this.LookAt(e.Target, LookMode.Fixed, e.View, e.Transition);
      this.LookAt(e.Target, e.View, e.Transition);
    }

    void OnTrackEvent(TrackEvent e)
    {
      this.SetView(e.Target, LookMode.Track, e.View, e.Transition);
    }

    void OnFreeLookEvent(FreeMoveEvent e)
    {
      //if (e.)
      // If the mode is not already set 
      // Set the movement mode of the camera to free look
      this.SetMovement(e.Target, MovementMode.Free, e.Movement, e.Transition);
      FreeLookTimer.Reset(e.Duration);
      Trace.Script("Now at free look!", this);
    }


  }
}