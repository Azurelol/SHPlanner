/******************************************************************************/
/*!
@file   HarvestAction.cs
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
  public class HarvestResourceAction : InteractAction
  {
    public override string Name { get { return "Harvest Resource"; } }
    protected override void OnSetup()
    {
      Preconditions.Add(new WorldState.Symbol("HasTool", true));
      Effects.Add(new WorldState.Symbol("HasCrystal", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      if (obj is ObjectResource)
        return true;
      return false;
    }
  }
}
