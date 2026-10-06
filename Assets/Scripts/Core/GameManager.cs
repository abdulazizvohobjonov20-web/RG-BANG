using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Central game state manager for RG BANG
/// Controls match flow, scoring, and game lifecycle
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float matchDuration = GameConstants.MATCH_DURATION;
    [SerializeField] private int maxPlayers = GameConstants.MAX_PLAYERS_PER_MATCH;

    private GameState currentGameState = GameState.MainMenu;
    private float matchTimeRemaining;
    private bool matchActive = false;

    private List<PlayerController> activePlayers = new List<PlayerController>();
    private Dictionary<string, int> playerStars = new Dictionary<string, int>();
    private string winnerPlayerId = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EventSystem.Instance.OnGameStart += StartMatch;
        EventSystem.Instance.OnGameEnd += EndMatch;
    }

    private void Update()
    {
        if (matchActive && matchTimeRemaining > 0)
        {
            matchTimeRemaining -= Time.deltaTime;
            EventSystem.Instance.TriggerTimerUpdated(matchTimeRemaining);

            if (matchTimeRemaining <= 0)
            {
                matchTimeRemaining = 0;
                EndMatch();
            }
        }
    }

    public void ChangeGameState(GameState newState)
    {
        currentGameState = newState;
        Debug.Log($"[GameManager] State changed to: {newState}");
    }

    public GameState GetCurrentGameState() => currentGameState;

    public void RegisterPlayer(PlayerController player)
    {
        if (!activePlayers.Contains(player))
        {
            activePlayers.Add(player);
            playerStars[player.GetPlayerId()] = 0;
            Debug.Log($"[GameManager] Registered player: {player.GetPlayerName()}");
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        activePlayers.Remove(player);
        playerStars.Remove(player.GetPlayerId());
    }

    public void AddStarToPlayer(string playerId, int starCount)
    {
        if (playerStars.ContainsKey(playerId))
        {
            playerStars[playerId] += starCount;
            EventSystem.Instance.TriggerPlayerStarCollected(playerStars[playerId]);
        }
    }

    public int GetPlayerStars(string playerId)
    {
        return playerStars.ContainsKey(playerId) ? playerStars[playerId] : 0;
    }

    public List<PlayerController> GetAllPlayers() => new List<PlayerController>(activePlayers);

    public Dictionary<string, int> GetLeaderboard()
    {
        return playerStars.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }

    private void StartMatch()
    {
        matchActive = true;
        matchTimeRemaining = matchDuration;
        currentGameState = GameState.Playing;
        Debug.Log("[GameManager] Match started!");
    }

    private void EndMatch()
    {
        matchActive = false;
        currentGameState = GameState.ResultsScreen;

        // Determine winner
        var leaderboard = GetLeaderboard();
        if (leaderboard.Count > 0)
        {
            winnerPlayerId = leaderboard.First().Key;
        }

        Debug.Log($"[GameManager] Match ended! Winner: {winnerPlayerId}");
        EventSystem.Instance.TriggerMatchOver();
    }

    public string GetWinnerId() => winnerPlayerId;

    public float GetMatchProgress()
    {
        return 1f - (matchTimeRemaining / matchDuration);
    }

    public float GetTimeRemaining() => matchTimeRemaining;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (EventSystem.Instance != null)
        {
            EventSystem.Instance.OnGameStart -= StartMatch;
            EventSystem.Instance.OnGameEnd -= EndMatch;
        }
    }
}
