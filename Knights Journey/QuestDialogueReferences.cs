using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;

public class QuestDialogueReferences : MonoBehaviour {

    public npcData myData;

    public QuestHolder questWereOn;

    public void incrementMyData()
    {
        myData.stepNum++;
        questWereOn.textIndex++;
    }

    public void incrementWithoutTextIndex()
    {
        myData.stepNum++;
    }

    /* public void checkStepNumber(int num)
     {
         if (questWereOn.stepNum == num)
         {
             VD.SetNode(questWereOn.nodeList[questWereOn.stepNum]);
         }
         else
         {
             VD.SetNode(questWereOn.alternateNodeList[questWereOn.stepNum]);
         }
     }*/

    public void setOurEndsData()
    {
        if (questWereOn.completed)
        {
            //VD.assigned.overrideStartNode = questWereOn.endNode;
            VD.SetNode(myData.endNode);
            return;
        }

        if (questWereOn.started)
        {
            VD.SetNode(myData.nodeList[myData.stepNum]);
            //using holder step number and steplist..
        }
        else
        {
            //  VD.assigned.overrideStartNode = holder.startNode;
            VD.SetNode(myData.alternateNodeList[0]);
            //VD.SetNode);
        }
    }

    public void checkStepNumberFromNpc(int num)
    {
        if (myData.stepNum >= num)
        {
            VD.SetNode(questWereOn.nodeList[myData.stepNum]);
        }
        else
        {
            VD.SetNode(questWereOn.alternateNodeList[myData.stepNum]);
        }
    }
    public void checkStepNumberFromPlayerBased(int num)
    {
        if (myData.stepNum >= num)
        {
            VD.SetNode(questWereOn.nodeList[questWereOn.stepNum+1]);
        }
        else
        {
            VD.SetNode(questWereOn.alternateNodeList[questWereOn.stepNum]);
        }
    }


    public void checkStepNumberFromNpcButMyLists(int num)
    {
        if (myData.stepNum >= num)
        {
            VD.SetNode(myData.nodeList[myData.stepNum]);
        }
        else
        {
            VD.SetNode(myData.alternateNodeList[myData.stepNum]);
        }
    }

    public void checkStepNumberFromNpcToPlayer(int num)
    {
        if (questWereOn.stepNum >= num)
        {
            VD.SetNode(myData.nodeList[myData.stepNum]);
        }
        else
        {
            VD.SetNode(myData.alternateNodeList[myData.stepNum]);
        }
    }



    /*    public saveSteps stepsToSave;

        public void checkSideStepQuest(int index)
        {
            if (stepsToSave.details[index].quest.holder.started)
            {
                VD.SetNode(stepsToSave.details[index].nodeList[stepsToSave.details[index].stepNum]);
            }
        }*/

}
/*[System.Serializable]
public class  questDetails
{
    public int stepNum;
    public List<int> alternativeNodeList;
    public List<int> nodeList;
    public Quest quest;
}
[System.Serializable]
public class saveSteps
{
    public List<questDetails> details;
}*/