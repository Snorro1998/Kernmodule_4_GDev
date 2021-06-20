using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageMazeCreate : Message
{
    public override GameEvent Type => GameEvent.GAME_MAZE_CREATE_NEW;
    public Maze maze;

    public MessageMazeCreate()
    {

    }

    public MessageMazeCreate(Maze _maze)
    {
        maze = _maze;
    }


    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        Vector2Int dimensions = new Vector2Int();
        dimensions.x = reader.ReadInt();
        dimensions.y = reader.ReadInt();
        List<Poss> tiles = new List<Poss>();

        for (int i = 0; i < dimensions.x * dimensions.y; i++)
        {
            int north = reader.ReadInt();
            int south = reader.ReadInt();
            int west = reader.ReadInt();
            int east = reader.ReadInt();
            bool wN = reader.ReadByte() == (byte)1 ? true : false;
            bool wS = reader.ReadByte() == (byte)1 ? true : false;
            bool wW = reader.ReadByte() == (byte)1 ? true : false;
            bool wE = reader.ReadByte() == (byte)1 ? true : false;
            tiles.Add(new Poss(i, north, south, west, east, wN, wS, wW, wE));
        }
        int startTile = reader.ReadInt();
        string mazeName = reader.ReadString().ToString();
        maze = new Maze(dimensions, tiles, startTile, mazeName);
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteInt(maze.dimensions.x);
        writer.WriteInt(maze.dimensions.y);
        foreach (var i in maze.tileConnections)
        {
            writer.WriteInt(i.northId);
            writer.WriteInt(i.southId);
            writer.WriteInt(i.westId);
            writer.WriteInt(i.eastId);
            writer.WriteByte(i.wallN ? (byte)1 : (byte)0);
            writer.WriteByte(i.wallS ? (byte)1 : (byte)0);
            writer.WriteByte(i.wallW ? (byte)1 : (byte)0);
            writer.WriteByte(i.wallE ? (byte)1 : (byte)0);
        }
        writer.WriteInt(maze.startTile);
        writer.WriteString(maze.name);
    }
}
