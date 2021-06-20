using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster : Character
{
    public Monster(string _name, int _statMaxHealth)
    {
        charName = _name;
        statMaxHealth = _statMaxHealth;
    }
}
