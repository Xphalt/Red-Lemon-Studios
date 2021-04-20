using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class ElementHazardAilments : MonoBehaviour
{
    private Rigidbody myRigid;

    internal CharacterBase userScript;
    internal Vector3 spawnPoint;
    internal SFXScript sfxScript = null;
    internal Teams team;
    internal bool successfulHit = false;

    public Color normalHitColour;
    public Color strongHitColour;
    public Color weakHitColour;
    public ElementTypes damageType;
    public bool changesColour = false;
    public bool dieOnHit = false;
    public string hitSound;
    public float statusEffectDuration;
    public float statusMagnitude;
    public bool hasEffect;
    public int damage;

    private void Awake()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    public void Initialise(float weaponDamage, CharacterBase newUser, Vector3 moveVel=new Vector3(), Vector3 spawn=new Vector3())
    {
        spawnPoint = (spawn == new Vector3()) ? transform.position : spawn;
        if (moveVel != new Vector3()) myRigid.velocity = moveVel;
        userScript = newUser;
        damage = Mathf.RoundToInt(weaponDamage * userScript.CalculateDamageMult());
        team = userScript.team;
    }

    public void RegisterHit()
    {
        userScript.IncreaseCombo();
        successfulHit = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (sfxScript != null) sfxScript.PlaySFX3D(hitSound, transform.position);
        if (dieOnHit) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!successfulHit)
        {
            userScript.MissShot(damage);
        }
    }
}