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
using System.Text;
using System.Collections.Generic;

namespace Prototype 
{
  public class PlannerWatch : StratusBehaviour 
  {
    [Tooltip("The planner currently being looked at")]
    [HideInInspector] public Planner Planner;
    public Dropdown Dropdown;
    public Text Name;
    public Text Plan;
    public Text Goal;
    public Text Action;
    public Text Blackboard;
    public ProgressBar.ProgressBarBehaviour ActionProgress;
    GameObject[] Agents;

    /// <summary>
    /// Configures the dropdown menu and sets the initial planner
    /// </summary>
    void Start()
    {
      this.ConfigureDropdown();
      //this.Subscribe();
      this.ChangePlanner(0);
    }
    
    /// <summary>
    /// Configures the dropdown menu
    /// </summary>
    void ConfigureDropdown()
    {
      // Get a list of all Planners on the scene
      Agents = GameObject.FindGameObjectsWithTag("Agent");
      if (Agents.Length == 0)
        throw new System.Exception("No agents in scene or they were not properly set. Make sure they have the tag 'Agent' on them!");

      var names = new List<string>();
      foreach (var agent in Agents)
        names.Add(agent.name);

      // Set the dropdown list
      Dropdown.ClearOptions();
      Dropdown.AddOptions(names);
    }

    /// <summary>
    /// Changes the current Planner that is being watched
    /// </summary>
    /// <param name="index">The index of the planner being watched</param>
    public void ChangePlanner(int index)
    {
      // Disconnect from all previous events
      this.Disconnect();
      this.Planner = Agents[index].GetComponent<Planner>();
      // Subscribe to its events
      this.Planner.gameObject.Connect<Planner.PlanFormulatedEvent>(this.OnPlanFormulatedEvent);
      this.Planner.gameObject.Connect<Planner.PlanExecutedEvent>(this.OnPlanExecutedEvent);
      this.Planner.gameObject.Connect<Planner.ActionSelectedEvent>(this.OnActionSelectedEvent);
      // Update the text
      if (this.Planner.CurrentPlan != null) this.Plan.text = Planner.CurrentPlan.Print();
      this.Goal.text = Planner.CurrentGoal.Name;

      Trace.Script("Now watching:" + Agents[index].name);
    }
    
    void Update()
    {
      if (!Planner)
        return;

      this.DisplayPlanner();

      //if (Planner.CurrentAction != null && ActionProgress != null)
      //  ActionProgress.Value = Planner.CurrentAction.Progress * 100f;
    }

    void DisplayPlanner()
    {
      this.DisplayBlackboard();
    }
      
    void DisplayBlackboard()
    {
      var message = new StringBuilder();
      message.AppendLine("Money = " + Planner.Blackboard.Money.Charges);
      message.AppendLine("Tools = " + Planner.Blackboard.Tool.Charges);
      Blackboard.text = message.ToString();
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
    


  }  
}
