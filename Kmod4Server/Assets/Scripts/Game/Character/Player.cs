using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

[System.Serializable]
public class Player : Character
{
    public NetworkConnection playerConnection;
    //public int score;

    public Player(string _playerName, NetworkConnection _playerConnection, int hp) : base(hp)
    {
        charName = _playerName;
        playerConnection = _playerConnection;
        score = 0;
    }

    /*
    public void GetPoint(int amount)
    {
        score += amount;
    }*/
}
