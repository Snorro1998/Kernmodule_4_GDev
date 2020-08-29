using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class TestPlayerManager : MonoBehaviour
{
    public PlayerDataDB playerDB;

    //public List<TestPlayerData> onlinePlayers = new List<TestPlayerData>();
    //public List<NetworkConnection> playerConnections = new List<NetworkConnection>();
    public List<OnlinePlayer> newOnlinePlayers = new List<OnlinePlayer>();

    [System.Serializable]
    public class OnlinePlayer
    {
        public TestPlayerData player;
        public NetworkConnection connection;
        public int lobbyID = -1;

        public OnlinePlayer(TestPlayerData _player, NetworkConnection _connection)
        {
            player = _player;
            connection = _connection;
        }
    }

    public string GetPlayerName(NetworkConnection _connection)
    {
        string txt = "";
        foreach (OnlinePlayer player in newOnlinePlayers)
        {
            if (player.connection == _connection)
            {
                txt = player.player.userName;
            }
        }
        return txt;
    }

    public NetworkConnection GetPlayerConnection(string _name)
    {
        NetworkConnection nw = new NetworkConnection();
        foreach (OnlinePlayer player in newOnlinePlayers)
        {
            if (player.player.userName == _name)
            {
                nw = player.connection;
            }
        }
        return nw;
    }

    [System.Serializable]
    public class PlayerDataDB
    {
        public List<TestPlayerData> players = new List<TestPlayerData>();
    }

    public enum LoginResult
    {
        nonExistingName,
        alreadyLogginIn,
        wrongPassword,
        success
    }

    public void LogOutPlayer(NetworkConnection nw)
    {
        /*
        for (int i = 0; i < newOnlinePlayers.Count; i++)
        {
            var player = newOnlinePlayers[i];
            if (player.connection == nw)
            {
                if (player.lobbyID != -1)
                {
                    TestServerBehaviour.Instance.lobbies[player.lobbyID].playerNames.Remove(player.player.userName);
                    //TODO laat anderen in lobby weten dat hij uitlogt
                }
                newOnlinePlayers.Remove(newOnlinePlayers[i]);
                return;
            }
        }
        
        Debug.Log("LogOutPlayer: Speler bestaat niet");
        */
    }

    public void LoginPlayer(NetworkConnection nw, TestPlayerData reqPlayer)
    {
        OnlinePlayer player = new OnlinePlayer(reqPlayer, nw);
        newOnlinePlayers.Add(player);
        /*
        playerConnections.Add(nw);
        onlinePlayers.Add(reqPlayer);
        */
    }

    /// <summary>
    /// Geeft het resultaat van een inlogpoging met gegevens
    /// </summary>
    /// <param name="_username"></param>
    /// <param name="_password"></param>
    /// <returns></returns>
    public LoginResult AttemptLogin(string _username, string _password, ref NetworkConnection nw)
    {
        var reqPlayer = playerDB.players.Find(item => item.userName == _username);
        //LoginResult result = LoginResult.alreadyLogginIn;
        //spelernaam bestaat
        if (reqPlayer != null)
        {
            foreach (OnlinePlayer player in newOnlinePlayers)
            {
                if (player.player == reqPlayer)
                {
                    return LoginResult.alreadyLogginIn;
                }
            }
            if (reqPlayer.password == _password)
            {
                LoginPlayer(nw, reqPlayer);
                return LoginResult.success;
            }
            else
            {
                return LoginResult.wrongPassword;
            }
        }
        else
        {
            return LoginResult.nonExistingName;
        }
        /*
        //speler is niet ingelogd
        if (!onlinePlayers.Contains(reqPlayer))
        {
            if(reqPlayer.password == _password)
            {
                LoginPlayer(nw, reqPlayer);
                result = LoginResult.success;
            }
            else
            {
                result = LoginResult.wrongPassword;
            }
        }
    }
    //spelernaam bestaat niet
    else
    {
        result = LoginResult.nonExistingName;
    }
    return result;*/
    }

    private void Start()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string fileContent = File.ReadAllText(Application.dataPath + "/save.txt");
            playerDB = JsonUtility.FromJson<PlayerDataDB>(fileContent);
        }
        else
        {
            playerDB = new PlayerDataDB();
            playerDB.players.Add(new TestPlayerData("Jan", "Klaas"));
            playerDB.players.Add(new TestPlayerData("Hopjesvla", "Mona"));
            playerDB.players.Add(new TestPlayerData("Philip", "Habing"));
            playerDB.players.Add(new TestPlayerData("Michael", "Jackson"));
            playerDB.players.Add(new TestPlayerData("Foxy", "Knot"));
            playerDB.players.Add(new TestPlayerData("Kulau", "Kat"));

            string fileContent = JsonUtility.ToJson(playerDB);
            File.WriteAllText(Application.dataPath + "/save.txt", fileContent);
        }
    }
}
