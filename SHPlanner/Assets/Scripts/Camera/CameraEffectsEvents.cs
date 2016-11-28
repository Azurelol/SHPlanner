/******************************************************************************/
/*!
@file   CameraEffectsEvents.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Stratus;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class CameraEffectsEvents 
  */
  /**************************************************************************/
  public partial class CameraEffects : MonoBehaviour
  {
    //--------------------------------------------------------------------------/
    // Events
    //--------------------------------------------------------------------------/
    public class ApplyEffectEvent : Stratus.Event { public Effect Effect; }
    public class PopEffectEvent : Stratus.Event { }
    public class GreyScaleEvent : Stratus.Event { }
    public class BlurEvent : Stratus.Event { }

    public class TwirlEffectEvent : Stratus.Event
    {
      public Vector2 Radius;
      public float Angle;

      public void setInterpolation(float endAngle, float duration = 1.0f)
      {
        Interpolating = true;
        Duration = duration;
        EndAngle = endAngle;
      }
      public bool Interpolating;
      public float Duration;
      public float EndAngle;
    }

    void OnTwirlEffectEvent(TwirlEffectEvent e)
    {
      var twirl = Add(typeof(Twirl)) as Twirl;
      twirl.shader = Shader.Find("Hidden/Twirt Effect Shader");
      twirl.radius = e.Radius;
      //twirl.angle = e.Angle;

      var seq = Actions.Sequence(gameObject.Actions());
      Actions.Property(seq, () => twirl.angle, e.Angle, e.Duration/2, Ease.Linear);

      if (e.Interpolating)
      {
        Actions.Property(seq, () => twirl.angle, e.EndAngle, e.Duration/2, Ease.Linear);
      }

    }

    //--------------------------------------------------------------------------/
    // Interface
    //--------------------------------------------------------------------------/
    public static void Apply(Stratus.Space space, Effect type)
    {
      var e = new CameraEffects.ApplyEffectEvent();
      e.Effect = type;
      space.Dispatch<CameraEffects.ApplyEffectEvent>(e);
    }

  }

}