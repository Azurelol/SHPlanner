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
using System.Text;  

namespace Prototype 
{
  [RequireComponent(typeof(Agent))]
  public partial class Planner : StratusBehaviour 
  {    
    //public class FSM
    //{      
    //  public interface State { void Update(FSM fsm); }
    //  Stack<State> Stack = new Stack<State>();
    //  public void Update(Agent agent)
    //  {
    //
    //  }
    //}

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class AssessEvent : Stratus.Event {}
    public class ActionSelectedEvent : Stratus.Event { public Action Action;
      public ActionSelectedEvent(Action action) { Action = action; } }
    public class PlanFormulatedEvent : Stratus.Event { public Plan Plan;
      public PlanFormulatedEvent(Plan plan) { Plan = plan; } }
    public class PlanExecutedEvent : Stratus.Event {}
    public class ReplanEvent : Stratus.Event {}

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// The currently set goal for this planner.
    /// </summary>
    public Goal CurrentGoal;

    /// <summary>
    /// The currently formulated plan
    /// </summary>
    public Plan CurrentPlan;

    /// <summary>
    /// The currently running action
    /// </summary>
    public Action CurrentAction;

    /// <summary>
    /// The current state of this agent.
    /// </summary>
    public WorldState CurrentState = new WorldState();

    /// <summary>
    /// The range at which objects will be considered.
    /// </summary>
    public float ConsiderationRange = 50.0f;

    /// <summary>
    /// A list of all interactive objects we can interact with.
    /// </summary>
    public List<InteractiveObject> InteractivesInRange = new List<InteractiveObject>();

    /// <summary>
    /// List of all available actions to this planner.
    /// </summary>
    public Action[] AvailableActions;

    /// <summary>
    /// Whether we are tracing for debugging purposes.
    /// </summary>
    public bool Tracing = false;
    
    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the planner.
    /// </summary>
    void Awake()
    {      
      this.Subscribe();
      this.AddActions();
      //this.CurrentPlan =  this.Formulate(CurrentGoal);
    }

    /// <summary>
    /// Subscribes to events.
    /// </summary>
    void Subscribe()
    {
      this.gameObject.Connect<AssessEvent>(this.OnAssessEvent);
      this.gameObject.Connect<Action.EndedEvent>(this.OnActionEndedEvent);
    }
    
    /// <summary>
    /// Adds all the actions available to this planner
    /// </summary>
    void AddActions()
    {
      AvailableActions = GetComponentsInChildren<Action>();
    }

    /// <summary>
    /// Invoked by the agent in order for this planner to formulate a plan.
    /// </summary>
    /// <param name="e"></param>
    void OnAssessEvent(AssessEvent e)
    {
      this.MakePlan();
    }

    /// <summary>
    /// Received when a plan has been formulated.
    /// </summary>
    /// <param name="e"></param>
    void OnPlanFormulatedEvent(PlanFormulatedEvent e)
    {
      // Run the plan
    }
    
    /// <summary>
    /// Received when an action has been canceled in the middle of a plan. This will make this
    /// agent replan.
    /// </summary>
    /// <param name="e"></param>
    void OnActionCanceledEvent(Action.CanceledEvent e)
    {
      Trace.Script("Action forcefully canceled. Replanning", this);
      this.CurrentAction.Reset();
      this.MakePlan();
    }

    /// <summary>
    /// Received when an action has finished
    /// </summary>
    /// <param name="e"></param>
    void OnActionEndedEvent(Action.EndedEvent e)
    {
      // Modify the current world state due to the previous action
      this.CurrentState.Merge(e.Action.Effects);
      ExecutePlan();
    }

    /// <summary>
    /// Executes the next action in the current plan
    /// </summary>
    void ExecutePlan()
    {
      // If there's nothing actions left in the plan, reassess?
      if (CurrentPlan.IsFinished)
      {
        this.CurrentGoal.Finish(this);
        this.gameObject.Dispatch<PlanExecutedEvent>(new PlanExecutedEvent());
        //if (Tracing) Trace.Script("The plan for " + this.CurrentGoal.Name + " has been fulfilled!", this);
        //this.gameObject.Dispatch<Agent.>
        return;
      }
      
      this.CurrentAction = CurrentPlan.Next();
      this.CurrentAction.Begin();      
      this.gameObject.Dispatch<ActionSelectedEvent>(new ActionSelectedEvent(this.CurrentAction));
    }
    
    /// <summary>
    /// Makes a plan given the current goal and actions available to this planner.
    /// </summary>
    void MakePlan()
    {
      this.Scan();
      this.CurrentPlan = Plan.Formulate(this, this.AvailableActions, this.CurrentState, this.CurrentGoal);
      if (this.CurrentPlan != null)
      {
        //Trace.Script("Executing plan!", this);
        this.gameObject.Dispatch<PlanFormulatedEvent>(new PlanFormulatedEvent(this.CurrentPlan));
        this.ExecutePlan();
      }
      else
      {
        //Trace.Script("The plan could not be formulated!", this);
      }
      
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
          //Trace.Script("Found " + interactive.Description, this);
          InteractivesInRange.Add(interactive);
        }

      }
    }

    /// <summary>
    /// Prints all actions available to this planner.
    /// </summary>
    /// <returns></returns>
    string PrintAvailableActions()
    {
      var builder = new StringBuilder();
      foreach(var action in AvailableActions)
      {
        builder.AppendFormat(" - {0}", action.Description);                
      }
      return builder.ToString();
    }

  
  }  
}
