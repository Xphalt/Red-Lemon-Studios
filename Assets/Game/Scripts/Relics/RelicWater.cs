using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class RelicWater : RelicBase
{
    public float healAmount;
    public int ammoCost;

    public float healDuration;
    public float healTickInterval;
    private float healStartTimer;
    private float healTickTimer;

    public float inUseSpeedMultiplier;
    private float defaultSpeedMultiplier;

    public override void Start()
    {
        base.Start();

        relicType = ElementTypes.Water;
        defaultSpeedMultiplier = speedMultiplier;
    }

    public override void Update()
    {
        base.Update();

        if (inUse)
        {
            healStartTimer += Time.deltaTime;
            if (healStartTimer > healDuration) EndAbility();

            else
            {
                healTickTimer += Time.deltaTime;
                if (healTickTimer > healTickInterval)
                {
                    characterScript.AddHealth(healAmount, ammoCost, relicType);
                    healTickTimer = 0;
                }
            }
        }
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        inUse = true;
        readyToUse = false;

        speedMultiplier = inUseSpeedMultiplier;
        characterScript.ActivatePassives();

        sfxScript.PlaySFX3D(activateSound, user.transform.position);
        
        return true;
    }

    public override void EndAbility()
    {
        base.EndAbility();

        healStartTimer = 0;
        healTickTimer = 0;

        speedMultiplier = defaultSpeedMultiplier;
        characterScript.ActivatePassives();
    }
}
