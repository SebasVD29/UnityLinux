using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_MoveTutorial : UI_Tutorials
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowTutorial();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HideTutorial();
        }
    }

    public override void UpdateBindingText()
    {
        if (inputAction == null || inputAction.action == null) return;

        string displayString = GetAllGamepadBindings(inputAction.action);

        //Debug.Log(displayString);

        inputText.text = $"Use {displayString} to Move!";
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
                // Agregar más filtros si quieres otros controles
            }
        }

        return string.Join(" or ", uniqueControls);
    }
}
