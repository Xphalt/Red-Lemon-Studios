﻿using System.Collections;
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
    public Image healthBarFill, relicBarFill, ammoBarFill, relicBarBorder, ammoBarBorder;
    public Text ammoText, relicText;

    //Elemental Group variables
    public Image equipedAmmoIcon;
    public Sprite airIcon, waterIcon, fireIcon, earthIcon;
    public Color lightPink = new Color(0.9f, 0.5f, 0.5f, 1);
    public Color lightBlue = new Color(0.09f, 0.2f, 0.9f, 1);
    public Color lightRed = new Color(1, 0.2f, 0.3f, 1);
    public Color lightGreen = new Color(0.2f, 0.7f, 0.2f, 1);

    public Color scrollSelected = new Color();
    public Color scrollNormal = new Color();

    //Pause Menu variables
    public string homeMenu;
    public GameObject pausePanel, toolbarPanel, controlsPanel, deathScreen, completeScreen;

    //Toolbar Menu and Toolbar Menu
    [TextArea(1, 40)] public List<string> RelicDescription = new List<string>();
    public Text RelicNameHolder, RelicInfoHolder;
    public Image ImageHolder;
    public List<Sprite> relicSprite = new List<Sprite>();

    public float healthBarFlashDuration;
    public Color healthBarFlashColour;
    private Color healthBarColour;
    private float healthBarFlashTimer = 0;
    private bool healthBarFlashed = false;

    public List<Image> crosshair;
    public float crossHighlightOpacity;
    public float crossDefaultOpacity;
    public GameObject secondCrosshair;
    public float secondCrosshairDuration;
    private float secondCrosshairTimer = 0;

    private void Awake()
    {
        healthBarColour = healthBarFill.color;
        secondCrosshair.SetActive(false);
    }

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (healthBarFlashed)
        {
            healthBarFlashTimer += Time.deltaTime;
            if (healthBarFlashTimer > healthBarFlashDuration)
            {
                healthBarFill.color = healthBarColour;
                healthBarFlashed = false;
                healthBarFlashTimer = 0;
            }
        }

        if (secondCrosshair.activeSelf)
        {
            secondCrosshairTimer += Time.deltaTime;
            if (secondCrosshairTimer > secondCrosshairDuration)
            {
                secondCrosshair.SetActive(false);
                secondCrosshairTimer = 0;
            }
        }
    }

    /*__________________________________________________________
    Health Bar code
    ____________________________________________________________*/

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float curHealth, bool damaged=false)
    {
        healthSlider.value = curHealth;
        if (damaged)
        {
            healthBarFill.color = healthBarFlashColour;
            healthBarFlashed = true;
        }
    }

    public void ShowHit()
    {
        secondCrosshair.SetActive(true);
        secondCrosshairTimer = 0;
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

    public void ToggleSliderSelection(bool toRelic)
    {
        if (toRelic)
        {
            ammoBarBorder.color = scrollNormal;
            relicBarBorder.color = scrollSelected;
            ammoText.enabled = false;
            relicText.enabled = true;
        }
        else
        {
            ammoBarBorder.color = scrollSelected;
            relicBarBorder.color = scrollNormal;
            ammoText.enabled = true;
            relicText.enabled = false;
        }
    }

    /*__________________________________________________________
    Ammo Selection panel code
    ____________________________________________________________*/

    public void HighlightSelectedAmmo()
    {
        switch (player.elementChanger.m_CurElement)
        {
            case ElementTypes.Air:
                equipedAmmoIcon.sprite = airIcon;
                ammoBarFill.color = lightPink;
                foreach (Image cross in crosshair) cross.color = lightPink;
                break;
            case ElementTypes.Water:
                equipedAmmoIcon.sprite = waterIcon;
                ammoBarFill.color = lightBlue;
                foreach (Image cross in crosshair) cross.color = lightBlue;
                break;
            case ElementTypes.Fire:
                equipedAmmoIcon.sprite = fireIcon;
                ammoBarFill.color = lightRed;
                foreach (Image cross in crosshair) cross.color = lightRed;
                break;
            case ElementTypes.Earth:
                equipedAmmoIcon.sprite = earthIcon;
                ammoBarFill.color = lightGreen;
                foreach (Image cross in crosshair) cross.color = lightGreen;
                break;
            default:
                break;
        }
    }

    public void HighlightCrosshair(bool highlight=false)
    {
        foreach (Image cross in crosshair) 
            cross.color = new Color(cross.color.r, cross.color.g, cross.color.b, (highlight) ? crossHighlightOpacity : crossDefaultOpacity);
    }

    /*__________________________________________________________
    Pause menu code
    ____________________________________________________________*/

    public bool PausePlay()
    {
        if (!toolbarPanel.activeSelf && !deathScreen.activeSelf && !completeScreen.activeSelf)
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            controlsPanel.SetActive(false);
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

    public void ShowControls(bool isShown)
    {
        controlsPanel.SetActive(isShown);
    }

    /*__________________________________________________________
    Tool bar view menu code
    ____________________________________________________________*/

    public bool ShowToolBarMenu()
    {
        if (!pausePanel.activeSelf && !deathScreen.activeSelf && !completeScreen.activeSelf)
        {
            toolbarPanel.SetActive(!toolbarPanel.activeSelf);
            return true;
        }
        return false;
    }

    public void SetRelicInfo(int relicType)
    {
        RelicNameHolder.text = ((ElementTypes)relicType).ToString();
        RelicInfoHolder.text = RelicDescription[relicType];
        ImageHolder.sprite = relicSprite[relicType];
    }

    public void ShowEndGame(bool dead)
    {
        Time.timeScale = 0;
        if (dead) deathScreen.SetActive(true);
        else completeScreen.SetActive(true);
    }

    public void LeaveEndGame()
    {
        Time.timeScale = 1;
        deathScreen.SetActive(false);
        completeScreen.SetActive(false);
    }
}