/******************************************************************************/
/*!
@file   DialogControls.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using Stratus.UI;

namespace Prototype 
{
  /// <summary>
  /// Attach this component to the dialog window
  /// </summary>
  public class DialogControls : StratusBehaviour 
  {


    void Update()
    {
      // Check for input here

      // Example:
      //if (Input.GetKeyDown(this.UpKey)) Navigate(Stratus.UI.Navigation.Up);
      //if (Input.GetKeyDown(this.DownKey)) Navigate(Stratus.UI.Navigation.Down);
      //if (Input.GetKeyDown(this.LeftKey)) Navigate(Stratus.UI.Navigation.Left);
      //if (Input.GetKeyDown(this.RightKey)) Navigate(Stratus.UI.Navigation.Right);
      //// Selection
      //if (Input.GetKeyDown(this.ConfirmKey)) this.Confirm();
      //if (Input.GetKeyDown(this.CancelKey)) this.Cancel();

    }

    void Navigate(Stratus.UI.Navigation dir)
    {
      this.gameObject.Dispatch<Link.NavigateEvent>(new Link.NavigateEvent(dir));
    }

    void Confirm()
    {
      this.gameObject.Dispatch<Link.ConfirmEvent>(new Link.ConfirmEvent());
    }

    void Cancel()
    {
      //Trace.Script("Cancel called", this);
      this.gameObject.Dispatch<Link.CancelEvent>(new Link.CancelEvent());
    }

  }  
}
