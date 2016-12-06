/******************************************************************************/
/*!
@file   ObjectShop.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Prototype
{
  public class ObjectShop : InteractiveObject
  {
    public Dictionary<string, int> Inventory = new Dictionary<string, int>();

    protected override void OnInteractiveObjectInitialized()
    {
      Inventory.Add("Tool", 50);
    }

    protected override void OnInteractiveObjectDestroyed()
    {

    }

    protected override void OnSubscribe()
    {

    }




  }

}