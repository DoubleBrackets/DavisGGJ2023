#region

using System;
using UnityEngine;

#endregion

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ExtendedPropertyAttribute : PropertyAttribute
{
    public readonly bool readOnly;
    public readonly bool shownByDefault;

    /// <summary>
    /// Draws a foldout nested inspector for object reference fields. Does not use custom inspectors
    /// </summary>
    /// <param name="readOnly">Can object be edited through foldout</param>
    /// <param name="shownByDefault">Foldout is shown by default</param>
    public ExtendedPropertyAttribute(bool readOnly = false, bool shownByDefault = true)
    {
        this.readOnly = readOnly;
        this.shownByDefault = shownByDefault;
    }
}
