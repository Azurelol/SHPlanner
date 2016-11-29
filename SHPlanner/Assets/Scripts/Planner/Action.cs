/******************************************************************************/
/*!
@file   PlanAction.cs
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
  [CreateAssetMenu(fileName = "Action", menuName = "Planner/Action")]
  /// <summary>
  /// An action is a single, atomic step within a plan that makes a character do something.
  /// </summary>
  public class Action : ScriptableObject
  {

    public bool Execute()
    {
      return false;
    }

    public void Start()
    {

    }

    public void End()
    {

    }

    public string Name;
    public string Description;
    public List<Plan.State> Preconditions = new List<Plan.State>();
    public List<Plan.State> Effects = new List<Plan.State>();
    public float Cost;
  }
}
