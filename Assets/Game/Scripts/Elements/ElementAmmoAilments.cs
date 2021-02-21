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

public class ElementAmmoAilments : MonoBehaviour
{
    public bool hasEffect;
    public int damage;
    public ElementTypes damageType;
    public float statusEffectDuration;
    public float statusMagnitude;

    internal CharacterBase user;
    internal bool successfulHit = false;
    internal Teams team;

    public void Initialise(float weaponDamage)
    {
        damage = Mathf.RoundToInt(weaponDamage * user.CalculateDamageMult());

        team = user.team;
    }

    public void RegisterHit()
    {
        user.IncreaseCombo();
        successfulHit = true;
    }

    private void OnDestroy()
    {
        if (!successfulHit)
        {
            user.MissShot(damage);
        }
    }
}