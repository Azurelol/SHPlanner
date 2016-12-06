/******************************************************************************/
/*!
@file   ObjectResourceRefinery.cs
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
  public class ObjectResourceRefinery : InteractiveObject
  {
    public int Money = 0;

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<ObjectResource.DropOffEvent>(this.OnDropOffResourceEvent);
      this.gameObject.Connect<ObjectResource.PickUpEvent>(this.OnPickUpResourceEvent);
    }

    void OnPickUpResourceEvent(ObjectResource.PickUpEvent e)
    {
    }

    void OnDropOffResourceEvent(ObjectResource.DropOffEvent e)
    {
    }

    protected override void OnInteractiveObjectInitialized()
    {

    }

    protected override void OnInteractiveObjectDestroyed()
    {

    }

    protected override void OnInteraction(Agent user)
    {      
    }

  }
}
