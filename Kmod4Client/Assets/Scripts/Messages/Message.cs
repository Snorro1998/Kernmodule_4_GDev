using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public abstract class Message
{
    public abstract MessageType Type { get; }

    public enum MessageType
    {
        invalid = 0,
        alive,
        textMessage,
        userLogin,
        userRegister,
        askLobbyList,
        sendLobbyList,
        sendLobbyInfo,
        joinLobby,
        startLobby,
        gameGiveTurn,
        gameAttack,
        imageSend,
        requestLobbyLeave,
        responseLobbyLeave
    }

    public virtual void Sending(ref DataStreamWriter writer)
    {
        writer.WriteUInt((uint)Type);
    }

    public virtual void Receiving(ref DataStreamReader reader)
    {

    }
}
/*
public abstract class Message
{
    public abstract MessageType Type { get; }

    public enum MessageType
    {
        invalid = 0,
        none,
        textMessage
    }

    public virtual void Sending(ref DataStreamWriter writer)
    {
        writer.WriteUInt((uint)Type);
    }

    public virtual void Receiving(ref DataStreamReader reader)
    {

    }
}
*/