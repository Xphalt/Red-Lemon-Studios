using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsScript_MattNDaniel : MonoBehaviour
{
    public enum Elements { Fire, Water, Air, Earth, ElementsSize };

    public Text ElementsText;

    public Elements m_CurElement;

    private void Start()
    {
        m_CurElement = Elements.Fire;
    }

    void Update()
    {
        ChangeElement();
    }

    void UpdateElementText()
    {
        ElementsText.text = m_CurElement.ToString();

        switch (m_CurElement)
        {
            case Elements.Fire:
                ElementsText.color = Color.red;
                break;
            case Elements.Water:
                ElementsText.color = Color.blue;
                break;
            case Elements.Air:
                ElementsText.color = Color.white;
                break;
            case Elements.Earth:
                ElementsText.color = Color.yellow;
                break;
        }
    }

    void ChangeElement()
    {
        m_CurElement += Mathf.FloorToInt(Input.mouseScrollDelta.y);

        if (m_CurElement >= Elements.ElementsSize)
        {
            m_CurElement -= Elements.ElementsSize;
        }
        else if (m_CurElement < 0)
        {
            m_CurElement += (int)Elements.ElementsSize;
        }

        UpdateElementText();
    }
}
