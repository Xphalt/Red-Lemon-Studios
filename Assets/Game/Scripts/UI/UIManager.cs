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
        HPText.text = "HP: " + currentHealth.ToString();
        Elements.Fire.ToString();
    }


    public void UpdateElementText(Elements ammoType, int ammoAmount, bool newColour=false)
    {
        ElementalAmmoText.text = ammoType + ": " + ammoAmount.ToString();

        if (newColour)
        {
            switch (ammoType)
            {
                case Elements.Fire:
                    ElementalAmmoText.color = Color.red;
                    break;
                case Elements.Water:
                    ElementalAmmoText.color = Color.blue;
                    break;
                case Elements.Air:
                    ElementalAmmoText.color = Color.white;
                    break;
                case Elements.Earth:
                    ElementalAmmoText.color = Color.yellow;
                    break;
            }
        }
        
    }
}
