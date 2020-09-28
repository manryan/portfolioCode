using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class Journal : MonoBehaviour {

    public GameObject activeText;

    public ScrollRect rect;

    public Image questNotifier;

    public Text notifier;

    public GameObject rewardPanel;

    public Text rewardText;

    public List<TextMeshProUGUI> mesh;

    public List<Button> buttons;

    public Text buttonText;

    public string[] words;

    public void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].onClick.Invoke();
            buttonText = buttons[i].transform.GetChild(0).GetComponent<Text>();
            buttonText.text = id.instance.questsActive[i].questName;
        }
        activeText.SetActive(false);
        activeText = mesh[0].gameObject;
        mesh[0].gameObject.SetActive(true);
        rect.content = mesh[0].rectTransform;
    }

    public void splitUp(int num)
    {
        words= id.instance.questsActive[num].mytext.text.Split(
   new[] { "\r\n", "\r", "\n" },
   System.StringSplitOptions.None);

        buttonText = buttons[num].transform.GetChild(0).GetComponent<Text>() ;

        if (id.instance.questsActive[num].reqs.Count > 0)
        {
            id.instance.questsActive[num].reqString = "You Have To Complete ";
            //  mesh.text = holder.reqString;
            for (int i = 0; i < id.instance.questsActive[num].reqs.Count; i++)
            {
                if (i == 0)
                {
                    if (id.instance.questsActive[num].reqs[i].completed == false)
                    {
                        id.instance.questsActive[num].reqString += id.instance.questsActive[num].reqs[i].questName;
                        // mesh.text += holder.reqString;
                    }
                    else
                    {
                        id.instance.questsActive[num].reqString += "<font=" + "BPtypewriteStrikethrough SDF" + ">" + id.instance.questsActive[num].reqs[i].questName + "</font>";
                        //   mesh.text += holder.reqString;
                    }
                    if (i + 1 >= id.instance.questsActive[num].reqs.Count)
                        id.instance.questsActive[num].reqString += ".";
                }
                else
                {
                    if (id.instance.questsActive[num].reqs[i].completed == false)
                    {
                        if (i + 1 >= id.instance.questsActive[num].reqs.Count)
                            id.instance.questsActive[num].reqString += ", and " + id.instance.questsActive[num].reqs[i].questName;
                        else
                            id.instance.questsActive[num].reqString += ", " + id.instance.questsActive[num].reqs[i].questName;
                        //     mesh.text += holder.reqString;
                    }
                    else
                    {
                        if (i + 1 >= id.instance.questsActive[num].reqs.Count)
                            id.instance.questsActive[num].reqString += ", and " + "<font=" + "BPtypewriteStrikethrough SDF" + ">" + id.instance.questsActive[num].reqs[i].questName + "</font>";
                        else
                            id.instance.questsActive[num].reqString += ", " + "<font=" + "BPtypewriteStrikethrough SDF" + ">" + id.instance.questsActive[num].reqs[i].questName + "</font>";
                    }
                    if (i + 1 >= id.instance.questsActive[num].reqs.Count)
                        id.instance.questsActive[num].reqString += ".";
                }
            }
        }
        else
        {
            id.instance.questsActive[num].reqString = "There are no Prequisites for this quest :)" + "\n";
            //  mesh.text = "<size=40>" + holder.questName + "</size>" + "\n" + holder.reqString;
        }
        loadRightInfo(num);

    }

    public void loadRightInfo(int num)
    {
        //  mesh.text = "<size=40>" + holder.questName + "</size>" + "\n" + holder.reqString;
        if (id.instance.questsActive[num].completed)
        {
            mesh[num].text = "";
            buttonText.color = Color.green;
            for (int i = 0; i < words.Length; i++)
            {
                mesh[num].text += "\n";
                mesh[num].text += words[i];
            }

            mesh[num].text = "<size=40>" + id.instance.questsActive[num].questName + "</size>" + "\n" + "<font=" + "BPtypewriteStrikethrough SDF" + ">" + id.instance.questsActive[num].reqString + id.instance.questsActive[num].startQuest + mesh[num].text;

            return;
        }
        if (id.instance.questsActive[num].started)
        {
            mesh[num].text = "<size=40>" + id.instance.questsActive[num].questName + "</size>" + "\n" + id.instance.questsActive[num].reqString + id.instance.questsActive[num].startQuest;
            buttonText.color = Color.yellow;
            for (int i = 0; i < id.instance.questsActive[num].steplist[id.instance.questsActive[num].textIndex]; i++)
            {
                mesh[num].text += "\n";
                mesh[num].text += words[i];
            }
        }
        else
        {
            buttonText.color = Color.red;
            mesh[num].text = "<size=40>" + id.instance.questsActive[num].questName + "</size>" + "\n" + id.instance.questsActive[num].reqString + id.instance.questsActive[num].startQuest;
        }
    }
    public void loadPage(int num)
    {
        activeText.SetActive(false);
        activeText = mesh[num].gameObject;
        mesh[num].gameObject.SetActive(true);
        rect.content = mesh[num].rectTransform;
        splitUp(num);
    }

    public void loadMoreTextToJournalPage(int num)
    {
        for (int i =id.instance.questsActive[num].steplist[id.instance.questsActive[num].textIndex - 1]; i < id.instance.questsActive[num].steplist[id.instance.questsActive[num].textIndex]; i++)
        {
            mesh[num].text += " \n ";
            mesh[num].text += words[i];
        }
    }
}