#region

using System;
using Febucci.UI;
using TMPro;
using UnityEngine;
using Yarn.Unity;

#endregion

public class SimpleLineDisplay : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private RunYarnLineEventChannelSO onRunDialogueLine;

    [SerializeField] private VoidEventChannelSO onDialogueComplete;
    [SerializeField] private VoidEventChannelSO onUserRequestViewAdvancement;

    [ColorHeader("Dependencies")]
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private CanvasGroup showGroup;

    [SerializeField] private TMP_Text speakingCharacterText;
    [SerializeField] private TextAnimatorPlayer typeWriterPlayer;

    private Action lineFinishedDisplaying;
    private bool isWaitingForContinue;
    private bool isTypwriterInProgress;

    private void OnEnable()
    {
        onRunDialogueLine.OnRaised += DisplayNewLine;
        onUserRequestViewAdvancement.OnRaised += AdvanceView;
        onDialogueComplete.OnRaised += DialogueDone;
    }

    private void OnDisable()
    {
        onRunDialogueLine.OnRaised -= DisplayNewLine;
        onUserRequestViewAdvancement.OnRaised -= AdvanceView;
        onDialogueComplete.OnRaised -= DialogueDone;
    }

    private void DialogueDone()
    {
        lineText.text = "";
        speakingCharacterText.text = "";
        showGroup.alpha = 0f;
    }

    private void DisplayNewLine(LocalizedLine line, Action onDialogueLineFinished)
    {
        string text = line.TextWithoutCharacterName.Text;
        DialogueDone();
        lineText.text = text;
        speakingCharacterText.text = line.CharacterName;

        lineFinishedDisplaying = onDialogueLineFinished;

        isTypwriterInProgress = true;
        isWaitingForContinue = true;
        typeWriterPlayer.onTextShowed.AddListener(TypeWriterFinished);
        showGroup.alpha = 1f;
    }

    private void TypeWriterFinished()
    {
        isTypwriterInProgress = false;
        typeWriterPlayer.onTextShowed.RemoveListener(TypeWriterFinished);
    }

    private void AdvanceView()
    {
        if (isTypwriterInProgress)
        {
            TypeWriterFinished();
            typeWriterPlayer.SkipTypewriter();
        }
        else if (isWaitingForContinue)
        {
            isWaitingForContinue = false;
            lineFinishedDisplaying.Invoke();
        }
    }
}