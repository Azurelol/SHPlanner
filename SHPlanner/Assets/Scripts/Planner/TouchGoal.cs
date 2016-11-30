using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Prototype
{
  [Serializable]
  public class TouchGoal : Goal
  {
    public override string Name { get { return "Touch"; } }
    public override string Description { get { return "Touches every object within range"; } }
  }
}