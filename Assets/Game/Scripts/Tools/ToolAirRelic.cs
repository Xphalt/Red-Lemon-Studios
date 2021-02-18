// Made by Daniel


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAirRelic : ToolBase
{
    public float grappleRange;
    private Vector3 grappleDir;
    public float grappleSpeed;
    public float bounceForce;
    public Vector3 crosshairPos;

    public int maxHits;
    private int hits;

    public override void Start()
    {
        base.Start();
        hits = maxHits;
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

                grappleDir = (target.point - player.transform.position).normalized * grappleSpeed;

                playerRigid.useGravity = false;
                playerRigid.velocity = grappleDir * grappleSpeed;

                inUse = true;
                playerScript.movementLocked = true;

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
                playerRigid.useGravity = true;
                playerRigid.velocity = Vector3.zero;
                playerRigid.AddForce(Vector3.up * bounceForce);

                hits = maxHits;
                inUse = false;
                playerScript.movementLocked = false;
            }
        }
    }
}
