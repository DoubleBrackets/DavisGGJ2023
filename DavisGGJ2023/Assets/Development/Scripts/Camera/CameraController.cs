using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private VoidEventChannelSO askStopFollowingTarget;
    [SerializeField] private TransformEventChannelSO askStartFollowingTransform;
    [SerializeField] private VoidEventChannelSO lockOnTarget;

    [ColorHeader("Dependencies")]
    [SerializeField] private CinemachineVirtualCamera followingCamera;

    private void OnEnable()
    {
        askStartFollowingTransform.OnRaised += StartFollowingTarget;
        askStopFollowingTarget.OnRaised += StopFollowingTarget;
        lockOnTarget.OnRaised += LockOnTarget;
    }

    private void OnDisable()
    {
        askStartFollowingTransform.OnRaised -= StartFollowingTarget;
        askStopFollowingTarget.OnRaised -= StopFollowingTarget;
        lockOnTarget.OnRaised -= LockOnTarget;
    }

    private void LockOnTarget()
    {
        var following = followingCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        following.Damping = Vector3.zero;
        DOTween.To(
            () => following.Damping,
            (a) => following.Damping = a,
            Vector3.one - Vector3.forward,
            0.1f);
    }

    private void StartFollowingTarget(Transform target)
    {
        followingCamera.m_Follow = target;
    }

    private void StopFollowingTarget()
    {
        followingCamera.m_Follow = null;
    }
}
