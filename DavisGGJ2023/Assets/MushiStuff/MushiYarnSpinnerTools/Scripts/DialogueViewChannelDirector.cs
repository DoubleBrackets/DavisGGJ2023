#region

using System;
using UnityEngine;
using Yarn.Unity;

#endregion

/// <summary>
/// Simple dialogue view that redirects requests into SO channels
/// </summary>
public class DialogueViewChannelDirector : DialogueViewBase
{
    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private VoidEventChannelSO onDialogueStarted;

    [SerializeField] private VoidEventChannelSO onDialogueComplete;
    [SerializeField] private RunYarnLineEventChannelSO onRunDialogueLine;
    [SerializeField] private RunYarnLineEventChannelSO onLineInterrupt;
    [SerializeField] private RunYarnOptionEventChannelSO onRunOptions;
    [SerializeField] private ActionEventChannelSO onDismissLine;
    [SerializeField] private VoidEventChannelSO onUserRequestedViewAdvancement;

    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private VoidEventChannelSO askInterruptLine;

    public override void DialogueStarted()
    {
        onDialogueStarted.RaiseEvent();
        askInterruptLine.OnRaised += requestInterrupt;
    }

    public override void DialogueComplete()
    {
        onDialogueComplete.RaiseEvent();
        askInterruptLine.OnRaised -= requestInterrupt;
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (onRunDialogueLine == null || !onRunDialogueLine.HasListeners)
        {
            onDialogueLineFinished?.Invoke();
        }
        else
        {
            onRunDialogueLine.RaiseEvent(dialogueLine, onDialogueLineFinished);
        }
    }

    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        if (onLineInterrupt == null || !onLineInterrupt.HasListeners)
        {
            onDialogueLineFinished?.Invoke();
        }
        else
        {
            onLineInterrupt.RaiseEvent(dialogueLine, onDialogueLineFinished);
        }
    }

    public override void DismissLine(Action onDismissalComplete)
    {
        if (onDismissLine == null || !onDismissLine.HasListeners)
        {
            onDismissalComplete?.Invoke();
        }
        else
        {
            onDismissLine.RaiseEvent(onDismissalComplete);
        }
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        if (onRunOptions == null) return;
        onRunOptions.RaiseEvent(dialogueOptions, onOptionSelected);
    }

    public override void UserRequestedViewAdvancement()
    {
        if (onUserRequestedViewAdvancement)
            onUserRequestedViewAdvancement.RaiseEvent();
    }
}