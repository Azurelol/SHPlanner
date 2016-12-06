/******************************************************************************/
/*!
@file   Blackboard.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;

namespace Prototype
{  
  public struct Consumable
  {
    public string Name;
    int CurrentCharges;
    public bool Empty { get { return CurrentCharges == 0; } }
    public int Charges { get { return CurrentCharges; } }
    public Consumable(string name)
    {
      Name = name;
      CurrentCharges = 0;
    }
    public void Add(int charges)
    {
      CurrentCharges += charges;
      Print();
    }
    public void Consume(int amount = 1)
    {
      if (CurrentCharges == 0)
        throw new System.Exception("Attempted to consume when there's no charges left!");
      CurrentCharges = CurrentCharges - amount;
      Print();      
    }
    void Print()
    {
      //Trace.Script(Name + "." + "Charges = " + Charges);
    }
    
  }


  [System.Serializable]
  public class Blackboard
  {
    public Consumable Money = new Consumable("Money");
    public Consumable Tool = new Consumable("Tool");
    [Tooltip("How much money this agent earns on a job done!")]
    public int Salary = 10;

  }

}