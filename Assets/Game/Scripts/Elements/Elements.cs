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
    public ElementTypes m_CurElement = (ElementTypes)0;

    public void ChangeElement(int cycleAmount = 1)
    {
        m_CurElement += cycleAmount;

        if (m_CurElement >= ElementTypes.ElementTypesSize)
        {
            m_CurElement = (ElementTypes)((int)m_CurElement % (int)ElementTypes.ElementTypesSize);
        }
        else if (m_CurElement < 0)
        {
            m_CurElement -= (int)ElementTypes.ElementTypesSize * Mathf.FloorToInt((float)m_CurElement / (float)ElementTypes.ElementTypesSize);
        }

        SetElement(m_CurElement);
    }

    public void SetElement(ElementTypes newElement)
    {
        m_CurElement = newElement;
    }
}
