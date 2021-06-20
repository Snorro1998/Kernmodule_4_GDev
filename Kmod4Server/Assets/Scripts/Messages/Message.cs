using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public abstract class Message
{
    public abstract GameEvent Type { get; }

    public virtual void Receiving(ref DataStreamReader reader)
    {
        
    }

    public virtual void Sending(ref DataStreamWriter writer)
    {
        writer.WriteUInt((uint)Type);
    }
}