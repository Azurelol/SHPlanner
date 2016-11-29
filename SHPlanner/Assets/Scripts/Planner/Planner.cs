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
  public class Planner : StratusBehaviour 
  {    
    public class MakePlanEvent : Stratus.Event { public Plan.Goal Goal; }


    /// <summary>
    /// The currently set goal for this planner.
    /// </summary>
    public Plan.Goal CurrentGoal;

    /// <summary>
    /// The current state of this agent.
    /// </summary>
    public Plan.State CurrentState;

    void Start()
    {
      this.gameObject.Connect<MakePlanEvent>(this.OnMakePlanEvent);
      this.Formulate(CurrentGoal);
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
    public Plan Formulate(Plan.Goal goal)
    {
      Trace.Script("Making plan to satisfy the goal: " + goal, this);

      var plan = new Plan();

      // Starting from the beginning, look for a plan that takes the planner's agent
      // from their current state to their goal




      return plan;
    }


    /// <summary>
    /// Scans the world for objects we can interact with.
    /// </summary>
    /// <returns></returns>
    bool ScanWorld()
    {
      return false;
    }


  
  }  
}
