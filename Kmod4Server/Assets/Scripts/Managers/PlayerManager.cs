using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

[System.Serializable]
public class PlayerManager : Singleton<PlayerManager>
{
    public List<Player> players = new List<Player>();

    public Player GetPlayerByName(string _playerName)
    {
        foreach (var p in players)
        {
            if (p.charName == _playerName)
            {
                return p;
            }
        }
        return null;
    }

    public Player GetPlayerByConnection(NetworkConnection connection)
    {
        foreach (var p in players)
        {
            if (p.playerConnection == connection)
            {
                return p;
            }
        }
        return null;
    }

    public bool PlayerIsLoggedIn(string playerName, NetworkConnection playerConnection)
    {
        bool nameOnline = GetPlayerByName(playerName) != null;
        bool connectionOnline = GetPlayerByConnection(playerConnection) != null;
        return (nameOnline || connectionOnline);
    }

    public void LogoutPlayer(NetworkConnection connection)
    {
        var player = GetPlayerByConnection(connection);
        if (player != null)
        {
            players.Remove(player);
            UpdateOnlinePlayers();
        }
    }

    public void LoginPlayer(string name, NetworkConnection connection)
    {
        players.Add(new Player(name, connection, 10));
        UpdateOnlinePlayers();
    }

    /// <summary>
    /// Stuurt actuele playerlist naar clients.
    /// </summary>
    public void UpdateOnlinePlayers()
    {
        GameManager.Instance.UpdatePlayers();
        var message = new MessagePlayersOnlineUpdate(players);
        ServerBehaviour.Instance.SendMessageToAll(message);
    }
}
