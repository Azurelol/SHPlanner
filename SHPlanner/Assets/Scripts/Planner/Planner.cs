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
    public class FSM
    {      
      public interface State { void Update(FSM fsm); }
      Stack<State> Stack = new Stack<State>();
      public void Update(Agent agent)
      {

      }
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class AssessEvent : Stratus.Event {}
    public class ActionSelectedEvent : Stratus.Event {}
    public class PlanFormulatedEvent : Stratus.Event { Plan Plan; }

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
    /// Modifies the current world state of this planner.
    /// </summary>
    /// <param name="e"></param>
    void OnModifyWorldStateEvent(Action.ModifyWorldStateEvent e)
    {
      this.CurrentState.Merge(e.Effects);
    }
    
    /// <summary>
    /// Makes a plan given the current goal and actions available to this planner.
    /// </summary>
    void MakePlan()
    {
      this.Scan();
      this.CurrentPlan = Plan.Formulate(this, this.AvailableActions, this.CurrentState, this.CurrentGoal);
      if (this.CurrentPlan != null)
        this.gameObject.Dispatch<PlanFormulatedEvent>(new PlanFormulatedEvent());

      this.gameObject.Dispatch<ActionSelectedEvent>(new ActionSelectedEvent());
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
          InteractivesInRange.Add(interactive);
        }

      }
      
      //if (Tracing)
      //{
      //  Trace.Script(InteractivesInRange, this);
      //}

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
        builder.AppendFormat(" - {0}", action.Name);                
      }
      return builder.ToString();
    }

  
  }  
}
