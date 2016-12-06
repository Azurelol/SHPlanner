/******************************************************************************/
/*!
@file   ObjectBuilding.cs
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
  public class ObjectBuilding : InteractiveObject
  {
    public class BuildEvent : Stratus.Event {}

    [Tooltip("How many processed resources it requires to build this")]
    public int ProcessedResourcesNeeded;
    public float ScaleFactor = 3f;
    [HideInInspector] public int CurrentResources = 0;
    [HideInInspector] public bool IsFinished = false;
/// <summary>
    /// The current progress in the bar as a percentage (0% - 100%)
    /// </summary>
    public float Progress { get { if (CurrentResources == 0.0f) return 0.0f; return ( (float)CurrentResources / (float)ProcessedResourcesNeeded) * 100.0f; } }

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<BuildEvent>(this.OnBuildEvent);
    }

    protected override void OnInteractiveObjectDestroyed()
    {      
    }

    protected override void OnInteractiveObjectInitialized()
    {
      this.Scale();
    }

    void OnBuildEvent(BuildEvent e)
    {
      this.CurrentResources++;
      this.Scale();
      if (this.CurrentResources == this.ProcessedResourcesNeeded)
        this.IsFinished = true;
    }

    void Scale()
    {
      var scaling = ( ((float)(CurrentResources + 1)) * ScaleFactor)/ (float)ProcessedResourcesNeeded ; 
      this.transform.localScale = this.transform.localScale * scaling;
    }

    protected override void OnInteraction(Agent user)
    {
      
    }
  }
}
