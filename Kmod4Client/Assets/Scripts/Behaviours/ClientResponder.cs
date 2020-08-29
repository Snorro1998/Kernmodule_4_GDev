using UnityEngine;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class ClientResponder : MonoBehaviour
{
    public TestClientBehaviour client;

    public ClientResponder(TestClientBehaviour _client)
    {
        client = _client;
    }

    public void HandleJoinLobbyResponse(NetworkConnection nw, DataStreamReader reader)
    {
        MessageJoinLobby message = MessageManager.ReadMessage<MessageJoinLobby>(reader) as MessageJoinLobby;
        string txt = "";

        switch(message.status)
        {
            default:
                txt = client.globals.LogMessages["lobbyCantEnterNonExist"];
                break;
            case MessageJoinLobby.LobbyStat.exists:
                client.ChangeGameState(TestClientBehaviour.GameState.lobby);
                txt = client.globals.LogMessages["lobbyEnterSuccess"];
                break;
            case MessageJoinLobby.LobbyStat.ingame:
                txt = client.globals.LogMessages["lobbyCantEnterBegun"];
                break;
        }
        client.chatManager.SendMessageToChat(txt);
        client.lobbyID = message.lobbyID;
    }

    public void HandleLoginResponse(NetworkConnection nw, DataStreamReader reader)
    {
        MessageLoginResponse msgLogin = MessageManager.ReadMessage<MessageLoginResponse>(reader) as MessageLoginResponse;
        switch (msgLogin.result)
        {
            case TestPlayerManager.LoginResult.alreadyLogginIn:
                client.chatManager.SendMessageToChat(client.globals.LogMessages["errorLoginUserAlready"]);
                break;
            case TestPlayerManager.LoginResult.nonExistingName:
                client.chatManager.SendMessageToChat(client.globals.LogMessages["errorLoginUserNonExist"]);
                break;
            case TestPlayerManager.LoginResult.wrongPassword:
                client.chatManager.SendMessageToChat(client.globals.LogMessages["errorLoginWrongPassword"]);
                break;
            case TestPlayerManager.LoginResult.success:
                client.chatManager.SendMessageToChat(client.globals.LogMessages["loginSucces"]);
                client.ChangeGameState(TestClientBehaviour.GameState.lobbyList);
                break;
        }
    }
}
