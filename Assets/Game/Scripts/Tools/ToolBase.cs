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
    protected Rigidbody playerRigid;
    protected FirstPersonController fpsScript;
    public ElementTypes toolType;

    internal bool inUse;

    public int maxCombo = 1;
    public float percentIncreasePerHit = 0;
    public float damagePercentRecievedOnMiss = 0;
    public bool missPenalty = false;

    public bool doubleJumpEnabled = false;
    public float knockBackMultiplier = 1;

    public float damageRecievedMultiplier = 1;
    public float speedMultiplier = 1;


    virtual public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        toolDisplayPos = GameObject.Find("ToolPlaceHolder").transform.position;
        playerScript = player.GetComponent<Player>();
        playerRigid = player.GetComponent<Rigidbody>();
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

            if (playerScript.currentTool == null)
            {
                playerScript.currentTool = GetComponent<ToolBase>();
                playerScript.ActivatePassives();
            }
            else gameObject.SetActive(false);
        }
    }
}