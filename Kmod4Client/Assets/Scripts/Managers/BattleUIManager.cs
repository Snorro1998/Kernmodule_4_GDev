using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ActiveBattlePanel
{
    PANEL_START,
    PANEL_ITEMS,
    PANEL_TARGETS,
    PANEL_SKILLS
}

public class BattleUIManager : Singleton<BattleUIManager>
{
    public GameObject enemyVisualsPanel;
    public GameObject playerVisualsPanel;
    public GameObject battleUIScreen;

    public GameObject characterVisualPrefab;
    public GameObject buttonPrefab;

    public ActiveBattlePanel activeBattlePanel;

    private Dictionary<ActiveBattlePanel, GameObject> panels = new Dictionary<ActiveBattlePanel, GameObject>();

    public GameObject mainPanelStart;
    public GameObject mainPanelItem;
    public GameObject mainPanelTarget;
    public GameObject mainPanelSkill;

    //Start panel
    public Button panelStartAttackButton;
    public Button panelStartItemButton;
    
    //Item panel
    public Button panelItemBackButton;

    //Target panel
    public Button panelTargetBackButton;
    public GameObject panelTargetTargetsCollection;

    //Skill panel
    public Button panelSkillBackButton;


    /// <summary>
    /// Werkt de targetknoppen bij.
    /// </summary>
    public void UpdateTargets()
    {
        UtilsMono.Instance.DestroyAllChildObjects(ref panelTargetTargetsCollection);
        foreach  (var m in GameManager.Instance.allMonsters)
        {
            Text text = Instantiate(buttonPrefab, panelTargetTargetsCollection.transform).GetComponentInChildren<Text>();
            text.text = m.charName;
            Button but = text.transform.parent.GetComponent<Button>();
            but.onClick.AddListener(delegate { GameManager.Instance.PerformAction(m.charName); });
        }
        foreach (var p in GameManager.Instance.allPlayers)
        {
            Text text = Instantiate(buttonPrefab, panelTargetTargetsCollection.transform).GetComponentInChildren<Text>();
            text.text = p.charName;
            Button but = text.transform.parent.GetComponent<Button>();
            but.onClick.AddListener(delegate { GameManager.Instance.PerformAction(p.charName); });
        }
    }

    /// <summary>
    /// Verandert het huidige menu als je op knoppen drukt.
    /// </summary>
    /// <param name="screen"></param>
    public void ChangePanel(ActiveBattlePanel screen)
    {
        foreach (var s in panels)
        {
            s.Value.SetActive(false);
        }

        if (panels.ContainsKey(screen))
        {
            panels[screen].SetActive(true);
        }
    }

    /// <summary>
    /// Pak bijbehorende visual van een karakter.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public CharacterDisplayV2 GetCharacterDisplayObject(Character obj)
    {
        foreach (Transform child in enemyVisualsPanel.transform)
        {
            var tmp = child.GetComponentInChildren<CharacterDisplayV2>();
            if (tmp.character == obj) return tmp;
        }
        foreach (Transform child in playerVisualsPanel.transform)
        {
            var tmp = child.GetComponentInChildren<CharacterDisplayV2>();
            if (tmp.character == obj) return tmp;
        }
        return null;
    }

    /// <summary>
    /// Toon de UI als de speler aan de beurt is.
    /// </summary>
    public void ShowBattleUI()
    {
        battleUIScreen.SetActive(true);
        ChangePanel(ActiveBattlePanel.PANEL_START);
    }

    /// <summary>
    /// Verberg de UI als de beurt voorbij is.
    /// </summary>
    public void HideBattleUI()
    {
        battleUIScreen.SetActive(false);
    }

    /// <summary>
    /// Maakt nieuwe visuals voor de karakters aan.
    /// </summary>
    /// <param name="_allMonsters"></param>
    public void CreateNewVisualsForCharacters(List<Monster> _allMonsters)
    {
        UtilsMono.Instance.DestroyAllChildObjects(ref enemyVisualsPanel);
        UtilsMono.Instance.DestroyAllChildObjects(ref playerVisualsPanel);
        GameManager.Instance.allMonsters = _allMonsters;
        foreach (var m in GameManager.Instance.allMonsters)
        {
            CharacterDisplayV2 obj = Instantiate(characterVisualPrefab, enemyVisualsPanel.transform).GetComponentInChildren<CharacterDisplayV2>();
            obj.character = m;
        }
        foreach (var p in GameManager.Instance.allPlayers)
        {
            CharacterDisplayV2 obj = Instantiate(characterVisualPrefab, playerVisualsPanel.transform).GetComponentInChildren<CharacterDisplayV2>();
            obj.character = p;
        }
    }

    private void InitDict()
    {
        panels.Add(ActiveBattlePanel.PANEL_START, mainPanelStart);
        panels.Add(ActiveBattlePanel.PANEL_ITEMS, mainPanelItem);
        panels.Add(ActiveBattlePanel.PANEL_TARGETS, mainPanelTarget);
        panels.Add(ActiveBattlePanel.PANEL_SKILLS, mainPanelSkill);
    }

    private void Start()
    {
        InitDict();
        panelStartAttackButton.onClick.AddListener(delegate { ChangePanel(ActiveBattlePanel.PANEL_SKILLS); });
        panelStartItemButton.onClick.AddListener(delegate { ChangePanel(ActiveBattlePanel.PANEL_ITEMS); });
        panelItemBackButton.onClick.AddListener(delegate { ChangePanel(ActiveBattlePanel.PANEL_START); });
        panelTargetBackButton.onClick.AddListener(delegate { ChangePanel(ActiveBattlePanel.PANEL_START); });
        panelSkillBackButton.onClick.AddListener(delegate { ChangePanel(ActiveBattlePanel.PANEL_START); });
    }
}
