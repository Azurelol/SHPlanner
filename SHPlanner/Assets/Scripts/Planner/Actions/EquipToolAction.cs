/******************************************************************************/
/*!
@file   UseToolAction.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using System;
using UnityEngine;
using Stratus;

namespace Prototype
{
  public class EquipToolAction : EquipmentAction
  {
    [Tooltip("How many times the tool can be used before it breaks")]
    public int Charges = 3;
    int RemainingCharges;

    public override string Description { get { return "Equip tool"; } }

    protected override void OnEquip()
    {
      RemainingCharges--;
      Trace.Script("Tool equipped. Charges left =" + this.RemainingCharges, this);

    }

    protected override void OnEquipmentBegin()
    {
      RemainingCharges = Charges;
    }

    protected override void OnEquipmentEnd()
    {
      
    }

    protected override void OnSetup()
    {
      Preconditions.Apply(new WorldState.Symbol("HasTool", true));
      Effects.Apply(new WorldState.Symbol("EquippedTool", true));
    }
    
  }
}
