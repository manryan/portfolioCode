using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum spellType { attboost = 1, defboost, poison, infect, stun, other, negatt, negdef};

public class SpellCard : Card, IPointerEnterHandler, IPointerExitHandler
{
    public string description;

    public GameObject descriptionText;

    public spellType type;

    public boost boostinfo;

    public UnityEvent ourmonster;

    public UnityEvent enemymonster;


    public UnityEvent onfield;

    void Start()
    {
        transform.GetChild(1).GetComponent<Text>().text = cardname;
        backside = transform.GetChild(2).gameObject;

        //set ui text to description
        descriptionText = transform.GetChild(0).GetChild(1).gameObject;

        descriptionText.transform.GetChild(0).GetComponent<Text>().text = description;
        boostinfo.spell = this;
    }

    public void Antidote()
    {
        if (logic.mcardref.attributes.Poisoned)
        {

            for (int i = 0; i < logic.mcardref.boost.boosts.Count; i++)
            {
                if (logic.mcardref.boost.boosts[i].spell.type == spellType.poison)
                {
                    logic.mcardref.boost.removeBoostUi(logic.mcardref.boost.boosts[i].spell);
                    logic.mcardref.attributes.Poisoned = false;
                    sendToGrave();
                    return;
                }
            }
        }
        returnToHand();
    }


    //set the description text on

    public override void play()
    {
        base.play();

        if (logic.containsEnemymonster(this))
        {
            //affect enemy if possible with card type
            onEnemy();
        }
        else if (logic.containsOurmonster(this))
        {
            //affect our monster if possible with card type
            onOurs();
        }
        else if (logic.containsfield(this))
        {
            onField();
            //aoe effect or etc?
        }
        else
        {
            //return to hand
            returnToHand();
        }
    }

    public virtual void onEnemy() {

        if (enemymonster.GetPersistentEventCount() > 0)
            enemymonster.Invoke();
        else
            returnToHand();
    }

    public virtual void onOurs() {

        if (ourmonster.GetPersistentEventCount() > 0)
            ourmonster.Invoke();
        else
            returnToHand();
    }

    public virtual void onField() {

        if (onfield.GetPersistentEventCount() > 0)
            onfield.Invoke();
        else
            returnToHand();
    }

    public virtual void undo() { }


    public override void checktomove()
    {
        //   base.checktomove();

        //if not our turn, return

        if (logic.turnM.turn != logic.sideNum || logic.turnM.dealing)
        {
            canmove = false;
            return;
        }
        else
            canmove = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //enable ui if we arent holding down mouse buttonW
        descriptionText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //enable ui if we arent holding down mouse buttonW
        if (!Input.GetMouseButton(0) || !canmove)
            descriptionText.SetActive(false);
    }

    public override void Clicked()
    {
        descriptionText.SetActive(false);
    }

    public void poisonThem()
    {

        if (logic.mcardref.attributes.canPoison())
        {
            logic.mcardref.attributes.Poisoned = true;
            logic.mcardref.boost.boosts.Add(boostinfo);
            logic.mcardref.boost.addBoostUi(this);
            sendToGrave();
        }
        else returnToHand();
    }
}