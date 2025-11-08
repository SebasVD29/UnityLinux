using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;

    private string lastScenePlayed;
    private bool dataLoaded;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLastPlayerPosition(Vector3 position) => lastPlayerPosition = position;

    public void ContinuePlay()
    {
        var data = SaveManager.instance.GetGameData();
        var IdWaypoint = "";

        // Si LevelTutorial es false → forzar Room0, si es true → forzar Room1
        if (data.LevelTutorial == false)
        {
            lastScenePlayed = "Room0-V1";
            IdWaypoint = "cabc906b-d169-4cd1-86a5-1566af1bba53";
        }
        else
        {
            lastScenePlayed = "Room1-V1"; //MainHub, test Room1
            IdWaypoint = "bd0de3d2-4bbe-4fa5-a00a-08e929f9485f";
        }

        ChangeScene(lastScenePlayed, RespawnType.NonSpecific, IdWaypoint);
    }

    public void RestartRun(string sceneName, RespawnType respwanType)
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1;
        StartCoroutine(RestartRunCo(sceneName, respwanType));
    }


    public void ChangeScene(string sceneName, RespawnType respwanType, string fromWaypointId)
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1;
        StartCoroutine(ChangeSceneCo(sceneName, respwanType, fromWaypointId));
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType, string fromWaypointId)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        UI_FadeScreen fadeScreen = null;

        if (currentScene != "MainMenu")
        {
            fadeScreen = FindFadeScreenUI();

            if (fadeScreen != null)
            {
                fadeScreen.DoFadeIn(); // transparente → negro
                Debug.Log("Change DoFadeIn!");
                yield return fadeScreen.fadeEffectCo;
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        // Espera a que la escena se cargue completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // --- Cargar datos después de la escena ---
        dataLoaded = false;

        if (SaveManager.instance != null)
        {
            SaveManager.instance.LoadGame();
        }

        // Notificar a GameManager que los datos se cargaron
        dataLoaded = true;

        yield return new WaitForSeconds(0.5f);

        fadeScreen = FindFadeScreenUI();
        fadeScreen.DoFadeOut(); // negro → transparente
        Debug.Log("Change DoFadeOut!");

        Player player = Player.instance;

        if (player == null)
            yield break;

        Vector3 position = GetNewPlayerPosition(respawnType, fromWaypointId);

        if (position != Vector3.zero)
            player.TeleportPlayer(position);
    }
    private IEnumerator RestartRunCo(string sceneName, RespawnType respawnType)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        UI_FadeScreen fadeScreen = null;

        // Fade-in antes de cambiar de escena
        if (currentScene != "MainMenu")
        {
            fadeScreen = FindFadeScreenUI();
            if (fadeScreen != null)
            {
                fadeScreen.DoFadeIn();
                Debug.Log("Restart Run - DoFadeIn!");
                yield return fadeScreen.fadeEffectCo;
            }
        }

        // Cargar escena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
            yield return null;

        // Cargar datos
        dataLoaded = false;
        if (SaveManager.instance != null)
            SaveManager.instance.LoadGame();
        dataLoaded = true;

        yield return new WaitForSeconds(0.5f);

        // Fade-out después de cargar
        fadeScreen = FindFadeScreenUI();
        fadeScreen?.DoFadeOut();
        Debug.Log("Restart Run - DoFadeOut!");

        if (Player.instance != null)
        {
            Player.instance.ResetPlayer();

            // Teleport si hay posición válida
            Vector3 position = GetNewPlayerPosition(respawnType,"Null");
            if (position != Vector3.zero)
                Player.instance.TeleportPlayer(position);
        }

        // Resetear UI de muerte
        if (UI.instance != null)
            UI.instance.ResetIU();
    }


    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance != null)
            return UI.instance.fadeScreenUI;
        else
            return FindFirstObjectByType<UI_FadeScreen>();
    }


    private Vector3 GetNewPlayerPosition(RespawnType type, string fromWaypointId)
    {

        if (type == RespawnType.Portal)
        {
            Object_Portal portal = Object_Portal.instnace;

            Vector3 position = portal.GetPosition();

            portal.SetTrigger(false);
            portal.DisableIfNeeded();

            return position;
        }

        if (type == RespawnType.NonSpecific)
        {
            var data = SaveManager.instance.GetGameData();
            var checkpoints = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);
            var unlockedCheckpoints = checkpoints
                .Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointId(), out bool unlocked) && unlocked)
                .Select(cp => cp.GetPosition())
                .ToList();

            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
                .Select(wp => wp.GetPositionAndSetTriggerFalse())
                .ToList();

            var selectedPositions = unlockedCheckpoints.Concat(enterWaypoints).ToList(); // combine two lists into one

            if (selectedPositions.Count == 0)
                return Vector3.zero;

            return selectedPositions.
                OrderBy(position => Vector3.Distance(position, lastPlayerPosition)) // arrange form lowest to highest by comparing distance
                .First();
        }

        return GetWaypointPosition(type, fromWaypointId);
    }

    private Vector3 GetWaypointPosition(RespawnType type , string fromWaypointId)
    {
        var waypoints = FindObjectsByType<Object_Waypoint> (FindObjectsSortMode.None);

        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type && point.ConnectedTo(fromWaypointId))
                return point.GetPositionAndSetTriggerFalse();
        }

        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type)
                return point.GetPositionAndSetTriggerFalse();
        }

        return Vector3.zero;
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "Room0-V1";

        dataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;

        data.lastPlayerPosition = Player.instance.transform.position;
        data.lastScenePlayed = currentScene;
        dataLoaded = false;
    }
}
