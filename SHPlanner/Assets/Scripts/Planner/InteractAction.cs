/******************************************************************************/
/*!
@file   InteractAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Prototype
{
  public abstract class InteractAction : Action
  {
    [ReadOnly]
    public InteractiveObject Target;
    bool HasInteracted = false;
    protected abstract bool OnValidateTarget(InteractiveObject obj);

    protected override void OnBegin()
    {
    }

    protected override void OnEnd()
    {
    }

    protected override bool OnValidate()
    {
      if (HasInteracted)
        return true;
      return false;
    }

    /// <summary>
    /// Sends an interact event to the object in question.
    /// </summary>
    protected override void OnExecute()
    {
      this.Target.gameObject.Dispatch<InteractiveObject.InteractEvent>(new InteractiveObject.InteractEvent());
    }


    protected override void OnReset()
    {
      this.HasInteracted = false;
    }

    /// <summary>
    /// Finds an eligible target for this action.
    /// </summary>
    void FindTarget()
    {
      // Look at the interactible objects that the agent has detected
      foreach (var interactive in this.Planner.InteractivesInRange)
      {
        if (this.OnValidateTarget(interactive))
        {
          this.Target = interactive;
          Trace.Script("Found a valid target = " + interactive.name, this);
        }

      }
    }

  }



}
