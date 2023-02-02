using System;
using System.Linq;
using UnityEngine;

public enum TransitionEffect
{
    None,
    FadeBlack
}

public class PersistentTransitionManager : DescriptionMonoBehavior
{
    [ColorHeader("Listening")]
    [SerializeField] private TransitionOutFuncChannelSO askTransitionOut;
    [SerializeField] private TransitionInFuncChannelSO askTransitionIn;

    [ColorHeader("Dependencies")]
    [SerializeField] private Transition[] transitions;

    [Serializable]
    private struct Transition
    {
        public TransitionEffect effect;
        public TransitionEffectPerformer performer;
    }

    private void OnEnable()
    {
        askTransitionOut.OnCalled += TransitionOut;
        askTransitionIn.OnCalled += TransitionIn;
    }
    
    private void OnDisable()
    {
        askTransitionOut.OnCalled -= TransitionOut;
        askTransitionIn.OnCalled -= TransitionIn;
    }

    private Coroutine TransitionOut(TransitionEffect effect, bool resetAfter)
    {
        if (effect == TransitionEffect.None) return null;
        return transitions.First(a => a.effect == effect).performer.PerformTransitionOut(resetAfter);
    }

    private Coroutine TransitionIn(TransitionEffect effect, bool resetAfter)
    {
        if (effect == TransitionEffect.None) return null;
        return transitions.First(a => a.effect == effect).performer.PerformTransitionIn(resetAfter);
    }
}
