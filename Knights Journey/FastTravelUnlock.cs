using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelUnlock : MonoBehaviour
{



    public string locationUnlocked;

    public FastTravelManager fTravelManager;

    public void OnTriggerEnter2D(Collider2D c)
    {

        if (c.gameObject.tag == "Player")
        {
  
            if(!id.instance.fastTravelSpots.Contains(locationUnlocked))
            {
                GameManager.instance.player.textDisplay.Enqueue("Unlocked" + locationUnlocked + " fast fravel");
                GameManager.instance.player.checkIfWeShouldHandle();
                id.instance.fastTravelSpots.Add(locationUnlocked);
                fTravelManager.fastTravelRef.adjustButtons();
                GameManager.instance.player.save();
            }
        }
    }
}