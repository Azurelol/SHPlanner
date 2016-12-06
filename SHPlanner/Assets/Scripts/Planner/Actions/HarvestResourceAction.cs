/******************************************************************************/
/*!
@file   HarvestAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;

namespace Prototype
{
  public class HarvestResourceAction : InteractAction
  {
    public override string Description { get { return "Harvest Resource"; } }
    
    protected override void OnSetup()
    {
      Preconditions.Apply(new WorldState.Symbol("EquippedTool", true));
      Effects.Apply(new WorldState.Symbol("EquippedTool", false));
      Effects.Apply(new WorldState.Symbol("HasResource", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      if (obj is ObjectResource)
        return true;
      return false;
    }

    protected override void OnInteract()
    {
      this.Target.gameObject.Dispatch<ObjectResource.HarvestEvent>(new ObjectResource.HarvestEvent());
    }

    protected override void OnInteractActionReset()
    {      
    }
  }
}
