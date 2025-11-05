using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_CharacterTutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText; // El texto hijo
    [Header("Input Action")]
    [SerializeField] protected InputActionReference inputActionMove;
    [SerializeField] protected InputActionReference inputActionSelect;
    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (tutorialText == null) return;

        string moveControls = "";
        string selectControls = "";

        // 🎮 Si hay mando conectado → usar bindings de mando
        if (Gamepad.current != null && inputActionMove?.action != null)
        {
            moveControls = GetAllGamepadBindings(inputActionMove.action);
            if (string.IsNullOrEmpty(moveControls))
                moveControls = inputActionMove.action.GetBindingDisplayString(group: "Gamepad");
        }
        else if (inputActionMove?.action != null) // 🖱️ teclado/ratón
        {
            moveControls = "Mouse";
        }

        // Selección
        if (inputActionSelect?.action != null)
        {
            if (Gamepad.current != null) // 🎮 mando
            {
                selectControls = inputActionSelect.action.GetBindingDisplayString(group: "Gamepad");
            }
            else // 🖱️ teclado/ratón
            {
                selectControls = "Right Click";
            }
        }

        tutorialText.text = $"Use {moveControls} and {selectControls} to Equip or Unequip items in the Character!";
    }
    public string GetAllGamepadBindings(InputAction action)
    {
        if (action == null) return "";

        HashSet<string> uniqueControls = new HashSet<string>();
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var binding = action.bindings[i];

            if (binding.groups != null && binding.groups.Contains("Gamepad"))
            {
                string path = binding.effectivePath; // ejemplo: "<Gamepad>/dpad/up"
                if (path.Contains("dpad"))
                    uniqueControls.Add("D-Pad");
                else if (path.Contains("leftStick"))
                    uniqueControls.Add("Left Stick");
                else if (path.Contains("rightStick"))
                    uniqueControls.Add("Right Stick");
            }else if (binding.groups != null && binding.groups.Contains("Key&Mouse"))
            {
                
            }
        }

        return string.Join(" or ", uniqueControls);
    }
}
