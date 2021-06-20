using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ConsumableItem
{
    public string itemName = "";
    public int amount = 0;
    public int effect = 1;

    public abstract BattleActions Type { get; }

    protected virtual void UseFunc(Character target, Character user)
    {

    }

    protected virtual void GiveFunc()
    {

    }

    public void Use(int _amount, Character target, Character user)
    {
        if (amount >= _amount)
        {
            amount = Mathf.Max(0, amount - _amount);
            UseFunc(target, user);
        }
        else
        {
            Debug.Log("Kan " + itemName + " niet gebruiken. Er is onvoldoende van");
        }
    }

    public void Give(int _amount)
    {
        amount += _amount;
        GiveFunc();
    }
}
