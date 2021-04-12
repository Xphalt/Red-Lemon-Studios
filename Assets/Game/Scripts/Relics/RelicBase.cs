using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static EnumHelper;

public class RelicBase : MonoBehaviour
{
    internal GameObject user = null;
    internal string userName = null;
    protected CharacterBase characterScript;
    protected Rigidbody characterRigid;
    public Animator myAnim;

    public ElementTypes relicType;

    internal bool inUse;
    internal bool collected = false;
    internal bool spawned = false;
    public bool inArena;

    public int maxCombo = 1;
    public float percentIncreasePerHit = 0;
    public float damagePercentRecievedOnMiss = 0;
    public bool missPenalty = false;

    public int maxJumps = 1;
    public float knockBackMultiplier = 1;

    public float damageRecievedMultiplier = 1;
    public float speedMultiplier = 1;

    internal float cooldownTimer = 0;
    public float relicCooldownDuration;
    private float lastEquippedTime;

    internal bool readyToUse = false;

    public Quaternion collectedRotation;
    public float rotationSpeed;

    public SFXScript sfxScript = null;
    
    public string collectionSound;
    public string activateSound;

    public virtual void Awake() {}

    public virtual void Start()
    {
        if (sfxScript == null) sfxScript = GameObject.FindGameObjectWithTag("SFXManager").GetComponent<SFXScript>();
        if (!myAnim) myAnim = GetComponent<Animator>();
        if (user == null)
        {
            if (inArena) gameObject.SetActive(spawned && !collected);
            else gameObject.SetActive(false);
        }
    }

    public virtual void SetUser(GameObject newUser, bool playSound=false)
    {
        if (playSound) sfxScript.PlaySFX2D(collectionSound);
        collected = true;
        user = newUser;
        userName = newUser.name;
        characterScript = user.GetComponent<CharacterBase>();
        characterRigid = user.GetComponent<Rigidbody>();
        readyToUse = true;
        transform.localRotation = collectedRotation;
        gameObject.GetComponent<ParticleSystem>().Clear();
        var em = gameObject.GetComponent<ParticleSystem>().emission;
        em.enabled = false;
        gameObject.GetComponent<ParticleSystem>().Stop();
    }

    public virtual void Update()
    {
        if (!collected) transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        Cooldown();
    }

    virtual public bool Activate() 
    {
        return readyToUse && !inUse; 
    }

    virtual public void EndAbility()
    {
        inUse = false;
    }

    public void Cooldown()
    {
        if (!readyToUse && user != null)
        {
            cooldownTimer += Time.deltaTime;
           
            if (cooldownTimer > relicCooldownDuration)
            {
                readyToUse = true;
                cooldownTimer = 0;
            }
        }
    }

    public void ReEquip()
    {
        if (!readyToUse)
        {
            cooldownTimer += Time.time - lastEquippedTime;

            if (cooldownTimer > relicCooldownDuration)
            {
                readyToUse = true;
                cooldownTimer = 0;
            }
        }
    }

    public void Unequip()
    {
        lastEquippedTime = Time.time;
    }

    public void SaveRelic(string saveID)
    {
        saveID = saveID + relicType.ToString() + "Relic";
        SaveManager.UpdateSavedString(saveID + "User", userName);
        SaveManager.UpdateSavedBool(saveID + "Spawned", spawned);
        SaveManager.UpdateSavedBool(saveID + "Collected", collected);
        SaveManager.UpdateSavedBool(saveID + "InUse", inUse);
        SaveManager.UpdateSavedBool(saveID + "ReadyToUse", readyToUse);
        SaveManager.UpdateSavedFloat(saveID + "CooldownTimer", cooldownTimer);
    }

    public void LoadRelic(string loadID)
    {
        loadID = loadID + relicType.ToString() + "Relic";
        userName = SaveManager.GetString(loadID + "User");
        spawned = SaveManager.GetBool(loadID + "Spawned");
        collected = SaveManager.GetBool(loadID + "Collected");
        inUse = SaveManager.GetBool(loadID + "InUse");
        readyToUse = SaveManager.GetBool(loadID + "ReadyToUse");
        cooldownTimer = SaveManager.GetFloat(loadID + "CooldownTimer");

        if (collected)
        {
            GameObject newUser = GameObject.Find(userName);
            newUser.GetComponent<CharacterBase>().AddRelic(gameObject);
        }

        else gameObject.SetActive(spawned && inArena);
    }
}