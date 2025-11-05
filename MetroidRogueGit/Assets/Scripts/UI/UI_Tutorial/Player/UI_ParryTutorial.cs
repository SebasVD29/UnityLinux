using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_ParryTutorial : UI_Tutorials, ISaveable
{
    [SerializeField] protected TextMeshProUGUI anotherInputText;
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

        //Debug.Log(displayString);

        inputText.text = $"Press {displayString} to block the enemy attacks!";
        anotherInputText.text = $"If you press {displayString} at the right moment, you can perfectly block the enemy's attacks!";
    }

    public void SaveData(ref GameData data)
    {
        data.LevelTutorial = true;
    }

    public void LoadData(GameData data)
    {
        
    }
}
