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
    public float damage;

    public int maxHits;
    private int hits;

    private void Start()
    {
        hits = maxHits;
        relicType = ElementTypes.Air;
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        RaycastHit[] grappleHits = Physics.RaycastAll(characterScript.GetForwardRay(), grappleRange);
        if (grappleHits.Length > 0)
        {
            RaycastHit target = grappleHits[0];

            grappleDir = (target.point - user.transform.position).normalized * grappleSpeed;

            characterRigid.useGravity = false;
            characterRigid.velocity = grappleDir * grappleSpeed;

            inUse = true;
            readyToUse = false;
            characterScript.movementLocked = true;
            characterScript.impactDamage = damage;

            return true;
        }

        return false;
    }

    public override void EndAbility()
    {
        if (inUse)
        {
            hits--;
            if (hits == 0)
            {
                characterRigid.useGravity = true;

                characterRigid.AddForce(Vector3.up * bounceForce);

                hits = maxHits;
                characterScript.movementLocked = false;
                characterScript.impactDamage = 0;
                base.EndAbility();
            }
        }
    }
}
