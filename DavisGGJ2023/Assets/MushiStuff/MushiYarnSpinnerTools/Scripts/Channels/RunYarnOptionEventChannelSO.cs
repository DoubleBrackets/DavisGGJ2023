#region

using System;
using UnityEngine;
using Yarn.Unity;

#endregion

/// <summary>
/// Event parameters are the DialogueOptions being run, and the onOptionSelected delegate (should be invoked when an option is chosen)
/// </summary>
[CreateAssetMenu(menuName = "Channels/Events/YarnSpinner/RunYarnOptionEventChannel", fileName = "NewRunYarnOptionEventChannel")]
public class RunYarnOptionEventChannelSO : GenericEventChannelSO<DialogueOption[], Action<int>>
{
}