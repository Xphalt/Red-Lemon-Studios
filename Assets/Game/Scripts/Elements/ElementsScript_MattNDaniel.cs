using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumHelper;

public class ElementsScript_MattNDaniel : MonoBehaviour
{
    public GameObject canvas;
    private UIManager UIScript;

    private GameObject player;
    private PlayerScript_Daniel playerScript;

    public Elements m_CurElement = (Elements)0;

    private void Start()
    {
        UIScript = canvas.GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript_Daniel>();
    }

    public void ChangeElement(int mouseScroll)
    {
        m_CurElement += mouseScroll;

        if (m_CurElement >= Elements.ElementsSize)
        {
            m_CurElement -= Elements.ElementsSize;
        }
        else if (m_CurElement < 0)
        {
            m_CurElement += (int)Elements.ElementsSize;
        }

        UIScript.UpdateElementText(m_CurElement, playerScript.Ammo[m_CurElement], true);
    }
}
