using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class CharacterBase : MonoBehaviour
{
    protected Rigidbody characterRigid;

    public Transform relicPlaceHolder;
    public GameObject weapon = null;
    
    public bool canFly = false;

    public float jumpForce;
    public float gravityMult = 1;
    public float floorDistance;
    public float airControl;
    public float maxHealth = 100.0f;
    public float knockbackRecovery; //How quickly AI returns to normal velocity after being knocked back (0-1)

    internal bool movementLocked = false;
    internal bool shifting; 
    internal bool immortal = false;
    internal float shiftDuration;
    internal float shiftingTimer;
    internal float shiftTransition;
    internal float postShiftMomentum;
    internal float curHealth; 
    internal float impactDamage = 0;
    internal int hitCombo = 0;

    internal Teams team;
    internal ElementShooting shooter;
    internal List<RelicBase> relicList = new List<RelicBase>();
    internal RelicBase currentRelic = null;
    internal Vector3 shiftVector;

    protected bool missPenalty = false;
    protected bool doubleJumpEnabled = false;
    protected bool hasJumpedTwice;
    protected bool isGrounded = false;
    protected bool jumping = false;
    protected int relicIndex = 0;
    protected int maxCombo = 1;
    protected float percentIncreasePerHit = 0;
    protected float damagePercentRecievedOnMiss = 0;
    protected float knockBackMultiplier = 1;
    protected float damageRecievedMultiplier = 1;
    protected float speedMultiplier = 1;

    public GameObject SFXManager = null;
    protected SFXScript sfxSctipt;

    public virtual void Start()
    {
        characterRigid = GetComponent<Rigidbody>();
        characterRigid.useGravity = !canFly;
        airControl = Mathf.Clamp(airControl, 0, 1);
        if (weapon != null) shooter = weapon.GetComponent<ElementShooting>();

        if (SFXManager == null) SFXManager = GameObject.FindGameObjectWithTag("SFXManager");
        sfxSctipt = SFXManager.GetComponent<SFXScript>();

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
            characterRigid.velocity = Vector3.Lerp(characterRigid.velocity, shiftVector, shiftTransition);

            if (shiftingTimer > shiftDuration)
                EndShift();
        }

        if (jumping)
        {
            characterRigid.AddForce(Vector3.up * (jumpForce + characterRigid.velocity.y * characterRigid.mass));
            jumping = false;
        }

        if (!canFly) characterRigid.AddForce(Physics.gravity * (gravityMult - 1), ForceMode.Acceleration);
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
        RaycastHit[] floorHits = Physics.RaycastAll(new Ray(transform.position, Vector3.down), floorDistance);
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

    public void Shift(Vector3 force, float duration, float momentumCarryOver = 1, float transition = 1, bool hostile = false)
    {
        movementLocked = true;
        shifting = true;
        shiftDuration = duration;
        shiftVector = force;
        shiftTransition = transition;

        if (hostile) shiftVector *= knockBackMultiplier;
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
        if (!immortal) curHealth -= value * damageRecievedMultiplier;        
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

    public virtual void AddRelic(GameObject newRelic)
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
        else newRelic.SetActive(false);
    }

    public virtual void ChangeRelic(int cycleAmount = 1)
    {
        if (Mathf.Abs(cycleAmount) != relicList.Count && relicList.Count > 0)
        {
            relicIndex += cycleAmount;

            if (relicIndex >= relicList.Count)
            {
                relicIndex %= relicList.Count;
            }
            else if (relicIndex < 0)
            {
                relicIndex -= relicList.Count * Mathf.FloorToInt((float)relicIndex / (float)relicList.Count); // Add at least one lot of total relic number
            }

            SetRelic(relicIndex);
        }
    }

    public void SetRelic(int index)
    {
        currentRelic.EndAbility();
        currentRelic.Unequip();
        currentRelic.gameObject.SetActive(false);

        relicIndex = index;
        currentRelic = relicList[relicIndex];
        currentRelic.ReEquip();
        currentRelic.gameObject.SetActive(true); 
        ActivatePassives();
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
