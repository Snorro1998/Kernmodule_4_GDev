using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : ConsumableItem
{
    public override BattleActions Type =>  BattleActions.DAMAGE;

    public Bomb()
    {
        itemName = "Bomb";
        effect = 8;
    }

    protected override void UseFunc(Character target, Character user)
    {
        base.UseFunc(target, user);
        GameManager.Instance.DamageCharacter(target, (uint)effect);
        //target.Damage((uint)effect);
    }

    protected override void GiveFunc()
    {
        base.GiveFunc();
    }
}