using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum combatState { attack = 1, defend };

public class MonsterCard : Card, IPointerEnterHandler, IPointerExitHandler
{

    public int attack;

    public int defence;

    public int health;

    public int turns;

    public int lives;

    public combatState combat;

    public int numofattacks;


    RectTransform hprect;

    [System.NonSerialized]
    public SpellBoosts boost;

    GameObject boostContainer;

    Image attdefpic;

    public Sprite[] attdeficons;

    int currentAtt;

    int currentDef;

    float hpbarwidth;

    RectTransform hpbarchild;

    public int attackTurns = 1;

    public monsterAttributes attributes;


    public void updateAttack()
    {

        transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = attackPower().ToString();
    }
    public void updateDefence()
    {
        transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = defencePower().ToString();
    }

    public void updateTurns()
    {
        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Spawn in " + turns.ToString();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //show spell boosts
        if(boost.boosts.Count>0)
        {
            boostContainer.SetActive(true);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //hide spell boosts
        if (boost.boosts.Count > 0)
        {
            boostContainer.SetActive(false);
        }
    }
    public override void sendToGrave()
    {
        lives = 0;
        hprect.sizeDelta = new Vector2(0, 30);

        //restart stats
        currentAtt = attack;
        currentDef = defence;

        //clear all boosts and ui
        boost.clearBoostsAndUi();

        updateAttack();
        updateDefence();

        if (cardState == State.Waiting)
        {
            //disable spawn screen
            transform.GetChild(1).gameObject.SetActive(false);
        }
        attributes.resetStates();

        base.sendToGrave();

    }



    public override void Awake()
    {
        base.Awake();

        attributes.mc = this;
        transform.GetChild(3).GetComponent<Text>().text = cardname;
        attdefpic = transform.GetChild(3).GetChild(0).GetComponent<Image>();
        transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = turns.ToString() + " Turns";
        lives = health;
        hprect = transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<RectTransform>();
        boost = GetComponent<SpellBoosts>();
        backside = transform.GetChild(5).gameObject;
        unplayable = transform.GetChild(4).gameObject;

        boostContainer = transform.GetChild(transform.childCount - 1).gameObject;
        hpbarchild = transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<RectTransform>();

            hpbarwidth = 164  - ((6-health)* 26 );

        hprect.sizeDelta = new Vector2(hpbarwidth , 30);
        hpbarchild.sizeDelta = new Vector2(164-hpbarwidth, 30);

        currentAtt = attack;
        currentDef = defence;
        updateAttack();
        updateDefence();
        numofattacks = 0;

        //initialize
        if (attackTurns == 0)
            attackTurns = 1;
    }

    public void nextTUrn()
    {
        turns--;
            updateTurns();

        if (turns < 1)
        { 
            numofattacks = 0;
            cardState = State.field;
            //disable spawn screen
            transform.GetChild(1).gameObject.SetActive(false);

        }
    }

    public void Damage(int dmg)
    {
        lives += dmg;
        if (lives > health)
        {
            lives = health;
            hprect.sizeDelta = new Vector2(hpbarwidth,30);
        }
        else if (lives <= 0)
        {
            sendToGrave();
        }
        else
        {
            hprect.sizeDelta = new Vector2(hpbarwidth - ((hpbarwidth / health) * (health-lives)), 30);

        }
    }


    Card baseclass;

    MonsterCard mc;

    int test;

    public int recoilDamage()
    {
        test = 0;

           for (int i = 0; i < logic.mcardref.defence; i+=2)
           {
               test++;

           }
           return test;
    }

    public int attackDifference()
    {
        test = 0;

        for (int i = 0; i < logic.mcardref.attack; i+=2)
        {
            test++;

        }
        return -test;
    }

    public override void play()
    {
        base.play();
        switch(cardState)
        {
            case State.inHand:
                //if we hit our field
                if (logic.containsfield(this))
                {
                    //hide turns text
                        transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);


                    //start turns if there are any
                    if (turns > 0)
                    {
                        //enable spawn screen
                        transform.GetChild(1).gameObject.SetActive(true);

                        cardState = State.Waiting;
                        updateTurns();
                    }
                    else
                    {
                        cardState = State.field;
                    }
                    combat = combatState.attack;
                    attdefpic.sprite = attdeficons[0];
                    logic.fieldMonsters.Add(this);

                    movedCard();



                    //increase amt played
                    logic.monstersplayed++;
                    logic.hand.Remove(this);
                    logic.adjustHandSpacing();

                    //if we played more than one monster set all other monsters in hand as unplayable

                    if(logic.monstersplayed>= logic.monsterswecanplay)
                    {
                        for (int i = 0; i < logic.hand.Count; i++)
                        {
                            if(logic.hand[i].unplayable!=null)
                            {
                                logic.hand[i].unplayable.SetActive(true);
                            }
                        }
                    }
                        
                }
                else
                {
                    returnToHand();
                }
              

                break;
            case State.Waiting:
                

                break;
            case State.field:
                //we attack
                if (logic.containsHand(this))
                {

                    //switch our combat state 

                    //switch the image
                    if (numofattacks < attackTurns)
                    {
                        if (combat == combatState.attack)
                        {
                            combat = combatState.defend;
                            attdefpic.sprite = attdeficons[1];
                        }
                        else
                        {
                            combat = combatState.attack;
                            attdefpic.sprite = attdeficons[0];
                        }
                    }
                    
                }
                else if (logic.containsEnemymonster(this))
                {
                    handleAttack();
                }
                //if it lands on enemy field and no enemies are there, steal from lifepoints
                else if (logic.containsenemyField(this))
                {
                    //steal lifepoints
                    if (numofattacks < attackTurns)
                    {
                        numofattacks++;


                        if (numofattacks == attackTurns)
                        {
                           unplayable.SetActive(true);
                        }
                    }

                    //STEAL LIFE POINTS ************

                }
                else
                {
                    //nothing there at all     
                //return to field
                }
                if(lives >0)
                returnToField();
                break;

        }
    }

