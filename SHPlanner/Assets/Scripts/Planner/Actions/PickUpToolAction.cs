/******************************************************************************/
/*!
@file   PickUpTool.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;

namespace Prototype
{
  public class PickUpToolAction : InteractAction
  {
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
      
    }

    protected override void OnInteractActionReset()
    {
      //Effects.Apply(new WorldState.Symbol("HasTool", false));
    }
  }

}
