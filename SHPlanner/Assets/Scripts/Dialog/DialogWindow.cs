/******************************************************************************/
/*!
@file   DialogWindow.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using Stratus.UI;
using UnityEngine.UI;
using System;

namespace Prototype
{
  /**************************************************************************/
  /*!
  @class DialogWindow 
  */
  /**************************************************************************/
  public class DialogWindow : Window
  {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    [Header("Configuration")]
    public WindowElement Dialog;
    public Image Portrait;
    public Text Speaker;    
    public Text Message;
    [Header("Choices")]
    public bool HideOnChoice = true;
    public GameObject ChoicePrefab;
    public int ChoiceLimit = 4;
    [Header("Transition")]
    public float TransitionDuration = 1.5f;
    public EventDispatcher OnOpen;
    public EventDispatcher OnClose;

    ObjectDialog Host;
    bool Choosing = false;


    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    protected override void OnWindowInitialize()
    {
      this.Space().Connect<ObjectDialog.DialogStartedEvent>(this.OnDialogStartedEvent);
      this.Space().Connect<ObjectDialog.DialogEndedEvent>(this.OnDialogEndedEvent);
      this.Space().Connect<Dialog.DescriptionEvent>(this.OnDialogDescriptionEvent);
      this.Space().Connect<Dialog.UpdateMessageEvent>(this.OnDialogUpdateMessageEvent);
      this.Space().Connect<Dialog.PresentChoicesEvent>(this.OnDialogPresentChoicesEvent);
      this.Space().Connect<Dialog.SelectChoiceEvent>(this.OnDialogSelectChoiceEvent);
      this.Space().Connect<Dialog.UpdateSpeakerEvent>(this.OnDialogUpdateSpeakerEvent);
      
      Tracing = false;
      
      // Subscribe to specific events
      OnDialogWindowSubscribe();
    }

    protected virtual void OnDialogWindowSubscribe()
    {
    }

    protected override void OnWindowOpen()
    {      
      // Deactivate all 'choice' links at the beginning    
      Links.Enable(false, true);      
      DeselectLink();
    }

    protected override void OnWindowClose()
    {
    }

    protected override void OnInterfaceConfirm()
    {      
      // If there's a current choice in the dialog...
      if (Choosing)
      {
        //Trace.Script("CurrentLink = " + CurrentLink.gameObject.name);        
        CurrentLink.gameObject.Dispatch<Link.ConfirmEvent>(new Link.ConfirmEvent());
        CurrentLink.gameObject.Dispatch<Link.DeselectEvent>(new Link.DeselectEvent());
      }
      // Otherwise continue the conversation
      else           
        this.Continue();
    }

    protected override void OnWindowCancel()
    {
      // Skip forward the dialog?
      if (Tracing) Trace.Script("Skipping!?");
    }

    //-------------------------------------------------------------------------
    // Events
    //-------------------------------------------------------------------------

    void OnDialogStartedEvent(ObjectDialog.DialogStartedEvent e)
    {
      //Trace.Script("Dialog started!");
      // Transition in
      if (this.OnOpen)
        this.OnOpen.gameObject.Dispatch<EventTrigger.TriggerEvent>(new EventTrigger.TriggerEvent());
      Host = e.Host;
      this.Open();
    }

    void OnDialogEndedEvent(ObjectDialog.DialogEndedEvent e)
    {
      // Transition out
      if (this.OnClose)
        this.OnClose.gameObject.Dispatch<EventTrigger.TriggerEvent>(new EventTrigger.TriggerEvent());
      var seq = Actions.Sequence(this);
      Actions.Delay(seq, this.TransitionDuration);
      Actions.Call(seq, this.Close);
      //this.Close();
    }

    void OnDialogUpdateMessageEvent(Dialog.UpdateMessageEvent e)
    {
      Message.text = e.Message;
    }

    void OnDialogUpdateSpeakerEvent(Dialog.UpdateSpeakerEvent e)
    {
      OnUpdateSpeaker(e.Character);
    }

    protected virtual void OnUpdateSpeaker(string character)
    {
      // Look for the speaker in the scene
      var findEvent = new DialogSpeaker.FindEvent();
      findEvent.Name = character;
      this.Space().Dispatch<DialogSpeaker.FindEvent>(findEvent);

      // If the speaker was found, use its portrait (and whatever else)
      if (findEvent.Speaker != null)
      {
      }
      else
      {
        Trace.Script("Speaker '" + character + "' not found!");
      }
    }


    void OnDialogDescriptionEvent(Dialog.DescriptionEvent e)
    {
      // Hide the portrait and remove the speaker name
      Portrait.enabled = false;
      Speaker.text = "";
    }

    /// <summary>
    /// Called upon when the current conversation presents choices to the player.
    /// </summary>
    /// <param name="e"></param>
    void OnDialogPresentChoicesEvent(Dialog.PresentChoicesEvent e)
    {
      if (HideOnChoice)
        Dialog.gameObject.SetActive(false);
           

      if (e.Choices.Count > ChoiceLimit)
      {
        Trace.Script(e.Choices.Count + " choices given. Only " + ChoiceLimit + " choices available!");
        foreach (var choice in e.Choices)
        {
          Trace.Script(choice);
        }
        throw new System.ArgumentException();
      }

      if (Tracing)
      {
        Trace.Script("Choices are:");

        foreach (var choice in e.Choices)
        {
          Trace.Script(choice);
        }
      }

      // For each given choice, activate a link and set its text
      for (int i = 0; i < e.Choices.Count; ++i)
      {
        var link = Links.Links[i];
        link.Text.text = e.Choices[i];
        link.Show(true);
        //link.Enabled = true;
      }

      // Allow the player to choose
      Choosing = true;
      this.SelectFirstLink();
    }

    /// <summary>
    /// Called upon when a choice is selected by the player.
    /// </summary>
    /// <param name="e"></param>
    void OnDialogSelectChoiceEvent(Dialog.SelectChoiceEvent e)
    {      
      SelectChoice(e.Choice);
    }

    /// <summary>
    /// Asks the current speaker (NPC with ObjectDialog) to continue the conversation.
    /// </summary>
    void Continue()
    {
      if (Tracing) Trace.Script("Continue!");
      Host.gameObject.Dispatch<Dialog.ContinueEvent>(new Dialog.ContinueEvent());
    }
    
    /// <summary>
    /// Called upon by one of the choice links, selecting a choice.
    /// </summary>
    /// <param name="choice">The selected choice.</param>
    public void SelectChoice(int choice)
    {
      if (Tracing) Trace.Script("Selecting '" + choice + "'");

      // Convert to 0-based indexing
      choice -= 1;

      Trace.Script("Calling from " + this.gameObject.name, this);
      //return;

      // Hide all choices
      Links.Enable(false);
      // Disable input
      DeselectLink();
      Choosing = false;
      // Enable the dialog window again
      if (HideOnChoice)
        Dialog.gameObject.SetActive(true);

      // Inform the current conversation of the choice
      var choiceEvent = new Dialog.SelectChoiceEvent();
      choiceEvent.Choice = choice;
      Host.gameObject.Dispatch<Dialog.SelectChoiceEvent>(choiceEvent);
    }

    





  }

}