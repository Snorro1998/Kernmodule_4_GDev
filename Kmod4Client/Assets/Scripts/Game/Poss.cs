using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Poss
{
    public int id = -1;

    public int northId = -1;
    public int southId = -1;
    public int westId = -1;
    public int eastId = -1;

    public bool wallN = true;
    public bool wallS = true;
    public bool wallW = true;
    public bool wallE = true;

    public List<int> GetDiggables()
    {
        List<int> tiles = new List<int>();
        if (northId != -1 && wallN) tiles.Add(northId);
        if (southId != -1 && wallS) tiles.Add(southId);
        if (westId != -1 && wallW) tiles.Add(westId);
        if (eastId != -1 && wallE) tiles.Add(eastId);
        return tiles;
    }

    public void DigWall(int index)
    {
        if (northId == index) wallN = false;
        else if (southId == index) wallS = false;
        else if (westId == index) wallW = false;
        else if (eastId == index) wallE = false;
    }

    public Poss(int _id)
    {
        id = _id;
    }

    public Poss(int _id, int _n, int _s, int _w, int _e)
    {
        id = _id;
        northId = _n;
        southId = _s;
        westId = _w;
        eastId = _e;
    }

    public Poss(int _id, int _n, int _s, int _w, int _e, bool _wN, bool _wS, bool _wW, bool _wE)
    {
        id = _id;
        northId = _n;
        southId = _s;
        westId = _w;
        eastId = _e;
        wallN = _wN;
        wallS = _wS;
        wallW = _wW;
        wallE = _wE;
    }
}
