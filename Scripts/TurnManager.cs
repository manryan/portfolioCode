using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public int turn;

    public GameObject popupscreen;

    public List<Logic> players;

    public bool dealing;

    public void nextTurn()
    {
        if (!dealing)
        {

            if (turn != -1)
                players[turn].hideHand();

            turn++;

            if (turn > 1)
            {
                turn = 0;
                players[turn].updateMonsterTurns();

            }
            else
            {
                players[turn].updateMonsterTurns();
            }
            //open pop up screen for current player
            popupscreen.SetActive(true);
            popupscreen.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Ready Up Player " + (turn + 1).ToString();



        }
    }

    public void ReadyUp()
    {
        popupscreen.gameObject.SetActive(false);

        players[turn].flipHandCards();

        //deal cards to that player
        players[turn].StartCoroutine(players[turn].dealACardFacingUp());
    }


    private void Awake()
    {
        
        //open pop up screen for player one;
        turn = -1;
    }


    IEnumerator waitbeforeDealDeck()
    {
        return null;
    }

    public void dealDeck()
    {

    }
}
