using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public interface InputProvider<TInputState, TInputEvents>
{
    public abstract void GetInputState(ref TInputState initial);

    public TInputEvents Events
    {
        get;
    }
}

public abstract class ProtagInputProvider : DescriptionBaseSO, InputProvider<PlayerInputState, PlayerInputEvents>
{
    public abstract void GetInputState(ref PlayerInputState initial);

    public abstract PlayerInputEvents Events { get; }
}
