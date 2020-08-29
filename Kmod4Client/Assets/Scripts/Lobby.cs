using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

[System.Serializable]
public class Lobby
{
    public string lobbyName = "Naamloos";
    public int gameID = -1;
    public enum State
    {
        waiting,
        inGame,
        defeated
    }
    public State state = State.waiting;
    public string monsterName = "Spaghetticode";
    public int monsterHP = 100;

    System.Random rand = new System.Random();
    public int playerTurnID = -1;

    [System.Serializable]
    public class Character
    {
        public string name = "Keesie";
        public int hp = 100;

        public Character(string _name, int _hp)
        {
            name = _name;
            hp = _hp;
        }

        public void ReceiveDamage(int damage)
        {
            hp -= damage;
        }
    }

    [System.Serializable]
    public class SinglePlayer : Character
    {
        public NetworkConnection connection;

        public SinglePlayer() : base("", 1)
        {

        }

        public SinglePlayer(string _name, NetworkConnection _connection, int _hp) : base(_name, _hp)
        {
            name = _name;
            connection = _connection;
            hp = _hp;
        }
    }

    public List<SinglePlayer> allPlayers = new List<SinglePlayer>();
    public List<SinglePlayer> playerTurnOrder = new List<SinglePlayer>();

    public void PlayerAttack(NetworkConnection nw)
    {
        string name = TestServerBehaviour.Instance.playerManager.GetPlayerName(nw);
        SendToAll(new MessageText(name + " valt aan!"));
        monsterHP--;
        NextTurn();
    }

    public void StartNewGame()
    {
        Debug.Log("startNewGame");
        //kopieert playerlijst naar playerorderlijst
        for (int i = 0; i < allPlayers.Count; i++)
        {
            playerTurnOrder.Add(allPlayers[i]);
        }
        //hussel playerorderlijst
        for (int j = 0; j < allPlayers.Count; j++)
        {
            int a = rand.Next(0, allPlayers.Count);
            SinglePlayer s = allPlayers[a];
            playerTurnOrder.RemoveAt(a);
            playerTurnOrder.Add(s);
        }
        state = State.inGame;
        MessageStartGame mess = new MessageStartGame();
        SendToAll(new MessageStartGame());
        SendToAll(new MessageText("Spel wordt gestart"));
        NextTurn();
    }

    public void MonsterTurn()
    {
        //kiest willekeurige speler
        int index = rand.Next(0, allPlayers.Count);
        SinglePlayer player = allPlayers[index];
        SendToAll(new MessageText(monsterName + " valt " + player.name + " aan!"));
        player.ReceiveDamage(1);
        //
        NextTurn();
    }

    public void GiveTurn(SinglePlayer currentPlayer)
    {
        MessageGiveTurn mess = new MessageGiveTurn();
        MessageManager.SendMessage(TestServerBehaviour.Instance.networkDriver, mess, currentPlayer.connection);
    }

    public void NextTurn()
    {
        Debug.Log("nextturn");
        playerTurnID++;
        if (playerTurnID == allPlayers.Count)
        {
            Debug.Log("monsterturn");
            MonsterTurn();
            playerTurnID = 0;
        }
        else
        {
            Debug.Log("playerturn");
            if (playerTurnID > allPlayers.Count - 1) playerTurnID = 0;
            GiveTurn(playerTurnOrder[playerTurnID]);
        }
    }

    public Lobby(string _name, int _gameID, State _state, List<SinglePlayer> players)
    {
        lobbyName = _name;
        gameID = _gameID;
        state = _state;
        allPlayers = players;
    }

    public void SendToAll<T>(T message) where T : Message
    {
        foreach (SinglePlayer player in allPlayers)
        {
            MessageManager.SendMessage(TestServerBehaviour.Instance.networkDriver, message, player.connection);
        }
    }

    public void PlayerJoin(string name)
    {
        allPlayers.Add(new SinglePlayer(name, TestServerBehaviour.Instance.playerManager.GetPlayerConnection(name), 100));
    }

    public void PlayerLeave(string name)
    {
        foreach (SinglePlayer player in allPlayers)
        {
            if (player.name == name)
            {
                allPlayers.Remove(player);
                playerTurnOrder.Remove(player);
                return;
            }
        }
    }
}
