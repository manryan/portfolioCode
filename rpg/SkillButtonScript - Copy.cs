using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonScript : MonoBehaviour
{

    public SkillsBase skill;

    public List<Button> outgoing = new List<Button>();

    public List<Button> ingoing = new List<Button>();

    public bool preReq;

    public GameObject description;

    public TextMeshProUGUI  descriptionText;

    public string status;

    public skilltreeActivator tree;

    public void Start()
    {
        displayDescription();
    }

    public void addSkillToUnlock(SkillsBase skill)
    {
        Debug.Log("clicked");
        //if skill points > 0
        if (tree.player.Checkpts(tree.skills.Count) && tree.skills.Contains(skill) == false)
        {
            GetComponent<Image>().sprite = skill.SkillIconUnlocked;
            tree.skills.Add(skill);
            description.SetActive(false);
            //GetComponent<Button>().onClick.AddListener(() => removeSkillToUnlock(skill));
            tree.selection();
        }
        //if(skill.)
    }

    public void removeSkillToUnlock(SkillsBase skill)
    {
        Debug.Log("clicked");
        //if skill points > 0
        if (tree.skills.Contains(skill))
        {
            GetComponent<Image>().sprite = skill.SkillIcon;
            tree.skills.Remove(skill);
            GetComponent<Button>().onClick.AddListener(() => addSkillToUnlock(skill));
        }
        //if(skill.)
    }

    public void OnEnable()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0);

       // description = GameObject.Find("Skill Description Box");

        //descriptionText = description.transform.GetChild(0).gameObject.GetComponent<Text>();

      //  transform.GetChild(0).GetComponent<Text>().text = skill.name;
    }

    public void displayDescription()
    {

        //if (!tree.rotating)
        //{
            description.SetActive(true);
            //description.transform.position = new Vector3(transform.position.x, transform.position.y + 500f);
            if (skill.Type == 1)
                status = "<u>Active</u>";
            else
                status = "<u>Passive</u>";

            description.transform.GetChild(1).GetComponent<Image>().sprite = skill.SkillIcon;
            if (ingoing.Count < 1)
                descriptionText.text = "<align=center>" + "<b>" + skill.SkillName + "</b>" + "\n" + "<size=75%>" + status +  "\n"  + "\n" + skill.SkillDescription + "</size>" + "</align>";
            else
            {
                string listNames = "<align=center>" + "<b>" + skill.SkillName + "</b>" + "\n" + "<size=75%>" + status + "\n"    + "\n" +  " Prerequisites: ";

                for (int i = 0; i < ingoing.Count; i++)
                {
                    listNames +=  ingoing[i].GetComponent<SkillButtonScript>().skill.SkillName;
                if (i != ingoing.Count-1)
                    listNames += ", ";

                if (i == ingoing.Count-1)
                    listNames += ".";
                }
            listNames += "\n" + skill.SkillDescription;

                descriptionText.text = listNames + "</size>" + "</align>";
            }
        //}
    }

    //public void hideDescription()
    //{
    //    description.SetActive(false);
    //}
}