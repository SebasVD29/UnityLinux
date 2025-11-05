using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UI_DisplayMode : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown displayModeDropdown;

    private void Start()
    {
        displayModeDropdown.ClearOptions();
        LoadDisplayMode();
    }
    void LoadDisplayMode()
    {
        // Convertimos enum ? lista de strings automáticamente
        var options = new List<string>(Enum.GetNames(typeof(DisplayMode)));
        displayModeDropdown.AddOptions(options);

        // Seleccionar el modo actual por defecto
        displayModeDropdown.value = GetCurrentModeIndex();
        displayModeDropdown.RefreshShownValue();

        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
    }
    private int GetCurrentModeIndex()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen: return (int)DisplayMode.Fullscreen;
            case FullScreenMode.FullScreenWindow: return (int)DisplayMode.BorderlessWindow;
            case FullScreenMode.Windowed: return (int)DisplayMode.Windowed;
            default: return (int)DisplayMode.Fullscreen;
        }
    }

    public void SetDisplayMode(int modeIndex)
    {
        DisplayMode selectedMode = (DisplayMode)modeIndex;

        switch (selectedMode)
        {
            case DisplayMode.Fullscreen:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case DisplayMode.BorderlessWindow:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case DisplayMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
}

public enum DisplayMode
{
    Fullscreen,
    BorderlessWindow,
    Windowed
}
