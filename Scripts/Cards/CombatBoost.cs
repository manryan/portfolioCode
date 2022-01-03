using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatBoost : SpellCard
{

    public override void onOurs()
    {
        base.onOurs();
    }

    public void affect()
    {
        //check if our card doesnt have a better attack boost?

        for (int i = 0; i < logic.mcardref.boost.boosts.Count; i++)
        {
            if (logic.mcardref.boost.boosts[i].spell.cardname == this.cardname)
            {
                returnToHand();
                return;
            }
            else if (logic.mcardref.boost.boosts[i].spell.type == type)
            {
                if (type == spellType.attboost || type == spellType.negatt)
                    if (Mathf.Abs(logic.mcardref.boost.boosts[i].attBoost) < Mathf.Abs(boostinfo.attBoost))
                    {

                        logic.mcardref.boost.boosts[i].spell.undo();
                        break;

                    }
                    else
                    {
                        returnToHand();
                        return;
                    }

                else //def boost
                {
                    if (Mathf.Abs(logic.mcardref.boost.boosts[i].defBoost) < Mathf.Abs(boostinfo.defBoost))
                    {

                        logic.mcardref.boost.boosts[i].spell.undo();
                        break;

                    }

                    else
                    {
                        returnToHand();
                        return;
                    }
                }
                
            }
            
        }

        logic.mcardref.boost.boosts.Add(boostinfo);

        if(type==spellType.attboost || type == spellType.negatt)
        logic.mcardref.updateAttack();
        else
            logic.mcardref.updateDefence();

        //show the effect somewhere through ui and spell boosts
        logic.mcardref.boost.addBoostUi(this);

        sendToGrave();

        //update the attack ui*************************
    }

    public override void onField()
    {
        returnToHand();
    }

    public override void onEnemy()
    {
        base.onEnemy();
    }

    public override void undo()
    {
        //update ui
        logic.mcardref.boost.removeBoostUi(this);


    }
}
