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

public class Enemy : MonoBehaviour
{
    public GameObject target;
    protected Rigidbody moving;
    public ElementTypes elementType;
    protected ElementTypes weakAgainst;
    protected ElementTypes strongAgainst;

    protected Player playerScript;

    public float strongDamageResist;
    public float weakDamageIncrease;

    ElementTypes statusEffect;

    public int HP = 100;
    public int damage = 10;

    public float attackInterval;
    private float attackTimer = 0;

    public float moveSpeed = 3.0f;
    public float playerDetectionRadius;
    public float wallDetectionRadius;

    protected float statusDuration;
    protected float statusTimer;
    protected bool statusEffectActive;
    protected float statusMagnitude;

    private float DOTTimer;
    private float DOTInterval = 1; //Placeholder. Not sure how it will be implemented long-term 

    public virtual void Start()
    {
        moving = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player");
        playerScript = target.GetComponent<Player>();

        moving.velocity = transform.forward * moveSpeed;
    }

    public virtual void Update()
    {
        attackTimer += Time.deltaTime;

        if (statusEffectActive)
        {
            statusTimer += Time.deltaTime;
            if (statusTimer > statusDuration)
            {
                statusEffectActive = false;
                statusTimer = 0;
                DOTTimer = 0;
            }
        }
    }

    public void ChasePlayer(bool runAway = false)
    {
        if (CanSeePlayer())
        {
            Vector3 newVelocity = (target.transform.position - gameObject.transform.position).normalized * moveSpeed;

            if (runAway) newVelocity *= -1;
            newVelocity.y = moving.velocity.y;
            moving.velocity = newVelocity;
        }
        else
        {
            moving.velocity = Vector3.zero;
        }
    }

    public bool CanSeePlayer()
    {
        RaycastHit castHit;
        if (!Physics.Raycast(transform.position, (target.transform.position - transform.position), out castHit)) return false;

        return (castHit.transform.gameObject == target);
    }

    public void Patrol()
    {
        if (moving.velocity == Vector3.zero) moving.velocity = transform.forward * moveSpeed;

        if (Physics.Raycast(transform.position, moving.velocity, wallDetectionRadius))
        {
            Vector3 newVelocity = Vector3.Cross(moving.velocity, Vector3.up).normalized;

            if (Random.Range(0, 2) == 1) newVelocity *= -1;

            newVelocity *= moveSpeed;
            newVelocity.y = moving.velocity.y;

            moving.velocity = newVelocity;

        }

        else moving.velocity = moving.velocity.normalized * moveSpeed;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            ElementAmmoAilments bulletInfo = collision.gameObject.GetComponent<ElementAmmoAilments>();
            TakeDamage(bulletInfo.damage);
            bulletInfo.RegisterHit();

            if (bulletInfo.hasEffect && bulletInfo.damageType == weakAgainst)
            {
                TriggerStatusEffect(bulletInfo);
            }
        }

        else if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }

    void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void TriggerStatusEffect(ElementAmmoAilments effectStats) { }
}
