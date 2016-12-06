/******************************************************************************/
/*!
@file   Plan_Astar.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using Stratus;
using System.Collections.Generic;

namespace Prototype
{
  public partial class Planner : StratusBehaviour
  {
    public partial class Plan
    {
      /// <summary>
      /// This planner uses A* to do a search for a valid path of actions that will lead
      /// to the desired state.
      /// </summary>
      public class AstarSearch
      {
        /// <summary>
        /// Represents a node in the graph of actions.
        /// </summary>
        public class Node
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
        // Properties
        //------------------------------------------------------------------------/
        bool Tracing = true;
        List<Node> OpenList = new List<Node>();
        Dictionary<WorldState, Action> ActionEffectsTable = new Dictionary<WorldState, Action>();
        Node StartingNode, DestinationNode;
        // Planner properties
        WorldState StartState;
        WorldState EndState;
        Action[] Actions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startingState"></param>
        /// <param name="goal"></param>
        /// <param name="actions"></param>
        public AstarSearch(WorldState startingState, WorldState goal, Action[] actions)
        {
          StartState = startingState.Copy();
          EndState = goal;
          Actions = actions;
        }

        /// <summary>
        /// Configures the search, creating the starting and destination nodes out of the states,
        /// as well as making the map.
        /// </summary>
        public void Initialize()
        {
          this.StartingNode = new Node(null, 0f, this.EndState, null);
          this.DestinationNode = new Node(null, 0f, this.StartState, null);
          
          this.MakeMap();
          // 1. Put the starting node on the open list
          PutOnList(OpenList, this.StartingNode, Node.ListStatus.OpenList);
        }
        
        void MakeMap()
        {
          // Make an action-effects table (Orkin, 2004)
          foreach(var action in Actions)
          {
            ActionEffectsTable.Add(action.Effects, action);
          }



        }

        /// <summary>
        /// Finds a solution to the goal using A*
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public List<Action> FindSolution()
        {


          while (!OpenList.Empty())
          {

          }

          var path = new List<Action>();
          return path;
        }

        static float CalculateHeuristicCost(Node node, Node target)
        {
          return 1f;
        }

        /// <summary>
        /// Puts a node on the list
        /// </summary>
        /// <param name="openList"></param>
        /// <param name="node"></param>
        /// <param name="list"></param>
        void PutOnList(List<Node> openList, Node node, Node.ListStatus list)
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

      }



    }
  }
}