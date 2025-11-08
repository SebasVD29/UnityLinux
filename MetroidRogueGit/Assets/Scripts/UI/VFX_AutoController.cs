using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1;
    [SerializeField] private bool randomOffSet = true;
    [SerializeField] private bool randomRotation = true;

    [Header("Random Rotation")]
    [SerializeField] private float minRotation =0;
    [SerializeField] private float maxRotation = 360;

    [Header("Random Position")]
    [SerializeField] private float xMinOffSet = -.3f;
    [SerializeField] private float xMaxOffSet = .3f;

    [SerializeField] private float yMinOffSet = -.3f;
    [SerializeField] private float yMaxOffSet = .3f;

    private void Start()
    {
        ApplyRandomOffSet();
        ApplayRandomRotation();

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);

    }

    private void ApplyRandomOffSet()
    {
        if (randomOffSet == false)
            return;

        float xOffSet = Random.Range(xMinOffSet, xMaxOffSet);
        float yOffSet = Random.Range(yMinOffSet, yMaxOffSet);

        transform.position = transform.position + new Vector3(xOffSet, yOffSet);

    }

    void ApplayRandomRotation()
    {
        if (randomRotation == false)
            return;

        float zRotation = Random.Range(minRotation, maxRotation);


        transform.Rotate(0, 0, zRotation);
    }


}
