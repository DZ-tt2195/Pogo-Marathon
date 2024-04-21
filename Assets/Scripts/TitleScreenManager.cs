using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] Button playGame;
    [SerializeField] TMP_Text errorText;
    [SerializeField] LevelSettings defaultSettings;
    TMP_InputField[] allInputFields;

    private void Start()
    {
        allInputFields = FindObjectsOfType<TMP_InputField>();
        playGame.onClick.AddListener(CustomSettings);
        LoadSettings(defaultSettings);
    }

    void LoadSettings(LevelSettings settings)
    {
        foreach (TMP_InputField inputField in allInputFields)
        {
            switch (inputField.name)
            {
                case "Size X":
                    inputField.text = settings.levelSizeX.ToString();
                    break;
                case "Size Y":
                    inputField.text = settings.levelSizeY.ToString();
                    break;
                case "Jewels":
                    inputField.text = settings.numJewels.ToString();
                    break;
                case "Spinners":
                    inputField.text = settings.numSpinners.ToString();
                    break;
                case "Flying":
                    inputField.text = settings.numFlying.ToString();
                    break;
            }
        }
    }

    void CustomSettings()
    {
        string errorMessage = "";

        foreach (TMP_InputField inputField in allInputFields)
        {
            // Update settings based on input field name
            switch (inputField.name)
            {
                case "Size X":
                    if (!int.TryParse(inputField.text, out CarryVariables.instance.settingsToUse.levelSizeX))
                        errorMessage = "Invalid size X.";
                    if (CarryVariables.instance.settingsToUse.levelSizeX < 10 || CarryVariables.instance.settingsToUse.levelSizeX > 50)
                        errorMessage = "Invalid size X.";
                    break;
                case "Size Y":
                    if (!int.TryParse(inputField.text, out CarryVariables.instance.settingsToUse.levelSizeY))
                        errorMessage = "Invalid size Y.";
                    if (CarryVariables.instance.settingsToUse.levelSizeY < 10 || CarryVariables.instance.settingsToUse.levelSizeY > 50)
                        errorMessage = "Invalid size Y.";
                    break;
                case "Jewels":
                    if (!int.TryParse(inputField.text, out CarryVariables.instance.settingsToUse.numJewels))
                        errorMessage = "Invalid number of jewels.";
                    if (CarryVariables.instance.settingsToUse.numJewels < 0)
                        errorMessage = "Invalid number of jewels.";
                    break;
                case "Spinners":
                    if (!int.TryParse(inputField.text, out CarryVariables.instance.settingsToUse.numSpinners))
                        CarryVariables.instance.settingsToUse.numSpinners = 0;
                    break;
                case "Flying":
                    if (!int.TryParse(inputField.text, out CarryVariables.instance.settingsToUse.numFlying))
                        CarryVariables.instance.settingsToUse.numFlying = 0;
                    break;
            }
        }

        if (errorMessage.Equals(""))
        {
            SceneManager.LoadScene(1);
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