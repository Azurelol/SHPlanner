/******************************************************************************/
/*!
@file   StructExtensions.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;


public static class StructExtensions
{
  /// <summary>
  /// Sets the alpha on the color instantly.
  /// </summary>
  /// <param name="color">The color whose alpha to change</param>
  /// <param name="alpha">The alpha value to set</param>
  public static void SetAlpha(this Color color, float alpha)
  {
    color = new Color(color.r, color.g, color.b, alpha);
  }

  /// <summary>
  /// Checks the specified value is within the range of this vector
  /// </summary>
  /// <param name="range">A vector containing a min-max range.</param>
  /// <param name="value">The value to check.</param>
  /// <returns>True if the value is within the range, false otherwise</returns>
  public static bool Contains(this Vector2 range, float value)
  {
    if (value > range.x && value < range.y)
      return true;

    return false;
  }

  /// <summary>
  /// Gets the average between the values of the vector.
  /// </summary>
  /// <param name="range">The vector containing two values.</param>
  /// <returns></returns>
  public static float Average(this Vector2 range)
  {
    return ((range.x + range.y) / 2);
  }




}

