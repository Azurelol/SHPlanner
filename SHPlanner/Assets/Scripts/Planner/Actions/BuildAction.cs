/******************************************************************************/
/*!
@file   BuildAction.cs
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
  public class BuildAction : InteractAction
  {
    public override string Description { get { return "Construct a building"; } }
    
    protected override void OnSetup()
    {
      Preconditions.Apply(new WorldState.Symbol("HasProcessedResource", true));
      Effects.Apply(new WorldState.Symbol("HasProcessedResource", false));
      Effects.Apply(new WorldState.Symbol("HasBuilt", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      var building = obj as ObjectBuilding;
      if (!building) return false;

      if (!building.IsFinished) return true;

      return false;
    }

    protected override void OnInteract()
    {
      this.Target.gameObject.Dispatch<ObjectBuilding.BuildEvent>(new ObjectBuilding.BuildEvent());
    }

    protected override void OnInteractActionReset()
    {
      //Effects.Apply(new WorldState.Symbol("HasBuilt", false));
    }
  }
}
