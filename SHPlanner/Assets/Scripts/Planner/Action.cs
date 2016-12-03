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
    public class StartEvent {}
    public class EndEvent {}
    public class ModifyWorldStateEvent : Stratus.Event { public WorldState Effects;
      public ModifyWorldStateEvent(WorldState state) { Effects = state; } }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    bool Active = false;
    protected Agent Agent;
    protected Planner Planner;
    public abstract string Name { get; }
    public WorldState Preconditions = new WorldState();
    public WorldState Effects = new WorldState();
    public bool IsInterruptible = false;
    public float Cost = 1f;
    //public Transform Target;

    //------------------------------------------------------------------------/
    // Inheritance
    //------------------------------------------------------------------------/
    protected abstract void OnSetup();
    protected abstract void OnBegin();
    protected abstract void OnEnd();
    protected abstract void OnExecute();
    protected abstract bool OnValidate();
    protected abstract void OnReset();

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
    public bool Update()
    {
      if (!this.Active)
        return false;

      // If the action has finished, end it
      if (this.Validate())
      {
        this.End();
        return true;
      }

      // Otherwise keep executing it.
      this.Execute();
      return false;
    }


    public bool CheckContextPrecondition()
    {
      return true;
    }

    /// <summary>
    /// Starts this action.
    /// </summary>
    public void Begin()
    {
      Trace.Script("", this.Agent);
      this.OnBegin();
    }

    /// <summary>
    /// Resets all values to this action, making it execute again from the beginning.
    /// </summary>
    public void Reset()
    {
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
    void Execute()
    {
      this.OnExecute();
    }    

    /// <summary>
    /// Ends this action, applying its effect.
    /// </summary>
    public void End()
    {
      this.OnEnd();
      Trace.Script("Applying effects to the state", this.Agent);
      this.gameObject.Dispatch<ModifyWorldStateEvent>(new ModifyWorldStateEvent(this.Effects));
    }

    void Approach()
    {

    }


  }


  //public class MoveAction : Action
  //{
  //  public float Range = 2.0f;
  //
  //  protected override void OnExecute()
  //  {
  //    this.Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, Target.position, Time.deltaTime);
  //  }
  //
  //  protected override void OnStart()
  //  {
  //   
  //  }
  //
  //  protected override void OnEnd()
  //  {
  //    //Effects.Add(State.AtLocation);
  //  }
  //
  //  protected override bool OnValidate()
  //  {
  //    // Check if we are within the right distance of the target
  //    if (Vector3.Distance(Agent.transform.position, Target.transform.position) < this.Range)
  //      return true;
  //
  //    return false;
  //  }
  //}

}
