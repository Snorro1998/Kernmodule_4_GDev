using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessagePing : Message
{
    public override GameEvent Type => GameEvent.PING;

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
    }
}
