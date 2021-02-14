using UnityEngine;
using static EnumHelper;

//Zack Pilgrim

public class EnemyScript_Zack : MonoBehaviour
{
    public int HP = 100;
    private int damage = 10;
    public float moveSpeed = 3.0f;
    private bool chasing = false;

    Elements statusEffect;

    private float statusDuration;
    private float statusTimer;
    private bool statusEffectActive;
    private float statusMagnitude;

    private float DOTTimer;
    private float DOTInterval = 1; //Placeholder. Not sure how it will be implemented long-term 

    public GameObject target;
    public Rigidbody moving;

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
                case Elements.Fire:
                    DOTTimer += Time.deltaTime;
                    if (DOTTimer > DOTInterval)
                    {
                        TakeDamage((int)statusMagnitude);
                        DOTTimer = 0;
                    }
                    break;

                case Elements.Water:
                    break;

                case Elements.Earth:
                    moving.velocity *= statusMagnitude;
                    break;

                case Elements.Air:
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            BulletScript bulletInfo = collision.gameObject.GetComponent<BulletScript>();
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
            collision.gameObject.GetComponent<PlayerScript_Daniel>().TakeDamage(damage);
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
