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
  [RequireComponent(typeof(Sensor))]
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
    /// The blackboard this agent is using.
    /// </summary>
    public Blackboard Blackboard;

    /// <summary>
    /// The sensor this planner is using
    /// </summary>
    [HideInInspector] public Sensor Sensor;

    /// <summary>
    /// List of all available actions to this planner.
    /// </summary>
    [HideInInspector] public Action[] AvailableActions;

    /// <summary>
    /// Whether we are tracing for debugging purposes.
    /// </summary>
    public bool Tracing = false;

    /// <summary>
    /// The agent this planner is working for
    /// </summary>
    Agent Agent;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Initializes the planner.
    /// </summary>
    void Awake()
    {
      this.Sensor = GetComponent<Sensor>();
      this.Agent = GetComponent<Agent>();
      this.Blackboard = Agent.Blackboard;
      this.Subscribe();
      this.AddActions();
      //this.CurrentPlan =  this.Formulate(CurrentGoal);
    }

    /// <summary>
    /// Subscribes to events.
    /// </summary>
    void Subscribe()
    {
      this.gameObject.Connect<WorldState.ModifySymbolEvent>(this.OnModifySymbolEvent);
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
    /// Modifies a single symbol of this planner's world state
    /// </summary>
    /// <param name="e"></param>
    void OnModifySymbolEvent(WorldState.ModifySymbolEvent e)
    {
      this.CurrentState.Apply(e.Symbol);
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
      //Trace.Script("Applying the effects of the action " + e.Action.Description, this);
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
      this.Sensor.Scan();
      this.CurrentPlan = Plan.Formulate(this, this.AvailableActions, this.CurrentState, this.CurrentGoal);
      if (this.CurrentPlan != null)
      {
        if (Tracing) Trace.Script("Executing new plan!", this);
        this.gameObject.Dispatch<PlanFormulatedEvent>(new PlanFormulatedEvent(this.CurrentPlan));
        this.ExecutePlan();
      }
      else
      {
        if (Tracing) Trace.Script("The plan could not be formulated!", this);
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
