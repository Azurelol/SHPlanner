/******************************************************************************/
/*!
@file   InkBinding.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@brief  Ink Reference: https://github.com/inkle/ink        
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;

namespace Prototype
{
  /// <summary>
  /// InkBinding Provides utility types and functions for interfacing with Ink.
  /// </summary>
  public class InkBinding
  {
    /// <summary>
    /// The Ink language provides 4 common value types
    /// </summary>
    public enum Types { Integer, Boolean, String, Float }
    
    [Serializable]
    public class Variable
    {
      public Types Type;
      public string Name;
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class VariableValueEvent : Stratus.Event
    {
      public string Name;
    }
    public abstract class VariableValueEvent<T> : VariableValueEvent
    {
      public T Value;
    }
    public class IntValueEvent : VariableValueEvent<int> {}
    public class BoolValueEvent : VariableValueEvent<bool> {}
    public class FloatValueEvent : VariableValueEvent<float> {}
    public class StringValueEvent : VariableValueEvent<string> {}
    //--------------------------------------------------------/
    public class FindVariableValueEvent : Stratus.Event
    {
      public GameObject Observer;
      public InkBinding.Variable Variable;
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    static public void ObserveVariable<T>(Ink.Runtime.Story conversation, string varName, Action<T> callback)
    {
      //Story.VariableObserver observer = callback;      
      //conversation.ObserveVariable(varName, observer);
    }

    /**************************************************************************/
    /*!
    @brief Retrieves the value of a given variable inside an Ink.Story
    @param story The Ink story object.
    @param varName The name of the variable.
    @return The value of the variable. Otherwise it will throw (lol)
    */
    /**************************************************************************/
    static public ValueType GetVariableValue<ValueType>(Ink.Runtime.Story story, string varName)
    {
      // Edge case
      if (typeof(ValueType) == typeof(bool))
      {
        var value = (int)story.variablesState[varName];
        return (ValueType)System.Convert.ChangeType(value, typeof(ValueType));
      }

      return (ValueType)story.variablesState[varName];
    }

    /**************************************************************************/
    /*!
    @brief Dispatches an event containing the value of a given variable.
    @param story The Ink story object.
    @param varName The name of the variable.
    @param target The recipient of the event.
    */
    /**************************************************************************/
    static public void DispatchValueEvent<EventType, ValueType>(Ink.Runtime.Story story, string varName, GameObject target)
      where EventType : InkBinding.VariableValueEvent<ValueType>, new()
    {
      EventType valueEvent = new EventType();
      valueEvent.Name = varName;
      valueEvent.Value = GetVariableValue<ValueType>(story, varName);      
      target.Dispatch<EventType>(valueEvent);
    }

    public static void PlayVoiceover(string clipName)
    {
      Trace.Script("Playing voiceover: '" + clipName + "'");
      // Dispatch the event
      var voiceoverEvent = new Dialog.PlayVoiceoverEvent();
      voiceoverEvent.Clip = clipName;
    }

    public static void PlayMusic(string trackName)
    {
    }

    public static bool LocalFlag(Stratus.Space space, string varName)
    {
      // Look for the flag among the space
      var retrieveFlagValueEvent = new DialogFlags.GetFlagValueEvent();
      retrieveFlagValueEvent.Name = varName;
      space.Dispatch<DialogFlags.GetFlagValueEvent>(retrieveFlagValueEvent);

      // Now return the value
      return retrieveFlagValueEvent.Value;
    }

    public static void SetLocalFlag(Stratus.Space space, string varName, bool value)
    {
      var eventObj = new DialogFlags.SetFlagValueEvent();
      eventObj.Name = varName;
      eventObj.Value = value;
      space.Dispatch<DialogFlags.SetFlagValueEvent>(eventObj);
    }

    public static bool GlobalFlag(string varName)
    {
      // Look for the flag among the gamesession
      var retrieveFlagValueEvent = new DialogFlags.GetFlagValueEvent();
      retrieveFlagValueEvent.Name = varName;
      GameSession.Dispatch<DialogFlags.GetFlagValueEvent>(retrieveFlagValueEvent);

      // Now return the value
      return retrieveFlagValueEvent.Value;

    }

    public static void SetGlobalFlag(string varName, bool value)
    {
      var eventObj = new DialogFlags.SetFlagValueEvent();
      eventObj.Name = varName;
      eventObj.Value = value;
      GameSession.Dispatch<DialogFlags.SetFlagValueEvent>(eventObj);
    }

    //public static void BindAllExternalFunctions(Ink.Runtime.Story story, Stratus.Space space)
    //{
    //  // Bind all external functions
    //  story.BindExternalFunction("Voiceover", new Action<string>(InkBinding.PlayVoiceover));
    //  story.BindExternalFunction("Music", new Action<string>(InkBinding.PlayMusic));
    //  story.BindExternalFunction("Speaker", new Action<string>(this.SetSpeaker));
    //  story.BindExternalFunction("Global", new Func<string, object>(this.GlobalFlag));
    //  story.BindExternalFunction("SetGlobal", new Action<string, bool>(this.SetGlobalFlag));
    //}




  }

}