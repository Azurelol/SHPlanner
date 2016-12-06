/******************************************************************************/
/*!
@file   BuyToolAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Prototype
{
  public class BuyToolAction : InteractAction
  {
    public override string Description { get { return "Buy tool"; } }
    public int ChargesGained = 3;
    
    protected override void OnSetup()
    {
      Preconditions.Apply(new WorldState.Symbol("HasMoney", true));
      //Effects.Apply(new WorldState.Symbol("HasMoney", false));
      Effects.Apply(new WorldState.Symbol("HasTool", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      var shop = obj as ObjectShop;
      if (!shop) return false;
      // Check that we have enough money to buy the tool
      if (this.Agent.Blackboard.Money.Charges >= shop.ToolCost)
      {
        return true;
      }

      return false;
    }

    protected override void OnInteract()
    {
      // This is called after the shop has run its interaction function.
      // If there's no money left on this agent
      if (this.Agent.Blackboard.Money.Empty)
      {
        Planner.Blackboard.Tool.Add(ChargesGained);
        this.Agent.gameObject.Dispatch<WorldState.ModifySymbolEvent>(new WorldState.ModifySymbolEvent(new WorldState.Symbol("HasMoney", false)));
      }
    }

    protected override void OnInteractActionReset()
    {      
    }



  }

}