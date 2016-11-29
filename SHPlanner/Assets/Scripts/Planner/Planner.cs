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
    /// <summary>
    /// An action is a single, atomic step within a plan that makes a character do something.
    /// </summary>
    public class Action
    {

      public abstract class Precondition
      {
        public abstract bool Check();
      }

      public abstract class Effect
      {

      }

      public List<Precondition> Preconditions = new List<Precondition>();
      public List<Effect> Effects = new List<Effect>();
      public float Cost;
    }

    /// <summary>
    /// A sequence of actions.
    /// </summary>
    public class Plan
    {
      /// <summary>
      /// A sequence of actions, where each action represents a state transition.
      /// </summary>
      public List<Action> Actions = new List<Action>();
    }

    public 


  
  }  
}
