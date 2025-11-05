using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Waypoint : MonoBehaviour
{
    public string waypointId;
    [SerializeField] private string transferToScene;
    [SerializeField] private string connectedWaypointId;
    [Space]
    [SerializeField] private RespawnType waypointType;
    [SerializeField] private RespawnType conntedWaypoint;
    [SerializeField] private Transform respwanPoint;
    [SerializeField] private bool canBeTriggered = true;

    public RespawnType GetWaypointType() => waypointType;

    public Vector3 GetPositionAndSetTriggerFalse()
    {
        canBeTriggered = false;
        return respwanPoint == null ? transform.position : respwanPoint.position;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(waypointId))
        {
            waypointId = System.Guid.NewGuid().ToString();
            //EditorUtility.SetDirty(this); // marca el objeto como modificado para guardarlo
        }
#endif
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + transferToScene;

        if (waypointType == RespawnType.Enter)
            conntedWaypoint = RespawnType.Exit;

        if (waypointType == RespawnType.Exit)
            conntedWaypoint = RespawnType.Enter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;
      
        GameManager.instance.ChangeScene(transferToScene, conntedWaypoint, waypointId);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
        
    }

    public bool ConnectedTo(string otherId)
    {
        return connectedWaypointId == otherId;
    }

    [ContextMenu("Generate New Waypoint ID")]
    private void GenerateNewId()
    {
        waypointId = System.Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);
        Debug.Log($"New waypointId generated for {gameObject.name}: {waypointId}");
    }
}
