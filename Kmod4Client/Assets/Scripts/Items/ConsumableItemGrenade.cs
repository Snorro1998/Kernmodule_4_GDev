using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ConsumableItem
{
    public override BattleActions Type => BattleActions.DAMAGE;

    public Grenade()
    {
        itemName = "Grenade";
        effect = 4;
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
