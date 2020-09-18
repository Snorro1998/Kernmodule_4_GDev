using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

[System.Serializable]
public class Lobby
{
    //monsterdata
    public byte[] monsterImgData;
    public string monsterName;

    public string lobbyName = "Naamloos";
    public int gameID = -1;
    public enum State
    {
        waiting,
        inGame,
        defeated
    }
    public State state = State.waiting;
    System.Random rand = new System.Random();
    public int playerTurnID = -1;

    public Character monster1 = new Character("Spaghetticode", 10);

    [System.Serializable]
    public class Character
    {
        public string name = "Niemand";
        public int hp = 7136;

        public Character()
        {

        }

        public Character(string _name, int _hp)
        {
            name = _name;
            hp = _hp;
        }

        public bool IsDead
        {
            get { return hp <= 0; }
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
        
        public SinglePlayer()// : base ("", 1)
        {

        }

        public SinglePlayer(string _name, NetworkConnection _connection, int _hp)// : base (_name, _hp)
        {
            name = _name;
            connection = _connection;
            hp = _hp;
        }     
    }

    public List<Character> turnOrder = new List<Character>();

    public List<SinglePlayer> allPlayers = new List<SinglePlayer>();
    public List<SinglePlayer> alivePlayers = new List<SinglePlayer>();

    public List<Character> allMonsters = new List<Character>();
    public List<Character> aliveMonsters = new List<Character>();

    public void AttackTarget(Character chr)
    {
        /*
        if (chr.GetType() == typeof(SinglePlayer))
        {
            Debug.Log("Een speler wordt aangevallen");
        }
        else
        {
            Debug.Log("Een monster wordt aangevallen");
        }*/
        int dmg = rand.Next(1, 3);
        chr.ReceiveDamage(dmg);
        if (chr.IsDead)
        {
            SendToAll(new MessageText(chr.name + " ging dood!"));
            if (chr.GetType() == typeof(SinglePlayer))
            {
                alivePlayers.Remove(chr as SinglePlayer);
            }
            else
            {
                aliveMonsters.Remove(chr);
            }
        }
    }

    public void PlayerAttack(NetworkConnection nw)
    {
        string name = TestServerBehaviour.Instance.playerManager.GetPlayerName(nw);
        // TODO implementeren dat je een target kunt kiezen
        Character target = monster1;
        SendToAll(new MessageText(name + " valt " + target.name + " aan!"));
        AttackTarget(target);
        NextTurn();
    }

    public T ChooseRandomFromList<T>(List<T> list)
    {
        T elem = default;
        if (list.Count > 0)
        {
            int index = rand.Next(0, list.Count);
            elem = list[index];
        }       
        return elem;
    }

    private void InitTurnOrderList()
    {
        turnOrder = new List<Character>();
        //kopieert playerlijst naar orderlijst
        foreach (SinglePlayer s in allPlayers)
        {
            turnOrder.Add(s);
            alivePlayers.Add(s);
        }
        foreach (Character c in allMonsters)
        {
            turnOrder.Add(c);
        }
        //hussel playerorderlijst
        for (int j = 0; j < allPlayers.Count; j++)
        {
            Character c = ChooseRandomFromList(turnOrder);
            turnOrder.Remove(c);
            turnOrder.Add(c);
        }
    }

    public void StartNewGame()
    {
        Debug.Log("startNewGame");
        //allMonsters.Add(monster1);
        //aliveMonsters.Add(monster1);

        //InitTurnOrderList();
        state = State.inGame;
        SendToAll(new MessageText("Spel wordt gestart"));
        InitRoom();
        //UpdateRoomInfo();
        //MessageStartGame mess = new MessageStartGame();
        //SendToAll(new MessageStartGame());
        /*NextTurn();*/
    }

    public void SendRoomInfo()
    {
        TestServerBehaviour.Instance.responder.SendImageToAllLobbyMembers(monsterImgData, this);
        //URLLoader.Instance.GetRandomImageForLobby(this);
        //stuur monsterafbeelding
        //stuur monsternaam
        //stuur playerlijst
        //stuur playerhps
    }

    public void InitRoom()
    {
        allMonsters.Add(monster1);
        aliveMonsters.Add(monster1);
        InitTurnOrderList();
        //URLLoader.Instance.GetRandomImageForLobby(this);
        //get image and its name from urlloader
    }

    public void MonsterTurn()
    {
        //kiest willekeurige speler
        SinglePlayer player = ChooseRandomFromList(alivePlayers);
        if (player != null)
        {
            SendToAll(new MessageText(monster1.name + " valt " + player.name + " aan!"));
            AttackTarget(player);
        }      
        //
        NextTurn();
    }

    private void PlayerTurn(SinglePlayer currentPlayer)
    {
        MessageGiveTurn mess = new MessageGiveTurn();
        MessageManager.SendMessage(TestServerBehaviour.Instance.networkDriver, mess, currentPlayer.connection);
    }

    public void GiveTurn(Character player)
    {
        if (player.GetType() == typeof(SinglePlayer))
        {
            PlayerTurn(player as SinglePlayer);
        }
        else
        {
            MonsterTurn();
        }
    }

    public void NextTurn()
    {
        if (aliveMonsters.Count == 0)
        {
            Debug.Log("Spelers hebben gewonnen!");
            SendToAll(new MessageText("Spelers hebben gewonnen!"));
            return;
        }
        else if (alivePlayers.Count == 0)
        {
            if (allPlayers.Count > 0)
            {
                Debug.Log("Alle spelers zijn gesneuveld!");
                SendToAll(new MessageText("Alle spelers zijn gesneuveld!"));
            }
            return;
        }
        //Debug.Log("nextturn");
        playerTurnID++;
        if (playerTurnID >= turnOrder.Count)
        {
            playerTurnID = 0;
        }
        Character currentPlayer = turnOrder[playerTurnID];
        if (currentPlayer == null)
        {
            Debug.Log("Iedereen is dood");
            return;
        }
        Debug.Log(currentPlayer.name + " is aan de beurt");
        GiveTurn(currentPlayer);
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

    public void ResetLobby()
    {
        state = State.waiting;
    }

    public void PlayerJoin(string name)
    {
        allPlayers.Add(new SinglePlayer(name, TestServerBehaviour.Instance.playerManager.GetPlayerConnection(name), 10));
    }

    public void PlayerLeave(string name)
    {
        foreach (SinglePlayer player in allPlayers)
        {
            if (player.name == name)
            {
                allPlayers.Remove(player);
                turnOrder.Remove(player);
                return;
            }
        }
        // reset de lobby als er geen spelers meer zijn
        if (allPlayers.Count == 0)
        {
            Debug.Log("resetlobby");
            ResetLobby();
        }
    }
}
