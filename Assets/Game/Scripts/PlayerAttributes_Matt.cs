/*
    Mateusz Szymanski
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerAttributes_Matt : MonoBehaviour
{

    public Text HealthText;
    public Text AmmoText;

    // for now this is public to make the tests easier, later should be moved to private and potentially made const
    public int m_MaxAmmo;

    private const float m_MaxHealth = 100.0f;
    private float m_CurHealth;
    private int m_CurAmmo;

    void Start()
    {
        m_CurHealth = m_MaxHealth;
        m_CurAmmo = m_MaxAmmo;
    }

    void Update()
    {
        HealthText.text = m_CurHealth.ToString();
        AmmoText.text = m_CurAmmo.ToString();

        if(m_CurHealth == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TakeDamage(float value)
    {
        m_CurHealth -= value;

        if(m_CurHealth < 0)
        {
            m_CurHealth = 0;
        }
    }

    public void AddHealth(float value)
    {
        m_CurHealth += value;

        if(m_CurHealth > m_MaxHealth)
        {
            m_CurHealth = m_MaxHealth;
        }
    }

    public void SubstractAmmo(int value)
    {
        m_CurAmmo -= value;

        if (m_CurAmmo < 0)
        {
            m_CurAmmo = 0;
        }
    }

    public void AddAmmo(int value)
    {
        m_CurAmmo += value;

        if (m_CurAmmo > m_MaxAmmo)
        {
            m_CurAmmo = m_MaxAmmo;
        }
    }

}
