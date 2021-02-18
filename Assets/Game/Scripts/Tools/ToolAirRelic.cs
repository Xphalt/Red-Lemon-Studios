// Made by Daniel


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAirRelic : ToolBase
{
    public float grappleRange;
    private Vector3 grappleDist;
    private bool isGrappling;
    public float grappleSpeed;
    private float savedGravityMultiplier;
    public Vector3 crosshairPos;

    public int maxHits;
    private int hits;

    public override void Start()
    {
        base.Start();
        hits = maxHits;
    }

    private void Update()
    {
        if (inUse)
        {
            playerController.Move(grappleDist * Time.deltaTime);
        }
    }

    public override bool Activate()
    {
        base.Activate();

        if (!inUse)
        {
            RaycastHit[] grappleHits = Physics.RaycastAll(Camera.main.ViewportPointToRay(crosshairPos), grappleRange);
            if (grappleHits.Length > 0)
            {
                RaycastHit target = grappleHits[0];

                grappleDist = (target.point - player.transform.position).normalized * grappleSpeed;

                savedGravityMultiplier = fpsScript.m_GravityMultiplier;
                fpsScript.m_GravityMultiplier = 0;

                if (playerController.isGrounded) hits++;

                inUse = true;

                return true;
            }
        }
        return false;
    }

    public override void EndAbility()
    {
        base.EndAbility();

        if (inUse)
        {
            hits--;
            if (hits == 0)
            {
                fpsScript.m_GravityMultiplier = savedGravityMultiplier;

                hits = maxHits;
                inUse = false;
            }
        }
    }
}
