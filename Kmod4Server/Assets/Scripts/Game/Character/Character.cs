using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string charName = "niemand";
    public int currentHp;
    public int statStrength = 1;
    public int statDefense = 1;
    public int statMaxHealth = 10;

    public int score = 0;

    public Character()
    {
        currentHp = statMaxHealth;
    }

    public Character(int _statMaxHealth)
    {
        statMaxHealth = _statMaxHealth;
        currentHp = statMaxHealth;
    }

    public bool IsAlive()
    {
        return currentHp > 0;
    }

    public void Heal(uint amount)
    {
        currentHp = Mathf.Min(currentHp + (int)amount, statMaxHealth);
    }

    public void Damage(uint amount)
    {
        currentHp = Mathf.Max(currentHp - (int)amount, 0);
    }

    public void GetPoint()
    {
        score++;
    }
}
