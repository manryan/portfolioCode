using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassScript : MonoBehaviour {

	// Use this for initialization
 
    public RawImage compass;
    GameObject player;

    private void Start()
    {
        player = GameObject.Find("CM_MainCamera").transform.GetChild(0).gameObject;
    }

    void Update()
    {

        compass.uvRect = new Rect(player.transform.localEulerAngles.y / 360f, 0, 1, 1);
        compass.maskable = true;
    }

}