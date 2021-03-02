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

    public List<ElementTypes> WandTypes = new List<ElementTypes>();
    public List<Material> WandMaterials = new List<Material>();
    private Dictionary<ElementTypes, Material> WandColourDictionary = new Dictionary<ElementTypes, Material>();
    private MeshRenderer myRenderer;

    private GameObject player;

    private UIManager UIScript;
    private Player playerScript;

    public ElementTypes m_CurElement = (ElementTypes)0;

    private void Start()
    {
        UIScript = canvas.GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();

        for (int index = 0; index < WandTypes.Count; index++)
        {
            WandColourDictionary.Add(WandTypes[index], WandMaterials[index]);
        }

        myRenderer = GetComponent<MeshRenderer>();
        myRenderer.material = WandColourDictionary[m_CurElement];
    }

    public void ChangeElement(int cycleAmount = 1)
    {
        m_CurElement += cycleAmount;

        m_CurElement = (ElementTypes)((int)m_CurElement % (int)ElementTypes.ElementTypesSize);

        UIScript.UpdateElementText(m_CurElement, playerScript.Ammo[m_CurElement], true);

        myRenderer.material = WandColourDictionary[m_CurElement];
    }
}
