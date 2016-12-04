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
    public partial class Plan
    {
      //------------------------------------------------------------------------/
      // Definitions
      //------------------------------------------------------------------------/
      /// <summary>
      /// Represents a node in the graph of actions.
      /// </summary>
      class Node
      {
        public enum ListStatus { OpenList, ClosedList }

        /// <summary>
        /// The parent of this node, whose preconditions this node fulfills
        /// </summary>
        public Node Parent;
        /// <summary>
        /// Whether this node is on the open or closed list
        /// </summary>
        public ListStatus Status;
        /// <summary>
        /// f(x) = g(x) + h(x): The current cost of the node
        /// </summary>
        public float Cost;
        /// <summary>
        /// g(x): How much it costs to get back to the starting node
        /// </summary>
        public float GivenCost = 0f;
        /// <summary>
        /// Everytime we do a search, we increment this. Behaves like a dirty bit.
        /// </summary>
        public int Iteration = 0;

        public WorldState State;
        public Action Action;
        public Node(Node parent, float cost, WorldState state, Action action)
        {
          Parent = parent;
          Cost = cost;
          State = state;
          Action = action;
        }
      }

      
      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// A sequence of actions, where each action represents a state transition.
      /// </summary>
      public LinkedList<Action> Actions = new LinkedList<Prototype.Action>();
      public bool IsFinished { get { return Actions.Count == 0; } }

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
        Actions.AddFirst(action);
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
        var usableActions = (from action in actions where action.CheckContextPrecondition() select action).ToArray();

        //if (planner.Tracing)
        //{
        //  Trace.Script("Making plan to satisfy the goal '" + goal.Name + "' with preconditions:" + goal.DesiredState.Print(), planner);
        //  Trace.Script("Actions available:" + planner.PrintAvailableActions(), planner);
        //}

        // Build up a tree of nodes
        List<Node> path = new List<Node>();
        Node starting = new Node(null, 0f, goal.DesiredState, null);
        // Look for a solution, backtracking from the goal's desired world state until
        // we have fulfilled every precondition leading up to it!
        var hasFoundPath = FindSolution(path, starting, usableActions);
        // If the path has not been found
        if (!hasFoundPath)
        {
          return new Plan();
        }
        
        //Trace.Script("The path has " + path.Count + " nodes!", planner);
                      
        // Make the plan
        var plan = new Plan();
        foreach (var node in path)
          plan.Add(node.Action);

        //Trace.Script("Formulated plan: \n" + plan.Print(), planner);
        return plan;
      }

      /// <summary>
      /// Looks for a solution, backtracking from the goal to the current world state
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="actions"></param>
      /// <param name="goal"></param>
      /// <returns></returns>
      static bool FindSolution(List<Node> path, Node parent, Action[] actions)
      {
        bool solutionFound = false;
        Node cheapestNode = null;

        //Trace.Script("Looking to fulfill the preconditions:" + parent.State.Print());

        // Look for actions that fulfill the preconditions
        foreach(var action in actions)
        {
          if (action.Effects.Satisfies(parent.State))
          {
            //Trace.Script(action.Description + " satisfies the preconditions");

            // Create a new node
            var node = new Node(parent, parent.Cost + action.Cost, action.Preconditions, action);

            // Replace the previous best node
            if (cheapestNode == null) cheapestNode = node;
            else if (cheapestNode.Cost > node.Cost) cheapestNode = node;
          }
          else
          {
            //Trace.Script(action.Description + " does not satisfy the preconditions");
          }
        }

        if (cheapestNode == null)
        {
          //Trace.Script("No actions could fulfill these preconditions");
          return false;
        }

        // Add the cheapest node to the path
        path.Add(cheapestNode);
        //Trace.Script("Adding " + cheapestNode.Action.Description + " to the path");

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
          bool found = FindSolution(path, cheapestNode, actionSubset);
          if (found)solutionFound = true;
        }

        return solutionFound;
      }

      /// <summary>
      /// Finds a solution to the goal using A*
      /// </summary>
      /// <param name="actions"></param>
      /// <param name="goal"></param>
      /// <returns></returns>
      static Queue<Action> FindSolution(WorldState startingState, Action[] actions, WorldState goal)
      {
        var openList = new List<Node>();
        var startingNode = new Node(null, 0f, goal, null);
        var destNode = new Node(null, 0f, startingState, null);

        // 1. Put the starting node on the open list
        PutOnList(openList, startingNode, Node.ListStatus.OpenList);

        while (!openList.Empty())
        {

        }

        var path = new Queue<Action>();
        return path;       
      }



      static float CalculateHeuristicCost(Node node, Node target)
      {
        return 1f;
      }

      static void PutOnList(List<Node> openList, Node node, Node.ListStatus list)
      {
        if (list == Node.ListStatus.OpenList)
        {
          openList.Add(node);
          node.Status = Node.ListStatus.OpenList;
        }
        else
        {
          node.Status = Node.ListStatus.ClosedList;
        }
      }

      public string Print()
      {
        var builder = new StringBuilder();
        foreach(var action in Actions)
        {         
          builder.AppendLine("- " + action.Description);
        }
        return builder.ToString();
      }



    }
  }

}
