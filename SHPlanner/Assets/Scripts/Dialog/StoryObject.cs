/******************************************************************************/
/*!
@file   VisualNovelDialog.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using Ink.Runtime;

namespace Prototype 
{
  public class StoryObject : ObjectDialog
  {    
    public class ReadEvent : Stratus.Event { public string Knot; }

    public StoryGlossary Glossary;

    protected override void OnBindExternalFunctions()
    {
      Story.BindExternalFunction("Enter", new Action<string>(this.CharacterEnter));
      Story.BindExternalFunction("Exit", new Action<string>(this.CharacterExit));
      Story.BindExternalFunction("Highlight", new Action<string>(this.CharacterHighlight));
      Story.BindExternalFunction("Background", new Action<string>(this.ChangeBackground));
    }

    void OnReadEvent(ReadEvent e)
    {
      this.Knot = e.Knot;
      this.Trigger();
    }

    void ChangeBackground(string background)
    {
      var e = new Dialog.ChangeBackgroundEvent();
      e.Background = background;
      this.Space().Dispatch<Dialog.ChangeBackgroundEvent>(e);
    }

    void CharacterEnter(string characterName)
    {
      var e = new Dialog.CharacterEnterEvent();
      e.CharacterName = characterName;
      this.Space().Dispatch<Dialog.CharacterEnterEvent>(e);
    }

    void CharacterExit(string characterName)
    {
      var e = new Dialog.CharacterExitEvent();
      e.CharacterName = characterName;
      this.Space().Dispatch<Dialog.CharacterExitEvent>(e);
    }

    void CharacterHighlight(string characterName)
    {
      var e = new Dialog.HighlightCharacterEvent();
      e.CharacterName = characterName;
      this.Space().Dispatch<Dialog.HighlightCharacterEvent>(e);
    }

  }  
}
