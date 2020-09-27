using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum icons { zeroIt =0, one, Environment};

public class MapScrolling : MonoBehaviour {

    public float[] iconIntervals;

    public float[] clampIntervalsX;

    public float[] clampIntervalsTop;

    public float[] clampIntervalsSide;

    Vector3 touchStart;
    Vector3 startingLocation;

    public boundary pboundary;

    public icons icon;

    public Camera cam;

    Vector3 origin;

    public Sprite[] mySprites;

    public GameObject player;

    public GameObject playericon;

    public struct boundary
    {
        public float Up, Down, Left, Right;

        public boundary(float up, float down, float left, float right)
        {
            Up = up; Down = down; Left = left; Right = right;
        }
    }

    private void OnEnable()
    {
       playericon.transform.position = new Vector3(player.transform.position.x, 200, player.transform.position.z);
    }

    public int index;

    void Awake()
    {
        player= GameObject.Find("Player");
        playericon = GameObject.Find("Player Icon");
        playericon.transform.position = new Vector3(player.transform.position.x, 200, player.transform.position.z);
        while ((index < iconIntervals.Length) && (transform.localScale.x > iconIntervals[index]))
        {
            index++;
            icon = (icons)index;
        }

        pboundary = new boundary(origin.y + clampIntervalsTop[scaleindex], origin.y - clampIntervalsTop[scaleindex], origin.x - clampIntervalsSide[scaleindex], origin.x + clampIntervalsSide[scaleindex]);
        cam.cullingMask |= 1 << LayerMask.NameToLayer(icon.ToString());
    }

    // Use this for initialization
    void Start () {
        cam = GameObject.Find("MapDemo/MapCamera").GetComponent<Camera>();
        Debug.Log(transform.position + "This is it");
        startingLocation = transform.position;
        origin = startingLocation;
        pboundary = new boundary(origin.y + ((Mathf.Pow(10f, transform.localScale.x) * (5f / transform.localScale.x))), origin.y - ((Mathf.Pow(10f, transform.localScale.x) * (5f / transform.localScale.x))), origin.x - ((Mathf.Pow(10f, transform.localScale.x) * (5f / transform.localScale.x))), origin.x + ((Mathf.Pow(10f, transform.localScale.x) * (5f / transform.localScale.x))));
        //pboundary = new boundary(startingLocation.y + 150f, startingLocation.y - 150f, startingLocation.x - 100f, startingLocation.x + 100f);
        
        pboundary = new boundary(origin.y + clampIntervalsTop[scaleindex], origin.y - clampIntervalsTop[scaleindex], origin.x - clampIntervalsSide[scaleindex], origin.x + clampIntervalsSide[scaleindex]);

        icon = (icons)index;
        cam.cullingMask |= 1 << LayerMask.NameToLayer(icon.ToString());
        
    }

    public int scaleindex;

    void zoom(float increment)
    {

        if (increment > 0f)
        {
            cam.cullingMask -= 1 << LayerMask.NameToLayer(icon.ToString());

            
           
            string myfloat = transform.localScale.x.ToString("F2");
            float testfloat = float.Parse(myfloat);
            while ((scaleindex < clampIntervalsX.Length) && (testfloat> clampIntervalsX[scaleindex]))
            {
                scaleindex++;
                
            }
            pboundary = new boundary(origin.y + (clampIntervalsTop[scaleindex]), origin.y - (clampIntervalsTop[scaleindex]), origin.x - (clampIntervalsTop[scaleindex] * 3f), origin.x + (clampIntervalsTop[scaleindex] * 3f));
            cam.cullingMask |= 1 << LayerMask.NameToLayer(icon.ToString());

            if (transform.localScale.x < 3.1f)
                transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, 1.1f, 3f), Mathf.Clamp(transform.localScale.y, 1.1f, 3f)) + new Vector3(increment, increment);


            if (transform.localScale.x >3.1f)
                transform.localScale = new Vector3(3.1f, 3.1f, 1);

            while((index < iconIntervals.Length) && (transform.localScale.x > iconIntervals[index]))
            {
                cam.cullingMask -= 1 << LayerMask.NameToLayer(icon.ToString());
                index++;
                icon = (icons)index;

            }

            cam.cullingMask |= 1 << LayerMask.NameToLayer(icon.ToString());
        }
        else
        {
            
            
            string myfloat = transform.localScale.x.ToString("F2");
            float testfloat = float.Parse(myfloat);
            while ((scaleindex >0) && (testfloat <= clampIntervalsX[scaleindex-1]))
            {
                scaleindex--;
 
            }


   
            pboundary = new boundary(origin.y + (clampIntervalsTop[scaleindex]), origin.y - (clampIntervalsTop[scaleindex]), origin.x - (clampIntervalsTop[scaleindex] * 3f), origin.x + (clampIntervalsTop[scaleindex] * 3f));
            

            if (transform.localScale.x>1f)
            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, 1.1f, 3f), Mathf.Clamp(transform.localScale.y, 1.1f, 3f)) + new Vector3(increment, increment);

            if (transform.localScale.x < 1f)
                transform.localScale = new Vector3(1, 1, 1);


            if ((index >= 1) && (transform.localScale.x < iconIntervals[index - 1]))
            {
                cam.cullingMask -= 1 << LayerMask.NameToLayer(icon.ToString());
                index--;
                icon = (icons)index;
            }
            cam.cullingMask |= 1 << LayerMask.NameToLayer(icon.ToString());
        }



    }


    void Update () {

       // handleIcons();

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            startingLocation = transform.position;
        }


        if (Input.GetMouseButton(0))
        {
            Vector2 direction = touchStart - Input.mousePosition;

            //Debug.Log(transform.position + "This is it");
            Vector3 vec = startingLocation - new Vector3(direction.x, direction.y);
            transform.position = new Vector3(Mathf.Clamp( vec.x, pboundary.Left, pboundary.Right), Mathf.Clamp(vec.y, pboundary.Down, pboundary.Up));
        }
      if(Mathf.Abs( Input.GetAxis("Mouse ScrollWheel"))>0)
        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }
}
