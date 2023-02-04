using Unity.Mathematics;
using UnityEngine;

public static class MathExtensionMethods 
{
    public static Vector2 ProjectOntoNormal(this Vector2 toProject, Vector2 normal)
    {
        return toProject - Vector2.Dot(toProject, normal) * normal;
    }

    public static float GetNormalizedDistanceAlongSegment(this Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 aToP = p - a;        
        Vector2 aToB = b - a;    

        float magnitudeAB = aToB.magnitude;       
        float ABAPproduct = Vector2.Dot(aToP, aToB);   
        float distance = ABAPproduct / magnitudeAB;

        return Mathf.Clamp(distance, 0f, 1f);
    }

    public static Quaternion GetAngle(this Vector2 vec)
    {
        return Quaternion.Euler(0,0, Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg);
    }
    
    public static Vector2 GetVector(this Quaternion angle)
    {
        return angle * Vector2.right;
    }
}
