using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlash : ConsumableItem
{
    public override BattleActions Type => BattleActions.DAMAGE;

    public SkillSlash()
    {
        itemName = "Slash";
        dontDecrease = true;
        effect = 2;
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