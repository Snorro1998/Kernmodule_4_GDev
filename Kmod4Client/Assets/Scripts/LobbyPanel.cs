using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : V2Singleton<LobbyPanel>
{
    public GameObject loadButtonPrefab;
    public Transform loadButtonTransform;

    public Text lobbyDetailTitle;
    public GameObject joinLobbyButton;
    public int selectedLobID = -1;

    /// <summary>
    /// Werkt het paneel met de lobbyinformatie bij
    /// </summary>
    /// <param name="lobbyID"></param>
    public void UpdateLobbyDetailPanel(int lobbyID)
    {
        joinLobbyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        selectedLobID = lobbyID;
        Lobby llob = default;
        foreach (Lobby lob in TestClientBehaviour.Instance.lobbies)
        {
            if (lob.gameID == selectedLobID)
            {
                llob = lob;
                break;
            }
        }
        // Propt alles in een tekstobject. Dit kan nog wel verbeterd worden
        string txt = selectedLobID == -1 ? "Geen lobby geselecteerd" : "De geselecteerde lobby bestaat niet meer";
        if (TestClientBehaviour.Instance.lobbies.Count == 0) txt = "Er bestaan geen lobbies!";
        if (llob != default)
        {
            txt = "Servernaam: " + llob.lobbyName + "\nStatus: " + llob.state.ToString() + "\n\nSpelers:\n";
            foreach (Lobby.SinglePlayer player in llob.allPlayers)
            {
                txt += player.name + "\n";
            }
            // Maakt de joinknop werkzaam als de lobby geldig is
            joinLobbyButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                TestClientBehaviour.Instance.JoinLobby(llob.lobbyName, llob.gameID);
            });
        }
        lobbyDetailTitle.text = txt;
    }

    /// <summary>
    /// Werkt de visuele lobbylijst bij
    /// </summary>
    public void UpdatePanel()
    {
        // Verwijdert oude knoppen
        foreach(Transform t in loadButtonTransform)
        {
            Destroy(t.gameObject);
        }
        if (TestClientBehaviour.Instance.lobbies.Count == 0)
        {
            Debug.Log("Er zijn geen lobbies!");
        }
        else
        {
            // Maakt nieuwe knoppen aan
            for (int i = 0; i < TestClientBehaviour.Instance.lobbies.Count; i++)
            {
                GameObject loadButtonObject = Instantiate(loadButtonPrefab);
                loadButtonObject.transform.SetParent(loadButtonTransform, false);

                int lobbyID = TestClientBehaviour.Instance.lobbies[i].gameID;
                string lobbyName = TestClientBehaviour.Instance.lobbies[i].lobbyName;

                loadButtonObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UpdateLobbyDetailPanel(lobbyID);
                });
                // Stelt de tekst van de knop gelijk aan de lobbynaam
                string txt = TestClientBehaviour.Instance.lobbies[i].lobbyName;
                loadButtonObject.GetComponentInChildren<Text>().text = txt;
            }
        }
        
        UpdateLobbyDetailPanel(selectedLobID);
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
