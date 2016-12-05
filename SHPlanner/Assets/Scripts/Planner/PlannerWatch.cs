/******************************************************************************/
/*!
@file   PlannerWatch.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;

namespace Prototype 
{
  public class PlannerWatch : StratusBehaviour 
  {
    [Tooltip("The planner currently being looked at")]
    public Planner Planner;

    public Text Name;
    public Text Plan;
    public Text Goal;
    public Text Action;
    public ProgressBar.ProgressBarBehaviour ActionProgress;

    void Start()
    {
      this.Subscribe();
      Watch();
    }

    void Subscribe()
    {
      if (!this.Planner)
        return;

      this.Planner.gameObject.Connect<Planner.PlanFormulatedEvent>(this.OnPlanFormulatedEvent);
      this.Planner.gameObject.Connect<Planner.PlanExecutedEvent>(this.OnPlanExecutedEvent);
      this.Planner.gameObject.Connect<Planner.ActionSelectedEvent>(this.OnActionSelectedEvent);
    }
    
    void Update()
    {
      if (Planner.CurrentAction != null && ActionProgress != null)
        ActionProgress.Value = Planner.CurrentAction.Progress * 100f;
    }

    void OnActionSelectedEvent(Planner.ActionSelectedEvent e)
    {
      if (this.ActionProgress) this.ActionProgress.SetFillerSize(0f);
      this.Action.text = e.Action.Description;
    }
    
    /// <summary>
    /// Received when the current plan has been successfully executed
    /// </summary>
    /// <param name="e"></param>
    void OnPlanExecutedEvent(Planner.PlanExecutedEvent e)
    {
      this.Plan.text = "";
      this.Action.text = "";
    }

    /// <summary>
    /// Received when a valid plan has been formulated by the planner
    /// </summary>
    /// <param name="e"></param>
    void OnPlanFormulatedEvent(Planner.PlanFormulatedEvent e)
    {
      this.Plan.text = e.Plan.Print();
    }

    /// <summary>
    /// Configures this watcher for a given planner
    /// </summary>
    void Watch()
    {
      this.Name.text = Planner.gameObject.name;
      this.Goal.text = Planner.CurrentGoal.Name;
    }


  }  
}
