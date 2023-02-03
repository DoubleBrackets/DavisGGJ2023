using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeightChanger : MonoBehaviour
{
    private HashSet<HeightBody2D> trackedBodies = new ();
    
    
    public void TryAddBody(HeightBody2D body)
    {
        if(CanAdd(body) && !trackedBodies.Contains(body))
            trackedBodies.Add(body);
    }

    public void RemoveBody(HeightBody2D body)
    {
        if(trackedBodies.Contains(body))
            trackedBodies.Remove(body);
    }

    public abstract void UpdateHeights(HashSet<HeightBody2D> trackedBodies);
    public abstract bool CanAdd(HeightBody2D toAdd);

    private void FixedUpdate()
    {
        UpdateHeights(trackedBodies);
    }
}
