
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public StatType Type;
    public float Value;
}

public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

    [Header("Details")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4;
    [SerializeField] private bool canBeUsed = true;

    [Header("Floaty")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = .1f;
    private Vector3 startPosition;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;

    }

    private void Update()
    {
        
        float yOffSet = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffSet);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeUsed == false) 
            return;

        statsToModify = collision.GetComponent<Entity_Stats>();

        StartCoroutine(BuffCo(buffDuration));
    }

    IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;
        ApplyBuff(true);

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);
 
        Destroy(gameObject);
    }

    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if(apply)
                statsToModify.GetStatByType(buff.Type).AddModifier(buff.Value, buffName);
            else
                statsToModify.GetStatByType(buff.Type).RemoveModifier(buffName);
        }
    }
}
