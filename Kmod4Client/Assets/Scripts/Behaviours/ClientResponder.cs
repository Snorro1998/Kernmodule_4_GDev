using UnityEngine;
using Unity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class ClientResponder
{
    public TestClientBehaviour client;
    //ergens anders neerzetten
    [HideInInspector]
    public List<byte> imgData = new List<byte>();
    [HideInInspector]
    public byte[] currentImg;
    
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

    public void ImageUpdate(NetworkConnection nw, DataStreamReader reader)
    {
        MessageImageSend message = MessageManager.ReadMessage<MessageImageSend>(reader) as MessageImageSend;
        Debug.Log("Ontvangt data van afbeelding van server, datType = " + message.dataType);

        // is de start van nieuwe afbeelding
        if (message.dataType == MessageImageSend.DataType.singlething || message.dataType == MessageImageSend.DataType.start)
        {
            imgData.Clear();// = new List<byte>();
        }
        foreach (byte b in message.imageData)
        {
            imgData.Add(b);
        }
        if (message.dataType == MessageImageSend.DataType.singlething || message.dataType == MessageImageSend.DataType.end)
        {
            currentImg = new byte[imgData.Count];
            for (int k = 0; k < currentImg.Length; k++)
            {
                currentImg[k] = imgData[k];
            }
            Debug.Log("currentImglengte = " + currentImg.Length);
            URLLoader.Instance.UpdateTexture(currentImg);
        }
    }
}
