// Made by Daniel


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicAir : RelicBase
{
    public float grappleRange = 100;
    private Vector3 grappleDir;
    public float grappleSpeed = 5;
    public float grappleSwing = 1;
    public float bounceForce = 500;
    public float damage = 10;

    public int maxHits = 1;
    private int hits;

    public override void Start()
    {
        base.Start();

        grappleSwing = Mathf.Clamp(grappleSwing, 0, 1);
        hits = maxHits;
        relicType = ElementTypes.Air;
    }

    private void FixedUpdate()
    {
        base.Update();

        if (inUse && grappleSwing < 1)
        {
            characterScript.SetVelocity(Vector3.Lerp(characterRigid.velocity, grappleDir * grappleSpeed, grappleSwing));
        }
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
            characterRigid.velocity = Vector3.Lerp(characterRigid.velocity, grappleDir * grappleSpeed, grappleSwing);

            inUse = true;
            readyToUse = false;
            characterScript.movementLocked = true;
            characterScript.impactDamage = damage;
            characterScript.immortal = true;

            sfxScript.PlaySFX3D(activateSound, user.transform.position);

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
                characterScript.immortal = false;
                base.EndAbility();
            }
        }
    }
}
