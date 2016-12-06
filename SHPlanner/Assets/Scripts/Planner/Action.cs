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
using System.Collections;
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
    public MonoBehaviour Target;
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
    protected abstract bool RequiresRange { get; }

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
      Trace.Script("Beginning '" + Description + "'", this);

      // If not yet within range of the target
      if (this.RequiresRange && !IsWithinRange())
      {
        this.ApproachNow();
      }

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
      // If not within range of the target, approach it
      if (this.RequiresRange && !IsWithinRange())
      {
        //this.Approach();
        return;
      }

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
      Trace.Script("Ending '" + Description + "'", this);
      this.OnEnd();
      //Trace.Script(Description + " : Applying effects to the state", this.Agent);
      var e = new Action.EndedEvent();
      e.Action = this;
      this.gameObject.Dispatch<Action.EndedEvent>(e);

      this.Reset();
    }

    void ApproachNow()
    {
      Trace.Script("Now approaching " + this.Target, this);
      var path = AStar.FindPath(transform.position, this.Target.transform.position).ToArray();
      Trace.Script("Path between '" + transform.position + "' and '" + this.Target.transform.position + "': ", this);
      foreach (var point in path)
      {
        Trace.Script("- " + point.Location, this);
      } 
      StartCoroutine(this.ApproachRoutine(path));
    }

    
    // Approach the target
    IEnumerator ApproachRoutine(WayPoint[] path)
    {
      //Trace.Script("Path:");
      //foreach (var point in path)
      //{
      //  Trace.Script("- " + point.Location, this);
      //} 

      foreach (var point in path)
      {
        float step = this.Agent.MovementSpeed * Time.deltaTime;
        transform.LookAt(point.Location);
        while (Vector3.Distance(transform.position, point.Location) > 0.1f)
        {
          if (IsWithinRange())
            break;
          // Keep the same y
          //point.Location.y = transform.position.y;
          //Trace.Script("MOVING!", this);
          transform.position = Vector3.MoveTowards(transform.position, point.Location, step);
          yield return new WaitForFixedUpdate();
        }
      }

      //if (!IsWithinRange())
      //  this.ApproachNow();

      Trace.Script("Reached " + this.Target, this);

      //while (!IsWithinRange())
      //{
      //
      //}
      //
      //int currentPoint = 0;
      //Vector3 target = path[currentPoint].Location;
      //while (currentPoint == path.Length - 1)
      //{
      //  Vector3 move = Vector3.zero;
      //  float dist = 2;
      //  var loc = path[currentPoint].Location;
      //  dist = (path[currentPoint].Location - transform.position).sqrMagnitude;
      //  if (dist < 1 && currentPoint != path.Length - 1)
      //  {
      //    ++currentPoint;
      //    target = path[currentPoint].Location;
      //  }
      //
      //  if (true)
      //  {
      //    move = target - transform.position;
      //    move = move.normalized * Time.deltaTime;
      //  }
      //
      //  Trace.Script("Moving!", this);
      //  transform.position += move;
      //}
      //
      //yield return null;
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

    /// <summary>
    /// Approches the target in a straight line at a speed specified by the agent.
    /// </summary>
    void Approach()
    {
      this.transform.localPosition = Vector3.MoveTowards(this.transform.position, this.Target.transform.position, Time.deltaTime * this.Agent.MovementSpeed);
      this.transform.LookAt(this.Target.transform);
    }


  } 

}
