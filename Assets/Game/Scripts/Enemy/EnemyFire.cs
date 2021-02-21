using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyFire : Enemy
{
    public float explosionDamage;
    public float explosionRadius;
    private bool canExplode = true;

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

        if (CanSeePlayer()) actionState = EnemyStates.Chasing;
        else actionState = EnemyStates.Patrolling;
        Jump();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject == target) Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (canExplode && GetDistance() < explosionRadius) playerScript.TakeDamage(explosionDamage);
    }

    public override void TriggerStatusEffect(ElementAmmoAilments effectStats)
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
