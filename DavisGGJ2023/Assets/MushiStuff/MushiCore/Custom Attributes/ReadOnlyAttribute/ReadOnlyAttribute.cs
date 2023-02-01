#region

using System;
using UnityEngine;

#endregion

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ReadOnlyAttribute : PropertyAttribute
{
}