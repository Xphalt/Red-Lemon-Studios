using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumHelper
{
    [Serializable] public enum ElementTypes { Fire, Water, Air, Earth, ElementTypesSize };
    public enum Teams { Player, Enemy, TeamsSize };
    public enum Layers { Default, TransparentFX, IgnoreRaycast, Empty1, Water, Empty2, Empty3, UI, Weapon, Relic, Player, Invisible, LayersSize }; 
    //Would be great if unity didn't have empty layers honestly
}