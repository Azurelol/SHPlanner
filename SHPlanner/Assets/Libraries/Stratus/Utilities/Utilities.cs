/******************************************************************************/
/*!
@file   Utilities.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;
using System;

namespace Stratus
{
  public class ArrayNavigate
  {
    public enum Direction { Forward, Backward, Up, Down, Left, Right }
  }

  /// <summary>
  /// Provides a generic way to navigate a 1D array using directional axis.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ArrayNavigate<T> : ArrayNavigate
  {

    T[] Array;
    int ElementCount { get { return Array.Length - 1; } }
    int ElementIndex = 0;

    public void Set(T[] array)
    {
      Array = array;
      ElementIndex = 0;
    }

    /// <summary>
    /// Retrieves the first element in the array.
    /// </summary>
    /// <returns></returns>
    public T First()
    {
      return Array[0];
    }

    /// <summary>
    /// Retrieves the last element in the array.
    /// </summary>
    /// <returns></returns>
    public T Last()
    {
      return Array[ElementCount];
    }

    /// <summary>
    /// Retrieves a random element in the array.
    /// </summary>
    /// <returns></returns>
    public T Random()
    {
      var randomIndex = UnityEngine.Random.Range(0, ElementCount);
      return Array[randomIndex];
    }

    public T Navigate(Direction dir)
    {
      if (dir == Direction.Left || dir == Direction.Up)
      {
        if (ElementIndex < ElementCount)
          ElementIndex++;
      }
      else if (dir == Direction.Right || dir == Direction.Down)
      {
        if (ElementIndex != 0)
          ElementIndex--;
      }

      return Array[ElementIndex];
    }
  }

 
  /// <summary>
  /// Counts down from the specified amount of time. 
  /// From <i>n</i> amount of time to 0.0f;
  /// </summary>
  public struct Countdown
  {
    float Total_;
    float Current_;        
    public float Total { get { return Total_; } }
    public float Current { get { return Current_; } }

    /// <summary>
    /// The current progress in the bar as a percentage (0% - 100%)
    /// </summary>
    public float Progress { get { if (Total == 0.0f) return 0.0f;  return (Current / Total) * 100.0f; } }

    /// <summary>
    /// Constructor for the countdown.
    /// </summary>
    /// <param name="total">The total amount of time to countdown.</param>
    public Countdown(float total)
    {
      Total_ = total;
      Current_ = total;
    }

    /// <summary>
    /// Resets the countdown.
    /// </summary>
    public void Reset()
    {
      Current_ = Total_;
    }

    public void Reset(float total)
    {
      Total_ = total;
      Current_ = Total;
    }

    /// <summary>
    /// Updates the timer by the default delta time (Time.deltaTime)
    /// </summary>
    /// <returns>True if is done, false otherwise</returns>
    public bool Update()
    {
      return Update(Time.deltaTime);
    }

    /// <summary>
    /// Updates the timer by the specified delta time.
    /// </summary>
    /// <returns>True if is done, false otherwise</returns>
    public bool Update(float dt)
    {
      if (Current_ <= 0.0f)
      {
        return true;
      }

      Current_ -= dt;
      return false;
    }
  }

  /// <summary>
  /// A counter which is incremented one at a time.
  /// </summary>
  [Serializable]
  public struct Counter
  {
    public int Total;
    public int Current;

    public bool IsFull
    {
      get
      {
        if (Current == Total)
          return true;
        return false;
      }
    }

    public float Percentage
    {
      get
      {
        return ( (float)Current / (float)Total) * 100.0f;
      }
    }

    public string Print { get { return Current + "/" + Total; } }

    public Counter(int total)
    {
      Total = total;
      Current = 0;
    }

    /// <summary>
    /// Increments this counter
    /// </summary>
    /// <returns>True if the counter is full, false otherwise</returns>
    public bool Increment()
    {
      if (IsFull)
        return true;

      Current++;

      if (IsFull)
        return true;

      return false;
    }

  }


}
