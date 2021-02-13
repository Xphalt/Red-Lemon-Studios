using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBase : MonoBehaviour
{
    protected GameObject player;
    protected Vector3 toolDisplayPos;
    protected PlayerScript_Daniel playerScript;

    virtual public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        toolDisplayPos = GameObject.Find("ToolPlaceHolder").transform.position;
        playerScript = player.GetComponent<PlayerScript_Daniel>();
    }

    virtual public void Activate() {}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            transform.position = toolDisplayPos;
            playerScript.tool = gameObject;
            playerScript.isToolAvailable = true;
            playerScript.toolActivate = GetComponent<ToolBase>();
        }
    }
}