    private void handleAttack()
    {
        if (numofattacks < attackTurns && combat == combatState.attack)
        {
            if (logic.mcardref.combat == combatState.defend)
            {
                if (logic.mcardref.defence < attack)
                {
                    //send them to grave
                    //     logic.mcardref.logic.fieldMonsters.Remove(logic.mcardref);
                    //       logic.mcardref.sendToGrave();

                    logic.mcardref.logic.fieldMonsters.Remove(logic.mcardref);
                    logic.mcardref.sendToGrave();
                    Damage(-recoilDamage());
                }
                else
                {
                    //send to grave
                    logic.fieldMonsters.Remove(this);
                    sendToGrave();
                    return;
                }
            }
            else
            {
                if (logic.mcardref.attack < attack)
                {
                    //send them to grave without any recoil
                    logic.mcardref.logic.fieldMonsters.Remove(logic.mcardref);
                    logic.mcardref.sendToGrave();
                }
                else
                {
                    //send our monster to grave, and recoil on them
                    logic.fieldMonsters.Remove(this);
                    sendToGrave();
                    logic.mcardref.logic.mcardref = this;
                    logic.mcardref.Damage(logic.mcardref.attackDifference());
                    return;
                }
            }

            if (health > 0)
            {

                numofattacks++;


                if (numofattacks == attackTurns)
                {
                    unplayable.SetActive(true);
                }
            }
        }
    }

    public void returnToField()
    {
        transform.SetParent(logic.field.transform);
        transform.SetSiblingIndex(spotindex);
        Debug.Log("hmm");
    }

    public override void checktomove()
    {
        if (logic.turnM.turn != logic.sideNum || unplayable.activeInHierarchy || logic.turnM.dealing || cardState == State.grave || cardState == State.Waiting)
        {
            canmove = false;
            return;
        }
        canmove = true;

        /*  if (logic.turnM.turn != logic.sideNum || unplayable.activeInHierarchy || logic.turnM.dealing || )
          {
              canmove = false;
              return;
          }

          if ((cardState == State.field ||  cardState== State.inHand))
          {
              //if our turn, can move
              canmove = true;
          }
          else
          {
              canmove = false;
          }*/
    }

    int total;

    public int attackPower()
    {
        total = 0;

        for (int i = 0; i < boost.boosts.Count; i++)
        {
            if(boost.boosts[i].attBoost!= 0)
            {
                total += boost.boosts[i].attBoost;
            }
        }
        return currentAtt + total;
    }

public int defencePower()
    {
        total = 0;

        for (int i = 0; i < boost.boosts.Count; i++)
        {
            if (boost.boosts[i].defBoost!= 0)
            {
                total += boost.boosts[i].defBoost;
            }
        }
        return currentDef + total;
    }
}
[System.Serializable]
public class monsterAttributes
{
    [System.NonSerialized]
    public MonsterCard mc;


    public bool immune;


    public bool canPoison()
    {
        if (immune || Poisoned)
        {
            return false;
        }
        else
            return true;
    }

    public bool Poisoned { get; set; }

    public void checkState()
    {
        if (Poisoned)
        {
            mc.Damage(-1);
        }
    }

    internal void resetStates()
    {
        if (Poisoned)
            Poisoned = false;
    }
}