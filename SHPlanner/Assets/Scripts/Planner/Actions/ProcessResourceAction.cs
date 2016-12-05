/******************************************************************************/
/*!
@file   ProcessResourceAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;

namespace Prototype
{
  public class ProcessResourceAction : InteractAction
  {
    public override string Description { get { return "Process Resource"; } }

    protected override void OnSetup()
    {
      Preconditions.Apply(new WorldState.Symbol("HasResource", true));
      Effects.Apply(new WorldState.Symbol("HasResource", false));
      Effects.Apply(new WorldState.Symbol("HasProcessedResource", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      if (obj is ObjectResourceRefinery)
        return true;
      return false;
    }

    protected override void OnInteract()
    {
      //this.Target.gameObject.Dispatch<ObjectResource.DropOffEvent>(new ObjectResource.DropOffEvent());
    }

    protected override void OnInteractActionReset()
    {
      //Effects.Apply("HasProcessedResource", false);
    }
  }
}
