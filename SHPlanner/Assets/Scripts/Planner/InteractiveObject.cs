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
    public abstract class InteractionEvent : Stratus.Event { public Agent Source; }
    public class InteractionStartedEvent : InteractionEvent { }
    public class InteractionEndedEvent : InteractionEvent { }
    
    public string Description = "Resource";
    /// <summary>
    /// Whether this object is being currently used
    /// </summary>
    public Agent CurrentUser;
    /// <summary>
    /// A SFX to be played when this object is interacted with
    /// </summary>
    public GameObject InteractionEffect;

    protected abstract void OnInteractiveObjectInitialized();
    protected abstract void OnInteractiveObjectDestroyed();
    protected abstract void OnInteraction(Agent user);
    protected abstract void OnSubscribe();

    void Start()
    {
      this.OnInteractiveObjectInitialized();
      this.Subscribe();
    }

    void Subscribe()
    {
      this.gameObject.Connect<InteractionStartedEvent>(this.OnInteractionStartedEvent);
      this.gameObject.Connect<InteractionEndedEvent>(this.OnInteractionEndedEvent);
      this.OnSubscribe();
    }

    void OnInteractionStartedEvent(InteractionStartedEvent e)
    {
      this.CurrentUser = e.Source;
      this.OnInteraction(e.Source);
    }

    void OnInteractionEndedEvent(InteractionEndedEvent e)
    {
      this.CurrentUser = null;
      this.SpawnParticles();
    }

    void SpawnParticles()
    {
      if (!InteractionEffect)
        return;

      var particles = GameObject.Instantiate(this.InteractionEffect, transform, false) as GameObject;
      particles.AddComponent<LifetimeObject>();

      //var seq = Actions.Sequence(this);
      //Actions.Delay()
    }

    public IEnumerator SpawnParticlesRoutine()
    {
      yield return new WaitForSeconds(1.5f);     

    }

    /// <summary>
    /// Determines whether this object can be used by that agent.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public bool CanBeUsed(Agent agent)
    {
      if (!this.CurrentUser) return true;
      return agent == this.CurrentUser;
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