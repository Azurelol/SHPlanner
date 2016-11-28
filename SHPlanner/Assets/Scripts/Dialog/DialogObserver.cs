/******************************************************************************/
/*!
@file   DialogObserver.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class DialogObserver 
  */
  /**************************************************************************/
  [RequireComponent(typeof(ObjectDialog))]
  public class DialogObserver : StratusBehaviour
  {
    //--------------------------------------------------------/
    // Events
    //--------------------------------------------------------/
    public InkBinding.Variable Variable;

    //--------------------------------------------------------/
    // Methods
    //--------------------------------------------------------/

    void Awake()
    {
      // We will be 
      this.gameObject.Connect<ObjectDialog.DialogEndedEvent>(this.OnDialogEndedEvent);
      // Received when a variable of a specific type has been retrieved
      this.gameObject.Connect<InkBinding.IntValueEvent>(this.OnIntValueEvent);
      this.gameObject.Connect<InkBinding.BoolValueEvent>(this.OnBoolValueEvent);
      this.gameObject.Connect<InkBinding.FloatValueEvent>(this.OnFloatValueEvent);
      this.gameObject.Connect<InkBinding.StringValueEvent>(this.OnStringValueEvent);
    }
    
    void OnDialogEndedEvent(ObjectDialog.DialogEndedEvent e)
    {
      this.FindVariableValue();
    }

    void OnIntValueEvent(InkBinding.IntValueEvent e)
    {
      Trace.Script(Variable.Name + " = " + e.Value);
    }

    void OnBoolValueEvent(InkBinding.BoolValueEvent e)
    {
      Trace.Script(Variable.Name + " = " + e.Value);
    }

    void OnFloatValueEvent(InkBinding.FloatValueEvent e)
    {
      Trace.Script(Variable.Name + " = " + e.Value);
    }

    void OnStringValueEvent(InkBinding.StringValueEvent e)
    {
      Trace.Script(Variable.Name + " = " + e.Value);
    }      
    
        
    void FindVariableValue()
    {
      var findValueEvent = new InkBinding.FindVariableValueEvent();
      findValueEvent.Observer = this.gameObject;
      findValueEvent.Variable = this.Variable;
      this.gameObject.Dispatch<InkBinding.FindVariableValueEvent>(findValueEvent);      
    }



  }

}