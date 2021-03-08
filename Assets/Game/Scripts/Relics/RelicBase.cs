using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static EnumHelper;

public class RelicBase : MonoBehaviour
{
    internal GameObject user = null;
    protected CharacterBase characterScript;
    protected Rigidbody characterRigid;
    public ElementTypes relicType;

    internal bool inUse;
    internal bool collected = false;

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
    internal bool readyToUse = false;

    public virtual void SetUser(GameObject newUser)
    {
        collected = true;
        user = newUser;
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

    public void SaveRelic(int id)
    {
        string identifier = "Relic" + id.ToString();
        SaveManager.UpdateSavedBool(identifier + "Collected", collected);
        SaveManager.UpdateSavedBool(identifier + "InUse", inUse);
        SaveManager.UpdateSavedBool(identifier + "ReadyToUse", readyToUse);
        SaveManager.UpdateSavedFloat(identifier + "CooldownTimer", cooldownTimer);
        SaveManager.UpdateSavedVector3(identifier + "Position", transform.position);
    }

    public void LoadRelic(int id)
    {
        string identifier = "Relic" + id.ToString();
        collected = SaveManager.GetBool(identifier + "Collected");
        inUse = SaveManager.GetBool(identifier + "InUse");
        readyToUse = SaveManager.GetBool(identifier + "ReadyToUse");
        cooldownTimer = SaveManager.GetFloat(identifier + "CooldownTimer");
        transform.position = SaveManager.GetVector3(identifier + "Position");
    }
}