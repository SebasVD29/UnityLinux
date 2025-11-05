using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


public class BossManager : MonoBehaviour
{
    [Header("Boss Setup")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private GameObject bossCanvas;
    [SerializeField] private GameObject bossGrid;


    [Header("Camera")]
    [SerializeField] private CamaraManager cameraManager;

    private bool bossTriggered = false;
    private Player player;
    private PlayerInputSet input;
    private Enemy_Reaper bossInstance;

    private void Start()
    {
        input = InputManager.Instance.InputSet;
        player = Player.instance;
        input.UI.Character.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bossTriggered || !collision.CompareTag("Player")) return;


        bossTriggered = true;
        
        StartCoroutine(BossIntroSequence());
        
    }

    private IEnumerator BossIntroSequence()
    {

        LockPlayer(true);

        bossPrefab.SetActive(true);

        bossInstance = bossPrefab.GetComponent<Enemy_Reaper>();

        if (bossInstance.facingDir != -1)
            bossInstance.Flip(); // mira hacia la izquierda

        cameraManager.FocusTarget(bossInstance.transform);
        bossGrid.SetActive(true);
        //yield return new WaitForSeconds(1f);

        bossInstance.IntroBoss(true);
        yield return new WaitForSeconds(0.6f);

        if (bossCanvas != null)
            bossCanvas.SetActive(true);

        yield return new WaitForSeconds(0.7f);


        cameraManager.FocusTarget(player.transform);

        LockPlayer(false);
        bossInstance.IntroBoss(false);
        gameObject.SetActive(false);
    }

    public void BossDeath()
    {
        Debug.Log("Reaper DEATH Manager");
        bossGrid.SetActive(false);
        bossCanvas.SetActive(false);
        bossPrefab.SetActive(false);
     
    }
   
    private void LockPlayer(bool value)
    {
        if (player == null) return;

        if (value)
        {
            input.Disable();
            player.SetVelocity(0, 0);
        }
        else
        {
            input.Player.Enable();
            input.UI.Options.Enable();
        }
    }
}
