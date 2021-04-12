/// <summary>
/// 
/// Script made by Linden and Daniel
/// 
/// By inheriting from the relic base
/// script, relics are able to be created
/// very easily and quickly
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicFire : RelicBase
{
    public float dashSpeed;
    public float dashDuration;
    public float postDashMomentum; //Veclocity retained by user after dash (0-1)
    private float dashTimer;

    public float damage;

    private Vector3 dashDist;

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

        Vector3 nonVerticalDirection = characterRigid.velocity;
        nonVerticalDirection.y = 0;

        if (nonVerticalDirection == Vector3.zero) nonVerticalDirection = user.transform.forward;

        dashDist = nonVerticalDirection.normalized * dashSpeed;

        characterScript.Shift(dashDist, dashDuration, postDashMomentum);
        characterScript.impactDamage = damage;
        characterScript.Immortalise();

        inUse = true;
        readyToUse = false;

        sfxScript.PlaySFX2D(activateSound);

        if (myAnim) myAnim.SetTrigger("Activate");

        return true;
    }

    public override void EndAbility()
    {
        base.EndAbility();
        dashTimer = 0;
        characterScript.impactDamage = 0;
        characterScript.Mortalise();
    }
}