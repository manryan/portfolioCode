using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using VIDE_Data;
using TMPro;
using UnityEngine.Events;

public class Quest : MonoBehaviour {

    public QuestHolder holder;

    public questReward reward = new questReward();

    public TextMeshProUGUI mesh;

    public Text buttonText;

    public Journal journal;

    public string[] words;

    public string path ="";

    public List<Quest> questsRequiringThisQuest = new List<Quest>();

    public List<GameObject> enemiesToKill;

    public bool moody;

    public List<QuestDialogueReferences> sideStepNpcs;

    public UnityEvent finishedQuestEvent;

    public UnityEvent onFinish;

    //public List<UnityEvent> eventStep = new List<UnityEvent>();

    /*    [System.Serializable]
    public class MyIntEvent : UnityEvent<int>
    {
    }

    public MyIntEvent eventclass;*/

    public void assignTheNpcs()
    {
        if(sideStepNpcs.Count>0)
        for (int i = 0; i < sideStepNpcs.Count; i++)
        {
            sideStepNpcs[i].myData = holder.npcDataStorage[i];
                sideStepNpcs[i].questWereOn = holder;
        }
    }

    public void triggered()
    {
        //Debug.Log("")
        PlayerPrefs.SetString(gameObject.name + "TimeSave", DateTime.Now.ToString());

    }


    public void checkStepNumber(int num)
    {
        if(holder.stepNum == num)
        {
            VD.SetNode(holder.nodeList[holder.stepNum]);
        }
        else
        {
            VD.SetNode(holder.alternateNodeList[holder.stepNum]);
        }
    }

    public void checkTimeSinceTriggered()
    {
        DateTime oldTime = DateTime.Parse(PlayerPrefs.GetString(gameObject.name));
        if(subtractit(oldTime))
        {
            //VD.SetNode()
        }
        else
        {

        }
    }

    public bool subtractit(DateTime date)
    {
        TimeSpan newtime = DateTime.Now.Subtract(date);
        if (newtime.TotalMinutes >= 5f)
        {
            return true;
        }
        return false;
    }

    public void loadPage()
    {
        journal.activeText.SetActive(false);
        journal.activeText = mesh.gameObject;
        mesh.gameObject.SetActive(true);
        journal.rect.content = mesh.rectTransform;
    }

/*    public void check(int num)
    {
        if (GameManager.instance.player.Check(itemsNeeded[num]))
        {

          //  VD.assigned.overrideStartNode = 13;
            VD.SetNode(holder.nodeList[holder.stepNum+1]);
        }
        else
        {
            VD.SetNode(holder.alternateNodeList[holder.stepNum]);
            //  VD.SetNode(11);
        }
    }*/

    public List<questItemsNeeded> itemsNeeded;

    public void checkIfEnough(int num)
    {
        for (int i = 0; i < itemsNeeded[num].itemsNeeded.Count; i++)
        {
            if (GameManager.instance.player.checkIfEnough(itemsNeeded[num].itemsNeeded[i], itemsNeeded[num].amountNeeded[i]))
            {
                continue;
            }
            else
            {
               VD.SetNode(holder.alternateNodeList[holder.stepNum]);
               return;
            }
        }
        VD.SetNode(holder.nodeList[holder.stepNum + 1]);
    }

   // public List<Item> itemsNeeded;

 //   public Item itemNeeded;

   public void takeAway(int num)
    {
        for (int i = 0; i < itemsNeeded[num].itemsNeeded.Count; i++)
            GameManager.instance.player.TakeAway(itemsNeeded[num].itemsNeeded[i], itemsNeeded[num].amountNeeded[i]);
        incrementStepNum();
    }

