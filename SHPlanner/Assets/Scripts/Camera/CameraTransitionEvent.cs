/******************************************************************************/
/*!
@file   CameraTransitionEvent.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Stratus;

namespace Prototype
{
  public class CameraTransitionEvent : EventDispatcher
  {
    public CameraTransition.TransitionEvent Transition = new CameraTransition.TransitionEvent();

    protected override void OnInitialize()
    {
      
    }

    protected override void OnTrigger()
    {
      //Trace.Script("heyo");
      this.Space().Dispatch<CameraTransition.TransitionEvent>(this.Transition);
    }

    public static void Dispatch(Stratus.Space space, float speed, float duration)
    {

    }

  }

}