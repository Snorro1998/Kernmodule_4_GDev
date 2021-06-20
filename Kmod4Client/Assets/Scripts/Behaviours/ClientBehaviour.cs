using UnityEngine;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class ClientSettings
{
    public ushort NETWORK_PORT = 9000;
}

public class ClientBehaviour : Singleton<ClientBehaviour>
{
    public NetworkDriver networkDriver;
    public NetworkConnection networkConnection;
    public ClientSettings settings;
    public bool connectedToServer;

    public string username;
    public string password;

    public InputField usernameField;
    public InputField passwordField;

    public void SendMessageToServer(Message message)
    {
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void DoLogin()
    {
        username = usernameField.text;
        password = passwordField.text;
        var message = new MessageLoginRequest(username, password);
        Debug.Log("stuurt loginverzoek");
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void DoRegister()
    {
        username = usernameField.text;
        password = passwordField.text;
        var message = new MessageRegisterRequest(username, password);
        Debug.Log("Stuurt registratieverzoek");
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void DoLogout()
    {
        var message = new MessageLogoutRequest();
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    delegate void GameEventHandler(DataStreamReader stream, object sender, NetworkConnection connection);

    static Dictionary<GameEvent, GameEventHandler> gameEventDictionary = new Dictionary<GameEvent, GameEventHandler>() {
        { GameEvent.PING, ClientFunctions.OnPing },
        { GameEvent.SERVER_QUIT, ClientFunctions.OnServerQuit},
        { GameEvent.LOGIN_RESPONSE, OnLoginResponse},
        { GameEvent.LOGOUT_RESPONSE, OnLogoutResponse},
        { GameEvent.GAME_MAZE_CREATE_NEW, MazeFunctions.OnReceiveMaze},
        { GameEvent.GAME_MAZE_REVEAL_TILE, MazeFunctions.OnRevealMazeTile},
        { GameEvent.GAME_BATTLE_START, BattleFunctions.OnBattleStart},
        { GameEvent.GAME_BATTLE_VICTORY,  BattleFunctions.OnBattleWin},
        { GameEvent.GAME_BATTLE_GIVE_TURN, BattleFunctions.OnBattleGiveTurn},
        { GameEvent.GAME_GET_ITEM, OnItemGet},
        { GameEvent.GAME_USE_ITEM, OnItemUse },
        { GameEvent.PLAYERMANAGER_UPDATE_PLAYERS, OnPlayerUpdate},
    };

    delegate void LoginFunc(object sender);

    static Dictionary<MessageLoginResponse.LoginResult, LoginFunc> loginFuncs = new Dictionary<MessageLoginResponse.LoginResult, LoginFunc>() {
        { MessageLoginResponse.LoginResult.ALREADY_LOGGED_IN, LoginFunctions.OnLoginResultAlreadyLoggedIn },
        { MessageLoginResponse.LoginResult.INVALID_PASSWORD, LoginFunctions.OnLoginResultInvalidPassword },
        { MessageLoginResponse.LoginResult.SUCCES, LoginFunctions.OnLoginResultSucces },
        { MessageLoginResponse.LoginResult.UNKNOWN_USERNAME, LoginFunctions.OnLoginResultUnknownUsername },
    };

    

    static void OnItemGet(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var message = MessageManager.ReadMessage<MessageGiveItem>(stream) as MessageGiveItem;
        Debug.Log("Je hebt " + message.amount + "X " + message.itemName + " gekregen!");
        ItemManager.Instance.GiveItem(message.itemName, message.amount);
    }

    static void OnItemUse(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var message = MessageManager.ReadMessage<MessageUseItem>(stream) as MessageUseItem;
        ItemManager.Instance.UseItem(message.itemName, message.amount, message.targetName, message.userName);
    }

    static void OnPlayerUpdate(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var message = MessageManager.ReadMessage<MessagePlayersOnlineUpdate>(stream) as MessagePlayersOnlineUpdate;
        PlayerManager.Instance.players = message.players;
        GameManager.Instance.UpdatePlayers();
        //PlayerManager.Instance.UpdatePlayerTestPanel();
    }

    static void OnLoginResponse(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        var message = MessageManager.ReadMessage<MessageLoginResponse>(stream) as MessageLoginResponse;
        var result = message.result;

        if (loginFuncs.ContainsKey(result))
        {
            loginFuncs[result].Invoke(sender);
        }
    }

    static void OnLogoutResponse(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        Debug.Log("Logout succesvol");
        ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.LOGIN_SCREEN, 0, 0);
    }

    private void InitSettings()
    {
        string path = Application.dataPath + "/settings.json";
        Debug.Log(path);
        if (File.Exists(path))
        {
            Debug.Log("Instellingenbestand bestaat. Instellingen worden nu ingeladen");
            var str = System.IO.File.ReadAllText(Application.dataPath + "/settings.json");
            settings = JsonUtility.FromJson<ClientSettings>(str);
        }
        else
        {
            Debug.Log("Instellingenbestand bestaat niet. Wordt nu aangemaakt met standaardinstellingen");
            settings = new ClientSettings();
            System.IO.File.WriteAllText(Application.dataPath + "/settings.json", JsonUtility.ToJson(settings));
        }
    }

    public void DisconnectFromServer()
    {
        connectedToServer = false;
        if (networkConnection.IsCreated)
        {
            networkConnection.Disconnect(networkDriver);
            networkConnection = default(NetworkConnection);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //InitScreens();
        //ChangeScreen(ActiveScreen.LOGIN_SCREEN);
    }

    private void ConnectToServer()
    {
        networkDriver = NetworkDriver.Create();
        networkConnection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = settings.NETWORK_PORT;
        networkConnection = networkDriver.Connect(endpoint);
    }

    void Start()
    {
        ConnectToServer();
    }

    public void OnDestroy()
    {
        DisconnectFromServer();
        if (networkDriver.IsCreated) networkDriver.Dispose();
    }

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        if (!networkConnection.IsCreated)
        {
            if (connectedToServer)
            {
                DisconnectFromServer();
            }
            return;
            /*
            if (!m_Done)
                chatManager.SendMessageToChat("Er ging iets mis terwijl de client probeerde te verbinden");
            return;*/
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = networkConnection.PopEvent(networkDriver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("verbonden met de server");
                connectedToServer = true;
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                GameEvent gameEventType = (GameEvent)stream.ReadUInt();
                //PrintDebugMessage(DebugMessageTypes.RECEIVE_MESSAGE_FROM_CLIENT, gameEventType.ToString());
                //Debug.Log("Ontvangt bericht van het type " + gameEventType + " van de server");

                if (gameEventDictionary.ContainsKey(gameEventType))
                {
                    gameEventDictionary[gameEventType].Invoke(stream, this, networkConnection);
                }
                else
                {
                    //PrintDebugMessage(DebugMessageTypes.UNKNOWN_MESSAGE_TYPE);
                    //Debug.Log("Onbekend berichttype");
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                DisconnectFromServer();
                //chatManager.SendMessageToChat(globals.LogMessages["serverDisconnect"]);
                //Debug.Log("disconnect");
                //networkConnection = default(NetworkConnection);
            }
        }
    }
}