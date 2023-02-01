#region

using System;
using UnityEngine;

#endregion

/// <summary>
/// Attribute for SCHeaders
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ColorHeaderAttribute : PropertyAttribute
{
    private const float defaultYOffset = 0.45f;
    private const float defaultPadding = 0.3f;

    public readonly string text;
    public readonly ColorHeaderColor color;
    public readonly float yOffset;
    public readonly float padding;

    /// <summary>
    /// Draw a colored header label in the inspector
    /// </summary>
    /// <param name="text">Display text</param>
    /// <param name="color">Preset color type. More color types can be added through extending enum type. Changeable through config SOs</param>
    /// <param name="padding">Space above and below header (is multiplied by default single line height)</param>
    /// <param name="yOffset">Y Offset (is multiplied by default single line height)</param>
    public ColorHeaderAttribute(
        string text,
        ColorHeaderColor color = ColorHeaderColor.Normal,
        float padding = defaultPadding,
        float yOffset = defaultYOffset
    )
    {
        this.text = text;
        this.color = color;
        this.padding = padding;
        this.yOffset = yOffset;
    }
}