/******************************************************************************/
/*!
@file   VisualNovelCharacter.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;

namespace Prototype 
{
  [RequireComponent(typeof(Animator))]
  public class StoryCharacter : StratusBehaviour 
  {
    public enum Expression { Default, Happy, Sad, Angry, Afraid, Excited }
    public enum State { Default, Talking, Listening, Enter, Exit }
    public StoryGlossary.Character Character;
    [ReadOnly] public State CurrentState;
    Image Image { get { return GetComponent<Image>(); } }
    Animator Animator;

    void Start()
    {      
    }

    public void Configure()
    {
      Animator = GetComponent<Animator>();
      //Trace.Script("Setting color", this);
      Image.color.SetAlpha(0.0f);
      Image.CrossFadeAlpha(1.0f, 1.5f, true);
      ChangeExpression(StoryCharacter.Expression.Default);
      ChangeState(State.Enter);
    }
    
    public void ChangeExpression(Expression expression)
    {
      switch (expression)
      {
        case Expression.Default:          
          Image.sprite = Character.Default;
          break;
      }
    }

    public void ChangeState(State state)
    {
      if (state == CurrentState)
        return;

      CurrentState = state;
      //Trace.Script(state, this);
      switch (state)
      {
        case State.Enter:
          Animator.SetTrigger("Enter");
          break;
        case State.Default:
          Animator.SetTrigger("Default");
          break;
        case State.Talking:
          Animator.SetTrigger("Talking");
          break;
        case State.Exit:
          //Animator.SetTrigger("Exit");
          break;
      }
    }

  
  }  
}
