using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageGiveTurn : Message
{
    public override GameEvent Type =>  GameEvent.GAME_BATTLE_GIVE_TURN;
    public string characterName;
    public bool isPlayer;

    public MessageGiveTurn()
    {

    }

    public MessageGiveTurn(string _characterName, bool _isPlayer)
    {
        characterName = _characterName;
        isPlayer = _isPlayer;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        characterName = reader.ReadString().ToString();
        isPlayer = reader.ReadByte() == 0 ? false : true;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(characterName);
        writer.WriteByte(isPlayer ? (byte)1 : (byte)0);
    }
}
