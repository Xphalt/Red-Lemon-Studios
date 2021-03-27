/// <summary>
/// 
/// Script made by Daniel
/// 
/// This adds the ailment
/// effects for the elemental
/// ammo
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class ElementHazardAilments : MonoBehaviour
{
    public bool hasEffect;
    public int damage;
    public ElementTypes damageType;
    public float statusEffectDuration;
    public float statusMagnitude;

    internal CharacterBase userScript;
    internal bool successfulHit = false;
    internal Teams team;
    internal Vector3 spawnPoint;

    public bool dieOnHit = false;

    public string hitSound;

    internal SFXScript sfxScript = null;

    public Color normalHitColour;
    public Color strongHitColour;
    public Color weakHitColour;

    public bool changesColour = false;

    public void Initialise(float weaponDamage, CharacterBase newUser, Vector3 spawn=new Vector3())
    {
        spawnPoint = (spawn == new Vector3()) ? transform.position : spawn;

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