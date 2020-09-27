using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeRotate : MonoBehaviour {


    //counts number of key presses

    public int keycount = 0;

    public int integer;

    public int side = 0;

    public skilltreeActivator skilltree;

    public void Start()
    {
        skilltree = GameObject.Find("skilltree activator").GetComponent<skilltreeActivator>();
    }


	// Update is called once per frame
/*	void FixedUpdate () {


        if (Input.GetKeyDown(KeyCode.D) && keycount ==0)
        {
            //begin rotation iterator to the right
            //    StartCoroutine(RotateMe(Vector3.up * 90, 0.8f, 90));

            skilltree.StartCoroutine(skilltree.unloadButtons(skilltree.skillPages[side],1));

            //increment counter
            keycount++;

            //reset counter when done
        //    Invoke("counterReset", 1.0f);

            if (side < 3)
                side++;
            else
                side = 0;
        }
        else if (Input.GetKeyDown(KeyCode.A) && keycount == 0)
        {
            //begin rotation iterator to the left
            // StartCoroutine(RotateMe(Vector3.up * -90, 0.8f, -90));
            skilltree.StartCoroutine(skilltree.unloadButtons(skilltree.skillPages[side], -1));

            //increment counter
            keycount++;

            //reset counter when done
        //    Invoke("counterReset", 1.0f);

            if (side > 0)
                side--;
            else
                side = 3;
        }
    }*/

        public void Left()
    {
        if (keycount == 0)
        {
            //begin rotation iterator to the left
            // StartCoroutine(RotateMe(Vector3.up * -90, 0.8f, -90));
            skilltree.StartCoroutine(skilltree.unloadButtons(skilltree.skillPages[side], -1));

            //increment counter
            keycount++;

            //reset counter when done
            //    Invoke("counterReset", 1.0f);

            if (side > 0)
                side--;
            else
                side = 3;
        }
    }

    public void Right()
    {
        if ( keycount == 0)
        {
            //begin rotation iterator to the right
            //    StartCoroutine(RotateMe(Vector3.up * 90, 0.8f, 90));

            skilltree.StartCoroutine(skilltree.unloadButtons(skilltree.skillPages[side], 1));

            //increment counter
            keycount++;

            //reset counter when done
            //    Invoke("counterReset", 1.0f);

            if (side < 3)
                side++;
            else
                side = 0;
        }
    }



    public void counterReset()
    {
        keycount = 0;
    }

   public IEnumerator RotateMe(Vector3 byAngles, float inTime, int val)
    {
        //set current rotation

        if (Mathf.Abs(integer) ==360)
            integer = 0;

        integer += val;

        var fromAngle = transform.rotation;

       // Debug.Log(angle);

        //set angle to turn to by adding the vector to objects rotation.

        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);

        //set time value between each rotation to be increased every frame and divided by iterator move time

        for (var t = 0f; t < 1; t +=0.1f)
        {
            Debug.Log(t);
            //spherically interpolate from last rotation to angle of choice.

            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);

            yield return null;
        }

        transform.eulerAngles = new Vector3(-90, 0, integer);

        skilltree.StartCoroutine(skilltree.loadButtons(skilltree.skillPages[side]));
    }
}
