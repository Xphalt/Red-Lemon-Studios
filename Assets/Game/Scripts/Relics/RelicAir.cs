// Made by Daniel


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicAir : RelicBase
{
    public float grappleRange;
    private Vector3 grappleDir;
    public float grappleSpeed;
    public float bounceForce;
    public Vector3 crosshairPos;

    public int maxHits;
    private int hits;

    private void Start()
    {
        hits = maxHits;
        relicType = ElementTypes.Air;
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

                grappleDir = (target.point - user.transform.position).normalized * grappleSpeed;

                characterRigid.useGravity = false;
                characterRigid.velocity = grappleDir * grappleSpeed;

                inUse = true;
                characterScript.movementLocked = true;

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
                characterRigid.useGravity = true;

                characterRigid.AddForce(Vector3.up * bounceForce);

                hits = maxHits;
                inUse = false;
                characterScript.movementLocked = false;
            }
        }
    }
}
