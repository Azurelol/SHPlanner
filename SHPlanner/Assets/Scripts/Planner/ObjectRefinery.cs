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
  public class ObjectRefinery : InteractiveObject
  {
    public class DeliverResourceEvent {}

    protected override void OnSubscribe()
    {
      
    }
    
    void OnDeliverResourceEvent(DeliverResourceEvent e)
    {
      Trace.Script("Received resource!", this);
    }

    protected override void OnInteractiveObjectInitialized()
    {
      
    }

    protected override void OnInteractiveObjectDestroyed()
    {
      
    }




  }
}
