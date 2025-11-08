using UnityEngine;

public class UI_DeathScreen : MonoBehaviour
{
    public void RestartRun()
    {
        GameManager.instance.RestartRun("Room1-V1", RespawnType.NonSpecific);
    }

 
}
