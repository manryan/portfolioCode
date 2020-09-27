using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.SceneManagement;using UnityEngine.UI;

public class ChallengeModeManager : MonoBehaviour
{

    public Entity player;

    public LayerMask targets;

    RaycastHit2D hit;

    public GameObject target;

    public List<targPositions> targpos;

    public int targlevel;

    public Text timeText;

    float time;

    public Text CountdownText;

    public float roundTime;

    public int challengeLevel;

    bool end;

    public GameObject loseMenu;

    public GameObject winMenu;

    public Text newRecordTimeText;

    public Text lostText;


    public GameObject pauseMenu;

    public AudioClip shootSfx;

    public AudioClip challengeClock;

    public AudioClip buzzer;

    public AudioClip beep;

    public AudioClip finalBeep;

    public AudioClip wongame;

    public void Pause()
    {


        if (Time.timeScale != 0)
        {
            SoundManager.call.musicSource.Pause();
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            SoundManager.call.musicSource.UnPause();
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    IEnumerator waitTillMoveTarg()
    {

       target.transform.position = new Vector2( targpos[targlevel].poslist[0].x +targpos[targlevel].poslist[1].x, targpos[targlevel].poslist[0].y + targpos[targlevel].poslist[1].y);
        yield return new WaitUntil(() => player.aiming);
        ping = true;
        StartCoroutine(beginPingPong());
    }

    IEnumerator soundeffect()
    {
        //assign target position
        if (targpos[targlevel].poslist.Count > 0)
        {
           StartCoroutine( waitTillMoveTarg());
            //start coroutine for moving it between those two values
        }
        else
        {
            ping = false;
            target.transform.position = targpos[targlevel].pos;
        }


    //start it off
        time = 3f;

        for (int i = 0; i < 3; i++)
        {
            SoundManager.call.PlaySingleSound(beep);
            timeText.text = time.ToString("0");
            time--;
            yield return new WaitForSeconds(1);
        }
        timeText.text = time.ToString("0");
        SoundManager.call.PlaySingleSound(finalBeep);
        yield return new WaitForSeconds(0.5f);
        SoundManager.call.Play(challengeClock);
        canShoot = true;
        StartCoroutine(countDown());
    }

    IEnumerator countDown()
    {
        time = 0f;

        timeText.gameObject.SetActive(false);
            player.gunholder.SetActive(true);
            player.anim.SetBool("gun", true);
            player.aiming = true;
        CountdownText.gameObject.SetActive(true);

        CountdownText.text = time.ToString("0.0") + "/" + roundTime;

        while (time<roundTime)
        {
         if(end)
            {
                break;
            }
            time += Time.deltaTime;
            CountdownText.text = time.ToString("0.0") + "/" + roundTime;
            yield return new WaitForSeconds(0);
        }
        player.aiming = false;
            SoundManager.call.musicSource.Stop();

        if(end)
        {
            SoundManager.call.PlaySingleSound(wongame);

            //you won
            if (GameManager.instance.values.gunLevel < challengeLevel)
            {

                GameManager.instance.values.gunTime.Add(time);

            //save, update etc
            GameManager.instance.values.gunLevel = challengeLevel;
                GameManager.instance.saveLevel();

                newRecordTimeText.text = I18n.langkey.Fields[GameManager.instance.langValue]["You won"] + ":)" +" \n " + I18n.langkey.Fields[GameManager.instance.langValue]["your current time was "] + time.ToString("0.0") + I18n.langkey.Fields[GameManager.instance.langValue][" Seconds"];
            }
            else
            {
                if(time < GameManager.instance.values.gunTime[challengeLevel-2])
                {
                    GameManager.instance.values.gunTime[challengeLevel - 2] = time;
                    GameManager.instance.saveLevel();
                    newRecordTimeText.text = I18n.langkey.Fields[GameManager.instance.langValue]["You won"] + ":)" + " \n " + I18n.langkey.Fields[GameManager.instance.langValue][" you have beat your best time! Your new record time is.... "] + time.ToString("0.0") + I18n.langkey.Fields[GameManager.instance.langValue][" Seconds"];
                }
                else
                {
                    newRecordTimeText.text = I18n.langkey.Fields[GameManager.instance.langValue]["You won"] + ":)" + " \n " + I18n.langkey.Fields[GameManager.instance.langValue]["your current time was "] + time.ToString("0.0") + I18n.langkey.Fields[GameManager.instance.langValue][" Seconds"];
                }
            }
            winMenu.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.call.PlaySingleSound(buzzer);
            loseMenu.gameObject.SetActive(true);
            lostText.text = I18n.langkey.Fields[GameManager.instance.langValue]["You lose"] + " :(";
            //tell em you failed
        }
    }

    void Start()
    {
        if (GameManager.instance.langValue == 1)
        {
            GameManager.instance.FindAllText();
            GameManager.instance.updateAllText();
        }

        SoundManager.call.musicSource.Stop();

        //    StartCoroutine(countDown());
        StartCoroutine(soundeffect());
        
    }

    bool canShoot;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            landedHit();
        }
    }

