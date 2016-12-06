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
    //[ReadOnly] public InteractiveObject Target;
    bool HasInteracted = false;
    protected override bool RequiresRange { get { return true; } }

    protected abstract bool OnValidateTarget(InteractiveObject obj);
    protected abstract void OnInteract();
    protected abstract void OnInteractActionReset();

    /// <summary>
    /// Signals to the interactive object that this agent will be interacting with it
    /// </summary>
    protected override void OnBegin()
    {
      // If something happened to the target, replan
      if (!this.Target)
      {
        this.gameObject.Dispatch<CanceledEvent>(new CanceledEvent());
      }           
      
    }

    /// <summary>
    /// Signals to the interactive object that this agent is done with it
    /// </summary>
    protected override void OnEnd()
    {
      if (this.Target)
        this.Target.gameObject.Dispatch<InteractiveObject.InteractionEndedEvent>(new InteractiveObject.InteractionEndedEvent());
    }    

    /// <summary>
    /// This action is validated as it hasn't interacted
    /// </summary>
    /// <returns></returns>
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

      // If the target has been destroyed, cancel this action
      // If something happened to the target, replan
      if (!this.Target)
      {
        this.gameObject.Dispatch<CanceledEvent>(new CanceledEvent());
        return;
      }

      // If the target is being currently used, look for another or wait
      //if (this.Target.CanBeUsed(this.Agent))
      //  return;





      //this.Target.gameObject.Dispatch<InteractiveObject.InteractEvent>(new InteractiveObject.InteractEvent());

      // We are interacting with it
      var interaction = new InteractiveObject.InteractionStartedEvent();
      interaction.Source = this.Agent;
      this.Target.gameObject.Dispatch<InteractiveObject.InteractionStartedEvent>(interaction);

      this.OnInteract();
      this.HasInteracted = true;
    }
    
    protected override void OnReset()
    {
      this.OnInteractActionReset();
      this.HasInteracted = false;
    }

    /// <summary>
    /// Finds an eligible target for this action.
    /// </summary>
    void FindTarget()
    {
      var targets = new Dictionary<float, InteractiveObject>();

      // Look at the interactible objects that the agent has detected
      foreach (var interactive in this.Planner.InteractivesInRange)
      {
        if (this.OnValidateTarget(interactive))
        {
          // Calculate the distance between this agent and the object
          float dist = Vector3.Distance(this.transform.position, interactive.transform.position);

          // If it's currently being used, do nothing
          //if (!interactive.CanBeUsed(this.Agent))
          //  continue;

          targets.Add(dist, interactive);

          //this.Target = interactive;
          //Trace.Script(Description + " : Found a valid target = " + interactive.name, this);
          //return;
        }
      }

      // Pick the nearest target
      InteractiveObject nearestTarget = null;
      float nearestDist = Mathf.Infinity;
      foreach(var target in targets)
      {
        if (target.Key < nearestDist)
        {
          nearestDist = target.Key;
          nearestTarget = target.Value;
        }
      }
      this.Target = nearestTarget;

      //Trace.Script(Description + " : No valid target found!", this);
    }




  }



}
