using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;


public class SpellBoosts : MonoBehaviour
{


    public List<boost> boosts;

    public GameObject boostUI;

    private void Awake()
    {
        boostUI = transform.GetChild(transform.childCount - 1).gameObject;
    }

   public GameObject returnInactiveChild()
    {
        for (int i = 0; i < boostUI.transform.childCount-1; i++)
        {
            if(boostUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text =="")
            {
                return boostUI.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }

    public void clearBoostsAndUi()
    {
        if (boosts.Count > 0)
        {
            for (int i = 0; i < boostUI.transform.childCount; i++)
            {
                if (boostUI.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    boostUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "";
                    boostUI.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            boosts.Clear();
        }
    }

    public void removeBoostUi(SpellCard card)
    {
        for (int i = 0; i < boostUI.transform.childCount; i++)
        {
            if (boostUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text== card.cardname)
            {
                boostUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text ="";
                boostUI.transform.GetChild(i).gameObject.SetActive(false);
               boostUI.transform.GetChild(i).SetAsLastSibling();
                boosts.Remove(card.boostinfo);
                break;

            }
        }   
    }
    GameObject temp;

    public void addBoostUi(SpellCard card)
    {
        temp = returnInactiveChild();
        temp.GetComponent<Image>().color = card.transform.GetChild(0).GetComponent<Image>().color;
        temp.SetActive(true);
        temp.transform.GetChild(0).GetComponent<Text>().text = card.cardname;
    }
}


[System.Serializable]
public class boost
{


    public int attBoost;

    public int defBoost;

    public SpellCard spell;

}