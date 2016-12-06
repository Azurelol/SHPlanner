/******************************************************************************/
/*!
@file   LifetimeObject.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Prototype 
{
  public class LifetimeObject : StratusBehaviour 
  {
    public float Lifetime = 1.5f;

    void Start()
    {
      var seq = Actions.Sequence(this);
      Actions.Delay(seq, this.Lifetime);
      Actions.Call(seq, this.Destroy);
    }

    void Destroy()
    {
      Destroy(this.gameObject);
    }

  
  }  
}
