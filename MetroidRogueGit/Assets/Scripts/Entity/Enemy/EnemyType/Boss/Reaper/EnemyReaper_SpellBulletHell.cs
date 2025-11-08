using System.Collections;
using UnityEngine;

public class EnemyReaper_SpellBulletHell : MonoBehaviour
{
    public GameObject BulletHellPrefab;
    public int divisiones = 24;
    public float moveSpeedFire = 4f;
    public float tiempoEntreDisparos = 0.1f;
    public float delayEntreBalas = 0.02f;
    public float rotacionIncremento = 10f;
    public int oleadas = 5;

    public float radioVisual = 0.5f; // para gizmos

    void Start()
    {
        
    }

    public float DuracionTotal()
    {
        // Aproximación del tiempo total de vida del patrón
        return (oleadas * ((divisiones * delayEntreBalas) + tiempoEntreDisparos)) + 1f;
    }

    public void SetUpPortal(Entity_Combat combat)
    {
        StartCoroutine(BulletHellAttack(combat));
    }


    IEnumerator BulletHellAttack(Entity_Combat combat)
    {
        float rotacionBase = 0f;

        for (int wave = 0; wave < oleadas; wave++)
        {
            for (float angulo = 0; angulo < 360; angulo += 360f / divisiones)
            {
                float anguloRotado = angulo + rotacionBase;
                CrearBala(anguloRotado, combat);
                yield return new WaitForSeconds(delayEntreBalas);
            }

            rotacionBase += rotacionIncremento;
            yield return new WaitForSeconds(tiempoEntreDisparos);
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void CrearBala(float angulo, Entity_Combat combat)
    {
        Vector2 direction = new Vector2(Mathf.Cos(angulo * Mathf.Deg2Rad), Mathf.Sin(angulo * Mathf.Deg2Rad));
        GameObject bala = Instantiate(BulletHellPrefab, transform.position, Quaternion.identity);
        bala.GetComponent<Object_BulletHell>().SetupBulletHell(combat);


        Rigidbody2D rb = bala.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * moveSpeedFire;

        bala.transform.right = direction;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radioVisual);
    }
}
