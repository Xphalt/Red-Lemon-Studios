﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyEarth : Enemy
{
    private bool rolling = false;
    public float rollSpeed = 15;
    public float rollDuration = 1;
    public float rollDamage = 25;

    public string chargeSound;
    public string chargeEndSound;


    public override void Start()
    {
        base.Start();
        elementType = ElementTypes.Earth;
        weakAgainst = ElementTypes.Air;
        strongAgainst = ElementTypes.Water;
        canFly = false;
    }

    public override void Update()
    {
        base.Update();

        if (attackTimer > rollDuration)
        {
            rolling = false;
            impactDamage = 0;
        }
        Animate();
        if (!rolling)
        {
            if (CanSeePlayer()) movementState = EnemyStates.Chasing;
            else if (!sentryMode) movementState = EnemyStates.Patrolling;

            Attack();
        }
    }

    public override bool Attack()
    {
        if (base.Attack() && CanSeePlayer())
        {
            rolling = true;
            impactDamage = rollDamage;
            Shift((target.transform.position - transform.position).normalized * rollSpeed, rollDuration, 0);

            sfxScript.PlaySFX3D(chargeSound, transform.position);
        }

        return false;
    }

    private void EndRoll()
    {
        rolling = false;
        impactDamage = 0;
        EndShift();

        sfxScript.PlaySFX3D(chargeEndSound, transform.position);
    }

    public override void Animate()
    {
        base.Animate();
        Animator MyAnim = gameObject.GetComponent<Animator>();
        if (movementState == EnemyStates.Chasing)
        {
            MyAnim.SetBool("Motion", true);
            MyAnim.SetBool("Attacking", true);
        }
        else if (movementState == EnemyStates.Idle)
        {
            MyAnim.SetBool("Motion", false);
            MyAnim.SetBool("Attacking", false);
        }
        else if (movementState == EnemyStates.Patrolling)
        {
            MyAnim.SetBool("Motion", true);
            MyAnim.SetBool("Attacking", false);
        }
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);

        Shift((effectStats.gameObject.transform.position - transform.position).normalized * effectStats.statusMagnitude, effectStats.statusEffectDuration, (1-knockbackRecovery), 1, true);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.TryGetComponent(out CharacterBase collisionCharacter))
        {
            if (collisionCharacter.team != team) EndRoll();
        }
    }

    public override void SaveEnemy(string saveID)
    {
        base.SaveEnemy(saveID);

        saveID = "Enemy" + saveID;
        SaveManager.UpdateSavedBool(saveID + "Rolling", rolling);

        if (!rolling && shifting) EndShift();
    }

    public override void LoadEnemy(string loadID)
    {
        base.LoadEnemy(loadID);

        loadID = "Enemy" + loadID;
        rolling = SaveManager.GetBool(loadID + "Rolling");
    }
}
