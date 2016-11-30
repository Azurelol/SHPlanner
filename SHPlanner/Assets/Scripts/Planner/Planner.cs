/******************************************************************************/
/*!
@file   Planner.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Prototype 
{
  [RequireComponent(typeof(Agent))]
  public class Planner : StratusBehaviour 
  {    
    public class MakePlanEvent : Stratus.Event { public Goal Goal; }


    /// <summary>
    /// The currently set goal for this planner.
    /// </summary>
    public Goal CurrentGoal;

    /// <summary>
    /// The currently formulated plan
    /// </summary>
    public Plan CurrentPlan;

    /// <summary>
    /// The current state of this agent.
    /// </summary>
    public State CurrentState;

    /// <summary>
    /// The range at which objects will be considered.
    /// </summary>
    public float ConsiderationRange = 50.0f;

    /// <summary>
    /// A list of all interactive objects we can interact with.
    /// </summary>
    public List<InteractiveObject> InteractivesInRange = new List<InteractiveObject>();

    public List<Action> AvailableActions = new List<Action>();

    void Awake()
    {
      this.gameObject.Connect<MakePlanEvent>(this.OnMakePlanEvent);
      this.CurrentGoal = new TouchGoal();
      //this.CurrentGoal.Target
      
      this.CurrentPlan =  this.Formulate(CurrentGoal);
    }

    void AddActions()
    {
      this.AvailableActions.Add(new MoveAction());
    }

    void OnMakePlanEvent(MakePlanEvent e)
    {
      Formulate(e.Goal);
    }

    /// <summary>
    /// Given a goal, formulates a plan.
    /// </summary>
    /// <param name="goal"></param>
    /// <returns></returns>
    public Plan Formulate(Goal goal)
    {
      this.Scan();
      Trace.Script("Making plan to satisfy the goal: " + goal.Description, this);

      var plan = new Plan();
      

      // Starting from the desired goal's state, look for a sequence
      // of actions that will lead to that state
      var currentState = CurrentState;
      while (currentState != goal.State)
      {
        // Look for the actions that would fulfill this goal
        foreach(var action in AvailableActions)
        {
          if (action.Effects.Contains(goal.State))
          {

          }
        }
      }


      return plan;
    }

    void Update()
    {
      // Update the current goal
      CurrentGoal.Update(Time.deltaTime);

    }

    /// <summary>
    /// Scans the world for objects we can interact with.
    /// </summary>
    /// <returns></returns>
    void Scan()
    {
      InteractivesInRange.Clear();

      var scan = Physics.OverlapSphere(transform.position, this.ConsiderationRange);
      foreach(var hit in scan)
      {
        var interactive = hit.GetComponent<InteractiveObject>();
        if (interactive != null)
        {
          Trace.Script("Found '" + interactive.name + "' within range!", this);
          InteractivesInRange.Add(interactive);
        }

      }
      
    }


  
  }  
}
