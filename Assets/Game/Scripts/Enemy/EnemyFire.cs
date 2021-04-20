using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EnemyFire : Enemy
{
    private bool canExplode = true;

    public GameObject explosion;
    public float explosionDamage;
    public float explosionRadius;
    public string explodeSound;
    public string jumpSound;

    public override void Awake()
    {
        base.Awake();

        if (explosion == null) explosion = GetComponentInChildren<FireExplosion>().gameObject;
        explosion.SetActive(false);
    }

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
        if (myAnim)
        {
            if (movementState == EnemyStates.Chasing)
            {
                myAnim.SetBool("Motion", true);
                myAnim.SetBool("Attacking", false);
                myAnim.SetBool("JumpingDown", false);

                if (characterRigid.velocity.y > 0)
                {
                    myAnim.SetBool("JumpingUp", true);
                    myAnim.SetBool("Motion", false);
                }
                else if (characterRigid.velocity.y < 0)
                {
                    myAnim.SetBool("JumpingUp", false);
                    myAnim.SetBool("JumpingDown", true);
                }
            }
            else if (movementState == EnemyStates.Idle)
            {
                myAnim.SetBool("Motion", false);
                myAnim.SetBool("Attacking", false);
                myAnim.SetBool("JumpingDown", false);
            }
            else if (movementState == EnemyStates.Patrolling)
            {
                myAnim.SetBool("Motion", true);
                myAnim.SetBool("Attacking", false);
                myAnim.SetBool("JumpingDown", false);
            }
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
        if (canExplode)
        {
            if (GetDistance() < explosionRadius) playerScript.TakeDamage(explosionDamage);

            sfxScript.PlaySFX3D(explodeSound, transform.position);
            explosion.SetActive(true);
            explosion.transform.SetParent(null);
            explosion.GetComponent<FireExplosion>().parent = gameObject.transform;
        }
        gameObject.SetActive(false);
        killed = true;
    }

    public override void TriggerStatusEffect(ElementHazardAilments effectStats)
    {
        base.TriggerStatusEffect(effectStats);
        if (myAnim != null) myAnim.SetBool("Ignited", false);
        canExplode = false;
    }

    public override void EndSatusEffect()
    {
        base.EndSatusEffect();
        if (myAnim != null) myAnim.SetBool("Ignited", true);
        canExplode = true;
    }
}
