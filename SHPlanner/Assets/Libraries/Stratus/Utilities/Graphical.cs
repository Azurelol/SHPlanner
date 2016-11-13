/******************************************************************************/
/*!
@file   Graphical.cs
@author Christian Sagel
@par    email: ckpsm\@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System.Collections;

namespace Stratus
{
  /// <summary>
  /// 
  /// </summary>
  public class Graphical
  {
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Tweens the alpha of this and all its children.
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="duration"></param>
    public static void Fade(MonoBehaviour target, float alpha, float duration)
    {
      foreach (var graphical in target.GetComponentsInChildren<Graphic>())
      {
        //Trace.Script("Fading '" + graphical.name + " to " + alpha);
        if (duration <= 0.0f)
          graphical.color = new Color(graphical.color.r, graphical.color.g, graphical.color.b, alpha);
        else
          graphical.CrossFadeAlpha(alpha, duration, true);

      }
    }

    /// <summary>
    /// Tweens the alpha of this and all its children after initializing it to 0.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="alpha"></param>
    /// <param name="duration"></param>
    public static void FadeIn(MonoBehaviour target, float alpha, float duration)
    {
      target.StartCoroutine(FadeRoutine(target, 0.0f, 0.0f));
      target.StartCoroutine(FadeRoutine(target, alpha, duration));
    }

    static IEnumerator FadeRoutine(MonoBehaviour target, float alpha, float duration)
    {
      yield return new WaitForEndOfFrame();
      Fade(target, alpha, duration);      
      yield return null;
    }
  }

}