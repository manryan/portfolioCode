using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<Team> teams;

    MatchManager mm;

    public List<GameObject> teamHealthbars;

   public float teamHealth;

    int activeCount;

    public GameObject pauseMenu;

    public void Pause()
    {


        if(Time.timeScale!=0)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
   
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    public Team winner;

    public void adjustTeamHealthBar(int teamNum)
    {
        teamHealth = 0;

        for (int i = 0; i < GameManager.instance.playerQuantity; i++)
        {
            teamHealth += teams[teamNum].teamMembers[i].attribute.health;
        }


        teamHealthbars[teamNum].transform.localScale = new Vector3((teamHealth / activeCount) * 0.01f, 1f);
    }

    public void adjustTeamStickmanNumbers(int teamNum)
    {
        for (int i = 0; i < activeCount; i++)
        {
            if (teams[teamNum].teamMembers[i].attribute.health<=0)
                teamHealthbars[teamNum].transform.parent.parent.parent.GetChild(1).GetChild(i).gameObject.SetActive(false);
        }

            /*     for (int i = 0; i < GameManager.instance.teamQuantity; i++)
                 {


                     for (int j = 0; j < GameManager.instance.playerQuantity; j++)
                     {
                         if (!teams[i].teamMembers[j].gameObject.activeInHierarchy)
                         teamHealthbars[i].transform.parent.parent.parent.GetChild(1).GetChild(j).gameObject.SetActive(false);
                     }
                 }*/
        }
    private void Awake()
    {
        activeCount = GameManager.instance.playerQuantity;

        for (int i = 0; i < GameManager.instance.teamQuantity; i++)
        {
            teamHealthbars[i].transform.parent.parent.parent.gameObject.SetActive(true);

            for (int j = 0; j < GameManager.instance.playerQuantity; j++)
            {
                teamHealthbars[i].transform.parent.parent.parent.GetChild(1).GetChild(j).gameObject.SetActive(true);

                teams[i].teamMembers[j].gameObject.SetActive(true);

                if (GameManager.instance.teamInfos[i].memberNames[j].Length > 0)
                    teams[i].teamMembers[j].attribute.name = GameManager.instance.teamInfos[i].memberNames[j];
                else
                {
                    teams[i].teamMembers[j].attribute.name = I18n.langkey.Fields[GameManager.instance.langValue][teams[i].teamName]  + (j + 1);
                }

                teams[i].teamMembers[j].assignName();
            }
        }
        mm = GetComponent<MatchManager>();

 
        mm.entities = new List<Entity>();

        for (int j = 0; j < GameManager.instance.playerQuantity; j++)
        {
            for (int i = 0; i < GameManager.instance.teamQuantity; i++)
            {
                mm.entities.Add(teams[i].teamMembers[j]);
            }
        }

        mm.activeEntity = mm.entities[0];
    }

     bool returnIfTeamDead(Team team)
    {
        for (int i = 0; i < team.teamMembers.Count; i++)
        {
            if (team.teamMembers[i] && team.teamMembers[i].gameObject.activeInHierarchy)
                return false;
            
        }
        return true;
    }

   public int amount;

   

    public bool isGameOver()
    {

        amount = 0;
        for (int i = 0; i < GameManager.instance.teamQuantity; i++)
        {
            //  if(tm.teams[(int) deadentities[i].team])

            if (!returnIfTeamDead(teams[i]))
            {
                winner = teams[i];
                amount++;
            }
        }

        Debug.Log(amount);

        if (amount > 1)
            return false;
        else
        {


            return true;
        }
    }
}

[System.Serializable]
public class Team
{
    public List<Entity> teamMembers;

    public string teamName;
}
