/******************************************************************************/
/*!
@file   EquipmentAction.cs
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
  public abstract class EquipmentAction : Action
  {
    protected override bool RequiresRange
    {
      get
      {
        return false;
      }
    }
    bool IsEquipped = false;
    protected abstract void OnEquip();
    protected abstract void OnEquipmentBegin();
    protected abstract void OnEquipmentEnd();

    protected override void OnBegin()
    {
      this.OnEquipmentBegin();
    }

    protected override void OnEnd()
    {
      this.OnEquipmentEnd();
    }

    protected override void OnExecute()
    {
      this.OnEquip();
      this.IsEquipped = true;
    }

    protected override void OnReset()
    {
      this.IsEquipped = false;
    }    

    protected override bool OnValidate()
    {
      if (IsEquipped)
        return true;
      return false;    
    }
  }
}
