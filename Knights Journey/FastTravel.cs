using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FastTravel : MonoBehaviour
{

    public GameObject fastTravelPanel;

    public GameObject confirmationPanel;

    public GameObject declinedPanel;

  //  public Button 

    public Transform buttonsHolder;

    public int index;

    public Text noLocationsUnlockedText;

    public string PoolLocation;

    public Button confirmButton;

    public Text confirmationText;

    public Text declinedText;

    public List<locationsData> allLocations = new List<locationsData>();

    public void checkIfWeTravel(locationsData data)
    {
        if(data.location == PoolLocation)
        {
            //  Debug.Log(data.location);
            //  Debug.Log("already here");
            declinedText.text = "Can't travel to" + PoolLocation + "as you already are at " + PoolLocation;
            declinedPanel.SetActive(true);
        }
        else
        {
            PoolLocation = data.location;
            confirmationText.text = "Are you sure you want to travel to " + PoolLocation + "?";
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(delegate { travel(data); });
            confirmationPanel.SetActive(true);
        }
    }

    public void travel(locationsData data)
    {
        GameManager.instance.player.saveEverythingExceptEnemies();
        id.instance.fastTravelling = true;
        id.instance.x = data.position.x;
        id.instance.y = data.position.y;
        Time.timeScale = 1;
        SceneManager.LoadScene(data.sceneName);
    }

    public void activate(string location)
    {
        PoolLocation = location;
        if (!buttonsHolder.GetChild(0).GetComponent<Button>().interactable)
            adjustButtons();
        fastTravelPanel.SetActive(true);
    }
    public void adjustButtons()
    {
        for (int i = 0; i < allLocations.Count; i++)
        {
            if (!buttonsHolder.GetChild(i).GetComponent<Button>().interactable) 
            {
                if (id.instance.fastTravelSpots.Contains(allLocations[i].location))
                {
                    buttonsHolder.GetChild(i).gameObject.SetActive(true);
                    index = i;
                    buttonsHolder.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { checkIfWeTravel(allLocations[index]); });
                    // buttonsHolder.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { travel(allLocations[index]); });
                    buttonsHolder.GetChild(i).GetComponent<Button>().interactable = true;
                    buttonsHolder.GetChild(i).GetChild(0).GetComponent<Text>().text = allLocations[i].location;
                }
            }
        }
       if( !buttonsHolder.GetChild(0).GetComponent<Button>().interactable)
        {
            noLocationsUnlockedText.enabled = true;
        }
       else
        {
            noLocationsUnlockedText.enabled = false;
        }
    }
}

[System.Serializable]
public class locationsData
{
    public string location; 

    public string sceneName;

    public Vector3 position;
}