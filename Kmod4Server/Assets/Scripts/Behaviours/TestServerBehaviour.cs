using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class TestServerBehaviour : V2Singleton<TestServerBehaviour>
{
    private static ushort networkPort = 9000;
    // Moet in elk geval kleiner zijn dan 30 om te voorkomen dat de verbinding automatisch verbroken wordt
    private static float pingInterval = 5;
    private bool pingCoroutineRunning = false;

    public NetworkDriver networkDriver;
    private NativeList<NetworkConnection> networkConnections;

    public TestPlayerManager playerManager;
    private ServerResponder responder;// = new ServerResponder();

    public List<Lobby> lobbies = new List<Lobby>();

    private Lobby GetPlayerLobby(NetworkConnection nw)
    {
        string nname = playerManager.GetPlayerName(nw);
        Lobby lob = null;
        foreach (Lobby llob in lobbies)
        {
            foreach (Lobby.SinglePlayer player in llob.allPlayers)
            {
                if (player.name == nname)
                {
                    lob = llob;
                    break;
                }
            }
        }
        return lob;
    }

    public MessageJoinLobby.LobbyStat CheckIfLobbyExists(string _lobbyName, int _lobbyID/*, out Lobby lobby*/)
    {
        foreach (Lobby lob in lobbies)
        {
            if (lob.lobbyName == _lobbyName && lob.gameID == _lobbyID)
            {
                if (lob.state == Lobby.State.waiting)
                {
                    //lobby = lob;
                    return MessageJoinLobby.LobbyStat.exists;
                }
                else
                {
                    //lobby = lob;
                    return MessageJoinLobby.LobbyStat.ingame;
                }
            }
        }
        //lobby = null;
        return MessageJoinLobby.LobbyStat.nonexist;
    }

    public void StartLobby(NetworkConnection nw)
    {
        Lobby lob = GetPlayerLobby(nw);
        lob.StartNewGame();
    }

    public void PlayerAttacks(NetworkConnection nw)
    {
        Lobby lob = GetPlayerLobby(nw);
        lob.PlayerAttack(nw);
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
    /// Accepteert nieuwe verbindingen
    /// </summary>
    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = networkDriver.Accept()) != default(NetworkConnection))
        {
            //voegt verbinding toe
            networkConnections.Add(c);
            ChatManager.Instance.SendMessageToChat("Nieuwe client is met de server verbonden");
            if (!pingCoroutineRunning)
            {
                StartCoroutine(PingClients());
            }
        }
    }

    /// <summary>
    /// Routine die voorkomt dat verbindingen automatisch verbroken worden
    /// </summary>
    /// <returns></returns>
    private IEnumerator PingClients()
    {
        pingCoroutineRunning = true;
        while (networkConnections.Length > 0)
        {
            yield return new WaitForSeconds(pingInterval);
            MessageAlive msgAlive = new MessageAlive();
            for (int i = 0; i < networkConnections.Length; i++)
            {
                MessageManager.SendMessage(networkDriver, msgAlive, networkConnections[i]);
            }
        }
        pingCoroutineRunning = false;
    }

    private void StartServer()
    {
        ChatManager.Instance.SendMessageToChat("De server wordt gestart");
        networkDriver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = networkPort;
        if (networkDriver.Bind(endpoint) != 0)
        {
            ChatManager.Instance.SendMessageToChat("De server kon zichzelf niet binden aan de poort " + networkPort);
        }
        else
        {
            ChatManager.Instance.SendMessageToChat("De server is probleemloos opgestart");
            networkDriver.Listen();
        }
        networkConnections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    public void SendMessageToClients(MessageText message)
    {
        for (int i = 0; i < networkConnections.Length; i++)
        {
            MessageManager.SendMessage(networkDriver, message, networkConnections[i]);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        responder = new ServerResponder(this);
        playerManager = gameObject.AddComponent<TestPlayerManager>();

        Lobby lob = new Lobby("Testlobby", 0, Lobby.State.waiting, new List<Lobby.SinglePlayer>());
        Lobby lob2 = new Lobby("Testlobby2", 1, Lobby.State.inGame, new List<Lobby.SinglePlayer>());
        Lobby lob3 = new Lobby("Testlobby3", 2, Lobby.State.defeated, new List<Lobby.SinglePlayer>());
        lobbies.Add(lob);
        lobbies.Add(lob2);
        lobbies.Add(lob3);
    }

    void Start()
    {
        StartServer();     
    }

    public void OnDestroy()
    {
        networkDriver.Dispose();
        networkConnections.Dispose();
    }

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        CleanUpConnections();
        AcceptNewConnections();

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
                    //ChatManager.Instance.SendMessageToChat("De server ontvangt data van client " + networkConnections[i].InternalId);
                    Message.MessageType type = (Message.MessageType)stream.ReadUInt();

                    switch(type)
                    {
                        default:
                            //ChatManager.Instance.SendMessageToChat("Niet gedefinieerd wat te doen bij dit berichttype of het is onbekend of corrupt");
                            break;
                        case Message.MessageType.textMessage:
                            MessageText message = MessageManager.ReadMessage<MessageText>(stream) as MessageText;
                            //stuurt het naar zijn eigen chat en naar alle clients
                            ChatManager.Instance.SendMessageToChat(message.txt.ToString());
                            /*if (message.txt.ToString() == "send")
                            {
                                Lobby lob = GetPlayerLobby(Sender);
                                lob.SendToAllMembers("TestLobbyMessage");
                                return;
                            }*/
                            /*for (int j = 0; j < networkConnections.Length; j++)
                            {
                                MessageManager.SendMessage(networkDriver, message, networkConnections[j]);
                            }*/
                            SendMessageToClients(message);
                            break;
                        case Message.MessageType.login:
                            responder.HandleLogin(Sender, stream);
                            break;
                        case Message.MessageType.askLobbyList:
                            //ChatManager.Instance.SendMessageToChat("client vraagt lobbylijst op");
                            responder.SendLobbyList(Sender, stream);
                            break;
                        case Message.MessageType.joinLobby:
                            responder.HandleLobbyJoin(Sender, stream);
                            break;
                        case Message.MessageType.startLobby:
                            ChatManager.Instance.SendMessageToChat("Spel wordt gestart");
                            StartLobby(Sender);
                            break;
                        case Message.MessageType.gameAttack:
                            PlayerAttacks(Sender);
                            break;
                    }
                    /*
                    uint number = stream.ReadUInt();

                    ChatManager.Instance.SendMessageToChat("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;

                    var writer = networkDriver.BeginSend(NetworkPipeline.Null, networkConnections[i]);
                    writer.WriteUInt(number);
                    networkDriver.EndSend(writer);
                    */
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    ChatManager.Instance.SendMessageToChat("Een client heeft de verbinding verbroken");
                    playerManager.LogOutPlayer(networkConnections[i]);
                    networkConnections[i] = default(NetworkConnection);
                }
            }
        }
    }
}