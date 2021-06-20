using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : Singleton<MazeGenerator>
{
    public Transform panelMap;
    public GridLayoutGroup panel;
    public GameObject prefabMazeTile;

    public Vector2Int mazeDimensions = new Vector2Int(6, 3);

    public List<Poss> tiles = new List<Poss>();
    public List<Poss> visitedTiles = new List<Poss>();
    public List<MazeTileVisual> visualObjects = new List<MazeTileVisual>();
    public Maze currentMaze;

    public bool UpdateMaze = false;

    public List<int> FilterVis(List<int> inList)
    {
        if (inList.Count > 0)
        {
            for (int i = inList.Count - 1; i >= 0; i--)
            {
                int val = inList[i];
                foreach (var t in visitedTiles)
                {
                    if (t.id == val)
                    {
                        inList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        return inList;
    }

    private void CreateMazeTiles()
    {
        for (int y = 0; y < mazeDimensions.y; y++)
        {
            for (int x = 0; x < mazeDimensions.x; x++)
            {
                var i = mazeDimensions.x * y + x;
                var n = y < mazeDimensions.y - 1 ? i + mazeDimensions.x : -1;
                var s = y > 0 ? i - mazeDimensions.x : -1;
                var w = x > 0 ? i - 1 : -1;
                var e = x < mazeDimensions.x - 1 ? i + 1 : -1;
                tiles.Add(new Poss(i, n, s, w, e));
            }
        }
    }

    private Maze InitMazeTiles(string mazeName)
    {
        Poss startTile = tiles[Random.Range(0, tiles.Count)];
        Poss currentTile = startTile;

        for (int i = 0; i < mazeDimensions.x * mazeDimensions.y; i++)
        {
            if (!visitedTiles.Contains(currentTile)) visitedTiles.Add(currentTile);
            else i--;

            var inn = FilterVis(currentTile.GetDiggables());
            var index = inn.Count > 0 ? inn[Random.Range(0, inn.Count)] : -1;
            if (index != -1)
            {
                var nextTile = tiles[index];
                nextTile.DigWall(currentTile.id);
                currentTile.DigWall(nextTile.id);
                currentTile = nextTile;
            }
            else
            {
                for (int j = visitedTiles.Count - 1; j >= 0; j--)
                {
                    currentTile = visitedTiles[j];
                    inn = FilterVis(currentTile.GetDiggables());
                    index = inn.Count > 0 ? inn[Random.Range(0, inn.Count)] : -1;
                    if (index != -1) break;
                }
            }
        }

        return new Maze(mazeDimensions, tiles, startTile.id, mazeName);
    }

    public void CreateMazeVisuals()
    {
        if (!UpdateMaze) return;
        UpdateMaze = false;
        int startId = currentMaze.startTile;
        for (int t = panel.transform.childCount - 1; t >= 0; t--)
        {
            Destroy(visualObjects[t].gameObject);
        }
        visualObjects.Clear();

        panel.constraintCount = mazeDimensions.x;
        int i = 0;
        foreach (var t in currentMaze.tileConnections)
        {
            MazeTileVisual vis = Instantiate(prefabMazeTile, panel.transform).GetComponent<MazeTileVisual>();
            vis.tile = t;
            vis.button.onClick.AddListener(delegate { vis.printDet(); });
            visualObjects.Add(vis);
            if (i != startId) vis.Hide();
            i++;
        }
    }

    public Maze CreateNewMaze(string mazeName)
    {
        tiles.Clear();
        visitedTiles.Clear();

        CreateMazeTiles();
        return InitMazeTiles(mazeName);
    }

    public void InitMaze(string mazeName)
    {
        currentMaze = CreateNewMaze(mazeName);
        CreateMazeVisuals();
    }

    public void ClientOnTileClick(Poss tile)
    {
        var message = new MessageMazeRevealTile(tile.id);
        ClientBehaviour.Instance.SendMessageToServer(message);
    }

    public void RevealNeighs(Poss tile)
    {
        var n = tile.northId;
        var s = tile.southId;
        var w = tile.westId;
        var e = tile.eastId;

        if (n != -1 && !tile.wallN) visualObjects[n].Show();
        if (s != -1 && !tile.wallS) visualObjects[s].Show();
        if (w != -1 && !tile.wallW) visualObjects[w].Show();
        if (e != -1 && !tile.wallE) visualObjects[e].Show();
    }

    public void RevealNeighs(int index)
    {
        RevealNeighs(currentMaze.tileConnections[index]);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        panel.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    }
}
