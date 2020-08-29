using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageText : Message
{
    public override MessageType Type => MessageType.textMessage;
    public NativeString64 txt = "werk nou mee aub";

    public MessageText()
    {

    }

    public MessageText(NativeString64 _txt)
    {
        txt = _txt;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(txt);
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        txt = reader.ReadString();
    }
}

#if false
public class MessageText : Message
{
    public string txt = "";
    public override MessageType Type => MessageType.textMessage;
    //public override MessageType Ttype { get => base.Ttype; set => base.Ttype = value; }
    /*
    public MessageText(string _txt)
    {
        txt = _txt;
    }*/

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(txt);
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        txt = reader.ReadString().ToString();
    }
}
#endif