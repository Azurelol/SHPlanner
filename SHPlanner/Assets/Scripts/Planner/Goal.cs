using UnityEngine;
using System.Collections;
using System;

namespace Prototype
{
  [Serializable]
  public abstract class Goal
  {
    public enum Status
    {
      /// <summary>
      /// The goal is waiting to be activated
      /// </summary>
      Inactive,
      /// <summary>
      /// The goal has been activated and will be processed
      /// each update step.
      /// </summary>
      Active,
      /// <summary>
      /// The goal has completed and will be removed on the next
      /// update.
      /// </summary>
      Completed,
      /// <summary>
      /// The goal has failed and will either replan or be removed
      /// on the next update.
      /// </summary>
      Failed
    }

    /// <summary>
    /// The desired state for this goal.
    /// </summary>
    public State State;
    /// <summary>
    /// The target of this goal.
    /// </summary>
    public Transform Target;
    public abstract string Name { get; }
    public abstract string Description { get; }

    public Status Update(float dt)
    {


      return Status.Active;
    }

  }








}