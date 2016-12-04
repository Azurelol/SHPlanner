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

    void Start()
    {
      this.Subscribe();
      Watch();
    }

    void Subscribe()
    {
      if (!this.Planner)
        return;
      //this.Planner.
    }

    void OnPlanFormulatedEvent(Planner.PlanFormulatedEvent e)
    {

    }

    void Watch()
    {
      this.Name.text = Planner.gameObject.name;
      this.Goal.text = Planner.CurrentGoal.Name;
    }


  }  
}
