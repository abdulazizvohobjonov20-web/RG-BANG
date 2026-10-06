using UnityEngine;
using System;

/// <summary>
/// Global event system for RG BANG
/// Decouples systems from direct references
/// </summary>
public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action OnMatchOver;
    public event Action<int> OnPlayerStarCollected;
    public event Action<string> OnPlayerDamaged;
    public event Action<string, string> OnAbilityCast;
    public event Action<string, int> OnPlayerEliminated;
    public event Action<string> OnPlayerRespawned;
    public event Action<float> OnTimerUpdated;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action<int> OnPlayerLeftMatch;

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

    public void TriggerGameStart() => OnGameStart?.Invoke();
    public void TriggerGameEnd() => OnGameEnd?.Invoke();
    public void TriggerMatchOver() => OnMatchOver?.Invoke();
    public void TriggerPlayerStarCollected(int starCount) => OnPlayerStarCollected?.Invoke(starCount);
    public void TriggerPlayerDamaged(string playerId) => OnPlayerDamaged?.Invoke(playerId);
    public void TriggerAbilityCast(string playerId, string abilityName) => OnAbilityCast?.Invoke(playerId, abilityName);
    public void TriggerPlayerEliminated(string playerId, string killedBy) => OnPlayerEliminated?.Invoke(playerId, killedBy);
    public void TriggerPlayerRespawned(string playerId) => OnPlayerRespawned?.Invoke(playerId);
    public void TriggerTimerUpdated(float timeRemaining) => OnTimerUpdated?.Invoke(timeRemaining);
    public void TriggerGamePaused() => OnGamePaused?.Invoke();
    public void TriggerGameResumed() => OnGameResumed?.Invoke();
    public void TriggerPlayerLeftMatch(int playersRemaining) => OnPlayerLeftMatch?.Invoke(playersRemaining);
}
