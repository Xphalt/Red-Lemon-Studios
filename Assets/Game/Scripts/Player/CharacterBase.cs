using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class CharacterBase : MonoBehaviour
{
    protected Rigidbody characterRigid;

    public float jumpForce;
    public float gravityMult = 1;
    public float floorDistance;

    protected bool hasJumpedTwice;
    protected bool isGrounded = false;
    protected bool jumping = false;
    internal bool movementLocked = false;

    internal bool shifting;
    internal Vector3 shiftingVector;
    internal float shiftingDuration;
    internal float shiftingTimer;

    public float airControl;

    internal int hitCombo = 0;

    public GameObject weapon = null;
    internal ElementShooting shooter;
    internal Teams team;

    public float maxHealth = 100.0f;
    protected float curHealth;

    protected int maxCombo = 1;
    protected float percentIncreasePerHit = 0;
    protected float damagePercentRecievedOnMiss = 0;
    protected bool missPenalty = false;

    protected bool doubleJumpEnabled = false;
    protected float knockBackMultiplier = 1;

    protected float damageRecievedMultiplier = 1;
    protected float speedMultiplier = 1;

    public virtual void Start()
    {
        characterRigid = GetComponent<Rigidbody>();
        airControl = Mathf.Clamp(airControl, 0, 1);
        if (weapon != null) shooter = weapon.GetComponent<ElementShooting>();

        curHealth = maxHealth;
    }
    
    public virtual void Update()
    {
        CheckGround();
    }

    public virtual void FixedUpdate()
    {
        if (shifting)
        {
            shiftingTimer += Time.deltaTime;
            characterRigid.AddForce(shiftingVector);

            if (shiftingTimer > shiftingDuration)
            {
                movementLocked = false;
                shifting = false;
                shiftingTimer = 0;
            }
        }

        if (jumping)
        {
            characterRigid.AddForce(Vector3.up * (jumpForce + characterRigid.velocity.y * characterRigid.mass));
            jumping = false;
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            ElementAmmoAilments bulletInfo = collision.gameObject.GetComponent<ElementAmmoAilments>();

            if (bulletInfo.team != team)
            {
                TakeDamage(bulletInfo.damage);
                bulletInfo.RegisterHit();
            }
        }
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        if (!isGrounded)
        {
            newVelocity = Vector3.Lerp(characterRigid.velocity, newVelocity, airControl);
        }

        characterRigid.velocity = newVelocity;
    }

    protected void CheckGround()
    {
        RaycastHit[] floorHits = Physics.RaycastAll(new Ray(transform.position, -Vector3.up), floorDistance);
        isGrounded = false;
        foreach (RaycastHit floorHit in floorHits)
        {
            if (floorHit.transform.CompareTag("Floor"))
            {
                isGrounded = true;
                hasJumpedTwice = false;
                break;
            }
        }
    }

    protected void Jump()
    {
        if ((isGrounded || doubleJumpEnabled && !hasJumpedTwice) && !jumping)
        {
            jumping = true;
            hasJumpedTwice = !isGrounded;
        }
    }

    public void Shift(Vector3 force, float duration, bool hostile = false, bool lockMovement = false)
    {
        movementLocked = lockMovement;
        shifting = true;
        shiftingDuration = duration;
        shiftingVector = force;

        if (hostile) shiftingVector *= knockBackMultiplier;
    }

    public float CalculateDamageMult()
    {
        return 1 + Mathf.Min(hitCombo, maxCombo) * percentIncreasePerHit / 100;
    }

    public void IncreaseCombo()
    {
        if (hitCombo < maxCombo) hitCombo++;
    }

    public void MissShot(float bulletDamage)
    {
        if (missPenalty)
        {
            TakeDamage(bulletDamage * (damagePercentRecievedOnMiss / 100));
            hitCombo = 0;
        }
    }

    public virtual void TakeDamage(float value, ElementTypes damageType = ElementTypes.ElementTypesSize)
    {
        curHealth -= value * damageRecievedMultiplier;        
    }

    public virtual void AddHealth(float value)
    {
        curHealth += value;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
    }
}
