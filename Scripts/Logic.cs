using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Logic : MonoBehaviour
{
    [System.NonSerialized]
   public List<RaycastResult> results = new List<RaycastResult>();

    public int sideNum;

    internal void hideHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].backside.SetActive(true);
        }

        for (int i = 0; i < fieldMonsters.Count; i++)
        {
            fieldMonsters[i].unplayable.SetActive(false);
        }
    }

    public int lifepoints;

    public bool canmove;

    public int monsterswecanplay = 1;


    public int monstersplayed;

    int monstersplayable;

    public Field field;

    public List<Card> fieldMonsters;

    public List<Card> deck = new List<Card>();

    public List<Card> hand = new List<Card>();

    public TurnManager turnM;

    public List<Card> cardsichose;

    GameObject obj;

    public Transform deckPos;

    public Transform gravepos;

    public List<Card> grave;

    private void Awake()
    {
        turnM = transform.root.GetComponent<TurnManager>();

        //assign deck using game manager etc

        field.l = this;

        for (int i = 0; i < 20; i++)
        {
            //       obj=   (GameObject)Instantiate(cardsichose[0].gameObject, transform.position, Quaternion.identity, transform);

            obj = (GameObject)Instantiate(cardsichose[i].gameObject, transform.position, Quaternion.identity, deckPos);

            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);

            cardref = obj.GetComponent<Card>();

            cardref.logic = this;
            
        deck.Add(cardref);

            cardref.cardState = State.inDeck;

            

            //activate its backside
           // deck[i].transform.GetChild(deck[i].transform.childCount - 1);
        }
        //deal 5 cards to hands through turn manager?
        StartCoroutine(dealfirstcards());

        //adjust hand spacing
     //   adjustHandSpacing();
    }
    int dc;
    int dcm;

    public IEnumerator dealfirstcards()
    {
        turnM.dealing = true;
       dc = deck.Count - 1;
      dcm = deck.Count - 6;
        for (int i =dc ; i > dcm; i--)
        {
            while (true)
            {
                deck[i].transform.position = new Vector3(deck[i].transform.position.x + 10f, deck[i].transform.position.y);

                if (hand.Count > 0)
                {
                    if (deck[i].transform.position.x > transform.GetChild(0).transform.position.x-105f)
                    {
                        break;
                    }

                }
                else
                {
                    if (deck[i].transform.position.x > transform.position.x)
                    {
                        break;
                    }
                }


                yield return null;
            }
            deck[i].transform.SetParent(transform);
            deck[i].transform.SetAsFirstSibling();
            hand.Add(deck[i]);
            deck[i].transform.localScale = Vector3.one;
            deck[i].backside.transform.localScale = Vector3.one;
            deck.Remove(deck[i]);
        }

        adjustHandSpacing();


        if (sideNum == 0)
        {
        turnM.dealing = false;
            turnM.nextTurn();
            turnM.popupscreen.gameObject.SetActive(true);
        }
    }

    public void flipHandCards()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].unplayable != null)
                hand[i].unplayable.SetActive(false);

            hand[i].backside.SetActive(false);
            hand[i].cardState = State.inHand;
        }
    }

    MonsterCard mc;

    public void updateMonsterTurns()
    {
        //first reset amount of monsters we can play this turn since some cards can increase it :)

        monsterswecanplay = 1;

        //need some kind of poison check and a poison indicator for cards

        //this only poisons the player whos turn it is **** :)

        for (int i = 0; i < fieldMonsters.Count; i++)
        {
             mc = (MonsterCard)fieldMonsters[i];
            if (mc)
            {
                mc.attributes.checkState();

            }
        }


        monstersplayed = 0;
        //update monsters spawn time and reset number of attack
        for (int i = 0; i < fieldMonsters.Count; i++)
        {
            mc = (MonsterCard)fieldMonsters[i];  
            if(mc)
            {
                if(mc.cardState==State.Waiting)
                mc.nextTUrn();
                else
                {
                    mc.numofattacks = 0;
                    mc.transform.GetChild(4).gameObject.SetActive(false);
                }
            }
        }
    }

    int zero;

    public void adjustHandSpacing()
    {
        if (hand.Count >= 7)
        {
            transform.GetComponent<GridLayoutGroup>().spacing = new Vector2(-((180 * (hand.Count)) - 1400) / (float)(hand.Count - 1), 0);
        }
        else
        {

            zero = 0;
            for (int i = 0; i < hand.Count-1; i++)
            {
                zero += 5;
            }

            transform.GetComponent<GridLayoutGroup>().spacing = new Vector2(30 + zero, 0);
        }
    }

    internal bool containsHand(MonsterCard monsterCard)
    {
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log(results[i].gameObject.name);
            if (results[i].gameObject.transform == transform)
            {
                return true;
            }
        }
        return false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(dealACardFacingUp());
    }



    public IEnumerator dealACardFacingDown()
    {
        while (true)
        {
            deck[deck.Count-1].transform.position = new Vector3(deck[deck.Count - 1].transform.position.x + 10f, deck[deck.Count - 1].transform.position.y);

            if (hand.Count > 0)
            {
                if (deck[deck.Count - 1].transform.position.x > transform.GetChild(0).transform.position.x - 105f)
                {
                    break;
                }

            }
            else
            {
                if (deck[deck.Count - 1].transform.position.x > transform.position.x)
                {
                    break;
                }
            }


            yield return null;
        }
        deck[deck.Count - 1].transform.SetParent(transform);
        deck[deck.Count - 1].transform.SetAsFirstSibling();
        hand.Add(deck[deck.Count - 1]);
        deck[deck.Count - 1].transform.localScale = Vector3.one;
        deck[deck.Count - 1].backside.transform.localScale = Vector3.one;
        deck.Remove(deck[deck.Count-1]);
    }

    public IEnumerator dealACardFacingUp()
    {
        if (deck.Count > 1)
        {
            turnM.dealing = true;

            for (int i = 0; i < 1; i++)
            {
                while (true)
                {
                    if (deck[deck.Count - 1].transform.localScale.x < 1f)
                    {
                        deck[deck.Count - 1].transform.localScale = new Vector3(deck[deck.Count - 1].transform.localScale.x + 0.025f, 1);
                        if (deck[deck.Count - 1].backside.activeInHierarchy && deck[deck.Count - 1].transform.localScale.x >= 0f)
                        {
                            deck[deck.Count - 1].backside.SetActive(false);
                        }
                        deck[deck.Count - 1].transform.position = new Vector3(deck[deck.Count - 1].transform.position.x + 3f, deck[deck.Count - 1].transform.position.y);
                    }
                    else
                    {
                        if (deck[deck.Count - 1].transform.localScale.x > 1)
                            deck[deck.Count - 1].transform.localScale = new Vector3(1f, 1);

                        deck[deck.Count - 1].transform.position = new Vector3(deck[deck.Count - 1].transform.position.x + 10f, deck[deck.Count - 1].transform.position.y);

                        if (hand.Count > 0)
                        {
                            //was 105
                            if (deck[deck.Count - 1].transform.position.x > transform.GetChild(0).transform.position.x - 135f)
                            {
                                break;
                            }

                        }
                        else
                        {
                            if (deck[deck.Count - 1].transform.position.x > transform.position.x)
                            {
                                break;
                            }
                        }
                    }



                    yield return null;
                }
                deck[deck.Count - 1].cardState = State.inHand;
                deck[deck.Count - 1].transform.SetParent(transform);
                deck[deck.Count - 1].transform.SetAsFirstSibling();
                hand.Add(deck[deck.Count - 1]);
                deck.Remove(deck[deck.Count - 1]);
            }
            adjustHandSpacing();
            turnM.dealing = false;

            if (hand.Count < 1)
            {
                //end game
            }
        }
        else
        {
            //end game
        }

    }

    public void adjustFieldSpacing()
    {
        if (fieldMonsters.Count >= 7)
        {
           field.transform.GetComponent<GridLayoutGroup>().spacing = new Vector2(-((180 * (fieldMonsters.Count)) - 1400) / (float)(fieldMonsters.Count - 1), 0);
        }
        else

        {
            field.transform.GetComponent<GridLayoutGroup>().spacing = new Vector2(30, 0);
        }
    }

    public bool containsfield(Card card)
    {
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log(results[i].gameObject.name);
            if (results[i].gameObject.name == "Field " + sideNum)
            {
               card.transform.SetParent(results[i].gameObject.transform);
                return true;
            }
        }
        return false;
    }

    public int returnEnemyNum()
    {
        if (sideNum == 0)
        {
            return 1;
        }
        else
            return 0;
    }

    public bool containsenemyField(Card card)
    {
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.name == "Field " + returnEnemyNum().ToString())
            {
                if (results[i].gameObject.transform.GetComponent<Field>().l.fieldMonsters.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
 Card cardref;

    [System.NonSerialized]
   public MonsterCard mcardref;

    public bool containsEnemymonster(Card card)
    {
        for (int i = 0; i < results.Count; i++)
        {
            if(results[i].gameObject.tag=="Monster")
            {
                mcardref = results[i].gameObject.GetComponent<MonsterCard>();

             if (  mcardref.logic !=card.logic && (mcardref.cardState==State.Waiting || mcardref.cardState==State.field))
                {
                    //attack
                return true;
                }
            }
        }
        return false;
    }

    public bool containsOurmonster(Card card)
    {
        for (int i = 0; i < results.Count; i++)
        {
            if(results[i].gameObject.tag=="Monster")
            {
                mcardref = results[i].gameObject.GetComponent<MonsterCard>();

             if (  mcardref.logic ==card.logic && (mcardref.cardState==State.Waiting || mcardref.cardState==State.field))
                {
                    //attack
                return true;
                }
            }
        }
        return false;
    }
}
