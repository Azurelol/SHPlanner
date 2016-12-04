/******************************************************************************/
/*!
@file   ProcessResourcesGoal.cs
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
  public class ProcessResourcesGoal : Goal
  {
    public override string Name { get { return "Process Resources"; } }
    public override void OnSetup()
    {
      DesiredState.Add(new WorldState.Symbol("HasProcessedResource", true));
    }
  }
}
