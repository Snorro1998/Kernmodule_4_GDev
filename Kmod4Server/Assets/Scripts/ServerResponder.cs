using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class ServerResponder
{
    public static readonly int MAX_BYTES_PER_MESSAGE_FOR_IMAGES = 1300;
    public TestServerBehaviour server;

    public ServerResponder(TestServerBehaviour _server)
    {
        server = _server;
    }

    public void SendImageToAll(byte[] imgData)
    {
        //byte[] testByte = new byte[] { 0, 1, 2, 3, 4, 5 };
        //int maxBytesPerMessage = 10;
        int nMessages = Mathf.CeilToInt((float)imgData.Length / (float)MAX_BYTES_PER_MESSAGE_FOR_IMAGES);
        //Debug.Log("testbytelength = " + testByte.Length + ", maxpermess = " + MAX_BYTES_PER_MESSAGE_FOR_IMAGES + ", nmess = " + nMessages);

        for (int i = 0; i < nMessages; i++)
        {
            List<byte> dataToSend = new List<byte>();
            int dataSize = MAX_BYTES_PER_MESSAGE_FOR_IMAGES;
            // int baseIndex = i * MAX_BYTES_PER_MESSAGE_FOR_IMAGES;
            

            for (int j = 0; j < MAX_BYTES_PER_MESSAGE_FOR_IMAGES; j++)
            {
                if (i * MAX_BYTES_PER_MESSAGE_FOR_IMAGES + j >= imgData.Length)
                {
                    //Debug.Log("eindbericht heeft een lengte van " + j);
                    dataSize = j;
                    break;
                }
                //Debug.Log("elem = " + testByte[i * MAX_BYTES_PER_MESSAGE_FOR_IMAGES + j]);
                dataToSend.Add(imgData[i * MAX_BYTES_PER_MESSAGE_FOR_IMAGES + j]);
            }
            // tijdelijke lijst omzetten zodat je eindelijk een geknipt deel van de data hebt.
            byte[] sendData = new byte[dataSize];
            for (int k = 0; k < dataSize; k++)
            {
                sendData[k] = dataToSend[k];
            }

            MessageImageSend.DataType dat = MessageImageSend.DataType.singlething;
            
            if (i == 0)
            {
                if (i == nMessages - 1)
                {
                    dat = MessageImageSend.DataType.singlething;
                }
                else
                {
                    dat = MessageImageSend.DataType.start;
                }
            }
            else if (i == nMessages - 1)
            {
                dat = MessageImageSend.DataType.end;
            }
            else
            {
                dat = MessageImageSend.DataType.middle;
            }

            MessageImageSend message = new MessageImageSend(sendData, (uint)(sendData.Length), dat);
            foreach (var player in server.playerManager.newOnlinePlayers)
            {
                MessageManager.SendMessage(server.networkDriver, message, player.connection);
            }
        }
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
        // registreer nieuwe gebruiker
        if (result == TestPlayerManager.LoginResult.nonExistingName && message.newUser == 1)
        {
            // TODO voeg naam aan db toe
            server.playerManager.AddNewUserToDB(message.userName.ToString(), message.password.ToString());
            result = TestPlayerManager.LoginResult.success;
        }

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
