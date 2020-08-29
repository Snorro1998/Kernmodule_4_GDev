using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class ServerResponder
{
    public TestServerBehaviour server;

    public ServerResponder(TestServerBehaviour _server)
    {
        server = _server;
    }

    /// <summary>
    /// Verwerkt inkomende inloggegevens en geeft hierop een antwoord
    /// </summary>
    /// <param name="nw"></param>
    /// <param name="reader"></param>
    public void HandleLogin(NetworkConnection nw, DataStreamReader reader)
    {       
        MessageLogin message = MessageManager.ReadMessage<MessageLogin>(reader) as MessageLogin;
        ChatManager.Instance.SendMessageToChat("Client probeert in te loggen met de gegevens '" + message.userName + "', '" + message.password + "'");
        TestPlayerManager.LoginResult result = server.playerManager.AttemptLogin(message.userName.ToString(), message.password.ToString(), ref nw);
        MessageLoginResponse loginResponse = new MessageLoginResponse(result);
        MessageManager.SendMessage(server.networkDriver, loginResponse, nw);
    }

    /// <summary>
    /// Stuurt een actuele lobbylijst
    /// </summary>
    /// <param name="nw"></param>
    /// <param name="stream"></param>
    public void SendLobbyList(NetworkConnection nw, DataStreamReader reader)
    {
        MessageLobbyList message = new MessageLobbyList(server.lobbies);
        MessageManager.SendMessage(server.networkDriver, message, nw);
    }

    /// <summary>
    /// Kijkt of verzochte lobby bestaat en stuurt hierop een antwoord
    /// </summary>
    /// <param name="nw"></param>
    /// <param name="reader"></param>
    public void HandleLobbyJoin(NetworkConnection nw, DataStreamReader reader)
    {
        var message = MessageManager.ReadMessage<MessageJoinLobby>(reader) as MessageJoinLobby;
        //Lobby lob = new Lobby();
        var result = server.CheckIfLobbyExists(message.lobbyName, message.lobbyID);
        //Debug.Log("Ontvangt lobbyjoinverzoek van client. Naam = " + message.lobbyName + ", id = " + message.lobbyID + ", resultaat = " + result);
        int i = result == MessageJoinLobby.LobbyStat.exists ? message.lobbyID : -1;

        foreach (TestPlayerManager.OnlinePlayer player in server.playerManager.newOnlinePlayers)
        {
            if (player.connection == nw)
            {
                player.lobbyID = i;
            }
        }

        ChatManager.Instance.SendMessageToChat("Client wil lobby " + message.lobbyName + " met id " + message.lobbyID + " betreden. Status = " + result);

        if (result == MessageJoinLobby.LobbyStat.exists)
        {
            server.lobbies[i].PlayerJoin(server.playerManager.GetPlayerName(nw));
        }

        MessageJoinLobby response = new MessageJoinLobby(i, "", result);
        //MessageLobbyInfo response2 = new MessageLobbyInfo();
        MessageManager.SendMessage(server.networkDriver, response, nw);
    }
}
