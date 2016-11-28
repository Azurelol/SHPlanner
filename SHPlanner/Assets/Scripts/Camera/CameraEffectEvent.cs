/******************************************************************************/
/*!
@file   CameraEffectEvent.cs
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
  /**************************************************************************/
  /*!
  @class CameraEffectEvent 
  */
  /**************************************************************************/
  public class CameraEffectEvent : EventDispatcher
  {    
    public CameraEffects.Effect Type;
    public float Duration = 1.0f;


    protected override void OnInitialize()
    {

    }

    protected override void OnTrigger()
    {
      //CameraEffects.Apply(this.Type);
      DispatchEventEffect();
    }

    void DispatchEventEffect()
    {
      switch (Type)
      {
        case CameraEffects.Effect.Blur:
          this.Space().Dispatch<CameraEffects.BlurEvent>(new CameraEffects.BlurEvent());
          break;
        case CameraEffects.Effect.Pop:
          this.Space().Dispatch<CameraEffects.PopEffectEvent>(new CameraEffects.PopEffectEvent());
          break;
        case CameraEffects.Effect.Twirl:
          this.Twirl();
          break;
        default:
          break;
      }
    }

    void Twirl()
    {
      var e = new CameraEffects.TwirlEffectEvent();
      e.Radius = new Vector2(1.0f, 1.0f);
      e.Angle = 0.0f;
      e.setInterpolation(360f, this.Duration);
      this.Space().Dispatch<CameraEffects.TwirlEffectEvent>(e);
    }


  }
}