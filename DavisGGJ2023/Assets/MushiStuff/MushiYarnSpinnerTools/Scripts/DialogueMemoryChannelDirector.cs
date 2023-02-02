using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueMemoryChannelDirector : DescriptionMonoBehavior
{
    [ColorHeader("Listening")]
    [SerializeField] private YarnVariablesEventChannelSO askLoadYarnVariables;
    [SerializeField] private YarnVariablesFuncChannelSO getYarnVariables;
    
    [ColorHeader("Dependencies")]
    [SerializeField] private InMemoryVariableStorage inMemoryStorage;

    private void OnEnable()
    {
        askLoadYarnVariables.OnRaised += LoadYarnVariables;
        getYarnVariables.OnCalled += GetYarnVariables;
    }
    
    private void OnDisable()
    {
        askLoadYarnVariables.OnRaised -= LoadYarnVariables;
        getYarnVariables.OnCalled -= GetYarnVariables;
    }

    private void LoadYarnVariables(Dictionary<string, float> floatVars, Dictionary<string, string> stringVars, Dictionary<string, bool> boolVars)
    {
        inMemoryStorage.SetAllVariables(floatVars, stringVars, boolVars);
    }

    private (Dictionary<string, float>, Dictionary<string, string>, Dictionary<string, bool>) GetYarnVariables()
    {
        return inMemoryStorage.GetAllVariables();
    }
}
