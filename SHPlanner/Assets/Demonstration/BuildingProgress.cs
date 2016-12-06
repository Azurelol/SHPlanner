/******************************************************************************/
/*!
@file   BuildingProgress.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Prototype
{
  public class BuildingProgress : MonoBehaviour
  {

    public ObjectBuilding Building;
    public Text Text;
    
    // Update is called once per frame
    void Update()
    {
      if (!Building) return;
      Text.text = Building.name + " progress = " + Building.Progress + "%";
    }
  }

}