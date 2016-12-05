using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Prototype
{
  [Serializable]
  public abstract class Goal : StratusBehaviour
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
    /// The desired conditions to reach this goal.
    /// </summary>
    public WorldState DesiredState = new WorldState();
        
    public abstract void OnSetup();
    public abstract string Name { get; }

    void Start()
    {
      this.OnSetup();
    }

    public bool IsSatisfied()
    {
      return false;
    }

    public void Finish(Planner planner)
    {
      this.OnFinished(planner);
    }

    public abstract void OnFinished(Planner planner);


    //public Status Update(float dt)
    //{
    //  return Status.Active;
    //}

  }








}