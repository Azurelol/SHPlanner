/******************************************************************************/
/*!
@file   CameraEffects.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using Stratus;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class CameraEffects 
  */
  /**************************************************************************/
  [RequireComponent(typeof(Camera))]
  public partial class CameraEffects : MonoBehaviour
  {
    public bool Debugging = true;
    public enum Effect { Bloom, Grayscale, Blur, DepthOfField, Fisheye, MotionBlur, Twirl, Pop, Transition }    
    List<Component> ActiveEffects = new List<Component>();

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Awake()
    {
      this.Space().Connect<ApplyEffectEvent>(this.OnApplyEffectEvent);
      this.Space().Connect<PopEffectEvent>(this.OnPopEvent);
      this.Space().Connect<BlurEvent>(this.OnCameraBlurEffectEvent);
      this.Space().Connect<TwirlEffectEvent>(this.OnTwirlEffectEvent);
    }

    /**************************************************************************/
    /*!
    @brief  Received when applying effects with default parameters.
    */
    /**************************************************************************/
    void OnApplyEffectEvent(ApplyEffectEvent e)
    {
      //Trace.Script("Yo!");
      switch (e.Effect)
      {
        case Effect.Grayscale:
          Apply(typeof(Grayscale), Shader.Find("Hidden/Grayscale Effect"));
          break;
        case Effect.Blur:
          Apply(typeof(Blur), Shader.Find("Hidden/FastBlur"));
          break;
        //case Effect.Transition:
        //  Apply(typeof(ScreenTransitionImageEffect), Shader.Find("ScreenTransitionImageEffect"));
        //  break;
        case Effect.Pop:
          Remove(ActiveEffects[ActiveEffects.Count - 1].GetType());
          break;
        default:
          break;
      }
    }

    /**************************************************************************/
    /*!
    @brief  Adds the specified component to the stack of active effects.
    */
    /**************************************************************************/
    Component Add(System.Type type)
    {
      var effect = this.gameObject.AddComponent(type);
      ActiveEffects.Add(effect);
      return effect;
    }

    T Add<T>(T imageEffect) where T : class
    {
      var effect = this.gameObject.AddComponent(imageEffect.GetType());
      ActiveEffects.Add(effect);
      return effect as T;
    }

    /**************************************************************************/
    /*!
    @brief Adds the effect and sets it shader.
    @param shader The shader to use for the image base component
    */
    /**************************************************************************/
    void Apply(System.Type type, Shader shader)
    {
      if (Debugging) Trace.Script("Applying '" + type.Name + "'", this);
      var effect = Add(type) as ImageEffectBase;
      effect.shader = shader;
    }

    public static void Pop(Stratus.Space space)
    {
      var e = new CameraEffects.ApplyEffectEvent();
      e.Effect = Effect.Pop;
      space.Dispatch<CameraEffects.ApplyEffectEvent>(e);
    }

    void Remove(System.Type type)
    {
      if (Debugging) Trace.Script("Removing '" + type.Name + "'", this);
      ActiveEffects.RemoveAll(x => x.GetType() == type);
      Destroy(this.gameObject.GetComponent(type));      
    }

    void OnCameraBlurEffectEvent(BlurEvent e)
    {
      var blur = Add(typeof(BlurOptimized)) as BlurOptimized;
      blur.blurShader = Shader.Find("Hidden/FastBlur");
    }

    void OnPopEvent(PopEffectEvent e)
    {
      var type = ActiveEffects[ActiveEffects.Count - 1].GetType();
      Remove(type);
    }

  }

}