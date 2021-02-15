/// <summary>
/// 
/// Enum created by Matt
/// and was in the ElementShooting
/// script before refactor
/// 
/// Linden and Daniel refactored that
/// class and moved the enum to a helper 
/// class that helps scripts to
/// access and change the currently
/// selected element
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumHelper
{
    public enum ElementTypes { Fire, Water, Air, Earth, ElementTypesSize };
}