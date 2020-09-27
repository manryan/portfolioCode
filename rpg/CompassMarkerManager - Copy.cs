using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassMarkerManager : MonoBehaviour {

    public  Markers questMarker;

    public Transform[] whoWeTrack;

    public Texture[] textures;

    public GameObject activeMapicon;

    public GameObject[] mapicons;

    public void activate(int num)
    {
        questMarker.enabled = true;

        questMarker.texture = textures[num];

        questMarker.switchIn();

        questMarker.enemypos = whoWeTrack[num].gameObject;

       activeMapicon=  mapicons[num];

        activeMapicon.SetActive(true);
    }

    public void shutDown(int num)
    {

        questMarker.switchOut();

        questMarker.enemypos = null;

        questMarker.enabled = false;

           activeMapicon.SetActive(false);

        activeMapicon = null;
    }


}
