using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Manager : MonoBehaviour
{
    public Player player;
    public Slider healthSlider, relicSlider;
    public Image healthBarFill, relicBarFill;
    
    /*__________________________________________________________
    Health Bar code
    ____________________________________________________________*/

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float curHealth)
    {
        healthSlider.value = curHealth;
    }

    /*__________________________________________________________
    Relic Bar code
    ____________________________________________________________*/

    public void RefillRelicTimer(float maxTimeValue)
    {
        relicSlider.maxValue = maxTimeValue;
        relicSlider.value = maxTimeValue;
    }

    public void UpdateRelicTimer(float cooldownValue)
    {
        relicSlider.value = cooldownValue;
    }
}
