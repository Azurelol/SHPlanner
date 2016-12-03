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
    public class HarvestEvent : Stratus.Event { public ObjectResource Resource; }

    public string Name = "Resource";
    public int Count = 1;

    protected override void OnSubscribe()
    {
      this.gameObject.Connect<HarvestEvent>(this.OnHarvestEvent);
    }

    void OnHarvestEvent(HarvestEvent e)
    {

    }

    protected override void OnInteractiveObjectInitialized()
    {

    }

    protected override void OnInteractiveObjectDestroyed()
    {
      
    }




  }

}