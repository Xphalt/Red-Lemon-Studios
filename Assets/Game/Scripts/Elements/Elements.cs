/// <summary>
/// 
/// Script made by Daniel and Matt
/// 
/// Linden and Daniel refactored this
/// script to allow for it's complete
/// independance form other scripts
/// making it very easy to add in new
/// elements
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumHelper;

public class Elements : MonoBehaviour
{
    public GameObject canvas;
    private GameObject player;

    private UIManager UIScript;
    private Player playerScript;

    public ElementTypes m_CurElement = (ElementTypes)0;

    private void Start()
    {
        UIScript = canvas.GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
    }

    public void ChangeElement(int mouseScroll)
    {
        m_CurElement += mouseScroll;

        if (m_CurElement >= ElementTypes.ElementTypesSize)
        {
            m_CurElement -= ElementTypes.ElementTypesSize;
        }
        else if (m_CurElement < 0)
        {
            m_CurElement += (int)ElementTypes.ElementTypesSize;
        }

        UIScript.UpdateElementText(m_CurElement, playerScript.Ammo[m_CurElement], true);
    }
}
