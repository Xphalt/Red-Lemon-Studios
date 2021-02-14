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

    ElementTypes statusEffect;

    public int HP = 100;
    private int damage = 10;
    public float moveSpeed = 3.0f;
    private bool chasing = false;

    private float statusDuration;
    private float statusTimer;
    private bool statusEffectActive;
    private float statusMagnitude;

    private float DOTTimer;
    private float DOTInterval = 1; //Placeholder. Not sure how it will be implemented long-term 

    void Start()
    {
        moving = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (chasing)
        {
            moving.velocity = (target.transform.position - gameObject.transform.position).normalized * moveSpeed;
        }
        else if (!chasing)
        {
            moving.velocity = Vector3.zero;
        }

        if (statusEffectActive)
        {
            statusTimer += Time.deltaTime;
            if (statusTimer > statusDuration)
            {
                statusEffectActive = false;
                statusTimer = 0;
                DOTTimer = 0;
            }

            else switch(statusEffect)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            ElementAmmoAilments bulletInfo = collision.gameObject.GetComponent<ElementAmmoAilments>();
            TakeDamage(bulletInfo.damage);

            if (bulletInfo.hasEffect)
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

    private void OnTriggerEnter(Collider bubble)
    {
        if (bubble.tag == "Player")
        {
            chasing = true;
        }
        
    }

    private void OnTriggerExit(Collider bubble)
    {
        if (bubble.tag == "Player")
        {
            chasing = false;
        }
    }
}
