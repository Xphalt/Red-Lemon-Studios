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
    public GameObject pausePanel, toolbarPanel, controlsPanel, deathScreen, completeScreen, ammoButtonHighlight, relicButtonHighlight;
    public Transition transition;

    //Toolbar Menu and Toolbar Menu
    [TextArea(1, 40)] public List<string> RelicDescription = new List<string>();
    [TextArea(1, 40)] public List<string> AmmoDescription = new List<string>();
    public Text ToolNameHolder, ToolInfoHolder;
    public Image ImageHolder;
    public List<Sprite> relicSprite = new List<Sprite>();
    private int toolNumber = 0;
    private bool relicInfoType = true;

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

    public Image relicPopUp;
    public List<Text> popUpText;
    public float popUpDuration;
    private Color popUpColour;
    private Color popUpTextColor;
    private float popUpTimer = 0;
    private bool popUpActive = false;

    public Text waveCounter, enemyCounter, waveTimer;
    public Image enemyPanel;

    private void Awake()
    {
        healthBarColour = healthBarFill.color;
        secondCrosshair.SetActive(false);

        popUpColour = relicPopUp.color;
        popUpTextColor = popUpText[0].color;
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

        if (popUpActive)
        {
            popUpTimer += Time.deltaTime;

            if (popUpTimer > popUpDuration / 2)
            {
                relicPopUp.color = Color.Lerp(popUpColour, Color.clear, (popUpTimer - popUpDuration / 2) * 2);
                foreach (Text text in popUpText) text.color = Color.Lerp(popUpTextColor, Color.clear, (popUpTimer - popUpDuration / 2) * 2);
            }

            if (popUpTimer > popUpDuration)
            {
                popUpActive = false;
                relicPopUp.gameObject.SetActive(false);
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

    public void ShowRelicPopUp(string relicType)
    {
        relicPopUp.gameObject.SetActive(true);
        popUpText[0].text = relicType + " Relic Collected";
        popUpActive = true;
        popUpTimer = 0;

        relicPopUp.color = popUpColour;
        foreach (Text text in popUpText) text.color = popUpTextColor;
    }

    public void UpdateEnemyCounter(int enemiesAlive, int enemiesSpawned)
    {
        if (!enemyPanel.gameObject.activeSelf) ActivateEnemyCounter();
        enemyCounter.text = "Enemies Remaining: " + enemiesAlive.ToString() + "/" + enemiesSpawned.ToString();
    }

    public void UpdateWaveCounter(int waveNumber, int maxWaves)
    {
        if (!enemyPanel.gameObject.activeSelf) ActivateEnemyCounter();
        waveCounter.text = "Wave: " + waveNumber.ToString() + "/" + maxWaves.ToString();
        //if (waveNumber == maxWaves) waveTimer.gameObject.SetActive(false);
    }

    public void UpdateWaveTimer(int timer)
    {
        if (!enemyPanel.gameObject.activeSelf) ActivateEnemyCounter();
        waveTimer.text = "Next Wave In: " + timer.ToString();
    }

    public void ActivateEnemyCounter()
    {
        if (!enemyPanel.gameObject.activeSelf)
        {
            enemyPanel.gameObject.SetActive(true);
            enemyCounter.gameObject.SetActive(true);
            waveCounter.gameObject.SetActive(true);
            waveTimer.gameObject.SetActive(true);
        }
    }

    public void ClearEnemyCounter()
    {
        enemyPanel.gameObject.SetActive(false);
        enemyCounter.gameObject.SetActive(false);
        waveCounter.gameObject.SetActive(false);
        waveTimer.gameObject.SetActive(false);
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
        StartCoroutine(transition.LoadLevel(homeMenu));
    }

    public void ShowControls(bool isShown)
    {
        controlsPanel.SetActive(isShown);
    }

    /*__________________________________________________________
    Tool bar view menu code
    ____________________________________________________________*/

    public bool ShowToolBarMenu(ElementTypes currentAmmo, ElementTypes currentRelic=ElementTypes.ElementTypesSize)
    {
        if (!pausePanel.activeSelf && !deathScreen.activeSelf && !completeScreen.activeSelf)
        {
            toolbarPanel.SetActive(!toolbarPanel.activeSelf);

            if (toolbarPanel.activeSelf)
            {
                SetInfoType(relicInfoType);
                if (relicInfoType && currentRelic != ElementTypes.ElementTypesSize) SetToolInfo((int)currentRelic);
                else SetToolInfo((int)currentAmmo);
            }
            return true;
        }
        return false;
    }

    public void SetToolInfo(int toolType=-1)
    {
        if (toolType == -1) toolType = toolNumber;
        else toolNumber = toolType;
        ToolNameHolder.text = ((ElementTypes)toolType).ToString();
        ToolInfoHolder.text = (relicInfoType) ? RelicDescription[toolType] : AmmoDescription[toolType];
        ImageHolder.sprite = relicSprite[toolType];
    }

    public void SetInfoType(bool toRelic)
    {
        relicInfoType = toRelic;
        ammoButtonHighlight.SetActive(!toRelic);
        relicButtonHighlight.SetActive(toRelic);
        SetToolInfo();
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