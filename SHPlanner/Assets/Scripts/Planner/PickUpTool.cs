/******************************************************************************/
/*!
@file   PickUpTool.cs
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
  public class PickUpTool : InteractAction
  {
    public override string Name { get { return "Pick up tool"; } }

    protected override void OnSetup()
    {
      Effects.Add(new WorldState.Symbol("HasTool", true));
    }

    protected override bool OnValidateTarget(InteractiveObject obj)
    {
      throw new NotImplementedException();
    }
  }
}
