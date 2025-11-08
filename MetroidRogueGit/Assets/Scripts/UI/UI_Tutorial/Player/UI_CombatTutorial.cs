using UnityEngine;
using UnityEngine.InputSystem;

public class UI_CombatTutorial : UI_Tutorials
{
    [SerializeField] private GameObject enemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.SetActive(true);
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

        //Debug.Log(displayString);

        inputText.text = $"Press {displayString} to Attack!";
    }
}
