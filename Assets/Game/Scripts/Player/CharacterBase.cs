﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class CharacterBase : MonoBehaviour
{
    internal Teams team;
    internal ElementShooting shooter;
    internal List<RelicBase> relicList = new List<RelicBase>();
    internal RelicBase currentRelic = null;
    internal Vector3 shiftVector;
    internal Vector3 targetDirection;
    internal bool isGrounded = false;
    internal bool movementLocked = false;
    internal bool shifting; 
    internal bool immortal = false;
    internal bool killed = false;
    internal float shiftDuration;
    internal float shiftingTimer;
    internal float shiftTransition;
    internal float postShiftMomentum;
    internal float curHealth; 
    internal float impactDamage = 0;
    internal int hitCombo = 0;

    protected bool missPenalty = false;
    protected bool jumping = false;
    protected int maxJumps = 1;
    protected int currentJumps = 0;
    protected int relicIndex = 0;
    protected int maxCombo = 1;
    protected float percentIncreasePerHit = 0;
    protected float damagePercentRecievedOnMiss = 0;
    protected float knockBackMultiplier = 1;
    protected float damageRecievedMultiplier = 1;
    protected float speedMultiplier = 1;

    public SFXScript sfxScript;
    public Rigidbody characterRigid = null;
    public Transform relicPlaceHolder;
    public GameObject weapon = null;
    public List<string> dontJumpFrom = new List<string>();
    public bool canFly = false;
    public float jumpSpeed;
    public float gravityMult = 1;
    public float floorDistance;
    public float airControl;
    public float maxHealth = 100.0f;
    public float knockbackRecovery; //How quickly AI returns to normal velocity after being knocked back (0-1)

    public virtual void Awake()
    {
        if (weapon != null) shooter = weapon.GetComponent<ElementShooting>();
        if (characterRigid != null) characterRigid = GetComponent<Rigidbody>();

        if (sfxScript == null) sfxScript = FindObjectOfType<SFXScript>();
        characterRigid = GetComponent<Rigidbody>();
        curHealth = maxHealth;
    }

    public virtual void Start()
    {
        characterRigid.useGravity = !canFly;
        airControl = Mathf.Clamp(airControl, 0, 1);
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
            characterRigid.velocity = new Vector3(characterRigid.velocity.x, jumpSpeed, characterRigid.velocity.z);
            jumping = false;
        }

        if (!canFly && characterRigid.useGravity) characterRigid.AddForce(Physics.gravity * (gravityMult - 1), ForceMode.Acceleration);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out ElementHazardAilments effectStats))
        {
            if (effectStats.team != team)
            {
                TakeDamage(effectStats.damage, effectStats.damageType);
                effectStats.RegisterHit();
            }
        }

        if (currentRelic != null)
        {
            if (currentRelic.relicType == ElementTypes.Air && currentRelic.inUse)
            {
                if (collision.gameObject.TryGetComponent(out Interactable_Items interactable))
                {
                    IncreaseCombo();
                    interactable.Drop();
                }
                currentRelic.EndAbility();
            }

        }

        if (collision.gameObject.TryGetComponent(out CharacterBase collisionCharacter) && impactDamage != 0)
        {
            if (collisionCharacter.team != team)
            {
                collisionCharacter.TakeDamage(impactDamage, (currentRelic != null) ? currentRelic.relicType : ElementTypes.ElementTypesSize);
                IncreaseCombo();
            }
        }
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        targetDirection = newVelocity;
        if (!isGrounded)
        {
            newVelocity = Vector3.Lerp(characterRigid.velocity, newVelocity, airControl);
        }

        characterRigid.velocity = newVelocity;
    }

    protected void CheckGround()
    {
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit, floorDistance)) isGrounded = !dontJumpFrom.Contains(hit.transform.tag);
        else isGrounded = false;

        if (isGrounded && !jumping) currentJumps = 0;
        else currentJumps = Mathf.Max(currentJumps, 1);
    }

    protected void Jump()
    {
        if (maxJumps > currentJumps && !jumping && !movementLocked)
        {
            jumping = true;
            currentJumps++;
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
        if (shifting)
        {
            movementLocked = false;
            shifting = false;
            shiftingTimer = 0;

            characterRigid.velocity *= postShiftMomentum;
        }
    }

    public float CalculateDamageMult()
    {
        return 1 + Mathf.Min(hitCombo, maxCombo) * percentIncreasePerHit / 100;
    }

    public virtual void IncreaseCombo()
    {
        hitCombo++;
    }

    public virtual void MissShot(float bulletDamage)
    {
        if (missPenalty) TakeDamage(bulletDamage * (damagePercentRecievedOnMiss / 100));
        hitCombo = 0;
    }

    public virtual void TakeDamage(float value, ElementTypes damageType=ElementTypes.ElementTypesSize)
    {
        if (!immortal) curHealth -= value * damageRecievedMultiplier;     
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Immortalise()
    {
        immortal = true;
    }

    public virtual void Mortalise()
    {
        immortal = false;
    }

    public virtual void Die()
    {
        curHealth = 0;
        killed = true;
    }

    public virtual bool AddHealth(float value, int cost=0, ElementTypes costType=ElementTypes.ElementTypesSize)
    {
        if (curHealth >= maxHealth) return false;

        curHealth += value;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }

        return true;
    }

    public virtual bool IsPlayer() 
    {
        return false;
    }

    #region Relic

    public virtual void AddRelic(GameObject newRelic, bool playSound=false)
    {
        RelicBase relicScript = newRelic.GetComponent<RelicBase>();

        relicScript.SetUser(gameObject, playSound);
        newRelic.transform.position = relicPlaceHolder.position;

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
        if (relicList.Count > 0)
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

        maxJumps = currentRelic.maxJumps;
        knockBackMultiplier = currentRelic.knockBackMultiplier;

        damageRecievedMultiplier = currentRelic.damageRecievedMultiplier;
        speedMultiplier = currentRelic.speedMultiplier;

        hitCombo = 0;
    }
    #endregion

}
