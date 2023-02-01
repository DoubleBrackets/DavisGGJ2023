#region

using UnityEngine;
using Yarn.Unity;

#endregion

/// <summary>
/// Simple class for linking up channels to basic dialogue runner functionality
/// </summary>
public class DialogueRunnerChannelDirector : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private StringEventChannelSO askStartDialogueNode;

    [ColorHeader("Dependencies")]
    [SerializeField] private DialogueRunner dialogueRunner;

    private void OnEnable()
    {
        askStartDialogueNode.OnRaised += StartDialogueNode;
    }

    private void OnDisable()
    {
        askStartDialogueNode.OnRaised -= StartDialogueNode;
    }

    private void StartDialogueNode(string node)
    {
        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(node);
        }
        else
        {
            Debug.LogError($"Tried to start a new dialogue node {node} when dialogue was already running");
        }
    }
}