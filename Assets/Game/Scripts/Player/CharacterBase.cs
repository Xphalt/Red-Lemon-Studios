using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class CharacterBase : MonoBehaviour
{
    protected Rigidbody characterRigid;

    public Transform relicPlaceHolder;

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
    internal float postShiftMomentum;

    public float airControl;

    internal int hitCombo = 0;

    public GameObject weapon = null;
    internal ElementShooting shooter;
    internal Teams team;

    public float maxHealth = 100.0f;
    protected float curHealth;

    internal List<RelicBase> relicList = new List<RelicBase>();
    protected int relicIndex = 0;
    internal RelicBase currentRelic = null;

    protected int maxCombo = 1;
    protected float percentIncreasePerHit = 0;
    protected float damagePercentRecievedOnMiss = 0;
    protected bool missPenalty = false;

    protected bool doubleJumpEnabled = false;
    protected float knockBackMultiplier = 1;

    protected float damageRecievedMultiplier = 1;
    protected float speedMultiplier = 1;

    internal float impactDamage = 0;

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
            characterRigid.velocity = shiftingVector;

            if (shiftingTimer > shiftingDuration)
            {
                EndShift();
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
        if (collision.gameObject.CompareTag("Hazard"))
        {
            ElementHazardAilments hazardInfo = collision.gameObject.GetComponent<ElementHazardAilments>();

            if (hazardInfo.team != team)
            {
                TakeDamage(hazardInfo.damage);
                hazardInfo.RegisterHit();
            }
        }

        if (currentRelic != null)
        {
            if (currentRelic.relicType == ElementTypes.Air && currentRelic.inUse) currentRelic.EndAbility();
        }

        CharacterBase collisionCharacter;
        if (collision.gameObject.TryGetComponent<CharacterBase>(out collisionCharacter))
        {
            if (collisionCharacter.team != team) collisionCharacter.TakeDamage(impactDamage);
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

    public void Shift(Vector3 force, float duration, float momentumCarryOver = 1, bool hostile = false)
    {
        movementLocked = true;
        shifting = true;
        shiftingDuration = duration;
        shiftingVector = force;

        if (hostile) shiftingVector *= knockBackMultiplier;
        postShiftMomentum = momentumCarryOver;
    }

    public void EndShift()
    {
        movementLocked = false;
        shifting = false;
        shiftingTimer = 0;

        characterRigid.velocity *= postShiftMomentum;
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

    public virtual void TakeDamage(float value, ElementTypes damageType=ElementTypes.ElementTypesSize)
    {
        curHealth -= value * damageRecievedMultiplier;        
    }

    public virtual void AddHealth(float value, int cost=0, ElementTypes costType=ElementTypes.ElementTypesSize)
    {
        curHealth += value;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
    }

    public virtual bool IsPlayer() 
    {
        return false;
    }

    #region Relic

    public void AddRelic(GameObject newRelic)
    {
        RelicBase relicScript = newRelic.GetComponent<RelicBase>();

        newRelic.transform.position = relicPlaceHolder.position;
        relicScript.SetUser(gameObject);

        relicList.Add(relicScript);

        if (currentRelic == null)
        {
            currentRelic = relicScript;
            ActivatePassives();
        }
        else newRelic.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ChangeRelic(int cycleAmount = 1)
    {
        if (cycleAmount != relicList.Count && relicList.Count > 0)
        {
            currentRelic.EndAbility();
            currentRelic.GetComponent<MeshRenderer>().enabled = false;
            relicIndex += cycleAmount;

            if (relicIndex >= relicList.Count)
            {
                relicIndex -= relicList.Count;
            }
            else if (relicIndex < 0)
            {
                relicIndex += relicList.Count;
            }

            currentRelic = relicList[relicIndex];
            currentRelic.GetComponent<MeshRenderer>().enabled = true;

            ActivatePassives();
        }
    }

    public void UseRelic()
    {
        if (currentRelic != null)
        {
            if (currentRelic.inUse) currentRelic.EndAbility();

            else currentRelic.Activate();
        }        
    }

    public virtual Ray GetForwardRay()
    {
        return new Ray(transform.position, transform.forward);
    }

    public void ActivatePassives()
    {
        maxCombo = currentRelic.maxCombo;
        percentIncreasePerHit = currentRelic.percentIncreasePerHit;
        damagePercentRecievedOnMiss = currentRelic.damagePercentRecievedOnMiss;
        missPenalty = currentRelic.missPenalty;

        doubleJumpEnabled = currentRelic.doubleJumpEnabled;
        knockBackMultiplier = currentRelic.knockBackMultiplier;

        damageRecievedMultiplier = currentRelic.damageRecievedMultiplier;
        speedMultiplier = currentRelic.speedMultiplier;

        hitCombo = 0;
    }
    #endregion

}
