/******************************************************************************/
/*!
@file   PlanAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Prototype
{
  public abstract class Action : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public abstract class ActionEvent : Stratus.Event { public Action Action; }
    public class StartedEvent : ActionEvent { }
    public class EndedEvent : ActionEvent { }
    public class CanceledEvent : ActionEvent { }
    //public class ModifyWorldStateEvent : Stratus.Event { public WorldState Effects;
    //  public ModifyWorldStateEvent(WorldState state) { Effects = state; } }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    bool Active = false;
    protected Agent Agent;
    protected Planner Planner;
    public WorldState Preconditions = new WorldState();
    public WorldState Effects = new WorldState();
    public bool IsInterruptible = false;
    public float Cost = 1f;
    [Tooltip("How long it takes to execute this action")]
    public float Speed = 1f;
    protected Countdown ProgressTimer = new Countdown();
    [Tooltip("The range at which this action needs to be within the target")]
    public float Range = 2f;
    public float Progress { get { return ProgressTimer.Progress; } }
    public abstract string Description { get; }

    //------------------------------------------------------------------------/
    // Inheritance
    //------------------------------------------------------------------------/
    protected abstract void OnSetup();
    protected abstract void OnBegin();
    protected abstract void OnEnd();
    protected abstract void OnExecute();
    protected abstract bool OnValidate();
    protected abstract void OnReset();
    protected virtual bool OnCheckContextPrecondition() { return true; }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    void Start()
    {
      this.Agent = GetComponent<Agent>();
      this.Planner = GetComponent<Planner>();
      this.OnSetup();
    }

    /// <summary>
    /// Updates this action.
    /// </summary>
    /// <returns></returns>
    void Update()
    {
      if (!this.Active)
        return;

      // If the action has finished, end it
      if (this.Validate())      
        this.End();      
      // Otherwise keep executing it.
      else
        this.Execute();
    }

    /// <summary>
    /// Checks whether this action has any context preconditionsto be fulfilled
    /// </summary>
    /// <returns></returns>
    public bool CheckContextPrecondition()
    {
      return OnCheckContextPrecondition();
    }

    /// <summary>
    /// Starts this action.
    /// </summary>
    public void Begin()
    {            
      this.OnBegin();
      this.Active = true;
    }

    /// <summary>
    /// Resets all values to this action, making it execute again from the beginning.
    /// </summary>
    public void Reset()
    {
      this.Active = false;
      this.ProgressTimer.Reset(this.Speed);
      this.OnReset();
    }

    /// <summary>
    /// Checks whether the action has fulfilled its condition.
    /// </summary>
    /// <returns></returns>
    public bool Validate()
    {
      return this.OnValidate();
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    protected virtual void Execute()
    {
      // Start casting the action
      if (ProgressTimer.Update(Time.deltaTime))
      {
        //Trace.Script(Description + " : Executing!", this);
        this.OnExecute();
      }
      
      //Trace.Script(Description + " : Casting action...", this);

    }    

    /// <summary>
    /// Ends this action, applying its effect.
    /// </summary>
    void End()
    {
      this.OnEnd();
      //Trace.Script(Description + " : Applying effects to the state", this.Agent);
      var e = new Action.EndedEvent();
      e.Action = this;
      this.gameObject.Dispatch<Action.EndedEvent>(e);

      this.Reset();
    }
    

    void Approach()
    {

    }


  } 

}
