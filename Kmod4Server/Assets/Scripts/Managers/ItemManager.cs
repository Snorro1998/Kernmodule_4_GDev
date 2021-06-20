using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : Singleton<ItemManager>
{
    public List<ConsumableItem> items = new List<ConsumableItem>();
    public GameObject itemButtonPrefab;
    public GameObject itemUIScreen;
    private int nItems = 0;

    protected override void Awake()
    {
        base.Awake();
        InitItems();
        InitSkills();
    }

    private void InitItems()
    {
        items.Add(new Bomb());
        items.Add(new Grenade());
        nItems = items.Count;
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
        Debug.Log("item=" + item);
        if (item != null)
        {
            item.Use(_amount, GameManager.Instance.GetCharacterByName(target), GameManager.Instance.GetCharacterByName(user));
            UpdateItemButtons();
            SendClientUseItem(_itemName, _amount, target, user);
        }        
    }

    public void GiveItem(string itemName, int amount)
    {
        var item = GetItemByName(itemName);
        if (item != null && amount > 0)
        {
            item.Give(amount);
            SendClientGiveItem(itemName, amount);
        }
    }

    public void GiveRandomItem()
    {
        //Debug.Log(items.Count);
        int i = Random.Range(0, nItems);
        GiveItem(items[i].itemName, 1);
    }

    public void PrintNItems()
    {
        foreach (var i in items)
        {
            Debug.Log(i.itemName + ", " + i.amount);
        }
    }

    public void SendClientUseItem(string itemName, int amount, string target, string user)
    {
        var message = new MessageUseItem(itemName, amount, target, user);
        ServerBehaviour.Instance.SendMessageToAllOnline(message);
    }

    public void SendClientGiveItem(string itemName, int amount)
    {
        var message = new MessageGiveItem(itemName, amount);
        ServerBehaviour.Instance.SendMessageToAllOnline(message);
    }

    public void UpdateItemButtons()
    {
        UtilsMono.Instance.DestroyAllChildObjects(ref itemUIScreen);
        foreach (var i in items)
        {
            if (i.amount > 0)
            {
                Text text = Instantiate(itemButtonPrefab, itemUIScreen.transform).GetComponentInChildren<Text>();
                text.text = i.amount + "X " + i.itemName;
                Button but = text.transform.parent.GetComponent<Button>();
                but.onClick.AddListener(delegate { GameManager.Instance.SetCurrentAction(i.itemName); });
                but.onClick.AddListener(delegate { BattleUIManager.Instance.ChangePanel(ActiveBattlePanel.PANEL_TARGETS); });
            }
        }
    }
}
