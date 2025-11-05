using UnityEngine;

public class BackgroundDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f; // suficiente para matar al jugador
    [SerializeField] private bool instantRespawn = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;

        // Si el jugador tiene salud
        if (player.health != null)
        {
            player.health.ReduceHealth(damage);
        }

        if (instantRespawn)
        {
            RespawnPlayer(player);
        }
    }

    private void RespawnPlayer(Player player)
    {
        // Buscar último checkpoint o usar posición inicial
        Vector3 respawnPos = Object_Checkpoint.LastCheckpoint != null
            ? Object_Checkpoint.LastCheckpoint.GetPosition()
            : player.transform.position; // fallback

        player.TeleportPlayer(respawnPos);
 
    }
}
