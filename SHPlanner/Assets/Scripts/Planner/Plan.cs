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
using System.Linq;
using System.Text;

namespace Prototype
{
  public partial class Planner : StratusBehaviour
  {
    /// <summary>
    /// A plan is a sequence of actions that will fulfill a specified goal world state,
    /// given a starting world state.
    /// </summary>
    public partial class Plan
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// A sequence of actions, where each action represents a state transition.
      /// </summary>
      public LinkedList<Action> Actions = new LinkedList<Prototype.Action>();
      /// <summary>
      /// Whether this plan has finished running
      /// </summary>
      public bool IsFinished { get { return Actions.Count == 0; } }
      /// <summary>
      /// Whether A* should be used to formulate the plan
      /// </summary>
      static bool UseAstar = true;
      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Gets the next action in the sequence.
      /// </summary>
      /// <returns></returns>
      public Action Next()
      {
        var action = Actions.First();
        Actions.RemoveFirst();
        return action;
      }

      /// <summary>
      /// Adds an action to the plan
      /// </summary>
      /// <param name="action"></param>
      public void Add(Action action)
      {
        Actions.AddLast(action);
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

        // Get all valid actions whose context preconditions are true
        var usableActions = (from action
                             in actions
                             where action.CheckContextPrecondition() && !currentState.Satisfies(action.Effects)
                             select action).ToArray();

        if (planner.Tracing)
        {
          Trace.Script("Making plan to satisfy the goal '" + goal.Name + "' with preconditions:" + goal.DesiredState.Print(), planner);
          Trace.Script("Actions available:", planner);
          foreach (var action in usableActions)
            Trace.Script("- " + action.Description, planner);
        }

        // The path of actions
        List<Action> path;

        if (Plan.UseAstar)
        {
          AstarSearch search = new AstarSearch(currentState, goal.DesiredState, usableActions);
          search.Tracing = planner.Tracing;
          search.Initialize();
          path = search.FindSolution();
        }
        else
        {
          // Build up a tree of nodes
          path = new List<Action>();
          AstarSearch.Node starting = new AstarSearch.Node(null, 0f, goal.DesiredState, null);
          // Look for a solution, backtracking from the goal's desired world state until
          // we have fulfilled every precondition leading up to it!
          var hasFoundPath = FindSolution(path, starting, usableActions, planner);
          // If the path has not been found
          if (!hasFoundPath)
          {
            if (planner.Tracing) Trace.Script("No plan could be formulated!", planner);
            return new Plan();
          }
        }

        // If no solution was found
        if (path == null)
          return null;

        // Make the plan
        var plan = new Plan();
        foreach (var action in path)
          plan.Add(action);
        return plan;
      }

      /// <summary>
      /// Looks for a solution, backtracking from the goal to the current world state
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="actions"></param>
      /// <param name="goal"></param>
      /// <param name="currentState"></param>
      /// <returns></returns>
      static bool FindSolution(List<Action> path, AstarSearch.Node parent, Action[] actions, Planner planner)
      {
        bool solutionFound = false;
        AstarSearch.Node cheapestNode = null;

        if (planner.Tracing) Trace.Script("Looking to fulfill the preconditions:" + parent.State.Print());

        // Look for actions that fulfill the preconditions
        foreach (var action in actions)
        {
          if (action.Effects.Satisfies(parent.State))
          {
            if (planner.Tracing) Trace.Script(action.Description + " satisfies the preconditions");

            // Create a new node
            var node = new AstarSearch.Node(parent, parent.Cost + action.Cost, action.Preconditions, action);

            // Replace the previous best node
            if (cheapestNode == null) cheapestNode = node;
            else if (cheapestNode.Cost > node.Cost) cheapestNode = node;
          }
          else
          {
            if (planner.Tracing) Trace.Script(action.Description + " does not satisfy the preconditions");
          }
        }

        // If no satisfactory action was found
        if (cheapestNode == null)
        {
          // If the current state is already fulfilling this condition, we are done!
          if (planner.CurrentState.Satisfies(parent.State)) return true;
          // Otherwise, no valid solution could be found
          if (planner.Tracing) Trace.Script("No actions could fulfill these preconditions");
          return false;
        }

        // Add the cheapest node to the path
        path.Add(cheapestNode.Action);
        if (planner.Tracing) Trace.Script("Adding " + cheapestNode.Action.Description + " to the path");

        // If this action has no more preconditions left to fulfill
        if (cheapestNode.Action.Preconditions.IsEmpty)
        {
          //Trace.Script("No preconditions left!");
          solutionFound = true;
        }
        // Else if it has a precondition left to fulfill, keep looking
        else
        {
          var actionSubset = (from remainingAction in actions where !remainingAction.Equals(cheapestNode.Action) select remainingAction).ToArray();
          bool found = FindSolution(path, cheapestNode, actionSubset, planner);
          if (found) solutionFound = true;
        }

        return solutionFound;
      }

      /// <summary>
      /// Plans all the actions in this plan.
      /// </summary>
      /// <returns></returns>
      public string Print()
      {
        var builder = new StringBuilder();
        foreach (var action in Actions)
        {
          builder.AppendLine("- " + action.Description + " (" + action.Cost + ")");
        }
        return builder.ToString();
      }



    }
  }

}
