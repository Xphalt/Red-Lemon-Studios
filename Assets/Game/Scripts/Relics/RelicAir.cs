using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicAir : RelicBase
{    
    private Vector3 grappleDir;
    private Vector3 startPos;
    private int hits;

    public float grappleRange = 100;
    public float grappleSpeed = 5;
    public float grappleSwing = 1;
    public float bounceSpeed = 10;
    public float damage = 10;
    public int maxHits = 1;

    public override void Start()
    {
        base.Start();

        grappleSwing = Mathf.Clamp(grappleSwing, 0, 1);
        hits = maxHits;
        relicType = ElementTypes.Air;
    }

    private void FixedUpdate()
    {
        if (inUse)
        {
            if (grappleSwing < 1) characterScript.SetVelocity(Vector3.Lerp(characterRigid.velocity, grappleDir * grappleSpeed, grappleSwing));
            if ((user.transform.position - startPos).magnitude > grappleRange) EndAbility();
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

            if (characterScript.isGrounded && grappleDir.y < 0) return false;

            characterRigid.useGravity = false;
            characterRigid.velocity = Vector3.Lerp(characterRigid.velocity, grappleDir * grappleSpeed, grappleSwing);

            SetActive();

            characterScript.movementLocked = true;
            characterScript.impactDamage = damage;

            startPos = user.transform.position;

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

                characterRigid.velocity = new Vector3(characterRigid.velocity.x, bounceSpeed, characterRigid.velocity.z);
                hits = maxHits;
                characterScript.movementLocked = false;
                characterScript.impactDamage = 0;
                base.EndAbility();
            }
        }
    }
}
