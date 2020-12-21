using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disablePoster : MonoBehaviour
{
    public GameObject poster;

    public GameObject psoterCrumbled;

    public void enableCrumbledPoster()
    {
        psoterCrumbled.SetActive(true);
    }

        public void disableposter()
    {
        poster.SetActive(false);
        transform.GetComponent<Animator>().SetBool("poster", false);
    }
}
