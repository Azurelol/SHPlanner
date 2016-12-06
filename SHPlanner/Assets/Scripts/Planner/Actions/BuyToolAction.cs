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

namespace Prototype
{
  public class BuyToolAction : InteractAction
  {
    public override string Description { get { return "Buy tool"; } }
    
    protected override void OnSetup()
    {
      Preconditions.Apply(new WorldState.Symbol("HasMoney", true));
      Effects.Apply(new WorldState.Symbol("HasMoney", false));
      Effects.Apply(new WorldState.Symbol("HasTool", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      var shop = obj as ObjectShop;
      if (!shop) return false;

      return true;
    }

    protected override void OnInteract()
    {
            
    }

    protected override void OnInteractActionReset()
    {      
    }



  }

}