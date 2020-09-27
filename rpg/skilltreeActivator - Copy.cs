using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class skilltreeActivator : MonoBehaviour {

    public GameObject skillscam;

    public MapSkills mapskill;

    public List<SkillsBase> skills = new List<SkillsBase>();

    public List<Button> buttons = new List<Button>();

    SkillsBase skill;

    public int skillPoints;

    public GameObject confirmationPanel;

    public Player player;

    public GameObject cam;

    public GameObject compass;

    public GameObject ragebar;

    public GameObject favourites;

    public GameObject confirmationText;

    public GameObject temple;

    public GameObject[] arrays = new GameObject[3];

    public List<skillType> skillPages = new List<skillType>();

    public SkillTreeRotate rotateTemple;

    public GameObject description;

    public GameObject rawimage;

    public GameObject header;

    public bool rotating;

    public Text ptsText;

    public IEnumerator loadButtons(skillType type)
    {
        //start coroutine for them and have them fade in

        foreach(Button button in type.skillbuttons)
        {
            button.gameObject.SetActive(true);
        }
//        Debug.Log("fuck my life");
        var col = 0f;

        while (col < 1f)
        {
            col += 0.15f;
            foreach (Button button in type.skillbuttons)
            {
                button.GetComponent<Image>().color = new Color(1, 1, 1, col);
            }
            yield return null;
        }
        rotateTemple.counterReset();
        rotating = false;
    }

    public IEnumerator unloadButtons(skillType type, int dir)
    {
        rotating = true;
        description.SetActive(false);

        // have them fade out
        //while alpha >0
        var col = 1f;

        while (col > 0f)
        {
            col -= 0.1f;
            foreach (Button button in type.skillbuttons)
            {
                button.GetComponent<Image>().color = new Color(1, 1, 1, col);
                Debug.Log(button);
            }
            yield return null;
        }
        foreach (Button button in type.skillbuttons)
        {
            button.gameObject.SetActive(false);
        }
        rotateTemple.StartCoroutine(rotateTemple.RotateMe(Vector3.up * 90 * dir, 0.8f, 90* dir));
    }

    [System.Serializable]
        public class skillType
    {
        public List<Button> skillbuttons;
    }

    public skillType frank = new skillType
    {
        skillbuttons = new List<Button>()
    };

    public skillType vamp = new skillType
    {
        skillbuttons = new List<Button>()
    };

    public skillType werewolf = new skillType
    {
        skillbuttons = new List<Button>()
    };

    public skillType wendingo = new skillType
    {
        skillbuttons = new List<Button>()
    };

    public void Awake()
    {
        header = GameObject.Find("UI/Canvas/Header");


        skillPages.Add(frank);

        skillPages.Add(vamp);

        skillPages.Add(wendingo);

        skillPages.Add(werewolf);

        cam = GameObject.Find("Camera");

        compass = GameObject.Find("CompassPlaceholder");

        ragebar = GameObject.Find("HealthAndRageBar");

        favourites = GameObject.Find("FavoriteInGameUI");

        ptsText = GameObject.Find("UI/Canvas/Header/SkillTree Raw Image/SkillPoints Text").GetComponent<Text>();

        confirmationText = GameObject.Find("UI/Canvas/Header/SkillTree Raw Image/confirmationpage/Image/ConfirmationText");

        mapskill = GameObject.Find("UI/Canvas/Header/SkillTree Raw Image/mapskills").GetComponent<MapSkills>();

        temple = GameObject.Find("SM_Temple");

        rotateTemple = temple.GetComponent<SkillTreeRotate>();

        arrays = new GameObject[] { compass, ragebar, favourites };

        rawimage = GameObject.Find("UI/Canvas/Header/SkillTree Raw Image");


     description = GameObject.Find("UI/Canvas/Header/SkillTree Raw Image/Skill Description Box");



       

        

        //header = GameObject.Find("Header");
    }

    private void Start()
    {
        foreach (skillType skilltype in skillPages)
        {
            foreach (Button skillbutton in skilltype.skillbuttons)
            {
                SkillButtonScript sbs = skillbutton.GetComponent<SkillButtonScript>();
                sbs.tree = this;
                sbs.description = description;
                sbs.descriptionText = description.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            }
        }

        rawimage.SetActive(false);

        transform.parent.gameObject.SetActive(false);
    }

    // Use this for initialization
    void OnEnable()
    {

        //header.SetActive(true);
       rawimage.SetActive(true);
        
       /* foreach (GameObject obj in arrays)
            obj.SetActive(false);*/

       // cam.SetActive(false);

        var Player = GameObject.Find("Player");

        player = Player.GetComponent<Player>();

        confirmationPanel = GameObject.Find("UI/Canvas/Header/SkillTree Raw Image/confirmationpage");

        confirmationPanel.SetActive(false);

        foreach (skillType skilltype in skillPages)
        {
            foreach (Button skillbutton in skilltype.skillbuttons)
            {
                SkillButtonScript sbs = skillbutton.GetComponent<SkillButtonScript>();
                if (sbs.skill != null)
                {
                    //Debug.Log(sbs);
                    if (sbs.skill.unLocked)
                        sbs.ingoing.Clear();
                    if (sbs.ingoing.Count > 0)
                        skillbutton.interactable = false;

                    if (sbs.skill.unLocked)
                    {
                        skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIconUnlocked;

                        skillbutton.interactable = false;
                        if (sbs.skill.Type == 1)
                        {
                            skillbutton.interactable = true;
                            skillbutton.onClick.RemoveAllListeners();
                            skillbutton.onClick.AddListener(() => setSkillMapping(sbs.skill));
                        }
                    }
                    else
                    {

                        skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIcon;

                        if (sbs.ingoing.Count == 0)
                        {
                            skillbutton.onClick.RemoveAllListeners();
                            skillbutton.onClick.AddListener(() => sbs.addSkillToUnlock(sbs.skill));
                        }
                    }
                }
              //  skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIcon;
            }
        }

        ptsText.text = "Skillpoints: " + player.skillPtsAvail;
        
        StartCoroutine(loadButtons(skillPages[rotateTemple.side]));
        description.SetActive(false);

        mapskill.gameObject.SetActive(false);
    }

    public void setSkillMapping(SkillsBase skill)
    {
        //mapskill.gameObject.SetActive(true);
        //mapskill.activeSkillChoice = skill;
    }

  /*  public void addSkillToUnlock(SkillsBase skill)
    {
        Debug.Log("clicked");
        //if skill points > 0
        if (player.Checkpts(skills.Count) && skills.Contains(skill) == false)
        {
            skills.Add(skill);
        }
        //if(skill.)
    }*/

    public void selection()
    {
        if (skills.Count > 0)
        {

       //     temple.SetActive(false);

            confirmationPanel.SetActive(true);

            string listNames = "Are you sure you want to spend " + skills.Count + " Skillpoints on:" + "\n";

            for (int i = 0; i < skills.Count; i++)
            {
                listNames += "\n" +  "<b>"+ skills[i].SkillName + "</b>" + "\n" ;
            }
            confirmationText.GetComponent<Text>().text = listNames;
        }
    }

    public void confirm()
    {
        //mapskill.gameObject.SetActive(true);

        foreach (SkillsBase skill in skills)
        {
            skill.unLocked = true;
        }
        //skillPoints -= skills.Count;
        player.UnlockSkill(skills.Count, skills);
        skills.Clear();

        foreach (skillType skilltype in skillPages)
        {
            foreach (Button skillbutton in skilltype.skillbuttons)
            {
                SkillButtonScript sbs = skillbutton.GetComponent<SkillButtonScript>();
                if (sbs.skill != null)
                {
                    if (sbs.skill.unLocked)
                    {
                        skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIconUnlocked;
                        skillbutton.interactable = false;
                        if (sbs.skill.Type == 1)
                        {
                            skillbutton.interactable = true;
                            skillbutton.onClick.RemoveAllListeners();
                            skillbutton.onClick.AddListener(() => setSkillMapping(sbs.skill));
                        }

                        if (sbs.outgoing.Count > 0)
                        {
                            foreach (Button reqs in sbs.outgoing)
                            {
                                reqs.GetComponent<SkillButtonScript>().ingoing.Remove(skillbutton);
                                if (reqs.GetComponent<SkillButtonScript>().ingoing.Count == 0)
                                {
                                    reqs.interactable = true;
                                    reqs.onClick.RemoveAllListeners();
                                    reqs.onClick.AddListener(() => reqs.GetComponent<SkillButtonScript>().addSkillToUnlock(reqs.GetComponent<SkillButtonScript>().skill));
                                }
                            }
                        }
                    }
                    else
                    {
                        skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIcon;
                    }
                }
            }
            ptsText.text = "Skillpoints: " + player.skillPtsAvail;
        }
        
        confirmationPanel.SetActive(false);
        temple.SetActive(true);
    }

    public void cancel()
    {
        confirmationPanel.SetActive(false);

        foreach (SkillsBase skill in skills)
        {
            skill.unLocked = false;

        }
        skills.Clear();
        temple.SetActive(true);

        foreach (skillType skilltype in skillPages)
        {
            foreach (Button skillbutton in skilltype.skillbuttons)
            {
                SkillButtonScript sbs = skillbutton.GetComponent<SkillButtonScript>();
                if (sbs.skill != null)
                {
                    if (sbs.skill.unLocked)
                    {
                        skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIconUnlocked;
                    }
                    else
                    {
                        skillbutton.onClick.RemoveAllListeners();
                        skillbutton.onClick.AddListener(() => sbs.addSkillToUnlock(sbs.skill));
                        skillbutton.GetComponent<Image>().sprite = sbs.skill.SkillIcon;
                    }
                }
            }
        }
    }


    public void OnDisable()
    {
      //  cam.SetActive(true);

        foreach (GameObject obj in arrays)
            obj.SetActive(true);

        foreach (Button skillbutton in skillPages[rotateTemple.side].skillbuttons)
        {
            skillbutton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        rawimage.SetActive(false);

        cancel();
        //    header.SetActive(false);
    }
}