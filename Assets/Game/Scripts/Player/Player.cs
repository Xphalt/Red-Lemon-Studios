/// <summary>
/// 
/// Script made by Daniel and Daniel
/// 
/// Originally we had two scripts, this
/// one for player inputs, shooting and
/// using tools which was made primarily
/// by Daniel with Linden making a few
/// changes.
/// 
/// The other script was called 
/// "PlayerAttributes" and was created 
/// by Matt which contained the health
/// and ammo along with ways to increase
/// and decrease health, lose and gain ammo.
/// 
/// Me and Daniel kept Matts code and added
/// and refactored it into this script to
/// keep everything related to the player
/// together and to help with reusablillty.
/// 
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using static EnumHelper;

public class Player : MonoBehaviour
{
    public GameObject canvas;
    private UIManager UIScript;

    private FirstPersonController fpsScript;

    internal List<ToolBase> toolList = new List<ToolBase>();
    internal ToolBase currentTool = null;
    private int toolIndex = 0;

    public GameObject weapon;

    private bool switchingTools = false;

    internal ElementShooting shooter;
    internal Elements elementChanger;

    internal Dictionary<ElementTypes, int> Ammo = new Dictionary<ElementTypes, int>();

    public int m_MaxAmmo;

    private const float m_MaxHealth = 100.0f;
    private float m_CurHealth;

    private float toolTimer = 0;
    public float toolCooldownDuration;
    internal bool isToolAvailable;

    private void Start()
    {
        fpsScript = GetComponent<FirstPersonController>();
        isToolAvailable = false;
        shooter = weapon.GetComponent<ElementShooting>();
        elementChanger = weapon.GetComponent<Elements>();

        m_CurHealth = m_MaxHealth;
        for (int ammo = 0; ammo < (int)ElementTypes.ElementTypesSize; ammo++)
        {
            Ammo.Add((ElementTypes)0 + ammo, m_MaxAmmo);
        }

        UIScript = canvas.GetComponent<UIManager>();

        UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[elementChanger.m_CurElement], true);
    }

    void Update()
    {
        Inputs();

        ToolCooldown();
    }


    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            shooter.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            UseTool();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            if (!switchingTools) elementChanger.ChangeElement(Mathf.FloorToInt(Input.mouseScrollDelta.y));
            else ChangeTool(Mathf.FloorToInt(Input.mouseScrollDelta.y));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            switchingTools = !switchingTools;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) //OnCollisionEnter didn't work for some reason
    {
        if (currentTool != null)
        {
            if (currentTool.toolType == ElementTypes.Air) currentTool.EndAbility();
        }
    }

    #region StatManagement
    public void TakeDamage(float value)
    {
        m_CurHealth -= value;

        if (m_CurHealth <= 0)
        {
            m_CurHealth = 0;
            Respawn();
        }

        UIScript.UpdateHealthText((int)m_CurHealth);
    }

    public void AddHealth(float value)
    {
        m_CurHealth += value;

        if (m_CurHealth > m_MaxHealth)
        {
            m_CurHealth = m_MaxHealth;
        }

        UIScript.UpdateHealthText((int)m_CurHealth);
    }

    public void SubstractAmmo(int value, ElementTypes type)
    {
        Ammo[type] -= value;

        if (Ammo[type] < 0)
        {
            Ammo[type] = 0;
        }

        if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public void AddAmmo(int value, ElementTypes type)
    {
        Ammo[type] += value;

        if (Ammo[type] > m_MaxAmmo)
        {
            Ammo[type] = m_MaxAmmo;
        }
        
        if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public bool AmmoCheck()
    {
        return Ammo[elementChanger.m_CurElement] > 0;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Tool
    private void UseTool()
    {
        if (isToolAvailable)
        {
            if (currentTool.Activate())
            {
                isToolAvailable = false;
            }
        }
    }

    private void ToolCooldown()
    {
        if (!isToolAvailable)
        {
            toolTimer += Time.deltaTime;

            if (toolTimer > toolCooldownDuration)
            {
                isToolAvailable = true;
                toolTimer = 0;
            }
        }
    }

    private void ChangeTool(int mouseScroll)
    {
        if (mouseScroll != toolList.Count && toolList.Count > 0)
        {
            currentTool.EndAbility();
            currentTool.gameObject.SetActive(false);
            toolIndex += mouseScroll;

            if (toolIndex >= toolList.Count)
            {
                toolIndex -= toolList.Count;
            }
            else if (toolIndex < 0)
            {
                toolIndex += toolList.Count;
            }

            currentTool = toolList[toolIndex];
            currentTool.gameObject.SetActive(true);
        }
    }
    #endregion
}