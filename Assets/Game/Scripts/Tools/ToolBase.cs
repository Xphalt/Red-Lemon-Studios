/// <summary>
/// 
/// Script made by Linden and Daniel
/// 
/// This script is a base for any
/// future tools added in the future
/// and shouldn't need to be changed
/// unless something is required for
/// all tools. Tools can override
/// the activate function to have
/// their own abillities
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static EnumHelper;

public class ToolBase : MonoBehaviour
{
    protected GameObject player;
    protected Vector3 toolDisplayPos;
    protected Player playerScript;
    protected CharacterController playerController;
    protected FirstPersonController fpsScript;
    public ElementTypes toolType;

    internal bool inUse;

    virtual public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        toolDisplayPos = GameObject.Find("ToolPlaceHolder").transform.position;
        playerScript = player.GetComponent<Player>();
        playerController = player.GetComponent<CharacterController>();
        fpsScript = player.GetComponent<FirstPersonController>();
    }

    virtual public bool Activate() 
    {
        return true; 
    }

    virtual public void EndAbility() { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            transform.position = toolDisplayPos;
            playerScript.isToolAvailable = true;
            playerScript.toolList.Add(GetComponent<ToolBase>());
            if (playerScript.currentTool == null) playerScript.currentTool = GetComponent<ToolBase>();
            else gameObject.SetActive(false);
        }
    }
}