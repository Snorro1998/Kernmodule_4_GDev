using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageRequestLobbyLeave : Message
{
    public override MessageType Type => MessageType.requestLobbyLeave;
}
