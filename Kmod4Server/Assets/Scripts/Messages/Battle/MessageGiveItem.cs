using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageGiveItem : Message
{
    public override GameEvent Type =>  GameEvent.GAME_GET_ITEM;

    public string itemName;
    public int amount = 0;

    public MessageGiveItem()
    {

    }

    public MessageGiveItem(string _itemName, int _amount)
    {
        itemName = _itemName;
        amount = _amount;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        itemName = reader.ReadString().ToString();
        amount = reader.ReadInt();
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(itemName);
        writer.WriteInt(amount);
    }
}
