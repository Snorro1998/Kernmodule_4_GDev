using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLoginRequest : Message
{
    public override GameEvent Type => GameEvent.LOGIN_REQUEST;
    
    public string name;
    public string password;

    public MessageLoginRequest()
    {

    }

    public MessageLoginRequest(string _name, string _password)
    {
        name = _name;
        password = _password;
    }


    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        name = reader.ReadString().ToString();
        password = reader.ReadString().ToString();
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(name);
        writer.WriteString(password);
    }
}
