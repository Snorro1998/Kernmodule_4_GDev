using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLoginResponse : Message
{
    public override MessageType Type => MessageType.login;
    public TestPlayerManager.LoginResult result;
    
    public MessageLoginResponse()
    {

    }

    public MessageLoginResponse(TestPlayerManager.LoginResult _result)
    {
        result = _result;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteUInt((uint)result);
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        result = (TestPlayerManager.LoginResult)reader.ReadUInt();
    }
}
