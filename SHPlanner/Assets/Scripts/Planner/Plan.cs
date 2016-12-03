/******************************************************************************/
/*!
@file   Plan.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Prototype 
{
  public partial class Planner : StratusBehaviour
  {
    public partial class Plan
    {
      /// <summary>
      /// A sequence of actions, where each action represents a state transition.
      /// </summary>
      public Queue<Action> Actions = new Queue<Action>();

      public void Add(Action action)
      {
        Actions.Enqueue(action);
      }

      /// <summary>
      /// Given a goal, formulates a plan.
      /// </summary>
      /// <param name="goal"></param>
      /// <returns></returns>
      public static Plan Formulate(Planner planner, Action[] actions, WorldState currentState, Goal goal)
      {
        // Reset all actions
        foreach (var action in actions)
          action.Reset();



        var plan = new Plan();
        //plan.Actions.Add(new Action)

        // Starting from the desired goal's state, look for a sequence
        // of actions that will lead to that state
        var currentState = CurrentState;

        if (Tracing)
        {
          Trace.Script("Making plan to satisfy the goal '" + goal.Name + "' with preconditions:" + goal.DesiredState.Print(), this);
          Trace.Script("Actions available:" + PrintAvailableActions(), this);
        }

        foreach (var action in AvailableActions)
        {
          if (action.Effects.Satisfies(goal.DesiredState))
          {
            Trace.Script(action.Name + " with effects: " + action.Effects.Print() + " satisfies the preconditions", this);
          }
          else
          {
            Trace.Script(action.Name + " does not satisfy the preconditions!", this);
          }
        }
        //while (currentState != goal.DesiredState)
        //{
        //  // Look for the actions that would fulfill this goal
        //  foreach(var action in AvailableActions)
        //  {
        //    if (action.Effects.Properties.Contains(goal.DesiredState))
        //    {
        //
        //    }
        //  }
        //}

        Trace.Script("Formulated plan:" + plan.Actions, this);
        return plan;
      }





    }
  }

}
