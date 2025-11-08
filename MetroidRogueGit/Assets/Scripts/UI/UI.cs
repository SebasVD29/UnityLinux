using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class UI : MonoBehaviour
{
    public static UI instance;
    private PlayerInputSet input;

    [SerializeField] private GameObject[] uiElements;
    public bool alternativeInput { get; private set; }

    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }

    [SerializeField] private UI_Inventory inventoryUI;
    public UI_InGame inGameUI { get; private set; }
    public UI_Options optionsUI { get; private set; }
    public UI_FadeScreen fadeScreenUI { get; private set; }
    public UI_DeathScreen deathScreenUI { get; private set; }


    private bool inventoryEnabled;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
        statToolTip = GetComponentInChildren<UI_StatToolTip>();

        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        optionsUI = GetComponentInChildren<UI_Options>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);


        input = InputManager.Instance.InputSet;

        inventoryEnabled = inventoryUI.gameObject.activeSelf;
        
    }

    public void OnEnable()
    {
        input.UI.Character.performed += ctx => ToggleCharacterUI();

        input.UI.Options.performed += ctx =>
        {
            foreach(var element in uiElements)
            {
                if (element.activeSelf)
                {
                    Time.timeScale = 1;
                    SwitchToInGameUI();
                    return;
                }
            }

            Time.timeScale = 0;
            OpenOptionsUI();
        };
    }
    public void OpenDeathScreenUI()
    {
        SwitchTo(deathScreenUI.gameObject);
        input.Disable(); // pay attention to this if you use gamepad
    }

    public void OpenOptionsUI()
    {
        HideAllTooltips();
        StopPlayerControls(true);
        SwitchTo(optionsUI.gameObject);
    }

    public void SwitchToInGameUI()
    {
        HideAllTooltips();
        StopPlayerControls(false);
        SwitchTo(inGameUI.gameObject);

        inventoryEnabled = false;
    }
    private void SwitchTo(GameObject objectToSwitchOn)
    {
        foreach (var element in uiElements)
            element.gameObject.SetActive(false);

        objectToSwitchOn.SetActive(true);
    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach (var element in uiElements)
        {
            if (element.activeSelf)
            {
                StopPlayerControls(true);
                return;
            }
        }

        StopPlayerControls(false);
    }
   
    public void ToggleCharacterUI()
    {
        Debug.Log("ui");
        inventoryUI.transform.SetAsLastSibling();
        SetTooltipsAsLastSibling();
        fadeScreenUI.transform.SetAsLastSibling();

        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);
       
        HideAllTooltips();

        StopPlayerControlsIfNeeded();
    }
    public void HideAllTooltips()
    {
        itemToolTip.ShowToolTip(false, null);       
        statToolTip.ShowToolTip(false, null);
    }
    private void SetTooltipsAsLastSibling()
    {
        itemToolTip.transform.SetAsLastSibling();
        statToolTip.transform.SetAsLastSibling();
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
        {
            input.UI.Enable();
            input.Player.Disable();
        }
        else
        {
            input.Player.Enable();
            input.UI.Disable();
            input.UI.Character.Enable();
            input.UI.Options.Enable();
            
        }
    }

    public void ResetIU()
    {

        SwitchToInGameUI();
        input.UI.Enable();

    }
}
