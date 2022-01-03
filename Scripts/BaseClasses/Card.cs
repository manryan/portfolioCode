using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum State {inHand=1, inDeck, Waiting, field, grave };

public abstract class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string cardname;

    public State cardState;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;


    bool holding;

   protected int spotindex;

    [System.NonSerialized]
   public Logic logic;

    public GameObject backside;
    [System.NonSerialized]
    public GameObject unplayable;

   public virtual void play() {  }

    public virtual void checktomove()   {
        canmove = false;

        if (logic.turnM.turn != logic.sideNum || logic.turnM.dealing)
        {
            return;
        }
    }

    public void movedCard()
    {
        logic.deck.Remove(this);

        logic.adjustHandSpacing();
        logic.adjustFieldSpacing();
    }


    [System.NonSerialized]
    public bool canmove = true;


    public virtual void Awake()
    {

        //cardState = State.inHand;



        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = transform.root.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

      //  logic = transform.parent.GetComponent<Logic>();

      //  transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
         //if its our turn, and we can move our card
        checktomove();

        if (canmove && !logic.turnM.dealing)
        {
            holding = true;

            spotindex = transform.GetSiblingIndex();
            //vvvv done for rendering :)
            transform.SetParent(transform.root);

            StartCoroutine(ctrlcard());

            Clicked();
        }
    }

    public virtual void Clicked()  {  }

    public virtual void sendToGrave()
    {

        cardState = State.grave;
        transform.SetParent(logic.gravepos);
        logic.grave.Add(this);
        transform.position = logic.gravepos.position;
        GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
    }

    IEnumerator ctrlcard()
    {
        while(holding)
        {
            transform.position = Input.mousePosition;
            yield return null;
        }
    }


    public void returnToHand()
    {
        transform.SetParent(logic.transform);
        transform.SetSiblingIndex(spotindex);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canmove)
        {
            holding = false;

            transform.SetParent(null);

            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            logic.results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, logic.results);

            play();
        }

     //   originalparent.cast(this);

        //if not hovering over playing area, or another monster

        //transform.parent = null;


        //    LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        //otherwise, play this card etc. according to what u landed it on.



        //if(monstercard)

        //else if trap card - place face down

        //else if magic card, apply bonus etc.
    }

  

}
