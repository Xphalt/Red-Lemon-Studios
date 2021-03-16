using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyEarth : Enemy
{
    private bool rolling = false;
    public float rollSpeed = 15;
    public float rollDuration = 1;
    public float rollDamage = 25;


    public override void Start()
    {
        base.Start();
        elementType = ElementTypes.Earth;
        weakAgainst = ElementTypes.Air;
        strongAgainst = ElementTypes.Water;
    }

    public override void Update()
    {
        base.Update();

        if (attackTimer > rollDuration)
        {
            EndRoll();
        }

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
        }

        return false;
    }

    private void EndRoll()
    {
        rolling = false;
        impactDamage = 0;
        EndShift();
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);

        Shift((effectStats.gameObject.transform.position - transform.position).normalized * effectStats.statusMagnitude, effectStats.statusEffectDuration, (1-knockbackRecovery), 1, true);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        CharacterBase collisionCharacter;
        if (collision.gameObject.TryGetComponent<CharacterBase>(out collisionCharacter))
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
