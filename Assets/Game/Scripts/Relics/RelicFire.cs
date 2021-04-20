using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicFire : RelicBase
{
    private Vector3 dashDist;
    private float dashTimer;

    public float dashSpeed;
    public float dashDuration;
    public float postDashMomentum; //Veclocity retained by user after dash (0-1)
    public float damage;

    public override void Start()
    {
        base.Start();

        relicType = ElementTypes.Fire;
    }

    public override void Update()
    {
        base.Update();

        if (inUse)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer > dashDuration) EndAbility();
        }
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        Vector3 nonVerticalDirection = characterScript.targetDirection;
        nonVerticalDirection.y = 0;

        if (nonVerticalDirection == Vector3.zero) nonVerticalDirection = user.transform.forward;

        dashDist = nonVerticalDirection.normalized * dashSpeed;

        characterScript.Shift(dashDist, dashDuration, postDashMomentum);
        characterScript.impactDamage = damage;

        SetActive();

        return true;
    }

    public override void EndAbility()
    {
        base.EndAbility();
        dashTimer = 0;
        characterScript.impactDamage = 0;
    }
}