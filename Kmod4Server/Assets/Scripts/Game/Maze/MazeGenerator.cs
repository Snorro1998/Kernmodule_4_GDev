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

    public List<int> FilterVis(List<int> inList)
    {
        if (inList.Count > 0)
        {
            for (int i = inList.Count - 1; i >= 0; i--)
            {
                int val = inList[i];
                foreach(var t in visitedTiles)
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
        startTile.tyleType = TyleType.START;
        Poss endTile = null;

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
                if (currentTile != startTile && currentTile != endTile)
                {
                    int randomNumb = Random.Range(0, 150);
                    if (randomNumb <= 20) currentTile.tyleType = TyleType.TREASURE;
                    else if (randomNumb <= 50) currentTile.tyleType = TyleType.ENEMY;
                }
                currentTile.DigWall(nextTile.id);
                currentTile = nextTile;
            }
            //einde van een pad. Ga terug
            else
            {
                if (endTile == null)
                {
                    endTile = currentTile;
                    endTile.tyleType = TyleType.END;
                }
                else currentTile.tyleType = TyleType.TREASURE;
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

    public void CreateMazeVisuals(int startId)
    {
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
            vis.button.onClick.AddListener(delegate
            {
                vis.TileClicked();
            });

            if (t.tyleType == TyleType.ENEMY) vis.button.onClick.AddListener(delegate { GameManager.Instance.StartBattle(); });
            if (t.tyleType == TyleType.TREASURE) vis.button.onClick.AddListener(delegate { ItemManager.Instance.GiveRandomItem(); });
            if (t.tyleType == TyleType.END) vis.button.onClick.AddListener(delegate { GameManager.Instance.GotoNextMazeFloor();});

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
        CreateMazeVisuals(currentMaze.startTile);
    }

    public Poss GetTileById(int id)
    {
        Poss tile = null;
        foreach(var t in currentMaze.tileConnections)
        {
            if (t.id == id)
            {
                tile = t;
                break;
            }
        }
        return tile;
    }

    public void ServerOnTileClick(int index)
    {
        visualObjects[index].button.onClick.Invoke();
        /*var t = GetTileById(index);
        RevealNeighs(t);
        var message = new V10MessageMazeRevealTile(t.id);
        V10TestServerBehaviour.Instance.SendMessageToAll(message);
        */
        //V10TestServerBehaviour.Instance.SendRevealTileToClient(tile.id);
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

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        panel.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    }
}
