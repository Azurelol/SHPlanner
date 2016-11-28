/******************************************************************************/
/*!
@file   Dialog.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class Dialog 
  */
  /**************************************************************************/
  public partial class Dialog : StratusBehaviour
  {
    // -----------------
    // Conversation Flow
    // -----------------
    public abstract class CharacterEvent : Stratus.Event { public string CharacterName; }
    public class CharacterEnterEvent : CharacterEvent { }
    public class CharacterExitEvent : CharacterEvent { }

    public class UpdateMessageEvent : Stratus.Event { public string Message; }
    public class UpdateSpeakerEvent : Stratus.Event { public string Character; }
    public class DescriptionEvent : Stratus.Event { public string Description; }
    public class ContinueEvent : Stratus.Event { }

    // --------
    // Choices
    // --------
    public class PresentChoicesEvent : Stratus.Event
    {
      public List<string> Choices = new List<string>();
    }
    public class SelectChoiceEvent : Stratus.Event
    {
      public int Choice;
    }
    // --------
    // Effects
    // --------
    public class PlayVoiceoverEvent : Stratus.Event { public string Clip; }
    public class ChangeBackgroundEvent : Stratus.Event { public string Background; }
    public class HighlightCharacterEvent : CharacterEvent { }

    //public class StartedEvent : Stratus.Event {}
    //public class EndedEvent : Stratus.Event {}

    public TextAsset StoryFile;
    public Ink.Runtime.Story Story_;
    public Ink.Runtime.Story Story
    {
      get
      {
        /// If the story hasn't loaded...
        if (Story_ == null)
          Load();
        return Story_;
      }
    }
    public bool Tracing = false;
    bool Loaded = false;
    
    void Awake()
    {
      if (this.Loaded)
        return;

      this.Load();
    }

    void Load()
    {
      // Load the story file
      this.Story_ = new Ink.Runtime.Story(this.StoryFile.text);
      if (!Story)
      {
        Trace.Script("Failed to construct the conversation!", this);
      }

      // Bind all external functions
      Story.BindExternalFunction("Voiceover", new Action<string>(InkBinding.PlayVoiceover));
      Story.BindExternalFunction("Music", new Action<string>(InkBinding.PlayMusic));
      Story.BindExternalFunction("Speaker", new Action<string>(this.UpdateSpeaker));
      //Story.BindExternalFunction("Global", new Func<string, object>(this.GlobalFlag));
      //Story.BindExternalFunction("SetGlobal", new Action<string, bool>(this.SetGlobalFlag));

      this.Loaded = true;
    }

    /**************************************************************************/
    /*!
    @brief Updates the current speaker of the conversation.
    @param choice A 0-indexed choice.
    */
    /**************************************************************************/
    void UpdateSpeaker(string character)
    {
      if (Tracing) Trace.Script("The active speaker is now: '" + character + "'");
      var updateEvent = new Dialog.UpdateSpeakerEvent();
      updateEvent.Character = character;
      this.Space().Dispatch<Dialog.UpdateSpeakerEvent>(updateEvent);

    }


  }



}