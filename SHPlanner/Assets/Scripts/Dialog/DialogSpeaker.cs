/******************************************************************************/
/*!
@file   DialogSpeaker.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Stratus;

namespace Prototype 
{
  public class DialogSpeaker : StratusBehaviour 
  {
    public class FindEvent : Stratus.Event
    {
      public string Name;
      public DialogSpeaker Speaker;
    }
    
    void Start()
    {
      this.Space().Connect<FindEvent>(this.OnFindSpeakerEvent);
    }

    void OnFindSpeakerEvent(FindEvent e)
    {
    }


  }

}
