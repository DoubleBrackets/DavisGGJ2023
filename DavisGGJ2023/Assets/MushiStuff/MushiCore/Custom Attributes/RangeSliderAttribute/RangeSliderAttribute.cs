#region

using System;
using UnityEngine;

#endregion


[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class RangeSliderAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;

    /// <summary>
    ///  Attribute for drawing Vector2/Vector2Int as a range bound slider
    /// </summary>
    /// <param name="min">minimum allowed value</param>
    /// <param name="max">maximum allowed value</param>
    public RangeSliderAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
