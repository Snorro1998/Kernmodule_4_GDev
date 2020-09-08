using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLogin : Message
{
    public override MessageType Type => MessageType.userLogin;
    public NativeString64 userName = "Montana";
    public NativeString64 password = "Jones";
    public byte newUser = 0;

    public MessageLogin()
    {

    }

    public MessageLogin(NativeString64 _userName, NativeString64 _password, byte _newUser)
    {
        userName = _userName;
        password = _password;
        newUser = _newUser;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(userName);
        writer.WriteString(password);
        writer.WriteByte(newUser);
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        userName = reader.ReadString();
        password = reader.ReadString();
        newUser = reader.ReadByte();
    }
}