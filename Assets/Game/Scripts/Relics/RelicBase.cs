using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static EnumHelper;

public class RelicBase : MonoBehaviour
{
    internal GameObject user = null;
    internal string userName;
    protected CharacterBase characterScript;
    protected Rigidbody characterRigid;
    public ElementTypes relicType;

    internal bool inUse;
    internal bool collected = false;
    public bool inArena;

    public int maxCombo = 1;
    public float percentIncreasePerHit = 0;
    public float damagePercentRecievedOnMiss = 0;
    public bool missPenalty = false;

    public bool doubleJumpEnabled = false;
    public float knockBackMultiplier = 1;

    public float damageRecievedMultiplier = 1;
    public float speedMultiplier = 1;

    internal float cooldownTimer = 0;
    public float relicCooldownDuration;
    private float lastEquippedTime;

    internal bool readyToUse = false;

    public virtual void Awake()
    {
        if (user == null) gameObject.SetActive(inArena);
    }

    public virtual void SetUser(GameObject newUser)
    {
        collected = true;
        user = newUser;
        userName = newUser.name;
        characterScript = user.GetComponent<CharacterBase>();
        characterRigid = user.GetComponent<Rigidbody>();
        readyToUse = true;
    }

    public virtual void Update()
    {
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
        cooldownTimer += Time.time - lastEquippedTime;
        if (cooldownTimer > relicCooldownDuration)
        {
            readyToUse = true;
            cooldownTimer = 0;
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
        SaveManager.UpdateSavedBool(saveID + "Collected", collected);
        SaveManager.UpdateSavedBool(saveID + "InUse", inUse);
        SaveManager.UpdateSavedBool(saveID + "ReadyToUse", readyToUse);
        SaveManager.UpdateSavedFloat(saveID + "CooldownTimer", cooldownTimer);
    }

    public void LoadRelic(string loadID)
    {
        loadID = loadID + relicType.ToString() + "Relic";
        userName = SaveManager.GetString(loadID + "User");
        collected = SaveManager.GetBool(loadID + "Collected");
        inUse = SaveManager.GetBool(loadID + "InUse");
        readyToUse = SaveManager.GetBool(loadID + "ReadyToUse");
        cooldownTimer = SaveManager.GetFloat(loadID + "CooldownTimer");

        if (collected)
        {
            GameObject newUser = GameObject.Find(userName);
            SetUser(newUser);
            newUser.GetComponent<CharacterBase>().AddRelic(gameObject);
        }
    }
}