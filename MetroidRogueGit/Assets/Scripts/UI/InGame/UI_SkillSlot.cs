using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_SkillSlot : MonoBehaviour
{

    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button button;

    private Skill_DataSO skillData;

    public SkillType skillType;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;

    [SerializeField] private Sprite defaultBg;


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        button = GetComponent<Button>();
        skillIcon = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        gameObject.name = "UI_SkillSlot - " + skillType.ToString();
    }

    public void SetupSkillSlot(Skill_DataSO selectedSkill)
    {
        this.skillData = selectedSkill;

        Color color = Color.black; 
        color.a = .6f;

        cooldownImage.color = color;
        inputKeyText.text = inputKeyName;
        skillIcon.sprite = selectedSkill.icon;

    }

    public void StartCooldown(float cooldown)
    {
        cooldownImage.fillAmount = 1;
        StartCoroutine(CooldownCo(cooldown));
    }

    public void ResetCooldown() => cooldownImage.fillAmount = 0;
    private IEnumerator CooldownCo(float duration)
    {
        float timePassed = 0;

        while (timePassed < duration)
        {
            timePassed = timePassed + Time.deltaTime;
            cooldownImage.fillAmount = 1f - (timePassed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0;
    }
    public bool IsEmpty() => skillData == null;
    public SkillUnlockType GetSkillUnlockType() => skillData != null ? skillData.unlockData.unlockType : SkillUnlockType.None;
    public void ClearSlot()
    {
        
        inputKeyText.text = "Locked";
        skillIcon.sprite = defaultBg;
        cooldownImage.fillAmount = 1;

        //ResetCooldown();
    }
}
