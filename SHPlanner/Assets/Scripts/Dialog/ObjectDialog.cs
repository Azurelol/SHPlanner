/******************************************************************************/
/*!
@file   ObjectDialog.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using Ink.Runtime;

namespace Prototype
{
  /// <summary>
  /// Manages a conversation with an individual character inside an Ink story.
  /// </summary>
  public class ObjectDialog : EventDispatcher
  {
    public class DialogStartedEvent : Stratus.Event { public ObjectDialog Host; }   
    public class DialogEndedEvent : Stratus.Event {}     

    [Tooltip("Select the master dialog this object will use")] public Dialog Master;
    [Tooltip("Select an .ink file here!")] [SerializeField] public TextAsset StoryFile;
    [Tooltip("What knot in the conversation to start on")] public string Knot;

    /// <summary>
    /// The current knot sub-section we are on.
    /// </summary>
    string Stitch;
    [HideInInspector] public Ink.Runtime.Story Story;
    
    protected override void OnInitialize()
    {
      //Trace.Script("Initializing", this);
      this.Configure();
      this.gameObject.Connect<Dialog.ContinueEvent>(this.OnContinueEvent);
      this.gameObject.Connect<Dialog.SelectChoiceEvent>(this.OnSelectChoiceEvent);
      this.gameObject.Connect<InkBinding.FindVariableValueEvent>(this.OnFindVariableValueEvent);
    }

    protected override void OnTrigger()
    {
      if (!Story.canContinue)
      {
        if (Tracing) Trace.Script("The conversation is over!");
        return;
      }

      // Start dialog
      this.StartDialog();
    }

    void OnFindVariableValueEvent(InkBinding.FindVariableValueEvent e)
    {
      switch (e.Variable.Type)
      {
        case InkBinding.Types.Integer:
          InkBinding.DispatchValueEvent<InkBinding.IntValueEvent, int>
            (this.Story, e.Variable.Name, e.Observer);
          break;
        case InkBinding.Types.Boolean:
          InkBinding.DispatchValueEvent<InkBinding.BoolValueEvent, bool>
            (this.Story, e.Variable.Name, e.Observer);
          break;
        case InkBinding.Types.Float:
          InkBinding.DispatchValueEvent<InkBinding.FloatValueEvent, float>
            (this.Story, e.Variable.Name, e.Observer);
          break;
        case InkBinding.Types.String:
          InkBinding.DispatchValueEvent<InkBinding.StringValueEvent, string>
            (this.Story, e.Variable.Name, e.Observer);
          break;
      }
    }

    void OnContinueEvent(Dialog.ContinueEvent e)
    {
      this.UpdateDialog();
    }

    void OnSelectChoiceEvent(Dialog.SelectChoiceEvent e)
    {
      this.SelectChoice(e.Choice);
      // Continue with the conversation
      this.UpdateDialog();
    }

    /// <summary>
    /// Configures the Ink.Story object.
    /// </summary>
    protected virtual void Configure()
    {
      if (this.Master)
      {
        this.Story = this.Master.Story;
        if (Tracing) Trace.Script("Accessing the master story '" + this.Master.StoryFile.name + "'", this);
        return;
      }

      // Constructs the ink story from the text file
      this.Story = new Ink.Runtime.Story(this.StoryFile.text);
      if (!Story)
      {
        Trace.Script("Failed to construct the conversation!", this);
      }

      this.Story.BindExternalFunction("Voiceover", new Action<string>(InkBinding.PlayVoiceover));
      this.Story.BindExternalFunction("Music", new Action<string>(InkBinding.PlayMusic));
      this.Story.BindExternalFunction("Global", new Func<string, object>(this.GlobalFlag));
      this.Story.BindExternalFunction("SetGlobal", new Action<string, bool>(this.SetGlobalFlag));

      // Bind all external functions
      OnBindExternalFunctions();

    }

    protected virtual void OnBindExternalFunctions()
    {
      this.Story.BindExternalFunction("Speaker", new Action<string>(this.UpdateSpeaker));
    }


    public object GlobalFlag(string varName)
    {
      // Look for the flag among the gamesession
      var retrieveFlagValueEvent = new DialogFlags.GetFlagValueEvent();
      retrieveFlagValueEvent.Name = varName;
      GameSession.Dispatch<DialogFlags.GetFlagValueEvent>(retrieveFlagValueEvent);

      // Now return the value
      return retrieveFlagValueEvent.Value as object;

    }

    public void SetGlobalFlag(string varName, bool value)
    {
      var eventObj = new DialogFlags.SetFlagValueEvent();
      eventObj.Name = varName;
      eventObj.Value = value;
      GameSession.Dispatch<DialogFlags.SetFlagValueEvent>(eventObj);
    }    

    void LookAt(string target)
    {
      Trace.Script("Now looking at: " + target);
    }
    
    /// <summary>
    /// Starts the current dialog.
    /// </summary>
    void StartDialog()
    {
      if (Tracing) Trace.Script("Dialog started!");

      // If a knot has been selected...
      if (this.Knot.Length > 0)
      {
        this.JumpToKnot(this.Knot);
      }

      // Inform the space that dialog has started
      var announceEvent = new DialogStartedEvent();
      announceEvent.Host = this;
      this.gameObject.Dispatch<DialogStartedEvent>(announceEvent);
      this.Space().Dispatch<DialogStartedEvent>(announceEvent);
      // Open the dialog window
      //this.Space().Dispatch<DialogWindow.OpenEvent>(new GenericWindow<DialogWindow>.OpenEvent());
      // Update the first line of dialog
      this.UpdateDialog();
    }
    
    /// <summary>
    /// Ends the dialog.
    /// </summary>
    void EndDialog()
    {
      if (Tracing) Trace.Script("Ending dialog!");
      var dialogEnded = new DialogEndedEvent();
      this.gameObject.Dispatch<DialogEndedEvent>(dialogEnded);
      this.Space().Dispatch<DialogEndedEvent>(dialogEnded);
    }

    /// <summary>
    /// Updates the current dialog. This will check if the conversation can
    /// be continued.If it can't ,it will then check if there are any choices
    /// to be made.If there aren't, it will end the dialog.
    /// </summary>
    void UpdateDialog()
    {
      // If there is more dialog
      if (Story.canContinue)
      {
        //if (Tracing) Trace.Script("Updating conversation!");
        var message = this.Parse(this.Story.Continue());
        if (message != null)
        {
          //Trace.Script("Message not null : " + message);
          var updateEvent = new Dialog.UpdateMessageEvent();
          updateEvent.Message = message;
          this.Space().Dispatch<Dialog.UpdateMessageEvent>(updateEvent);
        }

        return;
      }
      
      // If we are given a choice
      if (Story.currentChoices.Count > 0)
      {
        //if (Tracing) Trace.Script("Presenting dialog choices!");
        var choicesEvent = new Dialog.PresentChoicesEvent();
        for (int i = 0; i < Story.currentChoices.Count; ++i)
        {
          choicesEvent.Choices.Add(Parse(Story.currentChoices[i].text));
        }
        this.Space().Dispatch<Dialog.PresentChoicesEvent>(choicesEvent);

        return;
      }

      // If we are done with the conversation, end dialog      
      this.EndDialog();
    }
    
    /// <summary>
    /// Jumps to the specified knot in the story.
    /// </summary>
    /// <param name="knotName">The name of the knot.</param>
    void JumpToKnot(string knotName)
    {
      //Trace.Script("Jumping to the knot '" + knotName + "'", this);
      this.Story.ChoosePathString(knotName + this.Stitch);
    }

    /// <summary>
    /// Updates the current stitch
    /// </summary>
    /// <param name="stitchName"></param>
    void UpdateStitch(string stitchName)
    {
      if (stitchName.Length == 0)
        return;

      this.Stitch = "." + stitchName;
      Trace.Script("Updating stitch to '" + this.Stitch + "'", this);
    }

    /// <summary>
    /// Parses the curent line of text for encoded commands.
    /// </summary>
    /// <param name="line">The line of text.</param>
    /// <returns></returns>
    string Parse(string line)
    {
      //Trace.Script("Parsing: " + line);

      // PREFIX: Command
      if (line.Contains("/End"))
      {
        var nextKnot = line.Split()[1];
        UpdateStitch(nextKnot);
        //if (Tracing) Trace.Script("//End called", this);
        this.EndDialog();
        return null;
      }
      // PREFIX: Description
      else if (line.Contains("/Description"))
      {
        this.DisplayDescription();
        line = line.Replace("/Description", "");
        //Trace.Script(line, this);
        return line;
      }
      // PREFIX: Speaker
      var speakerSeparator = ":";
      if (line.Contains(speakerSeparator))
      {
        var split = line.Split(':');
        if (split.Length < 2)
        {
          throw new ArgumentException("Incorrect use of separator ':' in dialog!");
        }

        // Set the speaker
        var speaker = split[0];
        UpdateSpeaker(speaker);
        
        // Get the message
        var message = split[1];
        // If there's no message (for example, by just having the speaker being set
        // prior to a choice selection, continue to the next line
        if (ConsistsOfWhiteSpace(message.Trim('\n')))
        {
          this.UpdateDialog();
          return null;
        }
        //Trace.Script(message + " / " + message.Length, this);
        //Trace.Script("Empty = " + ConsistsOfWhiteSpace(message.Trim('\n')), this);   

        // Return the message
        return message;

        // Update the line, now that speaker name has been extracted
        //line = message;
      }

      //var updateEvent = new Dialog.UpdateMessageEvent();
      //updateEvent.Message = line;
      //this.Space().Dispatch<Dialog.UpdateMessageEvent>(updateEvent);
      return line;
    }

    /// <summary>
    /// Selects a choice for the current conversation.
    /// </summary>
    /// <param name="choice">A 0-indexed choice.</param>
    void SelectChoice(int choice)
    {
      this.Story.ChooseChoiceIndex(choice);
    }
    
    /// <summary>
    /// Updates the current speaker of the conversation.
    /// </summary>
    /// <param name="speakerName">The name of the speaker.</param>
    void UpdateSpeaker(string speakerName)
    {
      //if (Tracing) Trace.Script("The active speaker is now: '" + speakerName + "'");

      // Look for the speaker in the scene

      // Update the speaker. This will have the camera look at it, the dialog update the portrait, etc.
      var updateEvent = new Dialog.UpdateSpeakerEvent();
      updateEvent.Character = speakerName;      
      this.Space().Dispatch<Dialog.UpdateSpeakerEvent>(updateEvent);
    }

    /// <summary>
    /// Displays a description. This will hide the portrait and the name 
    /// on the dialog window.
    /// </summary>
    void DisplayDescription()
    {
      //Trace.Script("Displaying description!", this);
      this.Space().Dispatch<Dialog.DescriptionEvent>(new Dialog.DescriptionEvent());
    }

    public bool ConsistsOfWhiteSpace(string s)
    {
      foreach (char c in s)
      {
        if (c != ' ') return false;
      }
      return true;

    }



  }

}