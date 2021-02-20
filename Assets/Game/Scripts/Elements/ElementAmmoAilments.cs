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

    internal Player player;
    internal bool successfulHit = false;

    public void SetDamage()
    {
        damage = Mathf.RoundToInt(damage * player.CalculateDamageMult());
    }

    public void RegisterHit()
    {
        player.IncreaseCombo();
        successfulHit = true;
    }

    private void OnDestroy()
    {
        if (player.currentTool != null)
        {
            if (player.currentTool.toolType == ElementTypes.Fire && !successfulHit)
            {
                player.MissShot(damage);
            }
        }
    }
}