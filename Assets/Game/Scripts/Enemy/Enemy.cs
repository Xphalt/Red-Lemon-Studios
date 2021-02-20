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
    public Rigidbody moving;
    public ElementTypes elementType;
    protected ElementTypes weakAgainst;
    protected ElementTypes strongAgainst;

    public float strongDamageResist;
    public float weakDamageIncrease;

    ElementTypes statusEffect;

    public int HP = 100;
    public int damage = 10;
    public float attackInterval;
    public float moveSpeed = 3.0f;
    public float detectionRadius;

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
    }

    void Update()
    {
        ChasePlayer();

        if (statusEffectActive)
        {
            statusTimer += Time.deltaTime;
            if (statusTimer > statusDuration)
            {
                statusEffectActive = false;
                statusTimer = 0;
                DOTTimer = 0;
            }

            else switch (statusEffect)
                {
                    case ElementTypes.Fire:
                        DOTTimer += Time.deltaTime;
                        if (DOTTimer > DOTInterval)
                        {
                            TakeDamage((int)statusMagnitude);
                            DOTTimer = 0;
                        }
                        break;

                    case ElementTypes.Water:
                        break;

                    case ElementTypes.Earth:
                        moving.velocity *= statusMagnitude;
                        break;

                    case ElementTypes.Air:
                        break;
                }
        }
    }

    private void ChasePlayer()
    {
        if ((target.transform.position - gameObject.transform.position).magnitude < detectionRadius)
        {
            Vector3 newVelocity = (target.transform.position - gameObject.transform.position).normalized * moveSpeed;
            newVelocity.y = moving.velocity.y;
            moving.velocity = newVelocity;
        }
        else
        {
            moving.velocity = Vector3.zero;
        }
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
                statusEffect = bulletInfo.damageType;
                statusMagnitude = bulletInfo.statusMagnitude;
                statusDuration = bulletInfo.statusEffectDuration;
                statusEffectActive = true;
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
}
