using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity entity;


    [Header("On Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .2f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("Steal Health VFX")]
    [SerializeField] private GameObject healthVfx;

    [Header("On Doing Damage VFX")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;
    [SerializeField] private Color hitVfxColor = Color.white;


    [Header("Element Color")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color electricVfx = Color.yellow;
    private Color originalHitVfxColor;


    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;

        originalHitVfxColor = hitVfxColor;
            
    }
    public void PlayOnDamageVfx()
    {
        if(onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }
    IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial;
    }

    IEnumerator PlayStatusVfxCo(Color color, float duration)
    {
        float tickInterval = .2f;
        float timeHasPassed = 0;

        Color lightColor = color * 1.2f;
        Color darkColor = color * .8f;

        bool toogle = false;


        while (timeHasPassed < duration)
        {
            sr.color = toogle ? lightColor : darkColor;
            toogle = !toogle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;
        }
        sr.color = Color.white;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(chillVfx, duration));

        if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(burnVfx, duration));

        if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(electricVfx, duration));

    }

    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;

    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);

        if(isCrit == false)
            vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;

        if (entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);

    }

    public void CreateOnHitVFX(Transform target, Color colorHit)
    {
        GameObject vfx = Instantiate(hitVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = colorHit;

    }

    public void CreateOnHealthVFX(Transform target)
    {
        Debug.Log("Curasao");

        if (healthVfx == null)
            return;
        

        GameObject vfx = Instantiate(healthVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
    public void UpdateOnHitColor(ElementType element)
    {

        if (element == ElementType.Ice) 
            hitVfxColor = chillVfx;

        if(element == ElementType.None)
            hitVfxColor = originalHitVfxColor;

    }

}
