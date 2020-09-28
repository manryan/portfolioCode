using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRadiusDetection : MonoBehaviour {

    public Player playa;



    public void OnTriggerStay2D(Collider2D c)
    {
        if (c.gameObject.tag == "wall")
        {
            playa.manager.checkSpot();
        }
    }
    public void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.tag == "wall")
        {
            playa.manager.checkSpot();
        }
    }
}
