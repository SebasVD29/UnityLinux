using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_StatsNavigator : MonoBehaviour
{
   
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private RectTransform content;  // el viewport content que contiene los UI_StatSlot
    [SerializeField] private float scrollSpeed = 1.5f;
    [SerializeField] private float inputDelay = 0.25f; // retardo entre cambios de selección

    private UI_StatSlot[] slots;
    private int currentIndex = 0;
    private float lastInputTime;

    private void Awake()
    {
        slots = content.GetComponentsInChildren<UI_StatSlot>();
    }

    private void Start()
    {
        //if (slots.Length > 0)
        //{
        //    SelectSlot(0);
        //}
    }

    private void Update()
    {
        if (Gamepad.current == null) return;

        float verticalInput = Gamepad.current.rightStick.ReadValue().y;

        // --- Scroll suave con stick ---
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            // Scroll positivo va hacia arriba en UI, negativo hacia abajo
            scrollBar.value = Mathf.Clamp01(scrollBar.value + verticalInput * scrollSpeed * Time.unscaledDeltaTime);

        }

        // --- Control de índice opcional si quieres navegar por slots internamente ---
        if (Time.time - lastInputTime > inputDelay)
        {
            if (verticalInput > 0.5f)
            {
                currentIndex = Mathf.Max(currentIndex - 1, 0);
                lastInputTime = Time.time;
            }
            else if (verticalInput < -0.5f)
            {
                currentIndex = Mathf.Min(currentIndex + 1, slots.Length - 1);
                lastInputTime = Time.time;
            }
        }
    }

    
}
