/******************************************************************************/
/*!
@file   Agent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype 
{
  public class Agent : StratusBehaviour 
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    /// <summary>
    /// Moves to the agent to the target destination. It will compute a path to it.
    /// </summary>
    public class MoveToTargetEvent : Stratus.Event
    {
      public Vector3 Target;
    }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    [Range(0.0f, 1.0f)] public float AssessmentPeriod = 0.4f;
    System.Action CurrentBehavior;
    Countdown AssesssmentTimer;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Connects to movement events (sent from the planner), and initializes the
    /// assessment timer.
    /// </summary>
    void Start()
    {
      this.Subscribe();
      AssesssmentTimer = new Countdown(this.AssessmentPeriod);
      this.CurrentBehavior = Idle;
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    void Subscribe()
    {
      this.gameObject.Connect<MoveToTargetEvent>(this.OnMoveToTargetEvent);
      this.gameObject.Connect<Planner.ActionSelectedEvent>(this.OnActionSelectedEvent);
    }

    /// <summary>
    /// The agent will pick an action after a specified assessment interval
    /// if it's currently idle.
    /// </summary>
    void Update()
    {
      if (AssesssmentTimer.Update(Time.deltaTime))
      {        
        if (this.CurrentBehavior == Idle)
        {
          this.Assess();
        }
        AssesssmentTimer.Reset();
      }
    }

    void OnActionSelectedEvent(Planner.ActionSelectedEvent e)
    {
      this.CurrentBehavior = Acting;
    }

    void OnMoveToTargetEvent(MoveToTargetEvent e)
    {
    }


    void Assess()
    {
      Trace.Script("Assessing the situation...", this);
      this.gameObject.Dispatch<Planner.AssessEvent>(new Planner.AssessEvent());
    }

    void Idle()
    {
    }

    void Acting()
    {

    }
    


    public void Move()
    {

    }

  
  }  
}
