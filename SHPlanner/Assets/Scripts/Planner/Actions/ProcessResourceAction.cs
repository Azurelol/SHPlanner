/******************************************************************************/
/*!
@file   ProcessResourceAction.cs
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
  public class ProcessResourceAction : InteractAction
  {
    public override string Description { get { return "Process Resource"; } }

    protected override void OnSetup()
    {
      Preconditions.Add(new WorldState.Symbol("HasResource", true));
      Effects.Add(new WorldState.Symbol("HasProcessedResource", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      if (obj is ObjectResourceRefinery)
        return true;
      return false;
    }

    protected override void OnInteract()
    {
      
    }


  }
}
