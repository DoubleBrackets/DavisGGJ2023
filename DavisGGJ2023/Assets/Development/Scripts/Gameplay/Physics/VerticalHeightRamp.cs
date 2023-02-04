using System;
using System.Collections.Generic;
using UnityEngine;

public class VerticalHeightRamp : MonoBehaviour
{
    [SerializeField] private BoxCollider2D coll;
    [SerializeField] private float heightBottom;
    [SerializeField] private float heightTop;

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
        Vector2 center = bounds.center;
        lowerY = center.y - bounds.extents.y;
        upperY = center.y + bounds.extents.y;

        angle = Mathf.Atan2(heightTop - heightBottom, upperY - lowerY);
    }

    public float EvaluateHeight(Vector2 pos)
    {
        float t = Mathf.InverseLerp(lowerY, upperY, pos.y);
        return Mathf.Lerp(heightBottom, heightTop, t);
    }
}
