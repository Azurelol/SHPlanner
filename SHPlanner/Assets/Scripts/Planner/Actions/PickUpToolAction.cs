/******************************************************************************/
/*!
@file   PickUpTool.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;
using UnityEngine;
using Stratus;

namespace Prototype
{
  public class PickUpToolAction : InteractAction
  {
    [Tooltip("How many charges are gained when picking up a tool")]
    public int ChargesGained = 2;
    public override string Description { get { return "Pick up tool"; } }

    protected override void OnSetup()
    {
      Effects.Apply(new WorldState.Symbol("HasTool", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      if (obj is ObjectTool)
        return true;
      return false;
    }

    protected override void OnInteract()
    {
      Planner.Blackboard.Tool.Add(ChargesGained);
    }

    protected override void OnInteractActionReset()
    {
      //Effects.Apply(new WorldState.Symbol("HasTool", false));
    }
  }

}
