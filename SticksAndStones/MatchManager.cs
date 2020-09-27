using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MatchManager : MonoBehaviour
{
    #region variables

    public List<Entity> entities;

    public List<Entity> deadentities;

    public List<Entity> hitentities;

    public Entity activeEntity;

    public TeamManager tm;

    public int num;

    int leftrightnum;

    public List<GameObject> menus;

    public RectTransform travelBar;

    Vector3 grenadepos;

    Vector3 vec;

    public RectTransform grenadeBar;

    public bool holdingDownPower;

    GameObject agrenade;

    public Camera cam;

    Rigidbody2D grenaderb;

    public GameObject movementPad;

    public Text actionsText;

    public GameObject landMineButton;

    public GameObject placeMineButton;

    [System.Serializable]
    public struct boundaries
    {
        public float left;

        public float right;
    }

    public boundaries boundary;

    public int camDirection;

    public float power;

    public GameObject[] sideCamButtons;

    int menuNumber;

    bool shouldSkip;

    public GameObject skipButton;

    float mapwaittime;

    public ObjectPool op;

    Transform landmineparentref;
    public LayerMask platforms;
    RaycastHit2D hit;
    RaycastHit2D wallhit;

    public float[] hitpositions;

    public GameObject healthbarrootchild;

    public GameObject endGameHolder;

    public Text endGameText;

    public AudioClip gameSong;

    public AudioClip shootSfx;

    public AudioClip explosionSfx;

    public AudioClip jump;

    int lv;

    #endregion

    public void Start()
    {
        //activeEntity = entities[0];
        if(GameManager.instance.langValue==1)
        {
            GameManager.instance.FindAllText();
            GameManager.instance.updateAllText();
        }

        StartCoroutine(scanMap());

        SoundManager.call.Play(gameSong);

        lv = GameManager.instance.langValue;
    }

    public void loadEndGame()
    {
        endGameHolder.SetActive(true);
        if (tm.amount > 0)
        {

            endGameText.text = I18n.langkey.Fields[lv][tm.winner.teamName]  + " "+ I18n.langkey.Fields[lv]["team won the game"];

            for (int i = 0; i < GameManager.instance.playerQuantity; i++)
            {
                endGameHolder.transform.GetChild(2).GetChild(i).gameObject.SetActive(true);
                endGameHolder.transform.GetChild(2).GetChild(i).GetChild(1).GetComponent<Text>().text = tm.winner.teamMembers[i].attribute.name;
            }
        }
        else
        {
            endGameText.text = I18n.langkey.Fields[lv]["Nobody wins"];
        }
    }

    IEnumerator scanMap()
    {
        yield return new WaitUntil(() => !GameManager.instance.adOpen);

        while(mapwaittime<1f)
            {
            mapwaittime += Time.deltaTime;
            if (shouldSkip)
            {
                StartCoroutine(returnCam());
                yield break;
            }
            yield return null;
        }

        while(cam.transform.position.x<boundary.right)
        {
            if(shouldSkip)
            {
                StartCoroutine(returnCam());
                yield break;
            }

            cam.transform.position += (new Vector3(0.05f, 0f, 0f) * Time.timeScale);
            yield return null;
        }

        mapwaittime = 0;

        while (mapwaittime < 1f)
        {
            mapwaittime += Time.deltaTime;
            if (shouldSkip)
            {
                StartCoroutine(returnCam());
                yield break;
            }
            yield return null;
        }

        skipButton.SetActive(false);
        StartCoroutine(returnCam());
    }

    public void skip()
    {
        skipButton.SetActive(false);
        shouldSkip = true;
        healthbarrootchild.gameObject.SetActive(true);
    }

    public void loadScene(string sceneName)
    {
        Time.timeScale = 1;

        if (GameManager.instance.prod == null || (GameManager.instance.prod != null && GameManager.instance.prod.noads == false))
        {
            if (GameManager.instance.ready())
            {
                GameManager.instance.addlistener();
                GameManager.instance.showAd(sceneName);
            }
            else if(!GameManager.instance.testinternet())
            {
                SceneManager.LoadScene("No Internet");
            }
            else
                SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

    }

    public IEnumerator launchGrenade()
    {
        goToMenu(3);
        power = 0;
        while (holdingDownPower)
        {
            if (power >= 200f)
            {
                power = 200f;
                grenadeBar.sizeDelta = new Vector2(power, 24f);
                break;
            }
            else
            {
                grenadeBar.sizeDelta = new Vector2(power, 24f);
                power++;
            }
            yield return null;
        }
        actionsText.text = "";

        activeEntity.aiming = false;
        agrenade = activeEntity.grenadeholder.transform.GetChild(0).gameObject;
        grenaderb = agrenade.transform.GetComponent<Rigidbody2D>();

        grenaderb.transform.parent = null;

        power /=2f;
        //   power = 94.5f;

        //   Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, activeEntity.grenadeholder.transform.localEulerAngles.z) * Vector2.right);
        //(Mathf.Clamp( 5f + (power/6.6666666f), 5f, 20f)
        agrenade.GetComponent<GrenadePhysics>().startOff(power);
        grenaderb.bodyType = RigidbodyType2D.Dynamic;

        grenaderb.AddForce(DegreeToVector2(activeEntity.grenadeholder.transform.localEulerAngles.z) * (5f + (power / 10f)), ForceMode2D.Impulse);

        StartCoroutine(swingArm(activeEntity.grenadeholder.transform));
        StartCoroutine(handleGrenade());
        stopFollowing = false;
        goToMenu(6);
        StartCoroutine(cameraFollow(agrenade.transform));
    }



    public void TakeCamCtrl()
    {

        // goToMenu(5);

        //enable side buttons
        stopFollowing = true;

        for (int i = 0; i < sideCamButtons.Length; i++)
        {
            sideCamButtons[i].SetActive(true);
        }
        
        goToMenu(5);
    }

    public void relinquishCtrl()
    {

        //disable side cam buttons
        for (int i = 0; i < sideCamButtons.Length; i++)
        {
            sideCamButtons[i].SetActive(false);
        }

        //return cam
        StartCoroutine(returnCam());
    }

    public IEnumerator controlCamera()
    {

        while (camDirection != 0)
        {
            cam.transform.position += new Vector3(camDirection * 10f * Time.deltaTime, 0, 0);
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, boundary.left, boundary.right), 0, -10);
            yield return null;
        }
    }

    public IEnumerator returnCam()
    {
        //goes back to entity after exploring

        while(cam.transform.position.x!=activeEntity.transform.position.x)
        {
            cam.transform.position = Vector3.MoveTowards(new Vector3(cam.transform.position.x, cam.transform.position.y, -10f) , new Vector3(activeEntity.transform.position.x, cam.transform.position.y, -10), 1f * Time.timeScale);
            yield return new WaitForSeconds(0);
        }

        //enable move cam button
        goToMenu(menuNumber);
        skip();
    }

    public bool stopFollowing;

    IEnumerator cameraFollow(Transform targ)
    {
        while(!stopFollowing)
        {
            cam.transform.position = Vector3.MoveTowards(new Vector3(cam.transform.position.x, cam.transform.position.y, -10f), new Vector3( targ.position.x, cam.transform.position.y, -10), 1f) ;
            yield return null;
        }

    }

    IEnumerator handleGrenade()
    {
        yield return new WaitUntil(() => !agrenade.activeInHierarchy);
        stopFollowing = true;
        activeEntity.grenadeholder.transform.localEulerAngles = Vector3.zero;
        agrenade.transform.parent = activeEntity.grenadeholder.transform;
        agrenade.transform.localPosition = new Vector3(-0.335f, -0.221f);
        activeEntity.grenadeholder.SetActive(false);
        agrenade.SetActive(true);
        activeEntity.anim.SetBool("grenade", false);


        StartCoroutine(waitBeforePassingUpTurn()); 
    }

    public Vector2 RadianToVector2(float radian)
    {
        if (activeEntity.facingRight)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        else
        {
            return new Vector2(-1* Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }

    public Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }


    public IEnumerator swingArm(Transform t)
    {

        if (t.localEulerAngles.z <=41f)
        {
            while (t.localEulerAngles.z < 235f)
            {

                if (Time.timeScale != 0)
                    vec = new Vector3(0, 0, t.localEulerAngles.z - 5f);

                t.localEulerAngles = new Vector3(0, 0, vec.z);
                yield return new WaitForSeconds(0);
            }
        }


            if (t.localEulerAngles.z > 41f)      
            while (t.localEulerAngles.z > 235f)
        {
                if(Time.timeScale!=0)
                vec = new Vector3(0, 0, (t.localEulerAngles.z -5f)-360f);


            t.localEulerAngles = new Vector3(0, 0, vec.z);
                yield return new WaitForSeconds(0);
            }
   
    }

    public void LaunchAction()
    {

        //instead of constantly readding a listener to button, we check what variables are.

            if (activeEntity.canWalk)
            {
                activeEntity.jump();
            }
            else if(activeEntity.aiming)
            {
                //begin coroutine to launch grenade
                StartCoroutine(launchGrenade());

            }
            else if(activeEntity.shooting)
            {
                shoot();
            }
            else if (activeEntity.kicking)
            {
                StartCoroutine(kickEm());
            }
            else if(activeEntity.placeMine)
        {
            StartCoroutine(throwMine());
        }
        
    }

    public void PlaceMine()
    {
        activeEntity.LandMine.transform.parent.gameObject.SetActive(false);
        activeEntity.LandMine.transform.parent = null;
        activeEntity.LandMine.transform.position = new Vector3(activeEntity.transform.position.x, activeEntity.transform.position.y - 0.65f, 0);
        activeEntity.anim.SetBool("grenade", false);
        activeEntity.LandMine = null;
        activeEntity.placeMine = false;
        actionsText.text = "";
        goToMenu(6);
        StartCoroutine(waitBeforePassingUpTurn());
        //end turn and change menu?
    }

    IEnumerator throwMine()
    {
        power = 0;
        while (holdingDownPower)
        {
            if (power >= 200f)
            {
                power = 200f;
                grenadeBar.sizeDelta = new Vector2(power, 24f);
                break;
            }
            else
            {
                grenadeBar.sizeDelta = new Vector2(power, 24f);
                power++;
            }
            yield return null;
        }
        power /= 50;

        //throw mine

        activeEntity.placeMine = false;
        landmineparentref = activeEntity.LandMine.transform.parent;
        activeEntity.LandMine.transform.parent = null;
        grenaderb = activeEntity.LandMine.GetComponent<Rigidbody2D>();
        grenaderb.bodyType = RigidbodyType2D.Dynamic;
        grenaderb.AddForce(DegreeToVector2(activeEntity.grenadeholder.transform.localEulerAngles.z) *  (2f+ power), ForceMode2D.Impulse);
        activeEntity.LandMine = null;
       StartCoroutine( swingArm(landmineparentref));
        actionsText.text = "";
        goToMenu(6);
        //swittch menus after
    }

    IEnumerator kickEm()
    {
        power = 0;
        while (holdingDownPower)
        {
            if (power >= 200f)
            {
                power = 200f;
                grenadeBar.sizeDelta = new Vector2(power, 24f);
                break;
            }
            else
            {
                grenadeBar.sizeDelta = new Vector2(power, 24f);
                power++;
            }
            yield return null;
        }
        power /= 2;

        activeEntity.anim.SetBool("kick", true);

        goToMenu(6);
    }

    public LayerMask entitycol;

    bool withinCameraView(Transform target)
    {
        if (target.position.x > cam.transform.position.x + 10f || target.position.x < cam.transform.position.x - 10f)
            return false;
        else
            return true;
    }

    public void Stay()
    {
        goToMenu(6);
        nextTurn();
    }


    public void shoot()
    {

     hit = Physics2D.Raycast(activeEntity.gunholder.transform.GetChild(2).position, DegreeToVector2(activeEntity.gunholder.transform.localEulerAngles.z), 10f, entitycol);

        if (hit.collider!=null && withinCameraView(hit.collider.transform.root))
        {
            wallhit = Physics2D.Raycast(activeEntity.gunholder.transform.GetChild(2).position, DegreeToVector2(activeEntity.gunholder.transform.localEulerAngles.z), Vector3.Distance(hit.point, activeEntity.gunholder.transform.GetChild(2).position), platforms);

            if (wallhit.collider == null)
            {
                if (hit.collider.transform.root.GetComponent<Entity>() != null)
                {
                    Debug.Log("hit an entity");
                    if (hit.collider.transform.GetSiblingIndex() == 0)
                    {
                        //headshot
                        hit.collider.transform.root.GetComponent<Entity>().modifyHealth(-50f);
                    }
                    else
                    {
                        //body shot
                        hit.collider.transform.root.GetComponent<Entity>().modifyHealth(-30f);

                    }
                }
                else
                {
                    hit.collider.transform.GetComponent<LandMineScript>().Explode();
                }
            }
        }
        //play gun shot sfx
        SoundManager.call.PlaySingleSound(shootSfx,0.2f);

        activeEntity.gunholder.SetActive(false);
        activeEntity.anim.SetBool("gun", false);
        activeEntity.shooting = false;
        actionsText.text = "";
       
        goToMenu(6);
        StartCoroutine(waitBeforePassingUpTurn());
    }

    public void kick()
    {
        goToMenu(3);
        actionsText.text = I18n.langkey.Fields[lv]["Kick"];
        grenadeBar.sizeDelta = new Vector2(0, 24f);
        activeEntity.kicking = true;
        activeEntity.anim.SetBool("kickstance", true);
    }

    public void grenade()
    {

        goToMenu(3);
        grenadeBar.sizeDelta = new Vector2(0, 24f);
        activeEntity.aiming = true;
        activeEntity.grenadeholder.transform.localEulerAngles = Vector3.zero;
        grenadepos= activeEntity.grenadeholder.transform.GetChild(0).position;
        activeEntity.grenadeholder.SetActive(true);
        activeEntity.grenadeholder.transform.GetChild(0).gameObject.SetActive(true);
        activeEntity.anim.SetBool("grenade", true);
        actionsText.text = I18n.langkey.Fields[lv]["Throw"];
    }
    int lastnum;


    public void nextTurn()
    {

        if (activeEntity.attribute.health <= 0 || activeEntity.inactive)
        {

            num = entities.IndexOf(activeEntity);
            num--;

            entities.Remove(activeEntity);
        }

        if (!tm.isGameOver())
        {
            if (num < entities.Count - 1)
            {
                //go to next entities turn
                activeEntity.canWalk = false;
                if (activeEntity.mc)
                    activeEntity.mc.enabled = false;
                num++;
                activeEntity = entities[num];
                activeEntity.startPos = activeEntity.transform.position;
                if (activeEntity.mc)
                    activeEntity.mc.enabled = true;

                //remove all replaceable button listenerS?

                //readd replaceable button listeners for this entity

            }
            else
            {
                //restart from the beggining of cycle
                activeEntity.canWalk = false;
                if (activeEntity.mc)
                    activeEntity.mc.enabled = false;
                num = 0;
                activeEntity = entities[0];
                activeEntity.startPos = activeEntity.transform.position;
                if (activeEntity.mc)
                    activeEntity.mc.enabled = true;
            }
            deadentities.Clear();
            StartCoroutine(goToNextEntity());
        }
        else
        {

            //open up lose screen
            loadEndGame();
        }

    }

    public IEnumerator goToNextEntity()
    {
        //goes back to entity after exploring

        while (cam.transform.position.x != activeEntity.transform.position.x)
        {
            cam.transform.position = Vector3.MoveTowards(new Vector3(cam.transform.position.x, cam.transform.position.y, -10f), new Vector3(activeEntity.transform.position.x, cam.transform.position.y, -10), 0.25f * Time.timeScale);
            yield return new WaitForSeconds(0);
        }

        //enable move cam button
        goToMenu(0);
    }


    public void goToMenu(int menuNum)
    {

        activeEntity.moveInput = 0;

        //menu 0 is beggining so cant return there

        //menu 1 is walk screen

        //menu 2 is shoot etc.
        for (int i = 0; i < menus.Count; i++)
        {
         menus[i].SetActive(false);
        }
      
        switch(menuNum)
        {
            case 1:
                //allowactive entity to walk;
                activeEntity.canWalk = true;

                actionsText.text = I18n.langkey.Fields[lv]["Jump"];
                activeEntity.startPos = activeEntity.transform.position;
                    activeEntity.handleTravelBar();
                        activeEntity.mc.enabled = true;

                stopFollowing = false;

                StartCoroutine(cameraFollow(activeEntity.transform));

                break;
            case 2:
                placeMineButton.SetActive(false);
                actionsText.text = "";
                if(activeEntity.grounded)
                stopFollowing = true;
                activeEntity.mc.enabled = false;
                if (activeEntity.aiming)
                {
                    activeEntity.grenadeholder.SetActive(false);
                    activeEntity.anim.SetBool("grenade", false);
                }
               else if(activeEntity.shooting)
                {
                    activeEntity.gunholder.SetActive(false);
                    activeEntity.anim.SetBool("gun", false);
                }
                else if (activeEntity.kicking)
                {
                    activeEntity.anim.SetBool("kickstance", false);
                }
                else if(activeEntity.placeMine)
                {
                    activeEntity.LandMine.transform.parent.gameObject.SetActive(false);
                    activeEntity.anim.SetBool("grenade", false);
                }
                resetAll();
                if(activeEntity.LandMine==null)
                {
                    landMineButton.SetActive(false);                
                }
                else
                {
                      landMineButton.SetActive(true);
                }

                break;
            case 4:
                loadGun();
                break;
        }
        if (menuNum != 5)
        {
            movementPad.SetActive(true);
            menuNumber = menuNum;
            actionbut.SetActive(true);
         
        }
        else
        {
            movementPad.SetActive(false);
            actionbut.SetActive(false);

        }

        menus[menuNum].SetActive(true);

    }

    public void resetAll()
    {
        activeEntity.canWalk = false;
        activeEntity.shooting = false;
        activeEntity.aiming = false;
        activeEntity.kicking = false;
        activeEntity.placeMine = false;
    }

    public GameObject actionbut;

  public  bool running;

  public  IEnumerator waitBeforePassingUpTurn()
    {
        running = true;

        Debug.Log("how many");
      yield return new WaitForSeconds(2f);

        if (hitentities.Count>0)
            for (int i = 0; i < hitentities.Count; i++)
            {
                if (!hitentities[i].grounded)
                    while (hitentities[i].grounded == false || hitentities[i].blownup)
                    {
                        if (hitentities[i].inactive)
                            break;
                        yield return new WaitForSeconds(0);
                    }

            }

        if(deadentities.Count>0)
        {
            for (int i = 0; i <deadentities.Count; i++)
            {
                if(!deadentities[i].inactive)
                    //!
                    while(deadentities[i].gameObject.activeInHierarchy)
                    {
                        yield return new WaitForSeconds(0);
                    }
                else
                {
                    while (deadentities[i].deathsign.activeInHierarchy || (deadentities[i].attribute.health==0 && deadentities[i].gameObject.activeInHierarchy))
                    {
                        yield return new WaitForSeconds(0);
                    }

                }
            }
        }

   Debug.Log("what called me?");

        running = false;
        nextTurn();
    }

    public void loadMine()
    {
        activeEntity.anim.SetBool("grenade", true);
        placeMineButton.gameObject.SetActive(true);
        activeEntity.LandMine.transform.parent.localEulerAngles = Vector3.zero;
        activeEntity.LandMine.transform.parent.gameObject.SetActive(true);
        activeEntity.placeMine = true;
        actionsText.text = actionsText.text = I18n.langkey.Fields[lv]["Throw"];
        goToMenu(3);
    }

    public void loadGun()
    {
        activeEntity.shooting = true;
        activeEntity.gunholder.SetActive(true);
        activeEntity.gunholder.transform.localEulerAngles = Vector3.zero;
        activeEntity.anim.SetBool("gun", true);
        actionsText.text = actionsText.text = I18n.langkey.Fields[lv]["Shoot"];
    }

    bool updown;

    int updownnum;

    public void holdUpDown(int updownnumber)
    {
        if (updownnumber != 0)
        {
            updown = true;
            updownnum = updownnumber;
            if(activeEntity.aiming)
            StartCoroutine(moveUpOrDown(activeEntity.grenadeholder.transform));
            else
            {
                StartCoroutine(moveUpOrDown(activeEntity.gunholder.transform));
            }
        }
        else
        {
            updown = false;
            updownnum = 0;
            
        }
    }

    public IEnumerator moveUpOrDown(Transform obj)
    {
        while (updown)
        {

           if(obj.localEulerAngles.z>41f)
            {
                vec = new Vector3(0, 0, Mathf.Clamp( (obj.localEulerAngles.z +(updownnum*1.2f))-360, -40, 40));

            }
           else
          vec = new Vector3(0,0, Mathf.Clamp(obj.localEulerAngles.z + (updownnum*1.2f), -40, 40));

            obj.localEulerAngles = new Vector3(0, 0, vec.z);
            yield return null;
        }
    }
         





    public void holdleftright(int leftrightnumber)
    {


        if (activeEntity.canWalk)
        {
            if (leftrightnumber != 0)
            {
                leftrightnum = leftrightnumber;
                activeEntity.moveInput = leftrightnum;
                activeEntity.mc.Movement();
                if (!activeEntity.blownup)
                    activeEntity.faceRightWay();
            }
            else
            {
                leftrightnum = 0;
                activeEntity.moveInput = 0;
                activeEntity.rb.velocity = Vector2.zero;
            }
        }
        else
        {
            activeEntity.faceRightWay();
            leftrightnum = leftrightnumber;
            activeEntity.moveInput = leftrightnum;
        }
    }

}
