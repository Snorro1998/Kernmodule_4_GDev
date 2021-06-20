using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeTileVisual : MonoBehaviour
{
    public GameObject wholeTile;
    public GameObject tileVisual;
    public GameObject pathVisualNorth;
    public GameObject pathVisualSouth;
    public GameObject pathVisualWest;
    public GameObject pathVisualEast;

    public Poss tile;
    public Text txt;
    public Button button;

    private IEnumerator ShowAnimation()
    {
        float xScale = 0.1f;
        float yScale = 0.1f;
        wholeTile.SetActive(true);

        while (xScale < 1.0f)
        {
            xScale = Mathf.Min(xScale + Time.deltaTime * 2, 1.0f);
            yScale = Mathf.Min(yScale + Time.deltaTime * 2, 1.0f);
            wholeTile.transform.localScale = new Vector2(xScale, yScale);
            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

    public void printDet()
    {
        MazeGenerator.Instance.ClientOnTileClick(tile);
        //button.interactable = false;
        //MazeGenerator.Instance.RevealNeighs(tile);
    }

    public void SetPathVisibility()
    {
        pathVisualNorth.SetActive(!tile.wallN);
        pathVisualSouth.SetActive(!tile.wallS);
        pathVisualWest.SetActive(!tile.wallW);
        pathVisualEast.SetActive(!tile.wallE);
    }

    public void Hide()
    {
        wholeTile.SetActive(false);
    }

    public void Show()
    {
        if (!wholeTile.activeSelf) StartCoroutine(ShowAnimation());
    }

    private void Start()
    {
        txt.text = "tile" + tile.id;
        SetPathVisibility();
        //Hide();
    }
}
