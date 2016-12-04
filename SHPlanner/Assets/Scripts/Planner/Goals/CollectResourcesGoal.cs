/******************************************************************************/
/*!
@file   CollectResourcesGoal.cs
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
  public class CollectResourcesGoal : Goal
  {
    public override string Name { get { return "Collect Resources"; } }
    public override void OnSetup()
    {
      DesiredState.Add(new WorldState.Symbol("HasDeliveredResource", true));
    }
  }
}
