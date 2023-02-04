using System;
using System.Collections.Generic;
using UnityEngine;

public class VerticalHeightRamp : MonoBehaviour
{
    [SerializeField] private BoxCollider2D coll;
    [SerializeField] private float heightBottom;
    [SerializeField] private float heightTop;
    [SerializeField] private float stairLength;

    [ReadOnly,SerializeField] private float lowerY;
    [ReadOnly,SerializeField] private float upperY;
    [ReadOnly,SerializeField] private float angle;

    public float Angle => angle;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Awake();
    }
#endif
    
    private void Awake()
    {
        var bounds = coll.bounds;
        float bottomPos = bounds.center.y - bounds.extents.y - bounds.center.z;
        lowerY = bottomPos;
        upperY = bottomPos + stairLength;

        angle = Mathf.Atan2(heightTop - heightBottom, stairLength);
    }

    public float EvaluateHeight(Vector2 horizontalPos)
    {
        float t = Mathf.InverseLerp(lowerY, upperY, horizontalPos.y);
        return Mathf.Lerp(heightBottom, heightTop, t);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var bounds = coll.bounds;
        Vector3 bottomPos = bounds.center - Vector3.up * bounds.extents.y;
        Gizmos.DrawLine(bottomPos, bottomPos + stairLength * Vector3.up);
        bottomPos -= bounds.center.z * Vector3.up;
        Gizmos.DrawLine(bottomPos, bottomPos + stairLength * Vector3.up);
    }
}
