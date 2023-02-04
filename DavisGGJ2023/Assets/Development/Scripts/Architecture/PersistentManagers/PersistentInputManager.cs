using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode
{
    Disabled,
    Gameplay,
    UI
}

public class PersistentInputManager : DescriptionMonoBehavior
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [ColorHeader("Input State Change Ask")] 
    [SerializeField] private InputModeEventChannelSO askInputModeChange;
    
    [ColorHeader("Input Dependencies", ColorHeaderColor.Dependencies)]
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private ProtagUserInputProvider inputProvider;

    private const string gameplayMap = "Gameplay";
    private const string uiMap = "UI";

    private InputMode currentInputMode;

    private void OnEnable() {
        askInputModeChange.OnRaised += SwitchInputMode;
        inputAsset.Enable();
    }

    private void OnDisable()
    {
        askInputModeChange.OnRaised -= SwitchInputMode;
    }

    private void SwitchInputMode(InputMode newInputMode)
    {
        switch (newInputMode)
        {
            case InputMode.Disabled:
                DisableAllActionMaps();
                break;
            case InputMode.Gameplay:
                SwitchToGameplay();
                break;
            case InputMode.UI:
                SwitchToUI();
                break;
        }

        if (currentInputMode != newInputMode)
        {
            currentInputMode = newInputMode;
            askInputModeChange.RaiseEvent(newInputMode);
        }
    }

    private void SwitchToGameplay()
    {
        SwitchActionMap(gameplayMap);
    }

    public void SwitchToUI()
    {
        SwitchActionMap(uiMap);
    }

    private void SwitchActionMap(string mapName)
    {
        DisableAllActionMaps();
        inputAsset.FindActionMap(mapName).Enable();
    }

    private void DisableAllActionMaps()
    {
        foreach (var map in inputAsset.actionMaps)
            map.Disable();
    }
}

