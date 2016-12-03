/******************************************************************************/
/*!
@file   InteractiveObject.cs
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
  public abstract class InteractiveObject : StratusBehaviour
  {    
    public class InteractEvent : Stratus.Event {}
    protected abstract void OnInteractiveObjectInitialized();
    protected abstract void OnInteractiveObjectDestroyed();
    protected abstract void OnSubscribe();

    void Start()
    {
      this.OnInteractiveObjectInitialized();
      this.Subscribe();
    }

    void Subscribe()
    {
      this.OnSubscribe();
    }

    protected override void OnDestroyed()
    {
      this.OnInteractiveObjectDestroyed();
    }

    public void Interact()
    {

    }


  }

}