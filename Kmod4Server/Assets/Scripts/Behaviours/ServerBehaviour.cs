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


public class ServerBehaviour : Singleton<ServerBehaviour>
{
    public NetworkDriver networkDriver;
    public NativeList<NetworkConnection> networkConnections;

    public ServerSettings settings;
    public bool printDebugMessages = true;

    delegate void GameEventHandler(DataStreamReader stream, object sender, NetworkConnection connection);

    static Dictionary<GameEvent, GameEventHandler> gameEventDictionary = new Dictionary<GameEvent, GameEventHandler>() {
        // link game events to functions...
        { GameEvent.PING, OnPing },
        { GameEvent.LOGIN_REQUEST, LoginLogoutFunctions.OnLoginRequest},
        { GameEvent.LOGOUT_REQUEST, LoginLogoutFunctions.OnLogoutRequest},
        { GameEvent.GAME_MAZE_REVEAL_TILE, MazeFunctions.OnMazeTileClicked},
        { GameEvent.GAME_USE_ITEM, OnItemUse}
    };
    

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
            settings = new ServerSettings();
            System.IO.File.WriteAllText(Application.dataPath + "/settings.json", JsonUtility.ToJson(settings));
        }
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
    }

    protected override void Awake()
    {
        base.Awake();
        InitSettings();
        //gameManager = gameObject.AddComponent<GameManager>();
    }

    void Start()
    {
        StartServer();
    }

    private void OnDestroy()
    {
        foreach (var connection in networkConnections)
        {
            var message = new MessageServerQuit();
            MessageManager.SendMessage(networkDriver, message, connection);
        }
        if (networkDriver.IsCreated) networkDriver.Dispose();
        if (networkConnections.IsCreated) networkConnections.Dispose();
    }


    private void Update()
    {
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