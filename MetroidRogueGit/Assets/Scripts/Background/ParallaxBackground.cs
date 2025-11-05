using Unity.Cinemachine;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    //[SerializeField] private GameObject cameraPadre;
    public Camera mainCamera;
    private float lastCameraPositionX;
    private float lastCameraPositionY;
    private float cameraHalfWidth;


    [SerializeField] private ParallaxLayer[] backgroundLayers;

    void Start()
    {

       

    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("❌ No se encontró una cámara con la tag 'MainCamera'.");
            return;
        }

        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        CalculateImageLenght();

        AlignBackgroundWithCamera(); 

        lastCameraPositionX = mainCamera.transform.position.x;
        lastCameraPositionY = mainCamera.transform.position.y;
    }

    private void FixedUpdate()
    {
        float currentCameraPositionX = mainCamera.transform.position.x;  
        float currentCameraPositionY = mainCamera.transform.position.y; 
        
        float distanceToMovX = currentCameraPositionX - lastCameraPositionX;
        float distanceToMoveY = currentCameraPositionY - lastCameraPositionY;

        lastCameraPositionX = currentCameraPositionX;
        lastCameraPositionY = currentCameraPositionY;

        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;


        foreach (ParallaxLayer layer in backgroundLayers)
        {
            
            layer.Move(distanceToMovX, distanceToMoveY);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    void CalculateImageLenght()
    {
        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.CalculeteImageWidth();
        }
    }
    private void AlignBackgroundWithCamera()
    {
        float camX = mainCamera.transform.position.x;
        float camY = mainCamera.transform.position.y;

        lastCameraPositionX = camX;
        lastCameraPositionY = camY;

        float cameraLeftEdge = camX - cameraHalfWidth;
        float cameraRightEdge = camX + cameraHalfWidth;

        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.ForceAlign(camX, camY);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge); // opcional
        }
    }

}
