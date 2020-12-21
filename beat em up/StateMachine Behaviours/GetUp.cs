using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUp : MonoBehaviour
{
    public Animator anim;

    public bool canGetup = false;
    private void OnEnable()
    {
        Invoke("getPlayerUp", 0.5f);
        Invoke("setGetUpTrue", 0.2f);
    }

    void setGetUpTrue()
    {
        canGetup = true;
    }

    public void getPlayerUp()
    {
       // Debug.Log("GetUp");
        anim.SetBool("GetUp", true);
        this.enabled = false;
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            if(canGetup == true)
            getPlayerUp();
    }
}
