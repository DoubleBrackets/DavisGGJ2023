using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeBlackEffect : TransitionEffectPerformer
{
    [ColorHeader("Dependencies")]
    [SerializeField] private Image fadeImage;

    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private float duration;

    private Tween tweener;
    
    public override Coroutine PerformTransitionOut(bool resetAfter)
    {
        return StartCoroutine(CoroutTransitionOut(resetAfter));
    }

    private IEnumerator CoroutTransitionOut(bool resetAfter)
    {
        if (tweener != null && tweener.IsPlaying())
        {
            DOTween.Kill(tweener);
        }
        
        tweener = fadeImage.DOFade(1f, duration);
        tweener.SetEase(Ease.OutCubic);
        if (resetAfter)
        {
            tweener.onComplete += () => fadeImage.color = new Color(0f, 0f, 0f, 0f);
        }

        yield return tweener.WaitForCompletion();
    }


    public override Coroutine PerformTransitionIn(bool resetAfter)
    {
        return StartCoroutine(CoroutTransitionIn(resetAfter));
    }
    
    private IEnumerator CoroutTransitionIn(bool resetAfter)
    {
        if (tweener != null && tweener.IsPlaying())
        {
            DOTween.Kill(tweener);
        }
        
        tweener = fadeImage.DOFade(0f, duration);
        tweener.SetEase(Ease.InCubic);
        
        if (resetAfter)
        {
            tweener.onComplete += () => fadeImage.color = new Color(0f, 0f, 0f, 0f);
        }

        yield return tweener.WaitForCompletion();
    }
}
