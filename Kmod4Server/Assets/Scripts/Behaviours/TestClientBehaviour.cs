using UnityEngine;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class TestClientBehaviour : V2Singleton<TestClientBehaviour>
{
    private static ushort networkPort = 9000;

    public NetworkDriver networkDriver;
    public NetworkConnection networkConnection;
    public bool connectedToServer;

    Dictionary<string, string> selfMess = new Dictionary<string, string>
    {
        { "clientStarting", "De client wordt gestart" },
        { "serverConnected", "De client is nu verbonden met de server" },
        { "serverReceiveData", "De client ontvangt data van de server" },
        { "serverDisconnect", "De server heeft de verbinding verbroken"},
        { "messageUnknown", "Onbekend of corrupt berichttype" },
        { "messageSendDefault", "Snaterdijke jongen"},
        { "errorLoginUserAlready", "De opgegeven gebruikersnaam is reeds ingelogd"},
        { "errorLoginUserNonExist", "De opgegeven gebruikersnaam bestaat niet"},
        { "errorLoginWrongPassword", "Verkeerd wachtwoord"},
        { "loginSucces", "Succesvol ingelogd"}
    };

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
        var message = new MessageLogin(userName, password);
        MessageManager.SendMessage(networkDriver, message, networkConnection);
    }

    public void HandleLoginResponse(NetworkConnection nw, DataStreamReader reader)
    {
        MessageLoginResponse msgLogin = MessageManager.ReadMessage<MessageLoginResponse>(reader) as MessageLoginResponse;
        switch(msgLogin.result)
        {
            case TestPlayerManager.LoginResult.alreadyLogginIn:
                ChatManager.Instance.SendMessageToChat(selfMess["errorLoginUserAlready"]);
                break;
            case TestPlayerManager.LoginResult.nonExistingName:
                ChatManager.Instance.SendMessageToChat(selfMess["errorLoginUserNonExist"]);
                break;
            case TestPlayerManager.LoginResult.wrongPassword:
                ChatManager.Instance.SendMessageToChat(selfMess["errorLoginWrongPassword"]);
                break;
            case TestPlayerManager.LoginResult.success:
                ChatManager.Instance.SendMessageToChat(selfMess["loginSucces"]);
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }  

    void Start()
    {
        ChatManager.Instance.SendMessageToChat(selfMess["clientStarting"]);
        networkDriver = NetworkDriver.Create();
        networkConnection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = networkPort;
        networkConnection = networkDriver.Connect(endpoint);
    }

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
                ChatManager.Instance.SendMessageToChat("Er ging iets mis terwijl de client probeerde te verbinden");
            return;*/
        }

        DataStreamReader reader;
        NetworkEvent.Type cmd;

        while ((cmd = networkConnection.PopEvent(networkDriver, out reader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                connectedToServer = true;
                ChatManager.Instance.SendMessageToChat(selfMess["serverConnected"]);

                //var message = new MessageText(selfMess["messageSendDefault"]);
                //MessageManager.SendMessage(networkDriver, message, networkConnection);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                //ChatManager.Instance.SendMessageToChat(selfMess["serverReceiveData"]);
                Message.MessageType type = (Message.MessageType)reader.ReadUInt();

                switch (type)
                {
                    default:
                        ChatManager.Instance.SendMessageToChat(selfMess["messageUnknown"]);
                        break;
                    case Message.MessageType.alive:
                        MessageAlive msgAlive = new MessageAlive();
                        MessageManager.SendMessage(networkDriver, msgAlive, networkConnection);
                        break;
                    case Message.MessageType.textMessage:
                        MessageText msgText = MessageManager.ReadMessage<MessageText>(reader) as MessageText;
                        ChatManager.Instance.SendMessageToChat(msgText.txt.ToString());
                        break;
                    case Message.MessageType.login:
                        HandleLoginResponse(networkConnection, reader);
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                ChatManager.Instance.SendMessageToChat(selfMess["serverDisconnect"]);
                networkConnection = default(NetworkConnection);
            }
        }
    }
}