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
        //whenever the level dropdown changes value, load that level's settings
        levelDropdown.onValueChanged.AddListener(value =>
        {
            LoadSettings(mainLevels[value]);
        });

        for (int i = 0; i<mainLevels.Count; i++) //add a new option to the dropdown for each level setting
        {
            levelDropdown.options.Add(new TMP_Dropdown.OptionData($"Level {i+1}"));
            levelDropdown.RefreshShownValue();
        }

        TMP_InputField[] allInputFields = FindObjectsOfType<TMP_InputField>(); //store all input fields into a dictionary
        foreach (TMP_InputField inputField in allInputFields)
            dictionary.Add(inputField.name, inputField);

        playGame.onClick.AddListener(CustomSettings);

        if (CarryVariables.instance.settingsToUse.levelSizeX == 0) //if the last level is invalid, load level 1
            LoadSettings(mainLevels[0]);
        else //otherwise load that level
            LoadSettings(CarryVariables.instance.settingsToUse);
    }

    /// <summary>
    /// transer all the settings in the numbers into the input fields
    /// </summary>
    /// <param name="settings"></param>
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

    /// <summary>
    /// convert all input field data into settings, handles any invalid numbers
    /// </summary>
    void CustomSettings()
    {
        string errorMessage = "";

        if (!int.TryParse(dictionary["Size X"].text, out CarryVariables.instance.settingsToUse.levelSizeX)) //size x isn't an integer
            errorMessage = "Invalid size X.";
        if (CarryVariables.instance.settingsToUse.levelSizeX < 10 || CarryVariables.instance.settingsToUse.levelSizeX > 30) //size x too small/large
            errorMessage = "Invalid size X.";

        if (!int.TryParse(dictionary["Size Y"].text, out CarryVariables.instance.settingsToUse.levelSizeY)) //size y isn't an integer
            errorMessage = "Invalid size Y.";
        if (CarryVariables.instance.settingsToUse.levelSizeY < 10 || CarryVariables.instance.settingsToUse.levelSizeY > 30) //size y too small/large
            errorMessage = "Invalid size Y.";

        if (!int.TryParse(dictionary["Jewels"].text, out CarryVariables.instance.settingsToUse.numJewels)) //jewels isn't an integer
            errorMessage = "Invalid number of jewels.";
        if (CarryVariables.instance.settingsToUse.numJewels < 0) //not enough jewels
            errorMessage = "Invalid number of jewels.";

        if (!int.TryParse(dictionary["Spinners"].text, out CarryVariables.instance.settingsToUse.numSpinners)) //spinners isn't an integer
            CarryVariables.instance.settingsToUse.numSpinners = 0;

        if (!int.TryParse(dictionary["Flying"].text, out CarryVariables.instance.settingsToUse.numFlying)) //flying isn't an integer
            CarryVariables.instance.settingsToUse.numFlying = 0;

        if (!int.TryParse(dictionary["Blank"].text, out CarryVariables.instance.settingsToUse.blanked)) //blank isn't an integer
            CarryVariables.instance.settingsToUse.blanked = 0;

        if (errorMessage.Equals("")) //if there's no error, load the game
        {
            SceneManager.LoadScene("Game");
        }
        else //display error text and delete in 3 seconds
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