    public bool Incomplete(int num)
    {
        for (int i = 0; i < num; i++)
        {
            if(!enemiesToKill[num].activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    public void checkIfKilled(int index)
    {
        if(Incomplete(index -1)==true)
        {
            VD.SetNode(holder.alternateNodeList[holder.stepNum]);
        }
        else
        {
            holder.textIndex++;
            holder.stepNum++;
            VD.SetNode(holder.nodeList[holder.stepNum]);
        }
    }

   
    public void Start()
    {
      string file = "Quests/"+  id.instance.saveIndex +"/"+path + id.instance.saveIndex;
        holder = (QuestHolder)Resources.Load(file);
        //buttonText.text = holder.questName;
     //   mesh.text = "<size=40>" + holder.questName + "</size";

      /*  words = holder.mytext.text.Split(
   new[] { "\r\n", "\r", "\n" },
   System.StringSplitOptions.None);*/

     //   splitUp();

        assignTheNpcs();

        if (finishedQuestEvent != null && holder.completed)
        {
            finishedQuestEvent.Invoke();
        }

        //     eventclass.Invoke(1);
    }

    public void checkifeventplayedoff()
    {
        Debug.Log("fired off");
    }

  /* public void splitUp()
    {
        if (holder.reqs.Count > 0)
        {
            holder.reqString = "You Have To Complete ";
            //  mesh.text = holder.reqString;
            for (int i = 0; i < holder.reqs.Count; i++)
            {
                if (i == 0)
                {
                    if (holder.reqs[i].completed == false)
                    {
                        holder.reqString += holder.reqs[i].questName;
                        // mesh.text += holder.reqString;
                    }
                    else
                    {
                        holder.reqString += "<font=" + "BPtypewriteStrikethrough SDF" + ">" + holder.reqs[i].questName + "</font>";
                        //   mesh.text += holder.reqString;
                    }
                    if (i + 1 >= holder.reqs.Count)
                        holder.reqString += ".";
                }
                else
                {
                    if (holder.reqs[i].completed == false)
                    {
                        if (i + 1 >= holder.reqs.Count)
                            holder.reqString += ", and " + holder.reqs[i].questName;
                        else
                            holder.reqString += ", " + holder.reqs[i].questName;
                        //     mesh.text += holder.reqString;
                    }
                    else
                    {
                        if (i + 1 >= holder.reqs.Count)
                            holder.reqString += ", and " + "<font=" + "BPtypewriteStrikethrough SDF" + ">" + holder.reqs[i].questName + "</font>";
                        else
                            holder.reqString += ", " + "<font=" + "BPtypewriteStrikethrough SDF" + ">" + holder.reqs[i].questName + "</font>";
                    }
                    if (i + 1 >= holder.reqs.Count)
                        holder.reqString += ".";
                }
            }
        }
        else
        {
            holder.reqString = "There are no Prequisites for this quest :)" + "\n";
            //  mesh.text = "<size=40>" + holder.questName + "</size>" + "\n" + holder.reqString;
        }
        loadRightInfo();

    }

    public void loadRightInfo()
    {
      //  mesh.text = "<size=40>" + holder.questName + "</size>" + "\n" + holder.reqString;
        if (holder.completed)
        {
            mesh.text = "";
            buttonText.color = Color.green;
            for (int i = 0; i < words.Length; i++)
            {
                mesh.text += "\n";
                mesh.text += words[i];
            }

            mesh.text = "<size=40>"+ holder.questName + "</size>" + "\n"  +  "<font=" + "BPtypewriteStrikethrough SDF" + ">" + holder.reqString + holder.startQuest + mesh.text;

            return;
        }
        if (holder.started)
        {
            mesh.text = "<size=40>" + holder.questName + "</size>" + "\n" + holder.reqString + holder.startQuest;
            buttonText.color = Color.yellow;
            for (int i = 0; i < holder.steplist[holder.textIndex]; i++)
            {
                mesh.text += "\n";
                mesh.text += words[i];
            }
        }
        else
        {
            buttonText.color = Color.red;
            mesh.text = "<size=40>" + holder.questName + "</size>" + "\n" + holder.reqString + holder.startQuest;
        }
    }*/

    public bool checkReqs()
    {
        if(holder.reqs.Count>0)
        foreach(QuestHolder quest in holder.reqs)
        {
            if (quest.completed == false)
            {
                return false;

            }
        }
        return true;
    }

    public void checkQuest()
    {
        if (checkReqs())
        {
            if (holder.completed)
            {
              //  VD.assigned.overrideStartNode = holder.endNode;
                VD.SetNode(holder.endNode);
                return;
            }

            if (holder.started)
            {
             //   VD.assigned.overrideStartNode = holder.nodeList[holder.stepNum];
                VD.SetNode(holder.nodeList[holder.stepNum]);
                //using holder step number and steplist..
            }
            else
            {
                //  VD.assigned.overrideStartNode = holder.startNode;
                VD.SetNode(holder.startNode);
                //VD.SetNode);
            }
        }
        else
        {
            VD.SetNode(holder.reqsNotMetNode);
        }
    }

    public int questIndex;

    public void startQuest()
    {
        journal.questNotifier.gameObject.SetActive(true);
        journal.notifier.text = "Started " +holder.questName;
        StartCoroutine(fadeOut());
        holder.started = true;
        journal.splitUp(questIndex);
        /*    for (int i = 0; i < holder.steplist[holder.stepNum]; i++)
            { 
                mesh.text += "\n";
            mesh.text += words[i];
            }*/
        journal.buttonText = journal.buttons[questIndex].transform.GetChild(0).GetComponent<Text>();
        journal.buttonText.color = Color.yellow;
       // incrementStepNum();
        GameManager.instance.player.save();
        //   holder.lineNum = holder.steplist[holder.stepNum];
        //  button.transform.GetChild(0).GetComponent<Text>().color = Color.yellow;
        //  holder.stepNum = 1;
    }

    public void incrementStepNum()
    {
        holder.stepNum++;
        holder.textIndex++;
        journal.loadMoreTextToJournalPage(questIndex);
        GameManager.instance.player.save();
    }

    public void incrementStepNumWithoutTextOrSaving()
    {
        holder.stepNum++;
       // holder.textIndex++;
      //  loadMoreTextToJournalPage();
      //  GameManager.instance.player.save();
    }

 /*   public void loadMoreTextToJournalPage()
    {
        for (int i = holder.steplist[holder.textIndex - 1]; i < holder.steplist[holder.textIndex]; i++)
        {
            mesh.text += " \n ";
            mesh.text += words[i];
        }
    }*/

    public IEnumerator completeit()
    {
        GameManager.instance.player.moveDialogue();
        journal.questNotifier.gameObject.SetActive(true);
        journal.notifier.text = "Completed " + holder.questName;
        StartCoroutine(fadeOut());
        buttonText.color = Color.green;
        holder.started = false;
        holder.completed = true;
        /* for (int i = holder.steplist[holder.steplist.Length-1]; i < words.Length; i++)
         {
             mesh.text += "\n";
             mesh.text += words[i];
         }
         mesh.text = "<size=40>" + holder.questName + "</size>"+ "\n" + "<font=" + "BPtypewriteStrikethrough SDF" + ">" + mesh.text;*/
        // splitUp();
        journal.splitUp(questIndex);
        //VD.assigned.overrideStartNode = holder.endNode;
        foreach (Quest quest in questsRequiringThisQuest)
        {
            journal.splitUp(quest.questIndex);
        }

        checkReward();

        yield return null;

        if (onFinish != null)
        {
            onFinish.Invoke();
        }
        // openRewardPanel();

        GameManager.instance.player.save();
    }

    public void completeQuest()
    {
        StartCoroutine(completeit());
    }

    public void openRewardPanel()
    {
        journal.rewardPanel.SetActive(true);
        journal.rewardText.text = reward.rewardText;
        Time.timeScale = 0;
    }

    public IEnumerator fadeOut()
    {
        journal.questNotifier.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(1);
        var col = 1f;
        while (col > 0f)
        {
            col -= 0.05f;
            journal.questNotifier.color = new Color(0, 0, 0, col);
            yield return null;
        }
        journal.questNotifier.gameObject.SetActive(false);
    }

    public void checkReward()
    {
        reward.rewardText = "Congratulations, you have completed " + holder.questName + ". ";
        if (reward.rewardItems.Count>0)
        {
            reward.rewardText += "You have been rewarded: ";

            for (int i = 0; i < reward.rewardItems.Count; i++)
            {

               ItemPickup ip = reward.rewardItems[i].GetComponent<ItemPickup>();


                //if invo full drop under the player and save them after
                ip.count = reward.amountGiven[i];
                GameObject testObj = GameManager.instance.player.objPoolManager.getReward(reward.rewardItems[i], transform.position);
                testObj.GetComponent<ItemPickup>().count = reward.amountGiven[i];
                GameManager.instance.player.addToInvo(testObj);
            //    GameManager.instance.player.addToInvo(reward.rewardItems[i]);

                if (i!= 0)
                    reward.rewardText += "and ";

                reward.rewardText += ip.count + " " + ip.item.itemName;

                if (reward.rewardItems.Count>1 && i + 1 != reward.rewardItems.Count)
                    reward.rewardText += ", ";
                //otherwise add it to invo
            }
            reward.rewardText += ".";
        }
        if (reward.additionalReward.Length>0)
        {
            reward.rewardText += "\n" + "In addition, " + reward.additionalReward;
        }
    }

}
[System.Serializable]
public class questReward
{
    public List<GameObject> rewardItems = new List<GameObject>();

    public List<int> amountGiven = new List<int>();

    public string rewardText;

    public string additionalReward;
}
[System.Serializable]
public class questItemsNeeded
{
    public List<Item> itemsNeeded = new List<Item>();

    public List<int> amountNeeded = new List<int>();
    
}