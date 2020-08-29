using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageAskLobbies : Message
{
    public override MessageType Type => MessageType.askLobbyList;
}