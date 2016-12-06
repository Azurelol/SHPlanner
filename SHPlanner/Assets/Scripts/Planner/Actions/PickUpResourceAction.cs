/******************************************************************************/
/*!
@file   PickUpResourceAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;
using Stratus;

namespace Prototype
{
  public class PickUpResourceAction : InteractAction
  {
    public override string Description { get { return "Pick up resource"; } }

    protected override void OnSetup()
    {
      Effects.Apply("HasResource", true);
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      var depot = obj as ObjectResourceDepot;
      if (depot == null) return false;

      // If there's resources in the refinery, it is valid
      if (depot.Resources > 0) return true;
      //Trace.Script("No resources available to pick up!", this);
      return false;
    }

    protected override void OnInteract()
    {
      this.Target.gameObject.Dispatch<ObjectResource.PickUpEvent>(new ObjectResource.PickUpEvent());
    }

    protected override void OnInteractActionReset()
    {
      //Effects.Apply("HasResource", false);
    }
  }
}
