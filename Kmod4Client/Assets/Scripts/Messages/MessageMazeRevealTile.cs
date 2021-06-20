using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageMazeRevealTile : Message
{
    public override GameEvent Type => GameEvent.GAME_MAZE_REVEAL_TILE;
    public int tileId = -1;

    public MessageMazeRevealTile()
    {

    }

    public MessageMazeRevealTile(int _tileId)
    {
        tileId = _tileId;
    }


    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        tileId = reader.ReadInt();
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteInt(tileId);
    }
}
