/******************************************************************************/
/*!
@file   PickUpResourceAction.cs
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
  public class PickUpResourceAction : InteractAction
  {
    public override string Description { get { return "Pick up resource"; } }

    protected override void OnSetup()
    {
      Effects.Add(new WorldState.Symbol("HasResource", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      var refinery = obj as ObjectResourceDepot;
      if (refinery == null) return false;
      // If there's resources in the refinery, it is valid
      if (refinery.Resources > 0) return true;
      Trace.Script("No resources available to pick up!", this);
      return false;
    }

    protected override void OnInteract()
    {
      this.Target.gameObject.Dispatch<ObjectResource.PickUpEvent>(new ObjectResource.PickUpEvent());
    }

  }
}
