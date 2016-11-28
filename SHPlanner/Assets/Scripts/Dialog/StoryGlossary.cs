/******************************************************************************/
/*!
@file   VisualNovelCharacters.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using System.Collections.Generic;

namespace Prototype 
{
  [CreateAssetMenu(fileName = "StoryGlossary", menuName = "Prototype/StoryGlossary")]
  public class StoryGlossary : ScriptableObject 
  {
    [Serializable]
    public class Character
    {
      public string Name;
      public Sprite Default;
      public Sprite Happy;
      public Sprite Angry;
      public Sprite Sad;
    }

    [Serializable]
    public class Background
    {
      public string Name;
      public Sprite Image;
    }

    public List<Character> Characters = new List<Character>();
    public List<Background> Backgrounds = new List<Background>();

    public Character FindCharacter(string name)
    {
      return Characters.Find(x => x.Name == name);
    }

    public Background FindBackground(string name)
    {
      return Backgrounds.Find(x => x.Name == name);
    }

  }  
}
