using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageBattleStart : Message
{
    public override GameEvent Type => GameEvent.GAME_BATTLE_START;
    public List<Monster> allMonsters = new List<Monster>();

    public MessageBattleStart()
    {

    }

    public MessageBattleStart(List<Monster> _allMonsters)
    {
        allMonsters = _allMonsters;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        allMonsters = Utils.ReadListFromStream<Monster>(ref reader);
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        Utils.WriteListToStream(ref writer, ref allMonsters);
    }
}
