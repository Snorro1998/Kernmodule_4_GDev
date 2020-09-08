using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System;
using Unity.Networking.Transport;

public class TestPlayerManager : MonoBehaviour
{
    public PlayerDataDB playerDB;
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
        for (int i = 0; i < newOnlinePlayers.Count; i++)
        {
            var player = newOnlinePlayers[i];
            if (player.connection == nw)
            {
                if (player.lobbyID != -1)
                {
                    TestServerBehaviour.Instance.lobbies[player.lobbyID].PlayerLeave(player.player.userName);
                    //TestServerBehaviour.Instance.lobbies[player.lobbyID].playerNames.Remove(player.player.userName);
                    //TODO laat anderen in lobby weten dat hij uitlogt
                }
                newOnlinePlayers.Remove(newOnlinePlayers[i]);
                return;
            }
        }
        Debug.Log("LogOutPlayer: Speler bestaat niet");
        /*
        int i = playerConnections.IndexOf(nw);
        if (i != -1)
        {
            Debug.Log("logoutplayer, i = " + i);
            playerConnections.RemoveAt(i);
            onlinePlayers.RemoveAt(i);
        } */     
    }

    public void LoginPlayer(NetworkConnection nw, TestPlayerData reqPlayer)
    {
        OnlinePlayer player = new OnlinePlayer(reqPlayer, nw);
        newOnlinePlayers.Add(player);
    }

    private bool PlayerIsOnline(TestPlayerData reqPlayer)
    {
        foreach (OnlinePlayer player in newOnlinePlayers)
        {
            if (player.player == reqPlayer)
            {
                return true;
            }
        }
        return false;
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
            /*
            foreach (OnlinePlayer player in newOnlinePlayers)
            {
                if (player.player == reqPlayer)
                {
                    return LoginResult.alreadyLogginIn;
                }
            }*/
            if (PlayerIsOnline(reqPlayer))
            {
                return LoginResult.alreadyLogginIn;
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
    }

    /// <summary>
    /// Haalt alle namen en bijbehorende gegevens uit de table 'RegistredUsers' van de MySQL database
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadFromSQLDatabase()
    {
        yield return StartCoroutine(DBManager.OpenURL("db_dump", ""));
        if (DBManager.response != "")
        {
            string txt = DBManager.response;
            txt.Trim();
            // Waarom kan ik niet txt.Split(", ") doen?
            string[] seperator = { "<br>" };
            string[] strlist = txt.Split(seperator, 9000, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strlist.Length; i += 4)
            {
                string playerID = strlist[i];
                string playerName = strlist[i + 1];
                string playerPassword = strlist[i + 2];
                string creationDate = strlist[i + 3];

                playerDB.players.Add(new TestPlayerData(playerName, playerPassword));
            }
        }
    }

    private IEnumerator AddNewUserToSQLDatabase(string name, string password)
    {
        yield return StartCoroutine(DBManager.OpenURL("db_playerregister", "playername=" + name + "&playerpassword=" + password));
        /*
        if (DBManager.response != "")
        {
            Debug.Log("Gebruiker toegevoegd aan mysql database");
        }
        else
        {
            Debug.Log("Kon gebruiker niet toevoegen aan mysql database");
        }*/
    }

    private void LoadFromJsonDatabase()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string fileContent = File.ReadAllText(Application.dataPath + "/save.txt");
            playerDB = JsonUtility.FromJson<PlayerDataDB>(fileContent);
        }
        else
        {
            playerDB = new PlayerDataDB();
            AddNewUserToDB("Jan","Klaas");
            AddNewUserToDB("Hopjesvla","Mona");
            AddNewUserToDB("Philip","Habing");
            AddNewUserToDB("Michael","Jackson");

            string fileContent = JsonUtility.ToJson(playerDB);
            File.WriteAllText(Application.dataPath + "/save.txt", fileContent);
        }
    }

    public void AddNewUserToDB(string name, string password)
    {
        playerDB.players.Add(new TestPlayerData(name, password));
        StartCoroutine(AddNewUserToSQLDatabase(name, password));
        // TODO voeg hem ook in sql database toe
    }

    private void Start()
    {
        playerDB = new PlayerDataDB();
        //StartCoroutine(LoadFromSQLDatabase());
        LoadFromJsonDatabase();
    }
}
