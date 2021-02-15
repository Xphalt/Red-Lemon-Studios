﻿/// <summary>
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

public class ToolCrystalBall : ToolBase
{
    private CharacterController playerController;

    private Vector3 dashDist;
    private float dashTimer = 0;
    public float dashDuration;
    private bool isDashing;
    public float DashSpeed;

    public override void Start()
    {
        base.Start();
        playerController = player.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isDashing)
        {
            dashTimer += Time.deltaTime;

            playerController.Move(dashDist * Time.deltaTime);

            if (dashTimer > dashDuration)
            {
                isDashing = false;
                dashTimer = 0;
            }
        }
    }

    public override bool Activate()
    {
        base.Activate();

        if (playerController.velocity == Vector3.zero) return false; 
        isDashing = true;

        dashDist = playerController.velocity.normalized * DashSpeed;

        return true;
    }
}