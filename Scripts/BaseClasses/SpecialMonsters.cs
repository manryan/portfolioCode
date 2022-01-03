using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SpecialMonsters : MonsterCard
{
    public string description;

    public GameObject descriptionText;

    public UnityEvent death;

    public void DrawCard()
    {
        //how to determine if to deal a card facing up or down?
        if (logic.turnM.turn!=logic.sideNum)
        logic.StartCoroutine(logic.dealACardFacingDown());
        else
            logic.StartCoroutine(logic.dealACardFacingUp());
    }

    void Start()
    {
        //set ui text to description
        descriptionText = transform.GetChild(0).GetChild(0).GetChild(2).gameObject;

        descriptionText.transform.GetChild(0).GetComponent<Text>().text = description;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        //enable ui if we arent holding down mouse buttonW
        descriptionText.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        //enable ui if we arent holding down mouse buttonW
        if (!Input.GetMouseButton(0) || !canmove)
            descriptionText.SetActive(false);
    }

    public override void Clicked()
    {
        descriptionText.SetActive(false);
    }

    public override void sendToGrave()
    {
        base.sendToGrave();
        death.Invoke();
        
    }
}
