/// <summary>
/// 
/// Script made by Daniel and Linden
/// 
/// Initially we didn't have a script
/// exclusively for things dislayed on
/// the UI so this script was created
/// to keep any UI related logic in
/// one place
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumHelper;

public class UIManager : MonoBehaviour
{
    public Text HPText;
    public Text ElementalAmmoText;

    public void UpdateHealthText(int currentHealth)
    {
        //HPText.text = "HP: " + currentHealth.ToString();
        ElementTypes.Fire.ToString();
    }

    //public void UpdateElementText(ElementTypes ammoType, int ammoAmount, bool newColour=false)
    //{
        //ElementalAmmoText.text = ammoType + ": " + ammoAmount.ToString();

        //if (newColour)
        //{
        //    switch (ammoType)
        //    {
        //        case ElementTypes.Fire:
        //            ElementalAmmoText.color = Color.red;
        //            break;
        //        case ElementTypes.Water:
        //            ElementalAmmoText.color = Color.blue;
        //            break;
        //        case ElementTypes.Air:
        //            ElementalAmmoText.color = Color.white;
        //            break;
        //        case ElementTypes.Earth:
        //            ElementalAmmoText.color = Color.yellow;
        //            break;
        //    }
        //}
        
    //}
}