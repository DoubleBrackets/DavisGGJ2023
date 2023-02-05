using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/RecomposeEffectProfile", fileName = "NewRecomposeEffect")]
public class CameraRecomposeProfileSO : DescriptionBaseSO
{
    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private float duration;
    public float Duration => duration;

    [ColorHeader("X Config", ColorHeaderColor.Config)] 
    [SerializeField] private AnimationCurve tiltCurve;
    [SerializeField] private float tiltMagnitude = 1;
    
    [ColorHeader("Y Config", ColorHeaderColor.Config)] 
    [SerializeField] private AnimationCurve panCurve;
    [SerializeField] private float panMagnitude = 1;
    
    [ColorHeader("Z Config", ColorHeaderColor.Config)] 
    [SerializeField] private AnimationCurve dutchCurve;
    [SerializeField] private float dutchMagnitude = 1;
    
    [ColorHeader("Zoom Config", ColorHeaderColor.Config)] 
    [SerializeField] private AnimationCurve zoomCurve;
    [SerializeField] private float zoomMagnitude = 1;

    public void ApplyRecomposeEffect(CinemachineRecomposer recomposer, float t, float multiplier = 1)
    {
        recomposer.m_Tilt += tiltCurve.Evaluate(t) * tiltMagnitude * multiplier;
        recomposer.m_Pan += panCurve.Evaluate(t) * panMagnitude * multiplier;
        recomposer.m_Dutch += dutchCurve.Evaluate(t) * dutchMagnitude * multiplier;
        recomposer.m_ZoomScale += zoomCurve.Evaluate(t) * zoomMagnitude * multiplier;
    }
}