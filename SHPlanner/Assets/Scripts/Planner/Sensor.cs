/******************************************************************************/
/*!
@file   Sensor.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using Stratus;
using System.Collections.Generic;

namespace Prototype
{
  public class Sensor : StratusBehaviour
  {
    class WorkingMemory
    {
      public List<InteractiveObject> InteractivesInRange = new List<InteractiveObject>();
    }
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    WorkingMemory Memory = new WorkingMemory();

    /// <summary>
    /// All interactive objects available found by this sensor
    /// </summary>
    public List<InteractiveObject> Interactives { get { return Memory.InteractivesInRange; } }

    /// <summary>
    /// The range at which objects will be considered.
    /// </summary>
    public float ConsiderationRange = 50.0f;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/

    /// <summary>
    /// Scans the world for objects we can interact with.
    /// </summary>
    /// <returns></returns>
    public void Scan()
    {
      Interactives.Clear();

      var scan = Physics.OverlapSphere(transform.position, this.ConsiderationRange);
      foreach (var hit in scan)
      {
        var interactive = hit.GetComponent<InteractiveObject>();
        if (interactive != null)
        {
          //Trace.Script("Found " + interactive.Description, this);
          Interactives.Add(interactive);
        }

      }
    }


  }

}