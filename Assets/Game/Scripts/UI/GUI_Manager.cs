using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Color lightPink = new Color(0.9f, 0.5f, 0.5f, 1);
    public Color lightBlue = new Color(0.09f, 0.2f, 0.9f, 1);
    public Color lightRed = new Color(1, 0.2f, 0.3f, 1);
    public Color lightGreen = new Color(0.2f, 0.7f, 0.2f, 1);

    //Pause Menu and Toolbar Menu variables
    public string homeMenu;
    public GameObject pausePanel, toolbarPanel;

    private void Awake()
    {
        pausePanel.SetActive(false);
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

    public void HighlightSelectedAmmo()
    {
        ResetElementalImages();
        switch (player.elementChanger.m_CurElement)
        {
            case ElementTypes.Air:
                airImage.sprite = selectedAirIcon;
                ammoBarFill.color = lightPink;
                break;
            case ElementTypes.Water:
                waterImage.sprite = selectedWaterIcon;
                ammoBarFill.color = lightBlue;
                break;
            case ElementTypes.Fire:
                fireImage.sprite = selectedFireIcon;
                ammoBarFill.color = lightRed;
                break;
            case ElementTypes.Earth:
                earthImage.sprite = selectedEarthIcon;
                ammoBarFill.color = lightGreen;
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

    /*__________________________________________________________
    Pause menu code
    ____________________________________________________________*/

    public bool PausePlay()
    {
        if (!toolbarPanel.activeSelf)
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            return true;
        }
        return false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        //maybe display a warning message about unsaved data being lose?
        SceneManager.LoadScene(homeMenu);

    }

    /*__________________________________________________________
    Tool bar view menu code
    ____________________________________________________________*/

    public bool ShowToolBarMenu()
    {
        if (!pausePanel.activeSelf)
        {
            toolbarPanel.SetActive(!toolbarPanel.activeSelf);
            return true;
        }
        return false;
    }
}
