using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CamaraManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraPadre;
    private CinemachineCamera virtualCamera;
    void Start()
    {
        FollowCamera();
    }
    private void Awake()
    {
        if (cameraPadre == null)
        {
            cameraPadre = GameObject.Find("Camara"); // Asegúrate de que el nombre coincida
        }

        if (virtualCamera == null && cameraPadre != null)
        {
            virtualCamera = cameraPadre.GetComponentInChildren<CinemachineCamera>();
        }

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
        FollowCamera();
    }
 
    void FollowCamera()
    {
        if (virtualCamera == null)
        {
            Debug.LogWarning("VirtualCamera no encontrada.");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player no encontrado.");
            return;
        }

        FocusTarget(player.transform);

    }

    public void FocusTarget(Transform newTarget)
    {
        virtualCamera.Target.TrackingTarget = newTarget;
    }
}
