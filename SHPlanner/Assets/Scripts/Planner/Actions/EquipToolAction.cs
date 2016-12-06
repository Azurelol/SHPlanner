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
    //[Tooltip("How many times the tool can be used before it breaks")]
    //public int Charges = 3;
    //int RemainingCharges;

    public override string Description { get { return "Equip tool"; } }

    protected override void OnEquip()
    {
      //Trace.Script("Tool equipped!", this);
      Planner.Blackboard.Tool.Consume();
      if (Planner.Blackboard.Tool.Charges <= 0)
      {
        //Trace.Script("All charges consumed. The tool broke!", this);
        this.Planner.gameObject.Dispatch<WorldState.ModifySymbolEvent>(new WorldState.ModifySymbolEvent(new WorldState.Symbol("HasTool", false)));
        //Effects.Apply("HasTool", false);
      }
      else
      {
        //Trace.Script("Charges remain on the tool!", this);
      }
    }

    protected override void OnEquipmentBegin()
    {
      //Trace.Script("Resetting charges");
      //RemainingCharges = Charges;
    }

    protected override void OnEquipmentEnd()
    {
      
    }

    protected override void OnSetup()
    {
      Preconditions.Apply("HasTool", true);
      Effects.Apply("EquippedTool", true);
    }
    
  }
}
