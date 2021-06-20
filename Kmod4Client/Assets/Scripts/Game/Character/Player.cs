using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

[System.Serializable]
public class Player : Character
{
    public NetworkConnection playerConnection;

    public Player(string _playerName, NetworkConnection _playerConnection, int hp)
    {
        charName = _playerName;
        playerConnection = _playerConnection;
    }
}
