using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

static class MazeFunctions
{
    /// <summary>
    /// Wordt aangeroepen wanneer een client op een tegel in het doolhof heeft geklikt.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="sender"></param>
    /// <param name="connection"></param>
    public static void OnMazeTileClicked(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var server = sender as ServerBehaviour;
        var message = MessageManager.ReadMessage<MessageMazeRevealTile>(stream) as MessageMazeRevealTile;
        MazeGenerator.Instance.ServerOnTileClick(message.tileId);
    }
}
