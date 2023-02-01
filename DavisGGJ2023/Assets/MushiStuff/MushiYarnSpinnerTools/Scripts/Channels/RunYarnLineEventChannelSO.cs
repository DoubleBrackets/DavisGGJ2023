#region

using System;
using UnityEngine;
using Yarn.Unity;

#endregion

/// <summary>
/// Event parameters are the line being run, and the onDialogueLineFinished delegate (should be invoked when line is done being displayed)
/// </summary>
[CreateAssetMenu(menuName = "Channels/Events/YarnSpinner/RunYarnLineEventChannel", fileName = "NewRunYarnLineEventChannel")]
public class RunYarnLineEventChannelSO : GenericEventChannelSO<LocalizedLine, Action>
{
}