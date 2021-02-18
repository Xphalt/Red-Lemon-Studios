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
        if (isGrappling)
        {
            playerController.Move(grappleDist * Time.deltaTime);
        }
    }

    public override bool Activate()
    {
        base.Activate();

        if (!isGrappling)
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ViewportPointToRay(crosshairPos), grappleRange);
            if (hits.Length > 0)
            {
                RaycastHit target = hits[0];

                grappleDist = (target.point - player.transform.position).normalized * grappleSpeed;

                savedGravityMultiplier = fpsScript.m_GravityMultiplier;
                fpsScript.m_GravityMultiplier = 0;

                isGrappling = true;

                return true;
            }
        }
        return false;
    }

    public override void EndAbility()
    {
        base.EndAbility();

        if (isGrappling)
        {
            hits--;
            if (hits == 0)
            {
                isGrappling = false;
                fpsScript.m_GravityMultiplier = savedGravityMultiplier;

                hits = maxHits;
            }
        }
    }
}
