/******************************************************************************/
/*!
@file   CameraEventDispatcher.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class CameraEventDispatcher 
  */
  /**************************************************************************/
  public class CameraEventDispatcher : EventDispatcher
  {
    // What type of event to dispatch
    public enum Type { LookAt, Zoom, ZoomBack, Reposition, Follow, Orbit }
    // Members
    public Type Event = Type.LookAt;
    public Transform Target;
    public CameraController.MovementConfiguration Movement = new CameraController.MovementConfiguration();
    public CameraController.OrientationConfiguration View = new CameraController.OrientationConfiguration();
    public CameraController.Transition Transition = new CameraController.Transition();

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    protected override void OnInitialize()
    {
    }

    protected override void OnTrigger()
    {
      this.Dispatch();
    }

    void Dispatch()
    {
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Delay(seq, this.Delay);
      switch (this.Event)
      {
        case Type.Follow:
          Actions.Call(seq, this.Zoom);
          break;
        case Type.LookAt:
          Actions.Call(seq, this.LookAt);
          break;
        case Type.Reposition:
          Actions.Call(seq, this.Reposition);
          break;
        case Type.Zoom:
          Actions.Call(seq, this.Zoom);
          break;
        case Type.ZoomBack:
          Actions.Call(seq, this.ZoomBack);
          break;
        default:
          break;
      }
    }

    void Zoom()
    {
      CameraController.Zoom(this.Space(), this.View, this.Transition);
    }

    void ZoomBack()
    {
      CameraController.ZoomBack(this.Space(), this.Transition);
    }

    void LookAt()
    {
      CameraController.LookAt(this.Space(), this.Target, this.View, this.Transition);
    }

    void Follow()
    {
      CameraController.Follow(this.Space(), this.Target, this.Movement, this.Transition);
    }

    void Reposition()
    {
      CameraController.Reposition(this.Space(), this.Target, this.Movement, this.Transition);
    }

  }

}