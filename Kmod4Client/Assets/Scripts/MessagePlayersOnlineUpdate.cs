using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MessagePlayersOnlineUpdate : Message
{
    public override GameEvent Type => GameEvent.PLAYERMANAGER_UPDATE_PLAYERS;
    public List<Player> players = new List<Player>();

    public MessagePlayersOnlineUpdate()
    {

    }

    public MessagePlayersOnlineUpdate(List<Player> _players)
    {
        players = _players;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        players = Utils.ReadListFromStream<Player>(ref reader);
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        Utils.WriteListToStream(ref writer, ref players);
    }
}
