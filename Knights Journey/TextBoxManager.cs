using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBoxManager : MonoBehaviour {


    public RectTransform vLayout;

    public TextMeshProUGUI tmp;

    public RectTransform size;

    public Button controller;

    public Text controllerText;

    public Scrollbar sb;

    public ScrollRect scrollrectholder;

    public void reduce()
    {
        size.sizeDelta = new Vector2(size.sizeDelta.x, 36);
        controller.onClick.RemoveAllListeners();
        controller.onClick.AddListener(increase);
        controllerText.text = "Expand";
        scrollrectholder.verticalScrollbar = null;
        sb.gameObject.SetActive(false);
        scrollrectholder.verticalNormalizedPosition = 0;
        // vLayout.position = new Vector2(vLayout.position.x, vLayout.sizeDelta.y + 50F);
    }

    public void increase()
    {
        size.sizeDelta = new Vector2(size.sizeDelta.x, 200);
        controller.onClick.RemoveAllListeners();
        controller.onClick.AddListener(reduce);
        controllerText.text = "Reduce";
        scrollrectholder.verticalScrollbar = sb;
        sb.gameObject.SetActive(true);
        scrollrectholder.verticalNormalizedPosition = 0;
    }

    public void Awake()
    {
        tmp = null;
        reduce();
        scrollrectholder.verticalScrollbar = null;
        sb.gameObject.SetActive(false);

        StartCoroutine(setTo("Welcome To My Game :) \n keep typing random garbage for testing purposes"));
    }


    public IEnumerator setTo(string text)
    {
        for (int i = 0; i < vLayout.childCount; i++)
        {
            if(!vLayout.GetChild(i).gameObject.activeInHierarchy)
            {
                tmp = vLayout.GetChild(i).GetComponent<TextMeshProUGUI>();
                tmp.gameObject.SetActive(true);
                break;
            }
        }
        if(!tmp)
        {
            tmp = vLayout.GetChild(0).GetComponent<TextMeshProUGUI>();
            tmp.gameObject.SetActive(true);
            tmp.transform.SetAsLastSibling();
        }

        tmp.text = text;
     //   tmp.transform.parent = tmp.transform.root;
        tmp.GetComponent<ContentSizeFitter>().enabled = true;
        yield return null;
        tmp.GetComponent<LayoutElement>().minHeight = tmp.rectTransform.sizeDelta.y;
        tmp.GetComponent<ContentSizeFitter>().enabled = false;
        yield return null;
        //vLayout.position = new Vector2(vLayout.position.x, vLayout.sizeDelta.y+100f);
        //sb.value = 0;
        scrollrectholder.verticalNormalizedPosition = 0;
        tmp = null;
    }

    public bool checkIfTheLastWasTheSame(string stringToCheck)
    {
        for (int i = 0; i < vLayout.childCount; i++)
        {
            if (!vLayout.GetChild(i).gameObject.activeInHierarchy)
            {
                if (vLayout.GetChild(i-1).GetComponent<TextMeshProUGUI>().text == stringToCheck)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        if (vLayout.GetChild(vLayout.childCount - 1).GetComponent<TextMeshProUGUI>().text == stringToCheck)
        {
            return true;
        }
        else
            return false;
    }
}
