/******************************************************************************/
/*!
@file   CameraTransition.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System;

namespace Prototype
{
  //[ExecuteInEditMode]
  [RequireComponent(typeof(Camera))]
  //[AddComponentMenu("Image Effects/Screen Transition")]
  public class CameraTransition : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    [Serializable]
    public class TransitionEvent : Stratus.Event
    {
      public Texture2D Mask;
      /// <summary>
      /// What to set the initial value at.
      /// </summary>
      public float InitialValue = 0.0f;
      /// <summary>
      /// The final value.
      /// </summary>
      public float Value = 1.0f;
      /// <summary>
      /// How fast the transition takes.
      /// </summary>
      public float Speed = 3.0f;
      /// <summary>
      /// How long the transition lasts.
      /// </summary>
      public float Duration = 0.0f;
    }        

    //------------------------------------------------------------------------/
    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    public Shader shader;
    [Range(0, 1.0f)] public float maskValue;
    public Color maskColor = Color.black;
    public Texture2D maskTexture;
    public bool maskInvert;
    private Material m_Material;
    private bool m_maskInvert;

    Material material
    {
      get
      {
        if (m_Material == null)
        {
          m_Material = new Material(shader);
          m_Material.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_Material;
      }
    }

    public bool Active;

    //------------------------------------------------------------------------/

    /// <summary>
    /// Initializes the CameraTransition component.
    /// </summary>
    void Start()
    {
      // Disable if we don't support image effects
      if (!SystemInfo.supportsImageEffects)
      {
        enabled = false;
        return;
      }

      // Subscribe to events
      this.Space().Connect<TransitionEvent>(this.OnTransitionEvent);
    }

    void OnDisable()
    {
      if (m_Material)
      {
        DestroyImmediate(m_Material);
      }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!enabled)
      {
        Graphics.Blit(source, destination);
        return;
      }

      material.SetColor("_MaskColor", maskColor);
      material.SetFloat("_MaskValue", maskValue);
      material.SetTexture("_MainTex", source);
      material.SetTexture("_MaskTex", maskTexture);

      if (material.IsKeywordEnabled("INVERT_MASK") != maskInvert)
      {
        if (maskInvert)
          material.EnableKeyword("INVERT_MASK");
        else
          material.DisableKeyword("INVERT_MASK");
      }

      Graphics.Blit(source, destination, material);
    }

    void OnTransitionEvent(TransitionEvent e)
    {
      // If there's a mask, change to it
      if (e.Mask) this.maskTexture = e.Mask;
      // Set the initial value instantly
      maskValue = e.InitialValue;
      // Create a sequence for the transition
      var seq = Actions.Sequence(this);
      Actions.Property(seq, () => maskValue, e.Value, e.Speed, Ease.Linear);
      
      // If the transition is of a fixed duration
      if (e.Duration > 0.0f)
      {
        Actions.Delay(seq, e.Duration);
        Actions.Property(seq, () => maskValue, e.InitialValue, e.Speed, Ease.Linear);
      }

    }

  }

}