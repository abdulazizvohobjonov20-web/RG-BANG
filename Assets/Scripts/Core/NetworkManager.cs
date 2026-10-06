using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Placeholder for future online multiplayer
/// Currently handles local player management and bot spawning
/// </summary>
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private List<string> connectedPlayers = new List<string>();
    private bool isOnline = false;

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

    /// <summary>
    /// Simulates player connection (ready for real multiplayer)
    /// </summary>
    public void ConnectPlayer(string playerId)
    {
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
            Debug.Log($"[NetworkManager] Player connected: {playerId}");
        }
    }

    /// <summary>
    /// Simulates player disconnection
    /// </summary>
    public void DisconnectPlayer(string playerId)
    {
        connectedPlayers.Remove(playerId);
        Debug.Log($"[NetworkManager] Player disconnected: {playerId}");
    }

    /// <summary>
    /// Check if player is connected
    /// </summary>
    public bool IsPlayerConnected(string playerId) => connectedPlayers.Contains(playerId);

    /// <summary>
    /// Broadcast ability usage to all clients (future implementation)
    /// </summary>
    public void BroadcastAbility(string playerId, string abilityName)
    {
        // TODO: Send to all clients via network
        Debug.Log($"[NetworkManager] Broadcasting ability: {playerId} cast {abilityName}");
    }

    /// <summary>
    /// Broadcast damage event
    /// </summary>
    public void BroadcastDamage(string damageDealer, string targetPlayer, float damageAmount)
    {
        // TODO: Send to all clients via network
        Debug.Log($"[NetworkManager] {damageDealer} dealt {damageAmount} damage to {targetPlayer}");
    }

    /// <summary>
    /// Broadcast star collection
    /// </summary>
    public void BroadcastStarCollection(string playerId, int totalStars)
    {
        // TODO: Send to all clients via network
        Debug.Log($"[NetworkManager] {playerId} collected star. Total: {totalStars}");
    }

    public bool IsOnlineMode() => isOnline;

    public void SetOnlineMode(bool online) => isOnline = online;

    public List<string> GetConnectedPlayers() => new List<string>(connectedPlayers);
}
