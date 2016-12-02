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
  public abstract class Action
  {
    
    protected abstract void OnStart();
    protected abstract void OnEnd();
    protected abstract void OnExecute();
    protected abstract bool OnValidate();

    public bool Update()
    {
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

    public void Start()
    {
      Trace.Script("", this.Agent);
      this.OnStart();
    }

    /// <summary>
    /// Resets all values to this action, making it execute again from the beginning.
    /// </summary>
    public void Reset()
    {

    }

    bool Validate()
    {
      return this.OnValidate();
    }

    void Execute()
    {
      this.OnExecute();
    }    

    public void End()
    {
      this.OnEnd();
      Trace.Script("", this.Agent);
    }

    public Agent Agent;
    public string Name;
    public string Description;
    public List<State> Preconditions = new List<State>();
    public List<State> Effects = new List<State>();
    public float Cost;
    public Transform Target;
  }


  public class MoveAction : Action
  {
    public float Range = 2.0f;

    protected override void OnExecute()
    {
      this.Agent.transform.position = Vector3.MoveTowards(Agent.transform.position, Target.position, Time.deltaTime);
    }

    protected override void OnStart()
    {
     
    }

    protected override void OnEnd()
    {
      //Effects.Add(State.AtLocation);
    }

    protected override bool OnValidate()
    {
      // Check if we are within the right distance of the target
      if (Vector3.Distance(Agent.transform.position, Target.transform.position) < this.Range)
        return true;

      return false;
    }
  }

}
