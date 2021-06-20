using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLoginResponse : Message
{
    public override GameEvent Type => GameEvent.LOGIN_RESPONSE;

    public enum LoginResult
    {
        UNKNOWN_USERNAME,
        INVALID_PASSWORD,
        ALREADY_LOGGED_IN,
        SUCCES
    }

    public LoginResult result;

    public MessageLoginResponse()
    {

    }

    public MessageLoginResponse(LoginResult _result)
    {
        result = _result;
    }


    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        result = (LoginResult)reader.ReadInt();
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteInt((int)result);
    }
}
