/******************************************************************************/
/*!
@file   VisualNovelDialogWindow.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Prototype 
{
  public class StoryWindow : DialogWindow 
  {
    [Header("Story Configuration")]
    public StoryGlossary Glossary;
    public HorizontalLayoutGroup CharacterCutouts;
    public Image Background;
    public GameObject CutoutPrefab;

    Dictionary<string, StoryCharacter> Characters = new Dictionary<string, StoryCharacter>();
    StoryCharacter CurrentSpeaker;

    protected override void OnDialogWindowSubscribe()
    {
      this.Space().Connect<Dialog.CharacterEnterEvent>(this.OnCharacterEnterEvent);
      this.Space().Connect<Dialog.CharacterExitEvent>(this.OnCharacterExitEvent);
      this.Space().Connect<Dialog.HighlightCharacterEvent>(this.OnCharacterHighlightEvent);
      this.Space().Connect<Dialog.ChangeBackgroundEvent>(this.OnChangeBackgroundEvent);
    }

    protected override void OnUpdateSpeaker(string character)
    {
      // If there was a previous speaker...
      if (CurrentSpeaker)
      {
        //Trace.Script("Changing state of previous speaker to default!", this);
        CurrentSpeaker.ChangeState(StoryCharacter.State.Default);
      }

      // Look for the character in the glossary
      CurrentSpeaker = Characters[character];
      //Trace.Script("Changing state of new speaker to talking!", this);
      CurrentSpeaker.ChangeState(StoryCharacter.State.Talking);
      Speaker.text = character;
    }

    void OnCharacterEnterEvent(Dialog.CharacterEnterEvent e)
    {
      Trace.Script(e.CharacterName + " has entered the conversation!", this);
      // Find the character's data in the glossary
      var characterInfo = Glossary.FindCharacter(e.CharacterName);
      // Instantiate a cutout for this character and configure it
      var go = Instantiate(CutoutPrefab);
      go.name = characterInfo.Name;
      var character = go.GetComponent<StoryCharacter>();
      character.Character = characterInfo;
      character.Configure();      
      go.GetComponent<RectTransform>().SetParent(CharacterCutouts.transform, false);      

      // Save the reference
      Characters.Add(e.CharacterName, character);
    }

    void OnCharacterExitEvent(Dialog.CharacterExitEvent e)
    {
      Trace.Script(e.CharacterName + " has exited the conversation!", this);
      RemoveCharacter(e.CharacterName);

      // Hide the character
      //Characters[e.CharacterName].gameObject.SetActive(false);
    }

    void RemoveCharacter(string name)
    {
      // Remove the character cutout
      var character = Characters[name];
      Characters.Remove(name);
      Destroy(character.gameObject);
    }
    
    void OnCharacterHighlightEvent(Dialog.HighlightCharacterEvent e)
    {

    }

    void OnChangeBackgroundEvent(Dialog.ChangeBackgroundEvent e)
    {
      Trace.Script(e.Background + " is now the background!", this);
      this.Background.sprite = Glossary.FindBackground(e.Background).Image;
    }

  }  
}
