#region

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

#endregion

public class LineOptionUI : DescriptionMonoBehavior
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text characterText;

    private Action<int> onSelected;
    private int optionIndex;

    public Selectable OptionSelectable => button;

    public void Initialize(DialogueOption option, int index, bool isAvailable, Action<int> onOptionSelected)
    {
        this.optionIndex = index;
        onSelected = onOptionSelected;
        var line = option.Line;

        displayText.text = line.TextWithoutCharacterName.Text;
        characterText.text = line.CharacterName;

        button.interactable = isAvailable;
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetSelectedUI()
    {
        button.Select();
    }

    public void SetNavigation(Selectable up, Selectable down)
    {
        var nav = button.navigation;
        nav.selectOnDown = down;
        nav.selectOnUp = up;
        button.navigation = nav;
    }

    private void OnButtonClick()
    {
        onSelected?.Invoke(optionIndex);
    }

    public void Remove()
    {
        button.onClick.RemoveListener(OnButtonClick);
        onSelected = null;
        Destroy(gameObject);
    }
}