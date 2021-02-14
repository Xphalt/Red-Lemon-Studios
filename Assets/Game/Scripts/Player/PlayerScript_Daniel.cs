/*
 * DANIEL BIBBY
 */


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnumHelper;

public class PlayerScript_Daniel : MonoBehaviour
{
    public GameObject canvas;
    private UIManager UIScript;

    internal GameObject tool;
    public GameObject weapon;

    internal ToolBase toolActivate;
    internal ElementShooting shooter;
    internal ElementsScript_MattNDaniel elementChanger;

    public int m_MaxAmmo;

    private const float m_MaxHealth = 100.0f;
    private float m_CurHealth;

    internal Dictionary<Elements, int> Ammo = new Dictionary<Elements, int>();

    private float toolTimer = 0;
    public float toolDuration;
    internal bool isToolAvailable;

    private void Start()
    {
        isToolAvailable = false;

        shooter = weapon.GetComponent<ElementShooting>();
        elementChanger = weapon.GetComponent<ElementsScript_MattNDaniel>();

        m_CurHealth = m_MaxHealth;
        for (int ammo = 0; ammo < (int)Elements.ElementsSize; ammo++)
        {
            Ammo.Add((Elements)0 + ammo, m_MaxAmmo);
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
            elementChanger.ChangeElement(Mathf.FloorToInt(Input.mouseScrollDelta.y));
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

    public void SubstractAmmo(int value)
    {
        Ammo[elementChanger.m_CurElement] -= value;

        if (Ammo[elementChanger.m_CurElement] < 0)
        {
            Ammo[elementChanger.m_CurElement] = 0;
        }
        UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[elementChanger.m_CurElement]);
    }

    public void AddAmmo(int value)
    {
        Ammo[elementChanger.m_CurElement] += value;

        if (Ammo[elementChanger.m_CurElement] > m_MaxAmmo)
        {
            Ammo[elementChanger.m_CurElement] = m_MaxAmmo;
        }
        UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[elementChanger.m_CurElement]);
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
            toolActivate.Activate();
            isToolAvailable = false;
        }
    }

    private void ToolCooldown()
    {
        if (!isToolAvailable)
        {
            toolTimer += Time.deltaTime;

            if (toolTimer > toolDuration)
            {
                isToolAvailable = true;
                toolTimer = 0;
            }
        }
    }
    #endregion
}