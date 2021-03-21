using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyFire : Enemy
{
    public float explosionDamage;
    public float explosionRadius;
    private bool canExplode = true;

    public string explodeSound;
    public string jumpSound;

    public override void Start()
    {
        base.Start();
        elementType = ElementTypes.Fire;
        weakAgainst = ElementTypes.Water;
        strongAgainst = ElementTypes.Air;
    }

    public override void Update()
    {
        base.Update();

        if (CanSeePlayer()) movementState = EnemyStates.Chasing;
        else if (!sentryMode) movementState = EnemyStates.Patrolling;

        if (!sentryMode)
        {
            bool newJump = !jumping;
            Jump();

            newJump = jumping && newJump;
            if (newJump) sfxScript.PlaySFX3D(jumpSound, transform.position);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject == target)
        {
            canExplode = true;
            Explode();
        }
    }

    public override void TakeDamage(float damage, ElementTypes damageType = ElementTypes.ElementTypesSize)
    {
        base.TakeDamage(damage, damageType);

        if (killed) Explode();
    }

    public void Explode()
    {
        if (canExplode && GetDistance() < explosionRadius)
        {
            playerScript.TakeDamage(explosionDamage);
            sfxScript.PlaySFX3D(explodeSound, transform.position);
        }
        gameObject.SetActive(false);
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);
        canExplode = false;
    }

    public override void EndSatusEffect()
    {
        base.EndSatusEffect();
        canExplode = true;
    }
}
