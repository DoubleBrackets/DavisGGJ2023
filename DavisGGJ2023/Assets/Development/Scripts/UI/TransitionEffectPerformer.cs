using UnityEngine;

public abstract class TransitionEffectPerformer : DescriptionMonoBehavior
{
    public abstract Coroutine PerformTransitionOut(bool resetAfter);
    public abstract Coroutine PerformTransitionIn(bool resetAfter);
}
