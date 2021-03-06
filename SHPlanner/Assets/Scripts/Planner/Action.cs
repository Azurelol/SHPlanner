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

    public enum ActionStatus { Moving, Executing, Inactive }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    [Tooltip("The current target of this action")]
    [ReadOnly]
    public MonoBehaviour Target;
    [Tooltip("Whether this action can be interrupted")]
    public bool IsInterruptible = false;
    [Tooltip("The cost of this action. Used by the planner's A*")]
    public float Cost = 1f;
    [Tooltip("How long it takes to execute this action")]
    public float Speed = 1f;
    [Tooltip("The range at which this action needs to be within the target")]
    public float Range = 2f;

    [ReadOnly] public ActionStatus CurrentStatus = ActionStatus.Inactive;

    public float Progress { get { return ProgressTimer.Progress; } }
    public abstract string Description { get; }
    protected abstract bool RequiresRange { get; }
    protected Agent Agent;
    protected Planner Planner;
    public WorldState Preconditions = new WorldState();
    public WorldState Effects = new WorldState();
    protected Countdown ProgressTimer = new Countdown();
    bool Active = false;
    bool Tracing = false;

    //------------------------------------------------------------------------/
    // Inheritance
    //------------------------------------------------------------------------/
    protected virtual void OnStart() {}
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
      this.OnStart();
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

    void OnObjectResourceDestroyedEvent()
    {

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
      if (Tracing) Trace.Script("Beginning '" + Description + "'", this);

      // If not yet within range of the target
      if (this.RequiresRange && !IsWithinRange())
      {
        this.Approach();
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
      this.CurrentStatus = ActionStatus.Inactive;
      this.Target = null;
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
        // If the target has been destroyed, cancel this action
        // If something happened to the target, replan
        if (!this.Target)
        {
          this.gameObject.Dispatch<CanceledEvent>(new CanceledEvent());
          return;
        }

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
      if (Tracing) Trace.Script("Ending '" + Description + "'", this);
      this.OnEnd();
      //Trace.Script(Description + " : Applying effects to the state", this.Agent);
      var e = new Action.EndedEvent();
      e.Action = this;
      this.gameObject.Dispatch<Action.EndedEvent>(e);

      this.Reset();
    }

    /// <summary>
    /// Approaches the current target of this action
    /// </summary>
    void Approach()
    {
      if (!this.Target)
        return;

      var path = AStar.FindPath(transform.position, this.Target.transform.position).ToArray();

      if (Tracing)
      {
        Trace.Script("Now approaching " + this.Target, this);
        Trace.Script("Path between '" + transform.position + "' and '" + this.Target.transform.position + "': ", this);
        foreach (var point in path)
        {
          Trace.Script("- " + point.Key.Location, this);
        }
      }

      this.CurrentStatus = ActionStatus.Moving;
      StartCoroutine(this.FollowPathRoutine(path));
    }

    /// <summary>
    /// Follows a path of waypoints to the currnet target
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator FollowPathRoutine(KeyValuePair<WayPoint, GameObject[]>[] path)
    {
      foreach (var point in path)
      {
        float step = this.Agent.MovementSpeed * Time.deltaTime;
        transform.LookAt(point.Key.Location);
        while (Vector3.Distance(transform.position, point.Key.Location) > 0.1f)
        {
          if (IsWithinRange())
            break;

          // @TODO: Ugh...
          if (transform.position == point.Key.Location)
            break;
          // Keep the same y
          //point.Location.y = transform.position.y;
          //Trace.Script("MOVING!", this);
          transform.position = Vector3.MoveTowards(transform.position, point.Key.Location, step);
          yield return new WaitForFixedUpdate();
        }

        foreach (var obj in point.Value)
        {
          obj.Destroy();
        }
      }

      this.CurrentStatus = ActionStatus.Executing;

      if (Tracing) Trace.Script("Reached " + this.Target, this);
    }

    /// <summary>
    /// Approches the target in a straight line
    /// </summary>
    IEnumerator ApproachRoutine()
    {
      while (!IsWithinRange())
      {
        this.transform.localPosition = Vector3.MoveTowards(this.transform.position, this.Target.transform.position, Time.deltaTime * this.Agent.MovementSpeed);
        this.transform.LookAt(this.Target.transform);
        yield return new WaitForFixedUpdate();
      }
    }

    /// <summary>
    /// Checks whether the agent is within range of its target
    /// </summary>
    /// <returns></returns>
    bool IsWithinRange()
    {
      // @TODO: This shouldn't be checked here
      if (!this.Target)
        return false;

      var targetDist = Vector3.Distance(this.Target.transform.position, this.transform.position);
      if (targetDist <= this.Range) return true;
      return false;
    }




  }

}
