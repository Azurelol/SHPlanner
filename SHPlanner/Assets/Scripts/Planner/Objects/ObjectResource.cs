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
    public class DestroyedEvent : ObjectResourceEvent { }
    public int Count = 1;
    int InitialCount;

    protected override void OnInteractiveObjectInitialized()
    {
      this.InitialCount = Count;
    }

    protected override void OnInteractiveObjectDestroyed()
    {
      // Inform the space that this object has been destroyed
      var e = new DestroyedEvent();
      e.Resource = this;
      this.Space().Dispatch<DestroyedEvent>(e);
    }

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<HarvestEvent>(this.OnHarvestEvent);
    }

    protected override void OnInteraction(Agent user)
    {      
    }

    void OnHarvestEvent(HarvestEvent e)
    {
      this.Count--;
      // Scale it down depending on its count
      var scaling = (float)Count / (float)InitialCount;
      this.transform.localScale = this.transform.localScale * scaling;

      //if (this.Count <= 0) Destroy(this.gameObject);
    }


  }

}