using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageUseItem : Message
{
    public override GameEvent Type => GameEvent.GAME_USE_ITEM;

    public string itemName;
    public int amount = 0;
    public string targetName;
    public string userName;

    public MessageUseItem()
    {

    }

    public MessageUseItem(string _itemName, int _amount, string _targetName, string _userName)
    {
        itemName = _itemName;
        amount = _amount;
        targetName = _targetName;
        userName = _userName;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        itemName = reader.ReadString().ToString();
        amount = reader.ReadInt();
        targetName = reader.ReadString().ToString();
        userName = reader.ReadString().ToString();
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(itemName);
        writer.WriteInt(amount);
        writer.WriteString(targetName);
        writer.WriteString(userName);
    }
}
