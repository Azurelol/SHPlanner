/******************************************************************************/
/*!
@file   ConstructBuildingsGoal.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype 
{
  public class ConstructBuildingsGoal : Goal
  {
    public override string Name { get { return "Construct Buildings"; } }

    public override void OnFinished(Planner planner)
    {
      planner.CurrentState.Apply("HasBuilt", false);
    }

    public override void OnSetup()
    {
      DesiredState.Apply(new WorldState.Symbol("HasBuilt", true));
    }
  }
}
