/******************************************************************************/
/*!
@file   DeliverResourceAction.cs
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
  public class DeliverResourceAction : InteractAction
  {
    public override string Description { get { return "Deliver Resource"; } }    

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      if (obj is ObjectResourceDepot)
        return true;
      return false;
    }

    protected override void OnSetup()
    {
      Preconditions.Add(new WorldState.Symbol("HasResource", true));
      Effects.Add(new WorldState.Symbol("HasDeliveredResource", true));
    }

    protected override void OnInteract()
    {
      var delivery = new ObjectResource.DropOffEvent();
      this.Target.gameObject.Dispatch<ObjectResource.DropOffEvent>(delivery);
    }
  }
}