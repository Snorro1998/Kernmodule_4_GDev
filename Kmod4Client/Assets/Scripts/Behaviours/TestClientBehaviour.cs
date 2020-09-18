using UnityEngine;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class TestClientBehaviour : V2Singleton<TestClientBehaviour>
{
    private static readonly ushort NETWORK_PORT = 9000;
    private static readonly float LOBBY_LIST_UPDATE_INTERVAL = 10;

    public NetworkDriver networkDriver;
    public NetworkConnection networkConnection;
    public bool connectedToServer;

    public GameObject loginPanel;
    public GameObject lobbyListPanel;
    public GameObject lobbyPanel;
    public GameObject gamePanel;
    public GameObject turnButton;

    public List<Lobby> lobbies = new List<Lobby>();

    public int lobbyID = -1;

    public enum GameState
    {
        login,
        lobbyList,
        lobby,
        game
    }

    public GameState gameState;
    private ClientResponder responder;
    public ChatManager chatManager;
    public GlobalVars globals = new GlobalVars();

    public void StartLobby()
    {
        var message = new MessageStartGame();
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    private IEnumerator LobbyListUpdate()
    {
        while(gameState == GameState.lobbyList)
        {
            var message = new MessageAskLobbies();
            MessageManager.SendMessage(networkDriver, message, networkConnection);
            yield return new WaitForSeconds(LOBBY_LIST_UPDATE_INTERVAL);
        }  
    }

    public void ChangeGameState(GameState _gameState)
    {
        gameState = _gameState;
        switch(gameState)
        {
            case GameState.login:
                loginPanel.SetActive(true);
                lobbyListPanel.SetActive(false);
                break;
            case GameState.lobbyList:
                loginPanel.SetActive(false);
                lobbyListPanel.SetActive(true);
                lobbyPanel.SetActive(false);
                StartCoroutine(LobbyListUpdate());
                //var message = new MessageAskLobbies();
                //MessageManager.SendMessage(networkDriver, message, networkConnection);
                break;
            case GameState.lobby:
                lobbyListPanel.SetActive(false);
                lobbyPanel.SetActive(true);
                break;
            case GameState.game:
                gamePanel.SetActive(true);
                lobbyPanel.SetActive(false);
                break;
        }
    }

    public void Attack()
    {
        MessageAttack message = new MessageAttack();
        MessageManager.SendMessage(networkDriver, message, networkConnection);
        turnButton.SetActive(false);
    }

    public void SendMessageToServer(NativeString64 txt)
    {
        MessageText message = new MessageText(txt);
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    private void DisconnectFromServer()
    {
        connectedToServer = false;
        networkConnection.Disconnect(networkDriver);
        networkConnection = default(NetworkConnection);
    }

    public void LoginToServer(NativeString64 userName, NativeString64 password)
    {
        var message = new MessageLogin(userName, password, 0);
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void RequestLobbyLeave()
    {
        var message = new MessageRequestLobbyLeave();
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void RegisterToServer(NativeString64 userName, NativeString64 password)
    {
        var message = new MessageLogin(userName, password, 1);
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void HandleLobbyList(NetworkConnection nw, DataStreamReader reader)
    {
        //chatManager.SendMessageToChat(selfMess["lobbyListReceive"]);
        MessageLobbyList message = MessageManager.ReadMessage<MessageLobbyList>(reader) as MessageLobbyList;
        lobbies = message.lobbies;
        LobbyPanel.Instance.UpdatePanel();
    }

    public void JoinLobby(string lobbyName, int lobbyID)
    {
        chatManager.SendMessageToChat("Client probeert lobby " + lobbyName + " te betreden met id " + lobbyID);
        MessageJoinLobby message = new MessageJoinLobby(lobbyID, lobbyName, MessageJoinLobby.LobbyStat.request);
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void LeaveLobby()
    {
        ChangeGameState(GameState.lobbyList);
        lobbyID = -1;
    }

    protected override void Awake()
    {
        base.Awake();
        responder = new ClientResponder(this);
    }  

    void Start()
    {
        ClientEvents.Instance.onLobbyStartEnter += startLobby;
        //ClientEvents.Instance.onImageSendEnter += ImageReceive;

        chatManager = ChatManager.Instance;
        chatManager.SendMessageToChat(globals.LogMessages["clientStarting"]);
        networkDriver = NetworkDriver.Create();
        networkConnection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = NETWORK_PORT;
        networkConnection = networkDriver.Connect(endpoint);
    }

    public void startLobby()
    {
        Debug.Log("Startlobby");
        chatManager.SendMessageToChat("Het spel wordt gestart");
        ChangeGameState(GameState.game);
        //throw new NotImplementedException();
    }
    /*
    private void ImageReceive(string a)
    {
        responder.ImageUpdate(networkConnection, reader);
    }*/

    public void OnDestroy()
    {
        DisconnectFromServer();
        networkDriver.Dispose();
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

        DataStreamReader reader;
        NetworkEvent.Type cmd;

        while ((cmd = networkConnection.PopEvent(networkDriver, out reader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                connectedToServer = true;
                chatManager.SendMessageToChat(globals.LogMessages["serverConnected"]);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                //chatManager.SendMessageToChat(selfMess["serverReceiveData"]);
                Message.MessageType type = (Message.MessageType)reader.ReadUInt();

                switch (type)
                {
                    default:
                        chatManager.SendMessageToChat(globals.LogMessages["messageUnknown"]);
                        break;
                    case Message.MessageType.alive:
                        MessageAlive msgAlive = new MessageAlive();
                        MessageManager.SendMessage(networkDriver, msgAlive, networkConnection);
                        break;
                    case Message.MessageType.textMessage:
                        MessageText msgText = MessageManager.ReadMessage<MessageText>(reader) as MessageText;
                        chatManager.SendMessageToChat(msgText.txt.ToString());
                        break;
                    case Message.MessageType.userLogin:
                        responder.HandleLoginResponse(networkConnection, reader);
                        break;
                    case Message.MessageType.sendLobbyList:
                        HandleLobbyList(networkConnection, reader);
                        break;
                    case Message.MessageType.joinLobby:
                        responder.HandleJoinLobbyResponse(networkConnection, reader);
                        //HandleJoinLobbyResponse(networkConnection, reader);
                        break;
                    case Message.MessageType.startLobby:
                        ClientEvents.Instance.StartEvent(ClientEvents.Instance.onLobbyStartEnter);
                        //startLobby();
                        break;
                    case Message.MessageType.gameGiveTurn:
                        Debug.Log("Krijgt beurt");
                        turnButton.SetActive(true);
                        break;
                    case Message.MessageType.imageSend:
                        //Debug.Log("Ontvangt een afbeelding van de server");
                        //ClientEvents.Instance.ImageSendEnter(networkConnection, reader);
                        responder.ImageUpdate(networkConnection, reader);
                        break;
                    case Message.MessageType.responseLobbyLeave:
                        LeaveLobby();
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                chatManager.SendMessageToChat(globals.LogMessages["serverDisconnect"]);
                networkConnection = default(NetworkConnection);
            }
        }
    }
}