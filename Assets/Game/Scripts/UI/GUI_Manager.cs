using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumHelper;

public class GUI_Manager : MonoBehaviour
{
    public Player player;

    //Slider Bar variables
    public Slider healthSlider, relicSlider, ammoSlider;
    public Image healthBarFill, relicBarFill, ammoBarFill;

    //Elemental Group variables
    public Image airImage, waterImage, fireImage, earthImage;
    public Sprite airIcon, waterIcon, fireIcon, earthIcon;
    public Sprite selectedAirIcon, selectedWaterIcon, selectedEarthIcon, selectedFireIcon;

    //private void Start()
    //{
    //    airIcon = Resources.Load<Sprite>("air_icon");
    //    waterIcon = Resources.Load<Sprite>("water_icon");
    //    earthIcon = Resources.Load<Sprite>("earth_icon");
    //    fireIcon = Resources.Load<Sprite>("fire_icon");

    //    airIcon = Resources.Load<Sprite>("air_icon_selected");
    //    waterIcon = Resources.Load<Sprite>("water_icon_selected");
    //    earthIcon = Resources.Load<Sprite>("earth_icon_selected");
    //    fireIcon = Resources.Load<Sprite>("fire_icon_selected");
        
    //}

    private void Update()
    {
        HighlightSelectedAmmo();
    }

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

    /*__________________________________________________________
    Ammo bar code
    ____________________________________________________________*/

    public void SetMaxAmmo(float maxAmmo)
    {
         ammoSlider.maxValue = maxAmmo;
         ammoSlider.value = maxAmmo;
    }

    public void UpdateAmmoCount(float curAmmoCount)
    {
         ammoSlider.value = curAmmoCount;
    }

    /*__________________________________________________________
    Ammo Selection panel code
    ____________________________________________________________*/

    private void HighlightSelectedAmmo()
    {
        switch (player.elementChanger.m_CurElement)
        {
            case ElementTypes.Air:
                ResetElementalImages();
                airImage.sprite = selectedAirIcon;
                break;
            case ElementTypes.Water:
                ResetElementalImages();
                waterImage.sprite = selectedWaterIcon;
                break;
            case ElementTypes.Fire:
                ResetElementalImages();
                fireImage.sprite = selectedFireIcon;
                break;
            case ElementTypes.Earth:
                ResetElementalImages();
                earthImage.sprite = selectedEarthIcon;
                break;
            default:
                break;
        }
    }

    private void ResetElementalImages()
    {
        airImage.sprite = airIcon;
        waterImage.sprite = waterIcon;
        fireImage.sprite = fireIcon;
        earthImage.sprite = earthIcon;
    }
}
