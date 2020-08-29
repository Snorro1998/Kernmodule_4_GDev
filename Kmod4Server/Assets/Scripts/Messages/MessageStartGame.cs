using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageStartGame : Message
{
    public override MessageType Type => MessageType.startLobby;
}
