using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Tutorials : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Canvas tutorialCanvas;
    [SerializeField] protected TextMeshProUGUI inputText;        // Texto donde se muestra la tecla/botón
                                                               //[SerializeField] private Image ejemploImagen;   // Ejemplo de icono opcional

    [Header("Input Action")]
    [SerializeField] protected InputActionReference inputAction;

    protected virtual void Awake()
    {
        // Asegúrate de que el tutorial esté oculto al inicio
        if (tutorialCanvas != null)
            tutorialCanvas.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        UpdateBindingText();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    protected virtual void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    public void ShowTutorial()
    {
        if (tutorialCanvas != null)
            tutorialCanvas.gameObject.SetActive(true);

        UpdateBindingText();
    }

    public void HideTutorial()
    {
        if (tutorialCanvas != null)
            tutorialCanvas.gameObject.SetActive(false);
    }
    public virtual void UpdateBindingText()
    {
        if (inputAction == null || inputAction.action == null) return;
        //string displayString;
        string displayString = Gamepad.current != null
            ? inputAction.action.GetBindingDisplayString(group: "Gamepad")
            : inputAction.action.GetBindingDisplayString(group: "Key&Mouse");

        

    }
    public void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
        {
            // Si se conecta un mando nuevo o cambia teclado → mando
            UpdateBindingText();
        }
    }
}
