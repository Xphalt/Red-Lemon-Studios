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

    public void Initialise(float weaponDamage, CharacterBase newUser)
    {
        userScript = newUser;
        damage = Mathf.RoundToInt(weaponDamage * userScript.CalculateDamageMult());
        team = userScript.team;
    }

    public void RegisterHit()
    {
        userScript.IncreaseCombo();
        successfulHit = true;
    }

    private void OnDestroy()
    {
        if (!successfulHit)
        {
            userScript.MissShot(damage);
        }
    }
}