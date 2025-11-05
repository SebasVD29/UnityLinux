using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CinemachineCamera))]
[RequireComponent(typeof(CinemachineConfiner2D))]
public class ConfinderController : MonoBehaviour
{




    [Tooltip("Tag del GameObject que contiene el Collider2D de confinamiento.")]
    [SerializeField] private string colliderTag = "Confinder";

    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        AssignConfiner();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignConfiner();
    }

    private void AssignConfiner()
    {
        GameObject confinderGO = GameObject.FindGameObjectWithTag(colliderTag);
        if (confinderGO == null)
        {
            Debug.LogWarning($"⚠️ No se encontró un GameObject con el tag '{colliderTag}'");
            return;
        }

        Collider2D collider = confinderGO.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("⚠️ El GameObject no tiene un Collider2D asignado.");
            return;
        }

        confiner.BoundingShape2D = collider;
        confiner.InvalidateBoundingShapeCache();
        //Debug.Log($"✅ Confiner asignado automáticamente a: {confinderGO.name}");
    }


}
