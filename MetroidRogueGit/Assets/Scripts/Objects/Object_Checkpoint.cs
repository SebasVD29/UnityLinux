using UnityEditor;
using UnityEngine;

public class Object_Checkpoint : MonoBehaviour
{
    [SerializeField] private string checkpointId;
    [SerializeField] private Transform respawnPoint;

    public bool isActive { get; private set; }
    public static Object_Checkpoint LastCheckpoint { get; private set; }


    private void Awake()
    {
   
    }

    public string GetCheckpointId() => checkpointId;

    public Vector3 GetPosition() => respawnPoint == null ? transform.position : respawnPoint.position;


    public void ActivateCheckpoint(bool activate)
    {
        isActive = activate;

        if (activate)
            LastCheckpoint = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            ActivateCheckpoint(true);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointId))
        {
            checkpointId = System.Guid.NewGuid().ToString();
        }
#endif
    }
}
