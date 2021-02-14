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
}