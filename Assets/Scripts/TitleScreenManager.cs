using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown levelDropdown;
    [SerializeField] Button playGame;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] List<LevelSettings> mainLevels = new();
    Dictionary<string, TMP_InputField> dictionary = new();

    private void Start()
    {
        levelDropdown.onValueChanged.AddListener(value =>
        {
            LoadSettings(mainLevels[value]);
        });

        for (int i = 0; i<mainLevels.Count; i++)
        {
            levelDropdown.options.Add(new TMP_Dropdown.OptionData($"Level {i+1}"));
            levelDropdown.RefreshShownValue();
        }

        TMP_InputField[] allInputFields = FindObjectsOfType<TMP_InputField>();
        foreach (TMP_InputField inputField in allInputFields)
            dictionary.Add(inputField.name, inputField);

        playGame.onClick.AddListener(CustomSettings);

        if (CarryVariables.instance.settingsToUse.levelSizeX == 0)
            LoadSettings(mainLevels[0]);
        else
            LoadSettings(CarryVariables.instance.settingsToUse);
    }

    void LoadSettings(LevelSettings settings)
    {
        dictionary["Size X"].text = settings.levelSizeX.ToString();
        dictionary["Size Y"].text = settings.levelSizeY.ToString(); 
        dictionary["Jewels"].text = settings.numJewels.ToString();
        dictionary["Spinners"].text = settings.numSpinners.ToString();
        dictionary["Flying"].text = settings.numFlying.ToString();
        dictionary["Blank"].text = settings.blanked.ToString();
        CarryVariables.instance.settingsToUse = settings;
        tutorialText.text = settings.tutorial;
    }

    void CustomSettings()
    {
        string errorMessage = "";

        if (!int.TryParse(dictionary["Size X"].text, out CarryVariables.instance.settingsToUse.levelSizeX))
            errorMessage = "Invalid size X.";
        if (CarryVariables.instance.settingsToUse.levelSizeX < 10 || CarryVariables.instance.settingsToUse.levelSizeX > 30)
            errorMessage = "Invalid size X.";

        if (!int.TryParse(dictionary["Size Y"].text, out CarryVariables.instance.settingsToUse.levelSizeY))
            errorMessage = "Invalid size Y.";
        if (CarryVariables.instance.settingsToUse.levelSizeY < 10 || CarryVariables.instance.settingsToUse.levelSizeY > 30)
            errorMessage = "Invalid size Y.";

        if (!int.TryParse(dictionary["Jewels"].text, out CarryVariables.instance.settingsToUse.numJewels))
            errorMessage = "Invalid number of jewels.";
        if (CarryVariables.instance.settingsToUse.numJewels < 0)
            errorMessage = "Invalid number of jewels.";

        if (!int.TryParse(dictionary["Spinners"].text, out CarryVariables.instance.settingsToUse.numSpinners))
            CarryVariables.instance.settingsToUse.numSpinners = 0;

        if (!int.TryParse(dictionary["Flying"].text, out CarryVariables.instance.settingsToUse.numFlying))
            CarryVariables.instance.settingsToUse.numFlying = 0;

        if (!int.TryParse(dictionary["Blank"].text, out CarryVariables.instance.settingsToUse.blanked))
            CarryVariables.instance.settingsToUse.blanked = 0;

        if (errorMessage.Equals(""))
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            errorText.text = errorMessage;
            Invoke(nameof(BlankErrorText), 3f);
        }
    }

    void BlankErrorText()
    {
        errorText.text = "";
    }
}