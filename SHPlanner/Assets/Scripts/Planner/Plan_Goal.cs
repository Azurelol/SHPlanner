using UnityEngine;
using System.Collections;
using System;

namespace Prototype
{
  public partial class Plan
  {

    public abstract class Goal
    {
      public abstract bool IsValid();

      /// <summary>
      /// The desired state for this goal.
      /// </summary>
      public State DesiredState;
    }

    public class CollectGoal : Goal
    {
      public override bool IsValid()
      {
        throw new NotImplementedException();
      }
    }

  }


}