using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageLogin : Message
{
    public override MessageType Type => MessageType.login;
    public NativeString64 userName = "Montana";
    public NativeString64 password = "Jones";
    /*//public LoginType loginType = LoginType.loginAttempt;

    public enum LoginType
    {
        loginAttempt,
        wrongPassword,
        alreadyLoggedIn,
        //failed,
        success
    }*/

    public MessageLogin()
    {

    }

    public MessageLogin(NativeString64 _userName, NativeString64 _password)
    {
        userName = _userName;
        password = _password;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(userName);
        writer.WriteString(password);
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        userName = reader.ReadString();
        password = reader.ReadString();
    }
}
