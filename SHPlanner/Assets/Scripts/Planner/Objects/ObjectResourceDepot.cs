/******************************************************************************/
/*!
@file   ObjectRefinery.cs
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
  public class ObjectResourceDepot : InteractiveObject
  {

    public int Resources = 0;

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<ObjectResource.DropOffEvent>(this.OnDropOffResourceEvent);
      this.gameObject.Connect<ObjectResource.PickUpEvent>(this.OnPickUpResourceEvent);
    }
    
    void OnPickUpResourceEvent(ObjectResource.PickUpEvent e)
    {
      //Trace.Script("Picked up resource!", this);
      Resources--;
    }

    void OnDropOffResourceEvent(ObjectResource.DropOffEvent e)
    {
      //Trace.Script("Received resource!", this);
      Resources++;
    }

    protected override void OnInteractiveObjectInitialized()
    {
      
    }

    protected override void OnInteractiveObjectDestroyed()
    {
      
    }




  }
}
