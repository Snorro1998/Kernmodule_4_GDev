using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    public Vector2Int dimensions = new Vector2Int();
    public List<Poss> tileConnections = new List<Poss>();
    public int startTile = -1;
    public string name;

    public Maze(Vector2Int _dimensions, List<Poss> _tileConnections, int _startTile, string _name)
    {
        dimensions = _dimensions;
        tileConnections = _tileConnections;
        startTile = _startTile;
        name = _name;
    }
}
