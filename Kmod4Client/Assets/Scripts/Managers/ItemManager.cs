using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : Singleton<ItemManager>
{
    public List<ConsumableItem> items = new List<ConsumableItem>();
    //public List<ConsumableItem> skills = new List<ConsumableItem>();
    public GameObject itemButtonPrefab;
    public GameObject itemUIScreen;
    public GameObject skillUIScreen;

    protected override void Awake()
    {
        base.Awake();
        InitItems();
        InitSkills();
        PrintNItems();
    }

    private void InitItems()
    {
        items.Add(new Bomb());
        items.Add(new Grenade());
    }

    private void InitSkills()
    {
        items.Add(new SkillSlash());
        GiveItem("Slash", 1);
    }

    public ConsumableItem GetItemByName(string _itemName)
    {
        ConsumableItem item = null;

        foreach (var i in items)
        {
            if (i.itemName == _itemName) return i;
        }

        return item;
    }

    public void UseItem(string _itemName, int _amount, string target, string user)
    {
        var item = GetItemByName(_itemName);
        if (item != null)
        {
            item.Use(_amount, GameManager.Instance.GetCharacterByName(target), GameManager.Instance.GetCharacterByName(user));
            UpdateItemButtons();
            //SendClientUseItem(_itemName, _amount, target, user);
        }
    }

    public void GiveItem(string _itemName, int _amount)
    {
        var item = GetItemByName(_itemName);
        if (item != null && _amount > 0)
        {
            item.Give(_amount);
            UpdateItemButtons();
        }
    }

    public void PrintNItems()
    {
        foreach (var i in items)
        {
            Debug.Log(i.itemName + ", " + i.amount);
        }
    }

    public void UpdateItemButtons()
    {
        UtilsMono.Instance.DestroyAllChildObjects(ref itemUIScreen);
        UtilsMono.Instance.DestroyAllChildObjects(ref skillUIScreen);
        foreach (var i in items)
        {
            if (i.amount > 0)
            {
                Transform parent = i.dontDecrease ? skillUIScreen.transform : itemUIScreen.transform;
                Text text = Instantiate(itemButtonPrefab, parent).GetComponentInChildren<Text>();
                text.text = i.amount > 1 ? i.amount + "X " + i.itemName : i.itemName;
                Button but = text.transform.parent.GetComponent<Button>();
                but.onClick.AddListener(delegate { GameManager.Instance.SetCurrentAction(i.itemName); });
                but.onClick.AddListener(delegate { BattleUIManager.Instance.ChangePanel(ActiveBattlePanel.PANEL_TARGETS); });
            }
        }
    }
}
