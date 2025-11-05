using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{

    [SerializeField] GameObject OptionsCanvas;
    [SerializeField] UI_InputManager uiInputManager;
   
    public void PlayBtn()
    {
        GameManager.instance.ContinuePlay();
    }
    public void OptionsBtn()
    {
        //gameObject.SetActive(false);
        //OptionsCanvas.SetActive(true);
        uiInputManager.ActivateMenu("UI_Options");

    }
    public void ExitBtn()
    {
        Application.Quit();
    }

}
