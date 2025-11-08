using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UI_InputManager : MonoBehaviour
{
    [Header("Referencias")]
    public EventSystem eventSystem;
    public MenuFocus[] menus;

    private GameObject currentMenu;

    private void Start()
    {
        if (eventSystem == null)
            eventSystem = EventSystem.current;

        // Activa solo el primer menú
        foreach (var m in menus)
            m.menuCanvas.SetActive(false);

        if (menus.Length > 0)
            ActivateMenu(menus[0].menuCanvas.name);
    }
    private void Update()
    {
        if (eventSystem.currentSelectedGameObject == null && currentMenu != null)
        {
            var menuFocus = System.Array.Find(menus, m => m.menuCanvas == currentMenu);
            if (menuFocus.firstSelected != null)
                eventSystem.SetSelectedGameObject(menuFocus.firstSelected);
        }
    }

    public void ActivateMenu(string menuName)
    {
        StartCoroutine(SwitchMenu(menuName));
    }

    private IEnumerator SwitchMenu(string menuName)
    {
        // Desactivar todos los menús
        foreach (var m in menus)
            m.menuCanvas.SetActive(false);

        yield return null; // esperar 1 frame para que se actualice la UI

        // Activar el menú solicitado
        foreach (var m in menus)
        {
            if (m.menuCanvas.name == menuName)
            {
                m.menuCanvas.SetActive(true);
                currentMenu = m.menuCanvas;

                yield return null; // esperar otro frame para asegurar que el layout se reconstruya

                if (eventSystem != null && m.firstSelected != null)
                {
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(m.firstSelected);
                }

                break;
            }
        }
    }

    public void ReturnToMainMenu()
    {
        ActivateMenu("UI_MainMenu");
    }
}

[System.Serializable]
public class MenuFocus
{
    public GameObject menuCanvas;     // SubCanvas principal
    public GameObject firstSelected;  // Elemento inicial del Canvas
}