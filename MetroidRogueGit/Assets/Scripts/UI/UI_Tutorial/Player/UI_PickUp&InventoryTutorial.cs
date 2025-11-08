using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PickUpTutorial : UI_Tutorials
{
    [Header("Input Action")]
    [SerializeField] protected InputActionReference anotherInputAction;
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

        string displayString = Gamepad.current != null
           ? inputAction.action.GetBindingDisplayString(group: "Gamepad")
           : inputAction.action.GetBindingDisplayString(group: "Key&Mouse");

        string anotherDisplayString = Gamepad.current != null
           ? anotherInputAction.action.GetBindingDisplayString(group: "Gamepad")
           : anotherInputAction.action.GetBindingDisplayString(group: "Key&Mouse");

        //Debug.Log(displayString + " / " + anotherDisplayString);

        inputText.text = $"Use {displayString} to Pick up Items! \n" +
                        $"And use {anotherDisplayString} to Open the Inventory!";
    }
}
