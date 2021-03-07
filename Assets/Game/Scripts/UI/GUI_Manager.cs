using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Manager : MonoBehaviour
{
    //Variables
    public Slider healthSlider;
    public Player player;
    public Image fill;
    

    private CharacterBase charBase;
    private float health;

    //Functions
    void Start()
    {
        charBase = player.GetComponent<CharacterBase>();
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float curHealth)
    {
        healthSlider.value = curHealth;
    }


}
