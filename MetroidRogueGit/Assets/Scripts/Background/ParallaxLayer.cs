using UnityEngine;

[System.Serializable]
public class ParallaxLayer 
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float parallaxMultiplierY;
    [SerializeField] private float imageWidthOffset = 10;

    private float imageFullWidth;
    private float imageHalfWidth;

    public void CalculeteImageWidth()
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        imageHalfWidth = imageFullWidth / 2;
    }

    public void Move(float distanceToMoveX, float distanceToMoveY)
    {
        background.position += (Vector3)(Vector2.right * distanceToMoveX * parallaxMultiplier
                              + Vector2.up * distanceToMoveY * parallaxMultiplierY);

    }

    public void LoopBackground(float cameraLeftEdge, float camereRightEgde)
    {
        float imageRightEdge = background.position.x + imageHalfWidth - imageWidthOffset;
        float imageLeftEdge = background.position.x - imageHalfWidth + imageWidthOffset;

        if (imageRightEdge < cameraLeftEdge)
            background.position += Vector3.right * imageFullWidth;
        else if (imageLeftEdge > camereRightEgde)
            background.position += Vector3.right * -imageFullWidth;
    }
    public void ForceAlign(float cameraX, float cameraY)
    {
        Vector3 newPos = new Vector3(cameraX * parallaxMultiplier, cameraY * parallaxMultiplierY, background.position.z);
        background.position = newPos;
    }
}
