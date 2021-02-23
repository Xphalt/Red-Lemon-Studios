/// <summary>
/// 
/// Script made by Zack
/// 
/// Linden added enemy death
/// when their health reaches 0
/// 
/// Daniel added enemies being
/// effected by ailments from
/// elemental ammo
/// 
/// </summary>

using UnityEngine;
using static EnumHelper;

public class Enemy : CharacterBase
{
    public enum EnemyStates { Idle, Chasing, Patrolling, Fleeing, EnemyStatesSize };

    public GameObject target;
    public ElementTypes elementType;
    protected ElementTypes weakAgainst;
    protected ElementTypes strongAgainst;

    protected Player playerScript;

    public float strongAgainstResist = 0.7f;
    public float weakAgainstIncrease = 1.5f;

    public float attackInterval;
    protected float attackTimer = 0;

    protected EnemyStates movementState;

    public float chaseSpeed = 5;
    public float patrolSpeed = 2;
    public float playerDetectionRadius = 50;
    public float wallDetectionRadius = 5;
    public bool canFly = false;

    protected float statusDuration;
    protected float statusTimer;
    protected bool statusEffectActive;
    protected float statusMagnitude;

    protected float DOTTimer;
    protected float DOTInterval = 1; //Placeholder. Not sure how it will be implemented long-term 

    public override void Start()
    {
        base.Start();
        team = Teams.Enemy;

        target = GameObject.FindGameObjectWithTag("Player");
        playerScript = target.GetComponent<Player>();

        characterRigid.velocity = transform.forward * chaseSpeed;
    }

    public override void Update()
    {
        base.Update();

        attackTimer += Time.deltaTime;

        if (statusEffectActive)
        {
            statusTimer += Time.deltaTime;
            if (statusTimer > statusDuration)
            {
                EndSatusEffect();
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 newVelocity = characterRigid.velocity;

        switch(movementState)
        {
            case EnemyStates.Chasing:
                newVelocity = (target.transform.position - transform.position).normalized * chaseSpeed;
                break;

            case EnemyStates.Fleeing:
                newVelocity = (transform.position - target.transform.position).normalized * chaseSpeed;
                break;

            case EnemyStates.Patrolling:
                Vector3 patrolDirection = characterRigid.velocity;
                patrolDirection.y = 0;

                patrolDirection.Normalize();

                if (patrolDirection == Vector3.zero) patrolDirection = transform.forward;

                if (Physics.Raycast(transform.position, patrolDirection, wallDetectionRadius))
                {
                    newVelocity = Vector3.Cross(patrolDirection, Vector3.up).normalized * patrolSpeed;
                    if (Random.Range(0, 2) == 1) newVelocity *= -1;
                }
                else newVelocity = patrolDirection * patrolSpeed;
                
                break;

            case EnemyStates.Idle:
                newVelocity = Vector3.zero;
                break;
        }

        if (!canFly) newVelocity.y = characterRigid.velocity.y;
        
        SetVelocity(newVelocity);
    }

    public bool CanSeePlayer()
    {
        RaycastHit castHit;
        if (!Physics.Raycast(transform.position, (target.transform.position - transform.position), out castHit)) return false;

        return (castHit.transform.gameObject == target);
    }

    public float GetDistance()
    {
        return (target.transform.position - transform.position).magnitude;
    }

    public virtual bool Attack()
    {
        if (attackTimer < attackInterval || !CanSeePlayer()) return false;

        attackTimer = 0;
        return true;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.tag == "Hazard")
        {
            ElementHazardAilments hazardInfo = collision.gameObject.GetComponent<ElementHazardAilments>();

            if (hazardInfo.hasEffect && hazardInfo.damageType == weakAgainst && hazardInfo.team != team)
            {
                TriggerStatusEffect(hazardInfo);
            }
        }
    }

    public override void TakeDamage(float damage, ElementTypes damageType = ElementTypes.ElementTypesSize)
    {
        if (damageType == weakAgainst) damage *= weakAgainstIncrease;
        else if (damageType == strongAgainst) damage *= strongAgainstResist;

        base.TakeDamage(damage, damageType);

        if (curHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void TriggerStatusEffect(ElementHazardAilments effectStats) 
    {
        statusEffectActive = true;
        statusTimer = 0;
        statusDuration = effectStats.statusEffectDuration;
        statusMagnitude = effectStats.statusMagnitude;
    }

    public virtual void EndSatusEffect()
    {
        statusEffectActive = false;
        statusTimer = 0;
        statusMagnitude = 0;
        statusDuration = 0;
    }
}
