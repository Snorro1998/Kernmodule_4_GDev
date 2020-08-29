using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageAlive : Message
{
    public override MessageType Type => MessageType.alive;
}