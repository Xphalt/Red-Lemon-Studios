using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicAir : RelicBase
{    
    private Vector3 grapplePoint;
    private Vector3 startPos;
    private int hits;
    private float grappleDuration;
    private float grappleTimer;

    public float grappleRange = 100;
    public float grappleSpeed = 25;
    public float grappleSwing = 1;
    public float bounceSpeed = 10;
    public float damage = 10;
    public int maxHits = 1;

    public override void Start()
    {
        base.Start();

        grappleSwing = Mathf.Clamp(grappleSwing, 0, 1);
        hits = maxHits;
        grappleDuration = grappleRange / grappleSpeed;
        relicType = ElementTypes.Air;
    }

    private void FixedUpdate()
    {
        if (inUse)
        {
            grappleTimer += Time.deltaTime;
            Vector3 grappleDir = (grapplePoint - user.transform.position).normalized;
            characterScript.SetVelocity(Vector3.Lerp(characterRigid.velocity, grappleDir * grappleSpeed, grappleSwing));
            if ((user.transform.position - startPos).magnitude > grappleRange || grappleTimer > grappleDuration) EndAbility();
        }
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        RaycastHit[] grappleHits = Physics.RaycastAll(characterScript.GetForwardRay(), grappleRange);
        if (grappleHits.Length > 0)
        {
            RaycastHit target = grappleHits[0];

            grapplePoint = target.point;
            startPos = user.transform.position;
            Vector3 grappleDir = (grapplePoint - startPos).normalized;

            characterRigid.useGravity = false;
            characterScript.SetVelocity(Vector3.Lerp(characterRigid.velocity, grappleDir * grappleSpeed, grappleSwing));

            SetActive();

            characterScript.movementLocked = true;
            characterScript.impactDamage = damage;

            grappleTimer = 0;
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
                grappleTimer = 0;
                characterScript.movementLocked = false;
                characterScript.impactDamage = 0;
                base.EndAbility();
            }
        }
    }
}
