using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumHelper
{
    [Serializable] public enum ElementTypes { Fire, Water, Air, Earth, ElementTypesSize };
    public enum Teams { Player, Enemy, TeamsSize };
}