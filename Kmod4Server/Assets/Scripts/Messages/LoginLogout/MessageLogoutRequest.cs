using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLogoutRequest : Message
{
    public override GameEvent Type => GameEvent.LOGOUT_REQUEST;

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
    }
}
