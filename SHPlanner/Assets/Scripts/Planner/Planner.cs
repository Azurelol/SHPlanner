/******************************************************************************/
/*!
@file   Planner.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype 
{
  public class Planner : StratusBehaviour 
  {
    public class Action
    {
      public abstract class Precondition
      {
        public abstract bool Check();
      }

      public abstract class Effect
      {

      }

    }
  
  }  
}
