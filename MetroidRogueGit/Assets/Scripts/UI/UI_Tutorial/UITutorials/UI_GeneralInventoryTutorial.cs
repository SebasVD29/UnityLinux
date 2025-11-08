using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_GeneralInventorylTutorial : UI_Tutorials
{
    private PlayerInputSet input;

    [SerializeField] private List<Image> allInventoryTutorials = new List<Image>();

    private int currentTutorialIndex = 0;
    protected override void Awake()
    {
        base.Awake();

        input = InputManager.Instance.InputSet;
    }

    public void SetupControlsUI()
    {
        if (input == null) return;

        input.UI.Submit.performed += ctx => ShowNextTutorial();
    }

    protected override void OnEnable()
    {
        UpdateBindingText();
        foreach (var tutorial in allInventoryTutorials)
            tutorial.gameObject.SetActive(false);

        currentTutorialIndex = 0;

        if (allInventoryTutorials.Count > 0)
            allInventoryTutorials[currentTutorialIndex].gameObject.SetActive(true);
    }

    private void ShowNextTutorial()
    {
        if (allInventoryTutorials.Count == 0) return;

        // Ocultar el actual
        allInventoryTutorials[currentTutorialIndex].gameObject.SetActive(false);

        // Avanzar al siguiente
        currentTutorialIndex++;

        if (currentTutorialIndex < allInventoryTutorials.Count)
        {
            // Mostrar el siguiente
            allInventoryTutorials[currentTutorialIndex].gameObject.SetActive(true);
        }
        else
        {
            var inventory = GetComponentInParent<UI_Inventory>();
            if (inventory != null)
                inventory.MarkTutorialAsComplete();

            // Ya no hay más -> ocultar el tutorial completo
            gameObject.SetActive(false);
        }
    }
    public override void UpdateBindingText()
    {
        if (inputAction == null || inputAction.action == null) return;

        string displayString = Gamepad.current != null
            ? inputAction.action.GetBindingDisplayString(group: "Gamepad")
            : inputAction.action.GetBindingDisplayString(group: "Key&Mouse");

        inputText.text = $"Press {displayString} to continue!";
    }
}
