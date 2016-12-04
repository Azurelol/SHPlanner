/******************************************************************************/
/*!
@file   ObjectResource.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;

namespace Prototype
{
  public class ObjectResource : InteractiveObject
  {
    public abstract class ObjectResourceEvent : Stratus.Event{ public ObjectResource Resource; }
    public class PickUpEvent : ObjectResourceEvent { }
    public class DropOffEvent : ObjectResourceEvent { }
    public class HarvestEvent : ObjectResourceEvent { }    
    public int Count = 1;

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<HarvestEvent>(this.OnHarvestEvent);
    }

    void OnHarvestEvent(HarvestEvent e)
    {
      this.Count--;
      if (this.Count <= 0) Destroy(this.gameObject);
    }

    protected override void OnInteractiveObjectInitialized()
    {

    }

    protected override void OnInteractiveObjectDestroyed()
    {
      
    }




  }

}