using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;


public enum RegisterResult
{
    SUCCES,
    ERROR_ALREADY_EXISTS,
    ERROR_DEFAULT,
}

public class ServerBehaviour : Singleton<ServerBehaviour>
{
    public NetworkDriver networkDriver;
    public NativeList<NetworkConnection> networkConnections;

    public ServerSettings settings;
    public bool printDebugMessages = true;
    public bool started = false;

    public int serverID = 1;
    public string serverPassword = "SuperGeheim";

    delegate void GameEventHandler(DataStreamReader stream, object sender, NetworkConnection connection);

    static Dictionary<GameEvent, GameEventHandler> gameEventDictionary = new Dictionary<GameEvent, GameEventHandler>() {
        // link game events to functions...
        { GameEvent.PING, OnPing },
        { GameEvent.LOGIN_REQUEST, LoginLogoutFunctions.OnLoginRequest},
        { GameEvent.REGISTER_REQUEST, LoginLogoutFunctions.OnRegisterRequest},
        { GameEvent.LOGOUT_REQUEST, LoginLogoutFunctions.OnLogoutRequest},
        { GameEvent.GAME_MAZE_REVEAL_TILE, MazeFunctions.OnMazeTileClicked},
        { GameEvent.GAME_USE_ITEM, OnItemUse}
    };

    public void OnRegisterRequestFunction(string username, string password, NetworkConnection connection)
    {
        StartCoroutine(RegisterRequester(username, password, connection));
    }

    public void OnloginRequestFunction(string username, string password, NetworkConnection connection)
    {
        StartCoroutine(LoginRequester(username, password, connection));
    }

    IEnumerator RegisterRequester(string username, string password, NetworkConnection connection)
    {
        yield return StartCoroutine(DBManager.OpenURL("player_register", "username=" + username + "", "password=" + password));
        var result = RegisterResult.ERROR_DEFAULT;
        yield return 0;
        if (DBManager.response != null)
        {
            var str = DBManager.response;
            if (str.Contains("ERROR_USERNAME_ALREADY_EXISTS")) result = RegisterResult.ERROR_ALREADY_EXISTS;
            else if (str.Contains("SUCCES")) result = RegisterResult.SUCCES;
        }

        if (result == RegisterResult.SUCCES)
        {
            PerformLogin(username, connection, MessageLoginResponse.LoginResult.SUCCES);
        }
    }

    private void PerformLogin(string username, NetworkConnection connection, MessageLoginResponse.LoginResult result)
    {
        if (result == MessageLoginResponse.LoginResult.SUCCES)
        {
            PlayerManager.Instance.LoginPlayer(username, connection);
        }

        var response = new MessageLoginResponse(result);
        MessageManager.SendMessage(networkDriver, response, connection);
    }

    IEnumerator LoginRequester(string username, string password, NetworkConnection connection)
    {
        yield return StartCoroutine(DBManager.OpenURL("player_login", "username=" + username + "", "password=" + password));
        var result = MessageLoginResponse.LoginResult.UNKNOWN_USERNAME;
        if (DBManager.response != null)
        {
            var str = DBManager.response;
            Debug.Log(str);
            if (str.Contains("SUCCES")) result = MessageLoginResponse.LoginResult.SUCCES;
            else if (str.Contains("ERROR_USERNAME_WRONG_PASSWORD")) result = MessageLoginResponse.LoginResult.INVALID_PASSWORD;
            else if (str.Contains("ERROR_USERNAME_UNKNOWN")) result = MessageLoginResponse.LoginResult.UNKNOWN_USERNAME;
        }

        DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.CLIENT_LOGIN_REQUEST, username, password);
        //TODO gegevenscheck
        if (result == MessageLoginResponse.LoginResult.SUCCES) result = PlayerManager.Instance.PlayerIsLoggedIn(username, connection) == true ? MessageLoginResponse.LoginResult.ALREADY_LOGGED_IN : MessageLoginResponse.LoginResult.SUCCES;
        //if (!nameInDb) result = MessageLoginResponse.LoginResult.UNKNOWN_USERNAME;
        //TODO niet weigeren als het spel al is gestart
        if (GameManager.Instance.gameStarted) result = MessageLoginResponse.LoginResult.GAME_STARTED;

