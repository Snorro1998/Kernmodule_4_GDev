using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class ServerResponder
{
    /*
    public ServerResponder(TestServerBehaviour _server)
    {
        server = _server;
    }

    public TestServerBehaviour server;*/

    /// <summary>
    /// Verwerkt inkomende inloggegevens en geeft hierop een antwoord
    /// </summary>
    /// <param name="nw"></param>
    /// <param name="reader"></param>
    public void HandleLogin(NetworkConnection nw, DataStreamReader reader)
    {       
        MessageLogin message = MessageManager.ReadMessage<MessageLogin>(reader) as MessageLogin;
        ChatManager.Instance.SendMessageToChat("Client probeert in te loggen met de gegevens '" + message.userName + "', '" + message.password + "'");
        TestPlayerManager.LoginResult result = TestServerBehaviour.Instance.playerManager.AttemptLogin(message.userName.ToString(), message.password.ToString(), ref nw);
        MessageLoginResponse loginResponse = new MessageLoginResponse(result);
        MessageManager.SendMessage(TestServerBehaviour.Instance.networkDriver, loginResponse, nw);
    }

    /// <summary>
    /// Stuurt een actuele lobbylijst
    /// </summary>
    /// <param name="nw"></param>
    /// <param name="stream"></param>
    public void SendLobbyList(NetworkConnection nw, DataStreamReader reader)
    {
        MessageLobbyList message = new MessageLobbyList(TestServerBehaviour.Instance.lobbies);
        MessageManager.SendMessage(TestServerBehaviour.Instance.networkDriver, message, nw);
    }

    public void HandleLobbyJoin(NetworkConnection nw, DataStreamReader reader)
    {
        /*
        MessageJoinLobby message = MessageManager.ReadMessage<MessageJoinLobby>(reader) as MessageJoinLobby;
        bool exists = TestServerBehaviour.Instance.CheckIfLobbyExists(message.lobbyName, message.lobbyID);
        int i = exists ? message.lobbyID : -1;
        //geeft player in playermanager nieuw lobbyid, ongeacht of het goed gaat of niet
        foreach (TestPlayerManager.OnlinePlayer player in TestServerBehaviour.Instance.playerManager.newOnlinePlayers)
        {
            if (player.connection == nw)
            {
                player.lobbyID = i;
            }
        }
        ChatManager.Instance.SendMessageToChat("Client wil lobby " + message.lobbyName + " met id " + message.lobbyID + " betreden. Bestaat = " + exists);
        TestServerBehaviour.Instance.lobbies[i].playerNames.Add(TestServerBehaviour.Instance.playerManager.GetPlayerName(nw));
        MessageJoinLobby response = new MessageJoinLobby(i, "", exists);
        MessageManager.SendMessage(TestServerBehaviour.Instance.networkDriver, response, nw);*/
    }
}
