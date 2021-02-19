/// <summary>
/// 
/// Script made by Linden and Daniel
/// 
/// By inheriting from the tool base
/// script, tools are able to be created
/// very easily and quickly
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFireRelic : ToolBase
{
    public float dashSpeed;
    public float dashDuration;
    private float dashTimer = 0;
    private Vector3 dashDist;

    public override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        if (inUse)
        {
            dashTimer += Time.deltaTime;

            playerRigid.AddForce(dashDist);

            if (dashTimer > dashDuration)
            {
                inUse = false;
                dashTimer = 0;
            }
        }
    }

    public override bool Activate()
    {
        base.Activate();

        Vector3 nonVerticalDirection = playerRigid.velocity;
        nonVerticalDirection.y = 0;

        if (nonVerticalDirection == Vector3.zero) nonVerticalDirection = player.transform.forward;

        dashDist = nonVerticalDirection.normalized * dashSpeed;

        inUse = true;

        return true;
    }
}