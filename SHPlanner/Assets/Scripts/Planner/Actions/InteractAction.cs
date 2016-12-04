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
    protected abstract void OnInteract();

    protected override void OnBegin() {}
    protected override void OnEnd() {}    
    protected override bool OnValidate()
    {
      if (HasInteracted)
        return true;
      return false;
    }

    protected override bool OnCheckContextPrecondition()
    {
      // Look for the target of this action
      this.FindTarget();
      return this.Target != null;
    }

    /// <summary>
    /// Sends an interact event to the object in question.
    /// </summary>
    protected override void OnExecute()
    {
      // If not within range of the target, approach it
      if (!IsWithinRange())
      {
        this.Approach();
        return;
      }

      //this.Target.gameObject.Dispatch<InteractiveObject.InteractEvent>(new InteractiveObject.InteractEvent());
      this.OnInteract();
      this.HasInteracted = true;
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
          //Trace.Script(Description + " : Found a valid target = " + interactive.name, this);
          return;
        }
      }

      Trace.Script(Description + " : No valid target found!", this);
    }

    /// <summary>
    /// Checks whether the agent is within range of its target
    /// </summary>
    /// <returns></returns>
    bool IsWithinRange()
    {
      var targetDist = Vector3.Distance(this.Target.transform.position, this.transform.position);
      if (targetDist <= this.Range) return true;
      return false;
    }

    void Approach()
    {
      //var dir = this.Target.transform.position - this.transform.position;
      //var moveEvent = new Movement.MoveEvent(dir);
      //moveEvent.Speed = 1f;      
      //this.gameObject.Dispatch<Movement.MoveEvent>(new Movement.MoveEvent(dir));
      //this.gameObject.Dispatch<Movement.LookAtEvent>(new Movement.LookAtEvent(dir));
      this.transform.localPosition = Vector3.MoveTowards(this.transform.position, this.Target.transform.position, Time.deltaTime * 3f);
      this.transform.LookAt(this.Target.transform);
    }


  }



}
