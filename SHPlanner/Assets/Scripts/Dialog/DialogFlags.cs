/******************************************************************************/
/*!
@file   DialogFlags.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using Ink.Runtime;
using System;
using System.Collections.Generic;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class DialogFlags 
  */
  /**************************************************************************/
  public class DialogFlags : MonoBehaviour
  {
    public enum FlagScope { Space, GameSession }

    public abstract class FlagValueEvent : Stratus.Event
    {
      public string Name;
      public bool Value;
    }

    public class GetFlagValueEvent : FlagValueEvent { }
    public class SetFlagValueEvent : FlagValueEvent { }

    public FlagScope Scope;
    public TextAsset Dialog;
    Ink.Runtime.Story Flags;
    //public Dictionary
    public bool Tracing = false;

    /**************************************************************************/
    /*!
    @brief  Initializes the DialogFlags.
    */
    /**************************************************************************/
    void Start()
    {
      this.Load();

      if (Scope == FlagScope.Space)
      {
        this.Space().Connect<GetFlagValueEvent>(this.OnRetrieveFlagValueEvent);
        this.Space().Connect<SetFlagValueEvent>(this.OnSetFlagValueEvent);
      }
      else if (Scope == FlagScope.GameSession)
      {
        GameSession.Connect<GetFlagValueEvent>(this.OnRetrieveFlagValueEvent);
        GameSession.Connect<SetFlagValueEvent>(this.OnSetFlagValueEvent);
        //DontDestroyOnLoad(this);
      }

    }

    void Load()
    {
      this.Flags = new Ink.Runtime.Story(this.Dialog.text);
      if (!Flags)
      {
        Trace.Script("Failed to load the story file!");
      }
      if (Tracing) Trace.Script("Loaded " + this.Dialog.name);
      //var v = Convert.ToBoolean(Flags.variablesState["IsItWorking"]);
      //Trace.Script(v);
    }

    void OnRetrieveFlagValueEvent(GetFlagValueEvent e)
    {
      if (Tracing) Trace.Script("Retrieving value of " + e.Name);
      e.Value = Convert.ToBoolean(Flags.variablesState[e.Name]); //InkBinding.GetVariableValue<bool>(this.Flags, e.VariableName);
    }

    void OnSetFlagValueEvent(SetFlagValueEvent e)
    {
      if (Tracing) Trace.Script("Setting " + e.Name + " to " + e.Value);
      Flags.variablesState[e.Name] = e.Value;
    }




  }

}