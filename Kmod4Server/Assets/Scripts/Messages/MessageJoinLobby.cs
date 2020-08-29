using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageJoinLobby : Message
{
    public override MessageType Type => MessageType.joinLobby;
    public int lobbyID = -1;
    public string lobbyName = "Hopjesvla";
    public LobbyStat status = LobbyStat.request;

    public enum LobbyStat
    {
        request,
        nonexist,
        ingame,
        exists
    }

    public MessageJoinLobby()
    {

    }

    public MessageJoinLobby(int _lobbyID, string _lobbyName, LobbyStat _status)
    {
        lobbyID = _lobbyID;
        lobbyName = _lobbyName;
        status = _status;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteInt(lobbyID);
        writer.WriteString(lobbyName);
        writer.WriteUInt((uint)status);
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        lobbyID = reader.ReadInt();
        lobbyName = reader.ReadString().ToString();
        status = (LobbyStat)reader.ReadUInt();
    }
}