    IEnumerator waitBeforeShooting()
    {
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }
    public void Shoot()
    {
        if (player.aiming && canShoot)
        {
            canShoot = false;
            StartCoroutine(waitBeforeShooting());

            SoundManager.call.PlaySingleSound(shootSfx, 0.2f);

            hit = Physics2D.Raycast(player.gunholder.transform.GetChild(2).position, DegreeToVector2(player.gunholder.transform.localEulerAngles.z), 10f, targets);


            if (hit.collider != null)
            {
                landedHit();
            }
        }
    }

    void landedHit()
    {
                //call next target
                targlevel++;

                if (targlevel < targpos.Count)
                {
                    if (targpos[targlevel].poslist.Count > 0)
                    {
                        ping = true;
                        StartCoroutine(beginPingPong());
                        //start coroutine for moving it between those two values
                    }
                    else
                    {
                        ping = false;
                        target.transform.position = targpos[targlevel].pos;
                    }
                }
                else
                {
                    ping = false;
                    end = true;
                }
    }

    IEnumerator beginPingPong()
    {
        ping = false;
        yield return new WaitForSeconds(0);
        ping = true;
        StartCoroutine(pingpong());
    }

    bool ping;

        public IEnumerator pingpong()
    {


        if (targpos[targlevel].pos.x == 0)
        {
            if (targpos[targlevel].pos.y == 0)
                while (ping)
                {
                    if (Time.timeScale != 0)
                        target.transform.position = new Vector3(Mathf.PingPong(Time.time * targpos[targlevel].speed, targpos[targlevel].poslist[0].x) + targpos[targlevel].poslist[1].x, targpos[targlevel].poslist[0].y, 0);
                    yield return null;
                }
            else
            {
                while (ping)
                {
                    if (Time.timeScale != 0)
                        target.transform.position = new Vector3(targpos[targlevel].poslist[0].x, Mathf.PingPong(Time.time * targpos[targlevel].speed, targpos[targlevel].poslist[0].y) + targpos[targlevel].poslist[1].y, 0);
                    yield return null;
                }
            }
        }//move diagonally
        else
        {
            while (ping)
            {
                Debug.Log("broken");


                if (Time.timeScale != 0)
                    target.transform.position = new Vector3(Mathf.PingPong(Time.time * targpos[targlevel].speed, targpos[targlevel].poslist[0].x) + targpos[targlevel].poslist[1].x, Mathf.PingPong(Time.time * targpos[targlevel].speed, targpos[targlevel].poslist[0].y) + targpos[targlevel].poslist[1].y, 0);
                yield return null;
            }
        }
    }

    public Vector2 RadianToVector2(float radian)
    {
        if (player.facingRight)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        else
        {
            return new Vector2(-1 * Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }

    public Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }




    bool updown;

    int updownnum;

    public void holdUpDown(int updownnumber)
    {
        if(player.aiming)
        if (updownnumber != 0)
        {
            updown = true;
            updownnum = updownnumber;
                StartCoroutine(moveUpOrDown(player.gunholder.transform));
            
        }
        else
        {
            updown = false;
            updownnum = 0;

        }
    }

    Vector3 vec;

    public IEnumerator moveUpOrDown(Transform obj)
    {
        while (updown)
        {

            if (obj.localEulerAngles.z > 41f)
            {
                vec = new Vector3(0, 0, Mathf.Clamp((obj.localEulerAngles.z + updownnum) - 360, -40, 40));

            }
            else
                vec = new Vector3(0, 0, Mathf.Clamp(obj.localEulerAngles.z + updownnum, -40, 40));

            obj.localEulerAngles = new Vector3(0, 0, vec.z);
            yield return null;
        }
    }

    
    public void holdleftright(int leftrightnumber)
    {
        if (player.aiming)
        {
            player.moveInput = leftrightnumber;
            player.faceRightWay();
        }
    }

    public void reloadScene()
    {
        if (GameManager.instance.prod == null)
        {
            GameManager.instance.addlistener();
            GameManager.instance.showAd(SceneManager.GetActiveScene().name);
        }

       // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void loadscene(string name)
    {

        if (Time.timeSinceLevelLoad > 5f)
        {
            if (GameManager.instance.prod == null || (GameManager.instance.prod != null && GameManager.instance.prod.noads == false))
            {
                if (GameManager.instance.ready())
                {
                    GameManager.instance.addlistener();
                    GameManager.instance.showAd(name);
                    Time.timeScale = 0;
                }
                else if (!GameManager.instance.testinternet())
                {
                    SceneManager.LoadScene("No Internet");
                }
                else
                {
                    SceneManager.LoadScene(name);
                    Time.timeScale = 1;
                }
            }
            else
            {
                SceneManager.LoadScene(name);
                Time.timeScale = 1;
            }
        }
        else
        {
            SceneManager.LoadScene(name);
            Time.timeScale = 1;
        }
    }
}
[System.Serializable]
public class targPositions
{
    public Vector3 pos;

    public float speed;

    public List<Vector3> poslist;
}