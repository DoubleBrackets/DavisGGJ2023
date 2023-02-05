#region

using TMPro;
using UnityEngine;

#endregion

public class DialogueDebugRunner : MonoBehaviour
{
    [SerializeField] private TMP_InputField nodeNameInputField;
    [SerializeField] private StringEventChannelSO askStartDialogue;
#if UNITY_EDITOR
    private void OnEnable()
    {
        nodeNameInputField.onSubmit.AddListener(RunDialogue);
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        nodeNameInputField.onSubmit.RemoveListener(RunDialogue);
    }

    public void RunDialogue(string nodeName)
    {
        askStartDialogue.RaiseEvent(nodeName);
    }
#endif
}