        PerformLogin(username, connection, result);
        /*
        if (result == MessageLoginResponse.LoginResult.SUCCES)
        {
            PlayerManager.Instance.LoginPlayer(username, connection);
        }

        var response = new MessageLoginResponse(result);
        MessageManager.SendMessage(networkDriver, response, connection);*/
        yield return 0;
    }


    /// <summary>
    /// Stuurt een bericht naar iedere client die verbonden is met de server.
    /// </summary>
    /// <param name="message"></param>
    public void SendMessageToAll(Message message)
    {
        foreach (var i in networkConnections)
        {
            MessageManager.SendMessage(networkDriver, message, i);
        }
    }

    /// <summary>
    /// Stuurt een bericht naar alle clients die ingelogd zijn.
    /// </summary>
    /// <param name="message"></param>
    public void SendMessageToAllOnline(Message message)
    {
        foreach (var p in PlayerManager.Instance.players)
        {
            MessageManager.SendMessage(networkDriver, message, p.playerConnection);
        }
    }

    static void OnItemUse(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var message = MessageManager.ReadMessage<MessageUseItem>(stream) as MessageUseItem;
        Debug.Log(message.userName + " gebruikt " + message.amount + "X " + message.itemName + " op " + message.targetName);
        ItemManager.Instance.UseItem(message.itemName, message.amount, message.targetName, message.userName);
    }


    /// <summary>
    /// Wordt aangeroepen wanneer de server een PING-bericht van een client ontvangt.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="sender"></param>
    /// <param name="connection"></param>
    static void OnPing(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        //ServerBehaviour server = sender as ServerBehaviour;
        //DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.PING);
    }

    /// <summary>
    /// Laadt het instellingenbestand van de server in en maakt hem aan als hij niet bestaat.
    /// </summary>
    private void InitSettings()
    {       
        string path = Application.dataPath + "/settings.json";
        //Debug.Log(path);
        if (File.Exists(path))
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_SETTINGS_FILE_EXISTS);
            var str = System.IO.File.ReadAllText(Application.dataPath + "/settings.json");
            settings = JsonUtility.FromJson<ServerSettings>(str);
        }
        else
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_SETTINGS_FILE_DOESNT_EXIST);
            UpdateServerSettings(new ServerSettings());
            //settings = new ServerSettings();
            //System.IO.File.WriteAllText(Application.dataPath + "/settings.json", JsonUtility.ToJson(settings));
        }
    }

    private void UpdateServerSettings(ServerSettings InSettings)
    {
        settings = InSettings;
        System.IO.File.WriteAllText(Application.dataPath + "/settings.json", JsonUtility.ToJson(InSettings));
    }

    /// <summary>
    /// Verwijdert oude verbindingen
    /// </summary>
    private void CleanUpConnections()
    {
        for (int i = 0; i < networkConnections.Length; i++)
        {
            if (!networkConnections[i].IsCreated)
            {
                networkConnections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    /// <summary>
    /// Accepteert nieuwe verbindingen.
    /// </summary>
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = networkDriver.Accept()) != default(NetworkConnection))
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.CLIENT_CONNECT);
            networkConnections.Add(c);
        }
    }

    /// <summary>
    /// Werkt verbindingen bij.
    /// </summary>
    private void UpdateConnections()
    {
        CleanUpConnections();
        AcceptNewConnections();
    }

    /// <summary>
    /// Routine die voorkomt dat verbindingen automatisch verbroken worden.
    /// </summary>
    /// <returns></returns>
    public IEnumerator PingClients()
    {
        while (true)
        {
            yield return new WaitForSeconds(settings.PING_INTERVAL);
            var message = new MessagePing();
            SendMessageToAll(message);
        }      
    }

    public IEnumerator ServerStartCoroutine()
    {
        InitSettings();
        Debug.Log("Server session id=" + settings.SESSION_ID);
        yield return StartCoroutine(DBManager.OpenURL("server_login", "server_id=" + serverID.ToString() + "", "server_password=" + serverPassword));
        
        if (DBManager.response.Contains("ERROR"))
        {
            Debug.LogError("The server could not connect to the database!");
            yield return 0;
        }
        else
        {
            Debug.Log("De server kon verbinden met de database!");
            var responseSessionID = DBManager.response.Trim();
            if (settings.SESSION_ID != responseSessionID)
            {
                Debug.Log("Started a new session");
                settings.SESSION_ID = responseSessionID;
                UpdateServerSettings(settings);
            }
            else
            {
                Debug.Log("Loaded existing session");
            }
            //Debug.Log(DBManager.response);
            //settings.SESSION_ID = DBManager.response.Trim();
            StartServer();
        }
        yield return 0;
    }

    private void StartServer()
    {
        DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_START, settings.NETWORK_PORT.ToString());
        networkDriver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = settings.NETWORK_PORT;
        if (networkDriver.Bind(endpoint) != 0)
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_START_ERROR_CANT_BIND_TO_PORT, settings.NETWORK_PORT.ToString());
        }
        else
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_START_SUCCES);
            networkDriver.Listen();
        }
        networkConnections = new NativeList<NetworkConnection>(settings.MAX_CONNECTIONS, Allocator.Persistent);
        StartCoroutine(PingClients());
        started = true;
    }

    protected override void Awake()
    {
        base.Awake();
        InitSettings();
        //gameManager = gameObject.AddComponent<GameManager>();
    }

    void Start()
    {
        StartCoroutine(ServerStartCoroutine());
        //StartServer();
    }

    private void OnDestroy()
    {
        if (networkConnections.IsCreated)
        {
            foreach (var connection in networkConnections)
            {
                var message = new MessageServerQuit();
                MessageManager.SendMessage(networkDriver, message, connection);
            }
        }
        
        if (networkDriver.IsCreated) networkDriver.Dispose();
        if (networkConnections.IsCreated) networkConnections.Dispose();
    }


    private void Update()
    {
        if (!started) return;
        networkDriver.ScheduleUpdate().Complete();
        UpdateConnections();

        DataStreamReader stream;
        for (int i = 0; i < networkConnections.Length; i++)
        {
            Assert.IsTrue(networkConnections[i].IsCreated);

            NetworkEvent.Type cmd;
            while ((cmd = networkDriver.PopEventForConnection(networkConnections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                NetworkConnection Sender = networkConnections[i];
                if (cmd == NetworkEvent.Type.Data)
                {
                    GameEvent gameEventType = (GameEvent)stream.ReadUInt();
                    //DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.RECEIVE_MESSAGE_FROM_CLIENT, gameEventType.ToString());

                    if (gameEventDictionary.ContainsKey(gameEventType))
                    {
                        gameEventDictionary[gameEventType].Invoke(stream, this, Sender);
                    }
                    else
                    {
                        DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.UNKNOWN_MESSAGE_TYPE);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    PlayerManager.Instance.LogoutPlayer(networkConnections[i]);
                    DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.CLIENT_DISCONNECT);
                    networkConnections[i] = default(NetworkConnection);
                }
            }
        }

        //TestFunc();
    }
}