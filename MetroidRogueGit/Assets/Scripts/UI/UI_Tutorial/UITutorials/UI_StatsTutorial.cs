using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_StatsTutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText; // El texto hijo
    [Header("Input Action")]
    [SerializeField] protected InputActionReference inputActionMoveMando; // joystick del mando

    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        string mandoControls = "";
       
        // Si hay mando conectado, usamos solo eso
        if (Gamepad.current != null && inputActionMoveMando != null && inputActionMoveMando.action != null)
        {
            mandoControls = GetAllGamepadBindings(inputActionMoveMando.action);

            if (string.IsNullOrEmpty(mandoControls))
                mandoControls = inputActionMoveMando.action.GetBindingDisplayString(group: "Gamepad");

            if (tutorialText != null)
                tutorialText.text = $"Use {mandoControls} to navigate through the character's statistics";
        }
        else
        {
            //Si no hay mando, mostrar scroll del mouse
            if (tutorialText != null)
                tutorialText.text = $"Use the Mouse to navigate through the character's statistics";
        }
    }
    public string GetAllGamepadBindings(InputAction action)
    {
        if (action == null) return "";

        HashSet<string> uniqueControls = new HashSet<string>();
        foreach (var binding in action.bindings)
        {
            if (binding.groups != null && binding.groups.Contains("Gamepad"))
            {
                string path = binding.effectivePath;
                if (path.Contains("dpad"))
                    uniqueControls.Add("D-Pad");
                else if (path.Contains("leftStick"))
                    uniqueControls.Add("Left Stick");
                else if (path.Contains("rightStick"))
                    uniqueControls.Add("Right Stick");
            }
        }

        return string.Join(" or ", uniqueControls);
    }
}

