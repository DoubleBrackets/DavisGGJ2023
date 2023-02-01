#region

using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

#endregion

public class SimpleOptionDisplay : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private RunYarnOptionEventChannelSO onRunOptions;

    [ColorHeader("Dependencies")]
    [SerializeField] private LineOptionUI lineOptionPrefab;

    private Action<int> optionSelected;

    private List<LineOptionUI> currentOptions = new();

    private void OnEnable()
    {
        onRunOptions.OnRaised += RunOptions;
    }

    private void OnDisable()
    {
        onRunOptions.OnRaised -= RunOptions;
    }

    private void RunOptions(DialogueOption[] options, Action<int> onOptionSelected)
    {
        optionSelected = onOptionSelected;
        List<LineOptionUI> availableOptions = new();
        for (int i = 0; i < options.Length; i++)
        {
            var lineOption = Instantiate(lineOptionPrefab, transform);
            bool isAvailable = options[i].IsAvailable;
            if (isAvailable)
            {
                availableOptions.Add(lineOption);
            }

            lineOption.Initialize(options[i], i, options[i].IsAvailable, OptionSelected);
            currentOptions.Add(lineOption);
        }

        int availableCount = availableOptions.Count;

        if (availableCount == 0)
        {
            Debug.LogError("No valid dialogue options");
        }

        for (int i = 1; i < availableCount - 1; i++)
        {
            availableOptions[i].SetNavigation(availableOptions[i - 1].OptionSelectable, availableOptions[i + 1].OptionSelectable);
        }

        availableOptions[0].SetNavigation(null, availableCount > 1 ? availableOptions[1].OptionSelectable : null);
        availableOptions[availableCount - 1].SetNavigation(availableCount > 1 ? availableOptions[availableCount - 2].OptionSelectable : null, null);

        availableOptions[0].SetSelectedUI();
    }

    private void OptionSelected(int i)
    {
        optionSelected?.Invoke(i);
        foreach (var option in currentOptions)
        {
            option.Remove();
        }

        currentOptions.Clear();
    }
}