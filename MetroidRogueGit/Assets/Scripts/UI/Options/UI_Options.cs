using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;
  

    [SerializeField] GameObject MainMenu;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25;


    [Header("Screen Settings")]
    public UI_ScreenResolutions screenResolutions { get; private set; }
    public UI_DisplayMode displayMode { get; private set; }


    [Header("BGM Volume Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParametr;

    [Header("SFX Volume Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParametr;

    private void Awake()
    {
        screenResolutions = GetComponentInChildren<UI_ScreenResolutions>();
        displayMode = GetComponentInChildren<UI_DisplayMode>();
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();


    }

    public void BGMSliderValue(float value)
    {
        float newValue = MathF.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParametr, newValue);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = MathF.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParametr, newValue);
    }

    public void GoMainMenuBTN() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NonSpecific, "Null");
    public void GoMainPuebloBTN() => GameManager.instance.ChangeScene("MainPueblo", RespawnType.NonSpecific, "Null");
    public void GoBackBtn()
    {
        gameObject.SetActive(false);
        MainMenu.SetActive(true);
    }

    private void OnEnable()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, .6f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParametr, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParametr, bgmSlider.value);
    }

    public void LoadUpVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, .6f);
    }

}
