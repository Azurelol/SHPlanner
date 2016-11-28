/******************************************************************************/
/*!
@file   VisualNovelEvent.cs
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
  public class StoryEvent : EventDispatcher
  {
    public string Knot;

    protected override void OnInitialize()
    {
      
    }

    protected override void OnTrigger()
    {
      var e = new StoryObject.ReadEvent();
      e.Knot = this.Knot;
      this.Space().Dispatch<StoryObject.ReadEvent>(e);
    }
  }
}
