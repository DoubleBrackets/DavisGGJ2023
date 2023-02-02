using UnityEngine;

public static class MathExtensionMethods 
{
    public static Vector2 ProjectOntoNormal(this Vector2 toProject, Vector2 normal)
    {
        return toProject - Vector2.Dot(toProject, normal) * normal;
    }
}
