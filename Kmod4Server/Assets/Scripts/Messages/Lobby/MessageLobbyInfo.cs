using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLobbyInfo : Message
{
    public override MessageType Type => MessageType.sendLobbyInfo;
    public Lobby lobby;

    public MessageLobbyInfo()
    {

    }

    public MessageLobbyInfo(Lobby _lobby)
    {
        lobby = _lobby;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        int varsInMessage = lobby.allPlayers.Count;
        writer.WriteInt(varsInMessage);
        writer.WriteString(lobby.lobbyName);
        writer.WriteInt(lobby.gameID);
        writer.WriteUInt((uint)lobby.state);
        foreach (Lobby.SinglePlayer player in lobby.allPlayers)
        {
            writer.WriteString(player.name);
            writer.WriteInt(player.hp);
        }
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        int varsInMessage = reader.ReadInt();
        string lobName = reader.ReadString().ToString();
        int lobGameID = reader.ReadInt();
        Lobby.State lobState = (Lobby.State)reader.ReadUInt();
        List<Lobby.SinglePlayer> players = new List<Lobby.SinglePlayer>();
        
        for (int j = 0; j < varsInMessage; j++)
        {
            string name = reader.ReadString().ToString();
            int hp = reader.ReadInt();
            //default omdat t erbij moet
            Lobby.SinglePlayer player = new Lobby.SinglePlayer(name, default, hp);
            players.Add(player);
        }

        lobby = new Lobby(lobName, lobGameID, lobState, players);
    }
}
