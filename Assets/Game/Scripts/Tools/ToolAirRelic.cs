using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAirRelic : ToolBase
{
    private CharacterController playerController;

    private Vector3 grappleDist;
    private bool isGrappling;
    public float grappleSpeed;

    public override void Start()
    {
        base.Start();
        playerController = player.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isGrappling)
        {
            playerController.Move(grappleDist * Time.deltaTime);
        }
    }

    public override bool Activate()
    {
        base.Activate();

        isGrappling = true;

        Vector3 nonVerticalVelocity = playerController.velocity;
        nonVerticalVelocity.y = 0;

        if (nonVerticalVelocity == Vector3.zero) grappleDist = playerController.transform.forward * grappleSpeed;
        else grappleDist = nonVerticalVelocity.normalized * grappleSpeed;

        grappleDist.y = 0;

        return true;
    }

    public override void EndAbility()
    {
        if (isGrappling)
        {
            isGrappling = false;
        }
    }
}
