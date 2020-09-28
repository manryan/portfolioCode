using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaHub : MonoBehaviour
{

    public string sceneToLoad;

    public bool started = true;

    public Player player=null;

    public Vector2 positionWeLandIn;

    public void Awake()
    {
        Invoke("startTimeDone", 3f);
    }

    public void startTimeDone()
    {
        started = false;

        if(player!=null)
            handleTravelManager();
    }

    public void loadScene()
    {
        if (!id.instance.scenesUnlocked.Contains(sceneToLoad))
            id.instance.scenesUnlocked.Add(sceneToLoad);

        id.instance.travelPosition = positionWeLandIn;
        GameManager.instance.player.save();
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            player = null;
        }
    }

    public void OnTriggerEnter2D(Collider2D c)
    {

        if (c.gameObject.tag == "Player")
        {
            if(player==null)
            player = c.transform.parent.GetComponent<Player>();

            if(!started)
            handleTravelManager();
        }
    }

    public void handleTravelManager()
    {
        player.traveler.enablePanel();
        player.traveler.yesButton.onClick.RemoveAllListeners();
        player.traveler.yesButton.onClick.AddListener(() => loadScene());
        player.traveler.sceneOptionsText.text = "Are you sure you would like to travel to " + sceneToLoad + "?";
    }
}
