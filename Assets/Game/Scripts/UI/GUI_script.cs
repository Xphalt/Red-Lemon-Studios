using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_script : MonoBehaviour
{
    //Variables
    public Slider healthSlider;
    private int health, maxHealth = 100;

    //Functions
    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        
    }

    void SetMaxHealth()
    {
        //sets health bar at max health when player's at max health
    }

    void UpdateHealth()
    {
        //updates health value and slider
    }


}
