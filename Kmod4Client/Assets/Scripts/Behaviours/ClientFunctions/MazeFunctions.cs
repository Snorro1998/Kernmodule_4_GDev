using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class MazeFunctions
{
    public static void OnReceiveMaze(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        var message = MessageManager.ReadMessage<MessageMazeCreate>(stream) as MessageMazeCreate;
        string snd = MazeGenerator.Instance.currentMaze != null ? "Run" : null;
        MazeGenerator.Instance.currentMaze = message.maze;
        MazeGenerator.Instance.UpdateMaze = true;
        ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_MAZE_SCREEN, 1.0f, 1.0f, snd);
        //MazeGenerator.Instance.CreateMazeVisuals();
    }

    public static void OnRevealMazeTile(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        var message = MessageManager.ReadMessage<MessageMazeRevealTile>(stream) as MessageMazeRevealTile;
        AudioManager.Instance.PlaySound("TileClicked");
        MazeGenerator.Instance.RevealNeighs(message.tileId);
        MazeGenerator.Instance.visualObjects[message.tileId].button.interactable = false;
    }
}
