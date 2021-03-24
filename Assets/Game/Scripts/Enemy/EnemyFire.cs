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

        if (!sentryMode && movementState == EnemyStates.Chasing)
        {
            bool newJump = !jumping;
            Jump();

            newJump = jumping && newJump;
            if (newJump) sfxScript.PlaySFX3D(jumpSound, transform.position);

        }

        Animate();
    }

    public override void Animate()
    {
        base.Animate();
        Animator MyAnim = gameObject.GetComponent<Animator>();
        if (movementState == EnemyStates.Chasing)
        {
            MyAnim.SetBool("Motion", true);
            MyAnim.SetBool("Attacking", false);
            MyAnim.SetBool("JumpingDown", false);

            if (characterRigid.velocity.y > 0)
            {
                MyAnim.SetBool("JumpingUp", true);
                MyAnim.SetBool("Motion", false);
            }
            else if (characterRigid.velocity.y < 0)
            {
                MyAnim.SetBool("JumpingUp", false);
                MyAnim.SetBool("JumpingDown", true);
            }
        }
        else if (movementState == EnemyStates.Idle)
        {
            MyAnim.SetBool("Motion", false);
            MyAnim.SetBool("Attacking", false);
            MyAnim.SetBool("JumpingDown", false);
        }
        else if (movementState == EnemyStates.Patrolling)
        {
            MyAnim.SetBool("Motion", true);
            MyAnim.SetBool("Attacking", false);
            MyAnim.SetBool("JumpingDown", false);
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
        myAnim.SetBool("Ignited", false);
        canExplode = false;
    }

    public override void EndSatusEffect()
    {
        base.EndSatusEffect();
        myAnim.SetBool("Ignited", true);
        canExplode = true;
    }
}
