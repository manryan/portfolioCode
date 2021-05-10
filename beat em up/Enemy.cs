using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum enemyState
{
    spawn = 1, idle = 2, approach = 3, wander, livemovement, walkingtospot, flinching, wait, down, jumping, rolling,
    stayaway, sidestep, attacking, approachthenjumpkick, walkconstantly, walknwait, twothreewait, onemore,
    waitafterwalk, blocking
}
public class Enemy : Entity
{
    #region GameManager Variables

    public NavMeshAgent nav;


    Enemy eref;

    int timeswalked;

    int numbertowalk;

    [HideInInspector]
    public bool jumpkicked;

    enemyState lastprocastinatestate;

    float rstime;

    float timeTowait;

    [HideInInspector]
    public Vector3 spot;


    float distxfromp;

    [HideInInspector]
    public EnemyFunctions efunct;

    public LayerMask wallsnobj;

    float distyfromp;

    [HideInInspector]
    public Vector3 sideStepSpot;

    [HideInInspector]
    public Collider[] sidestepcols;

    [HideInInspector]
    public Ray sidestepray;

    bool needstogetaway;

    [HideInInspector]
    public RaycastHit[] sidestepcasthit;

    [HideInInspector]
    public bool newspot;


    public List<GameObject> objectsinstep;

    [HideInInspector]
    public Vector3 oldSpot;

    public Transform player;

    public Player p;

    [HideInInspector]
    public Vector3 wanderspot;

    public enemyPositions enemyposinmanager;

    public NavMeshObstacle obsmain;

    bool reached;

    [HideInInspector]
    public int canTry;

    public LayerMask playerMask;
    [HideInInspector]
    public Overlapping olp;

    bool rayhit;

    [HideInInspector]
    public float rand;

    public int chanceToCheck;

   // public float timeBeforeWeWander;

    public Camera cam;

    Vector3 vec;

    Enemy refObj;

    [HideInInspector]
    public bool leftprocastinate;

    public LayerMask enemiesnobjs;

    Collider[] cols;

    int spotindex;

    public LayerMask playernenemies;

    float timer;

    public enemyState state;

    RaycastHit hit;

    Ray ray;

    RaycastHit wanderhit;

    public List<Collider> anyoneThere;

    float waitvalue;

    float waitmultiplier;

    float randomAtt;

    [HideInInspector]
    public bool canrotate = true;

    int kickorpunch;

    public float chanceToCrouch;

    public float crouchtest;

    [HideInInspector]
    public Ray wanderray;


    [HideInInspector]
    public Collider[] spotcols;

    //float timestocheckbeforewemove;

    [HideInInspector]
    public float attBlend;

    public LayerMask objectsnplayer;

    public LayerMask enemyCol;

    public List<Vector3> livePosition = new List<Vector3>();

    public float chanceToReact;

    public float chanceRoll;

    public LayerMask allobjects;

    public bool smartAi;

    [HideInInspector]
    public bool dynamic;

    Vector3 spotplaceholder;
    int randomIndex;

    [HideInInspector]
    public int timesWentDynamic;

    [HideInInspector]
    public Player pAtt;

    Vector3 compareAngle;

    public List<Vector3> jumpRollPositions;

    public LayerMask invisipne;

    public LayerMask wallsOnly;

    public RollHandle rollh;

    List<Vector3> liveposAlt;

    public bool AllowRs = true;

    RaycastHit wallcheck;

    Ray wallRay;

    Vector3 secondTravelPath;

    public bool sideStep;

    public LayerMask objects;

    [HideInInspector]
    public bool waiting;

    float chanceToWait;

    enemyPositions tempEP;

    [HideInInspector]
    public Vector3 livespotref;

    public LayerMask ebox;

    public LayerMask invisibleWalls;

    //temporary bools VVVVV

    bool canMoveCam;
    //only written for logic^^^

    [HideInInspector]
    public bool cango;

    float distanceBetweenUs;

    bool backup;


    [HideInInspector]
    public float distx;
    [HideInInspector]
    public float distz;

    [HideInInspector]
    public bool approaching;

    Vector3 altSpot;

    public bool rightSpot;

    float countdown;

    public float chanceToGoLiveSpot;

    float liveSpotPercent;

    float rotatecooldown;

    [HideInInspector]
    public bool purposelymiss;

    #endregion

    private void OnEnable()
    {
        //  GameManager.instance.enemies.Add(this);
        Invoke("setRbKinematic", 0.1f);
        lastprocastinatestate = enemyState.onemore;


        //ignore collisions with the invisible walls on the camera boundaries :) **8

        //******   ^^^ VERY IMPORTANT ^^^^^^  *****
    }

    private void Start()
    {
        GameManager.instance.enemies.Add(this);
        Invoke("assignP", 0.5f);
        Physics.IgnoreLayerCollision(12, 13);
    }

    void assignP()
    {
        p = GameManager.instance.players[0];
        player = p.transform;
    }

    float leftOfEnemy()
    {
        if (enemyposinmanager.whichOne < 2)
            return 1.5f;

        else
            return -1.5f;
    }

    public void resetdynamic()
    {
        timesWentDynamic = 0;
        dynamic = false;
    }

    public void resumeAgent()
    {
        if (anim.GetBool("rotate") == false)
            if (nav.enabled)
                nav.isStopped = false;
    }

    public void setRbKinematic()
    {
        rb.isKinematic = true;
    }


    void shuffleSpots()
    {
        //shufle our spots?

        //  numbers.Sort((a, b) => 1 - 2 * Random.Range(0, numbers.Count));

        for (int i = 0; i < livePosition.Count; i++)
        {
            spotplaceholder = livePosition[i];
            randomIndex = Random.Range(i, livePosition.Count);
            livePosition[i] = livePosition[randomIndex];
            livePosition[randomIndex] = spotplaceholder;
        }
    }
    public override void Jump()
    {
        if (state != enemyState.attacking)
            state = enemyState.jumping;
        rb.isKinematic = false;
        olp.time = 0;
        activateobs();
        if (blocking)
        {
            blocking = false;
            anim.SetBool("Block", false);
        }
        base.Jump();
    }

    public override void Roll()
    {

        if (canRoll())
        {
            shouldroll = true;
        }

        if (pAtt)
        {

        }
        else
            pAtt = p;

        if (shouldroll)
        {
            shouldroll = false;

            if (rollh.pickASpot())
            {
                //    base.Roll();
                //   Debug.Log("found a spot");
                actuallyRoll();

                //find good roll spot && add force :)
            }
            else
            {


                if (rollh.tryCardinals())
                {
                    //     Debug.Log("cardinals");
                    //        base.Roll();
                    actuallyRoll();
                }
                else
                {
                    //     Debug.Log("movehim");
                    //unsure how safe this function is tbh
                    if (rollh.tryToMoveAnEnemy())
                    {
                        //       base.Roll();
                        rollh.enemyToCheck.Roll();
                        actuallyRoll();
                    }
                    else
                    {
                        //         Debug.Log("no spot available for other enemy");

                        if (canjumpkick(0))
                            StartCoroutine(jumpkick(0));

                        //do a jump attack etc or take the hit.
                    }
                }
                //make sure they roll away from us :)
            }
        }
    }

    void actuallyRoll()
    {
        rollcontent();
        state = enemyState.rolling;
        rb.isKinematic = false;
        attacking = false;
        if (rotating)
        {
            rotating = false;
            anim.SetBool("Rotate", false);
        }

        activateobs();

        bc[0].size = new Vector3(bc[0].size.x, bc[0].size.y, 0.05f);
        bc[1].size = new Vector3(bc[1].size.x, bc[1].size.y, 0.05f);
        if (transform.position.x + rollh.positionValue.x > transform.position.x)
        {
            if (!facingRight)
            {

                Flip();
            }
        }
        else if (transform.position.x + rollh.positionValue.x < transform.position.x)
        {

            if (facingRight)
            {

                Flip();
            }
        }

        rollh.positionValue = new Vector3(rollh.positionValue.x, 0, rollh.positionValue.z);
        rb.AddForce(rollh.positionValue * 5f, ForceMode.Impulse);

        if (blocking)
        {
            blocking = false;
            anim.SetBool("Block", false);
        }
    }

    public void disableobs()
    {
        for (int i = 0; i < 2; i++)
        {
            obs[i].enabled = false;
        }
    }

    public override void stopRoll()
    {
        base.stopRoll();
        rb.isKinematic = true;
        returnregularboxsize();
        StartCoroutine(endstatefunct());
    }

    public void returnregularboxsize()
    {
        bc[0].size = new Vector3(bc[0].size.x, bc[0].size.y, 0.15f);
        bc[1].size = new Vector3(bc[1].size.x, bc[1].size.y, 0.15f);
    }

    //for landing ground
    public void handleEndState()
    {
        if (!playerHit && state != enemyState.down)
        {
            StartCoroutine(hitground());
        }
    }

    IEnumerator hitground()
    {
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("LandGround") && !anim.GetCurrentAnimatorStateInfo(0).IsName("HangInAir"));
        StartCoroutine(endstatefunct());
    }

    public IEnumerator endstatefunct()
    {
        if (!nav.enabled)
        {
            disableobs();
            yield return null;
            nav.enabled = true;
        }
        if (enemyposinmanager.movepositions.x == -1)
        {
            Join();
        }
        else
        {
            ConfirmedApproach();
            StartCoroutine(tryToDoLiveMovement());
            //        anim.SetFloat("Input", 1);
        }
    }

    public IEnumerator endstatefunctattack()
    {
        attacking = false;
        yield return new WaitUntil(()=> anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        if (!nav.enabled)
        {
            disableobs();
            yield return null;
            nav.enabled = true;
        }
        if (enemyposinmanager.movepositions.x == -1)
        {
            Join();
        }
        else
        {
            ConfirmedApproach();
            StartCoroutine(tryToDoLiveMovement());
            //        anim.SetFloat("Input", 1);
        }
    }

    public IEnumerator allowRightSpot()
    {
        AllowRs = false;
        //run a for until player is knocked over or 10 secs have passed?
        rstime = 15f;
        while (rstime > 0f)
        {
            rstime -= Time.deltaTime;
            yield return null;
        }
        AllowRs = true;
    }

    public float leftorRight()
    {
        if (p.transform.position.x < transform.position.x)
        {
            return 1;
        }
        else
            return -1;
    }


    bool returnToApproachState()
    {
        tempEP = p.enemyM.returnClosestAvailablePosition(this);

        if (enemyposinmanager == tempEP)
        {
            if (!nav.enabled)
            {
                disableobs();
                nav.enabled = true;
            }
            return true;
        }
        else
        {
            //    removeFromSpot();
            return false;
        }
    }

    void resetStateBeforeRetaliating()
    {
        //set blocking to false in animator

        anim.SetBool("Block", false);
    }

   public void stepaside()
    {
        attacking = false;
        anim.SetBool("Attacking", false);
        if (blocking)
        {
            blocking = false;
            anim.SetBool("Block", false);
        }

        state = enemyState.spawn;
        if (nav.enabled)
        {
            nav.ResetPath();
            nav.velocity = Vector3.zero;
        }
        resetStateBeforeRetaliating();
        efunct.StartCoroutine(efunct.stepaside());
    }

    public void handleDodges(Player playerAttacking)
    {

        pAtt = playerAttacking;

        distxfromp = Mathf.Abs(transform.position.x - pAtt.transform.position.x);
        distyfromp = Mathf.Abs(transform.position.z - pAtt.transform.position.z);

        olp.time = 0;
        //WE NEED A LARGER THRESHOLD FOR ROLLING AWAY IF CROUCHING

        //if we are able to retaliate or move away

        chanceRoll = Random.value;
        //if we arent hit or rolling or grounded etc

        if (ifCanRetaliate())
        {


            if (chanceRoll * 100f <= chanceToReact)
            {

                //maybe need to check if player is doing a jump attack?

                //change our state for our knowledge/if checks
                if (crouch)
                {
                    // if (distxfromp > 1.65f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.5f && distx > 0.8f) || (distxfromp > 1f && distyfromp > 0.35f)))))
                    if (p.crouch)
                    {
                        if (distxfromp > 1.8f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.65f && distx > 1f) || (distxfromp > 1.54f && distyfromp > 0.54f)))))
                        {

                            //side step/stop in your spot?
                            stepaside();
                        }
                        else
                        {
                            retaliate();
                        }
                    }
                    else
                    {
                        if (distxfromp > 1.65f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.5f && distx > 0.9f) || (distxfromp > 1f && distyfromp > 0.35f)))))
                        {
                            stepaside();
                            //side step/stop in your spot?
                        }
                        else
                        {
                            retaliate();
                        }

                    }
                }
                else
                {
                    if (p.crouch)
                    {
                        if (distxfromp > 1.8f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.65f && distx > 1f) || (distxfromp > 1.54f && distyfromp > 0.54f)))))
                        {
                            //      Debug.Log(Vector3.Distance(transform.position, pAtt.transform.position));
                            //       Debug.Log("sidestep");
                            stepaside();
                            //side step/stop in your spot?
                        }
                        else
                        {
                            //                   Debug.Log("retal");
                            resetStateBeforeRetaliating();
                            retaliate();
                        }
                    }
                    else
                    {
                        if ((distxfromp > 0.5f && distyfromp > 0.5f) || distxfromp >= 1.25f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.5f && distx > 0.8f) || (distxfromp > 1f && distyfromp > 0.35f) || (distxfromp > 0.6f && distyfromp > 0.6f)))))
                        {
                            //      Debug.Log(Vector3.Distance(transform.position, pAtt.transform.position));
                            //       Debug.Log("sidestep");
                            stepaside();
                            //side step/stop in your spot?
                        }
                        else
                        {
                            //                   Debug.Log("retal");
                            retaliate();
                        }
                    }
                }
            }
        }
    }

    //  public abstract void implementme();


    //incomplete
    bool ifCanRetaliate()
    {
        //if not blocking obviously..

        if (state!=enemyState.attacking && state!=enemyState.sidestep && grounded && !playerHit && state != enemyState.down && anim.GetBool("Blocked") == false && !rolling
            && state != enemyState.jumping && state != enemyState.flinching && state != enemyState.stayaway && ((!attacking && !rotating) || (attacking && rotating)))
            return true;
        else
        {
            // Debug.Log("cant retal");
            return false;
        }

    }

    public void ResetBlock()
    {
        if (blocking)
            Block();
    }

    public void blockhit()
    {
        if (!blocking)
        {
            state = enemyState.blocking;
            if (nav.enabled)
            {
                nav.ResetPath();
                nav.velocity = Vector3.zero;
                nav.enabled = false;
                activateobs();
            }
            Block();
        }
    }

    public virtual void retaliate()
    {
        //  Debug.Log(Vector3.Distance(transform.position, pAtt.transform.position));
        //  1.136486f
        //  1.11f

      /*  if(pAtt.jumping)
        {
            //idk if thats what i want exactly

            Roll();
            return;
        }*/

        if (pAtt.crouch)
        {
            if (crouch)
            {

                switch (pAtt.es.myHE.attackPos)
                {
                    case 1://ignore or attack back since player is uppercutting

                        returnToIdle();
                       // dorandomattack()

                        break;
                    case 2://block since can block punch
                        blockhit();
                        break;
                    case 3://roll away/jump since player kick ********************** Very important used to be just roll

                        //add either jump or roll based of dist etc make
                        if (distxfromp > 0.95f)
                        {
                            if (efunct.returnUsableSpots() >= 1)
                            {
                                //alternate between since always does roll?

                                Jump();
                            }
                            else
                                Roll();
                        }
                        else
                            Roll();

                        break;
                    case 5://enemy is jump kicking
                        Roll();
                        break;
                }
            }
            else
            {
                //
                //enemy is standing
                //

                switch (pAtt.es.myHE.attackPos)
                {
                    case 1:
                        //block since player is uppercutting

                        //smart ai may be able to crouch? :D

                        //if smart ai and didnt block
                        Debug.Log("crouch");
                        if (smartAi)
                        {
                            Crouch();
                            returnToIdle();
                        }
                        else
                        {
                            //else not smart ai. so block or take the hit?
                            blockhit();
                        }

                        break;

                    case 2:// player punches, sidestep (if far)?/ Roll
                        Roll();
                        //crouch and block? 


                        break;
                    case 3:// player kicks, roll away/ Jump?


                        //decide if we should counter by a jump kick or etc?

                        //otherwise avoid by a jump or roll. No places to roll to would lead to a jump kick

                        //if far enough but not too far, make jump

                        //used to be 0.6f
                        if (distxfromp > 0.9f)
                        {
                            if (efunct.returnUsableSpots() >= 1)
                            {
                                Jump();
                            }
                            else
                            {
                                //check all possible spaces to roll to, if there is an enemy there try to make them move so we can roll to their spot
                                Roll();
                            }

                        }
                        else
                        {
                            Roll();
                        }

                        break;
                    case 5://enemy is jump kicking
                           //we can block it?

                        Roll();
                        break;

                }
            }
        }
        else
        {
            //
            //player is standing
            //

            //maybe need to check if player is doing a jump attack?

            if (crouch)
            {
                if (pAtt.es.myHE.attackPos == 5)
                {
                    //player is doing a jump kick 
                    Roll();
                }
                else if (pAtt.es.myHE.attackPos >= 3)
                {
                    //Block since player does a low hit
                    blockhit();
                }
                else
                {
                    //ignore or attack? since its a high hit and enemy is crouched

                    returnToIdle();
                }

            }
            else
            {

 

                //
                //enemy is standing
                //
                if (pAtt.es.myHE.attackPos == 5)
                {
                    //player is doing a jump kick, should block?
                    Roll();
                   // Block();
                }
                else if (pAtt.es.myHE.attackPos >= 3)
                {
                    // player does a low attack, so Roll away?
                    Roll();

                    //maybe crouch and block? possibly?
                }
                else
                {
                    if (pAtt.es.myHE.attackPos != 1)
                    {
                        //smart ai may be able to crouch before the attack since its not a med kick
                        if (smartAi)
                        {
                            Crouch();
                            returnToIdle();
                        }
                        else
                        {
                            blockhit();
                        }
                        //can also block attack?
                    }
                    else
                    {
                        //Block since we have to, cant crouch to avoid it
                        blockhit();
                    }
                }
            }
        }
        if(!blocking)
            resetStateBeforeRetaliating();

    }


    bool livespotCast()
    {

        wanderray = new Ray(new Vector3(transform.position.x, 0.7f, transform.position.z), new Vector3(livespotref.x, 0.7f, livespotref.z) - new Vector3(transform.position.x, 0.7f, transform.position.z));
        //    wallRay = new Ray(new Vector3(transform.position.x, 0.7f, transform.position.z), new Vector3(livespotref.x, 0.7f, livespotref.z) - new Vector3(transform.position.x, 0.7f, transform.position.z));
        wallRay = wanderray;

        if (!Physics.Raycast(wanderray, out wanderhit, Vector3.Distance(transform.position, livespotref), objectsnplayer))
        {

            if (!Physics.Raycast(wallRay, out wallcheck, 1f, wallsOnly))
            {
                //do one more raycast check for walls so that the min distance between ourself and a wall is greater than 0.5f

                spotcols = Physics.OverlapSphere(new Vector3(livespotref.x, 0.7f, livespotref.z), 0.5f, objectsnplayer);

                if (spotcols.Length < 1)
                {
                    //   Debug.Log("found a spot");
                    state = enemyState.livemovement;
                    //not sure if i need these ********
                    nav.ResetPath();
                    nav.velocity = Vector3.zero;

                    ///////////
                    nav.destination = livespotref;

                    return true;
                }
            }
        }

        return false;
    }


    bool checkliveSpots()
    {
        shuffleSpots();
        liveposAlt = new List<Vector3>();
        for (int i = 0; i < livePosition.Count; i++)
        {
            if (enemyposinmanager.movepositions.z == 0.1f && livePosition[i].z == 1f)
                liveposAlt.Add(livePosition[i]);
            else if (enemyposinmanager.movepositions.z == -0.1f && livePosition[i].z == -1f)
            {
                liveposAlt.Add(livePosition[i]);
            }
        }
        for (int i = 0; i < liveposAlt.Count; i++)
        {
            livespotref = returnASpotCloseToOurSelectedSpot(liveposAlt[i]);
            if (livespotCast())
                break;

        }

        if (state == enemyState.livemovement)
            return true;
        else    //try to pick a spot closer to the side you have to go to
            for (int i = 0; i < livePosition.Count; i++)
            {
                //only check spots that werent checked :)

                if (!liveposAlt.Contains(livePosition[i]))
                {
                    livespotref = returnASpotCloseToOurSelectedSpot(livePosition[i]);

                    if (livespotCast())
                        break;
                }

            }
        if (state == enemyState.livemovement)
            return true;
        else
            return false;
    }

    Vector3 returnASpotCloseToOurSelectedSpot(Vector3 vec)
    {

      /*  if (vec.z > 0)
        {
            fu = 1;
        }
        else
        {
            fu = -1;
        }*/
        //return new Vector3(Mathf.Clamp(transform.position.x + (vec.x * collisionParent.localScale.x), cam.transform.position.x - 3.5f, cam.transform.position.x + 3.5f), 1.32f, Random.Range(Mathf.Clamp(transform.position.z + vec.z, -2f, 2f), Mathf.Clamp(transform.position.z + vec.z + (fu), -2f, 2f)));

        return new Vector3(Mathf.Clamp(Random.Range(transform.position.x + vec.x, transform.position.x + (vec.x * 2f)),
                   cam.transform.position.x - 3.5f, cam.transform.position.x + 3.5f), 1.32f, Mathf.Clamp(Random.Range(transform.position.z + vec.z, transform.position.z + vec.z * 2f), -2f, 2f)); ;

        /*     else
             {
                 return new Vector3(Mathf.Clamp(Random.Range(transform.position.x + (vec.x * 1f), transform.position.x + ((vec.x + 0.5f) * collisionParent.localScale.x)),
                     cam.transform.position.x - 3.5f, cam.transform.position.x + 3.5f), 1.32f, Random.Range(Mathf.Clamp(transform.position.z + vec.z, -2f, 2f),
                     Mathf.Clamp(transform.position.z + vec.z + (fu), -2f, 2f)));

             }*/
    }


    IEnumerator waitBeforeCanRepeatLive()
    {
        yield return new WaitForSeconds(3f);
        canTry = 0;
    }

    float chanceToSideStep()
    {
        if (rightSpot)
            return 10f;

        if (transform.position.x > player.position.x)
        {
            if (enemyposinmanager.whichOne > 1)
            {
                //  return (enemyposinmanager.whichOne * 5f);
                return 5f;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (enemyposinmanager.whichOne > 1)
            {
                return 0;
            }
            else
            {
                return 5f;
            }
        }
    }


    public bool areWeWithinCameraBounds()
    {
        if (transform.position.x > cam.transform.position.x + 3.5f || transform.position.x < cam.transform.position.x - 3.5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    float anotheroll;

    public IEnumerator tryToDoLiveMovement()
    {
        if (!dynamic)
        {
            dynamic = true;

            while (state == enemyState.approach)
            {
                yield return new WaitForSeconds(0.35f);

                if (state == enemyState.approach)
                {
                    if (areWeWithinCameraBounds())
                        if (Vector3.Distance(transform.position, spot) > 1f && Vector3.Distance(transform.position, spot) < 10f && canTry == 0)
                        {
                            liveSpotPercent = Random.value * 100f;

                            //if we are to the right and our spot is to the left, give a higher chance of side stepping? and vice verse


                            if (liveSpotPercent - chanceToSideStep() <= chanceToGoLiveSpot)
                            {
                                //change state and go to our spot

                                if (rightSpot)
                                {
                                    //roll again to see if its gonna be a regular livemovement or an alt movement

                                    anotheroll = Random.value * 100f;

                                    if (anotheroll < 50f)
                                    {

                                        if (checkliveSpots())
                                        {
                                            Debug.Log("reg");

                                            canTry = 1;
                                            olp.time = 0;
                                            StartCoroutine(waitBeforeCanRepeatLive());
                                            //  nav.velocity = Vector3.zero;

                                            break;
                                        }
                                        else
                                        {
                                            dynamic = false;
                                        }
                                    }
                                    else
                                    {
                                        //we arent side stepping, so just assign a spot near our target  so its easy to go to other side of him
                                        if (efunct.alternativeSideSpot()) //alt sidespot** is kinda rushed towards the spot
                                        {
                                            Debug.Log("alt");

                                            state = enemyState.livemovement;
                                            //not sure if i need these ********
                                            nav.ResetPath();
                                            //          nav.velocity = Vector3.zero;

                                            ///////////
                                            nav.destination = livespotref;
                                            canTry = 1;
                                            olp.time = 0;

                                            StartCoroutine(waitBeforeCanRepeatLive());
                                            break;
                                        }
                                        else
                                        {
                                            dynamic = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (checkliveSpots())
                                    {
                                        canTry = 1;
                                        olp.time = 0;
                                        StartCoroutine(waitBeforeCanRepeatLive());
                                        //  nav.velocity = Vector3.zero;

                                        break;
                                    }
                                    else
                                    {
                                        dynamic = false;
                                    }

                                }

                            }
                        }
                        else
                        {
                            //if not live movement? smaller pattern? idk?
                            //   if(state!= enemyState.livemovement || state!=)
                            //   yield break;

                        }
                }
            }

            if (state == enemyState.spawn || state == enemyState.idle || state == enemyState.attacking)
                dynamic = false;
        }
        else
        {
            timesWentDynamic++;

            //this should be  a rand num?

            if (timesWentDynamic > 3)
            {
                resetdynamic();
            }

            //since we cant do live movement, decide if should go directly to decision making tree, jump attack, etc.? based off amt of enemies approaching him?
        }
    }

    public void handleRotationEnemy()
    {
        if (state != enemyState.down && !playerHit && !rolling && !rotating && state != enemyState.sidestep && state != enemyState.attacking && !jumping && state != enemyState.walkconstantly)
            if (canrotate || closeenough() || p.rolling || Mathf.Abs(transform.position.x - player.position.x) > 1f)
                if (player.transform.position.x > transform.position.x)
                {
                    if (!facingRight)
                    {
                        StartCoroutine(rotate());
                    }
                    else
                    {
                        //  Flip();
                    }
                }
                else
                {
                    if (facingRight)
                    {
                        StartCoroutine(rotate());
                        //     Flip();
                    }
                }
    }

    IEnumerator rotate()
    {
        if (nav.enabled)
        {
            nav.isStopped = true;

        }
        canrotate = false;
        rotating = true;
        attacking = true;
        anim.SetBool("rotate", true);
        nav.velocity = Vector3.zero;
        rotatecooldown = 3f;
        cc.resetStates();
        cc.actions.Clear();
        while (rotatecooldown > 0f)
        {
            rotatecooldown -= Time.deltaTime;

            //needs to be more to this..
            // temp = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];

            //should check if we switched target somehow or something?

            if (Vector3.Distance(transform.position, spot) < 1.5f)
                break;

            yield return null;
        }
        canrotate = true;
    }


    private void FixedUpdate()
    {
        //if player or wandering?

        // if (nav.enabled && nav.velocity == Vector3.zero)
        //      anim.SetFloat("Input", 0);

        if (player)
            handleRotationEnemy();

    }

    public bool facingUs()
    {
        if ((p.facingRight && p.transform.position.x < transform.position.x) || (!p.facingRight && p.transform.position.x > transform.position.x))
        {
            return true;
        }
        else
            return false;
    }


    public void raycastCheck()
    {

        //if it is, raycast three times to the front of enemy, and 2 diagonal to it within the hit radius
        rayhit = false;

        for (int i = -1; i < 2; i++)
        {
            ray = new Ray(transform.position, (transform.position + new Vector3(-0.6f * -collisionParent.localScale.x, 0, (i / 20f) * 1.1f)) - transform.position);

            Debug.DrawRay(ray.origin, ray.direction * efunct.raydist(), Color.blue, 3f);

            if (Physics.Raycast(ray, out hit, efunct.raydist(), playerMask))
            {

                if (hit.collider != null)
                {
                    rayhit = true;
                    //  Debug.Log("hit something");
                    //change state

                    //break out
                    break;
                }
            }
        }
    }


    public bool closeenough()
    {
        distx = Mathf.Abs(transform.position.x - spot.x);
        distz = Mathf.Abs(transform.position.z - spot.z);

        if (crouch || p.crouch)
        {
            //.85f at the end changed

            //if ((nav.remainingDistance <= 0.025f && distx < 0.2f) ||  (Mathf.Abs( transform.position.z -spot.z)<0.05f && distx < 0.5f ))


            //player is crouched
            if (!crouch)
            {
                //used to be 0.4f for distx

                if (facingUs())
                {
                    if ((distz <= 0.01f && distx < 0.19f && transform.position.x - spot.x < 0) || (distz <= 0.01f && Mathf.Abs(transform.position.x - player.position.x) > 0.94f && distx < 0.19f))
                    {
                        //     Debug.Log("close enough");
                        return true;
                    }
                }
                else
                {
                    if ((distz <= 0.01f && distx < 0.19f && transform.position.x - spot.x >= 0) || (distz <= 0.01f && Mathf.Abs(transform.position.x - player.position.x) > 0.87f && distx < 0.19f))
                    {
                        //    Debug.Log("close enough");
                        return true;
                    }
                }
            }
            else if (crouch && !p.crouch)
            {
                if ((distz <= 0.011f && distx < 0.01f) || (distz <= 0.024f && Mathf.Abs(transform.position.x - player.position.x) > 0.3f && distx < 0.73f))
                    return true;

            }
            else
            {
                if ((distz <= 0.011f && distx < 0.01f) || (distz <= 0.024f && Mathf.Abs(transform.position.x - player.position.x) > 0.93f && distx < 0.4f))
                    return true;
            }
        }
        else
        {
            if ((distz <= 0.011f && distx < 0.01f) || (distz <= 0.03f && Mathf.Abs(transform.position.x - player.position.x) > 0.3f && Mathf.Abs(transform.position.x - player.position.x) < 0.72f
                && distx < 0.55f) || (distz <= 0.024f && distx < 0.5f && Mathf.Abs(transform.position.x - player.position.x) < 0.72f))
                return true;
        }

        return false;
    }

    void approach()
    {
        //1.32f
        assignSpot();

        //check if distance between us and our spot isnt far.
        // if (nav.remainingDistance <= 0.025f)
        if (closeenough())
        {
            raycastCheck();

            if (!rayhit)
            {
                //move closer? or wander?
                nav.destination = spot;
            }
            else
            {

                returnToIdle();
            }
        }
        else
        {
            //check for last time we did a fight shuffle?

            //give chance to shuffle if we havent in a while.?

            //move closer if the spot position changed and we dont need to find a closer spot
            //if (!checkIfWeShouldFindCloserSpot())
            if (oldSpot != spot)
            {

                nav.destination = spot;
                oldSpot = spot;
            }

        }

    }

    public override void Crouch()
    {
        base.Crouch();
    }

    //also accounts for objects like crates etc
    bool checkifsomethingisinourspot()
    {
        //used to be .115f
        spotcols = Physics.OverlapSphere(new Vector3(spot.x, 0.8f, spot.z), 0.115f, objects);
        //  spotcols = Physics.OverlapBox(new Vector3(spot.x, 0.8f, spot.z), new Vector3(1, 1, 0.115f), Quaternion.identity, enemiesnobjs);

        if (spotcols.Length > 0)
        {
            return true;
        }
        /*     for (int i = 0; i < spotcols.Length; i++)
             {

                 //add and not wandering
                 if (spotcols[i].transform.root.GetComponent<Enemy>() != null)
                 {
                     refObj = spotcols[i].transform.root.GetComponent<Enemy>();

                     //distance originally was .2f
                     if (refObj != this  && refObj.enemyposinmanager.movepositions.x==-1 && (refObj.state != enemyState.wander && refObj.state != enemyState.walkingtospot && refObj.state != enemyState.down))
                     {
                         //leave our spot;

                         return true;
                     }
                 }
                 else
                 {
                     return true;
                 }
             }*/


        return false;
    }

    Player aref;


    public void assignSpot()
    {

        if (!crouch && !p.crouch)
            spot = new Vector3((enemyposinmanager.movepositions.x + (enemyposinmanager.movepositions.x * 0.18f)) + player.transform.position.x, player.transform.position.y, enemyposinmanager.movepositions.z + player.transform.position.z);
        else if (p.crouch && crouch)
        {
            //spot = new Vector3(enemyposinmanager.movepositions.x + player.transform.position.x + (Mathf.Sign(valuesub()) / 3f), player.transform.position.y, enemyposinmanager.movepositions.z + player.transform.position.z);
            spot = new Vector3(enemyposinmanager.movepositions.x + player.transform.position.x + (efunct.valuesubsmaller()), player.transform.position.y, enemyposinmanager.movepositions.z + player.transform.position.z);
        }
        else
        {
            //players crouched
            if (p.crouch)
            {
                spot = new Vector3(enemyposinmanager.movepositions.x + player.transform.position.x + (efunct.valuesubsmaller()), player.transform.position.y, enemyposinmanager.movepositions.z + player.transform.position.z);


            }
            else if (crouch) //enemy crouched
            {
                spot = new Vector3((enemyposinmanager.movepositions.x + (efunct.valuesub() / 2f)) + player.transform.position.x, player.transform.position.y, enemyposinmanager.movepositions.z + player.transform.position.z);
            }
        }
    }

    void approachOrMoveOtherSide()
    {
        if (state != enemyState.approach)
            if (!sideStep)
            {
                if (rightSpot)
                {
                    //check if we can travel there are there migth be an obstacle :)

                    if (liveSpotPercent - (chanceToSideStep() * 2f) <= 50)
                    {
                        secondTravelPath = new Vector3(Mathf.Clamp(Random.Range(player.transform.position.x + leftOfEnemy(), player.transform.position.x + (leftOfEnemy() * 2)), cam.transform.position.x - 3.5f,
                            cam.transform.position.x + 3.5f), 1.32f, Mathf.Clamp(transform.position.z + Random.Range(-0.2f, 0.2f), -2f, 2f));

                        spotcols = Physics.OverlapSphere(new Vector3(secondTravelPath.x, 0.8f, secondTravelPath.z), 0.2f, objects);

                        while (spotcols.Length > 0)
                        {
                            secondTravelPath = new Vector3(Mathf.Clamp(Random.Range(player.transform.position.x + leftOfEnemy(), player.transform.position.x + (leftOfEnemy() * 2)), cam.transform.position.x - 3.5f,
                                cam.transform.position.x + 3.5f), 1.32f, Mathf.Clamp(transform.position.z + Random.Range(-0.2f, 0.2f), -2f, 2f));
                            spotcols = Physics.OverlapSphere(new Vector3(secondTravelPath.x, 0.8f, secondTravelPath.z), 0.1f, objects);
                        }

                        nav.destination = secondTravelPath;
                        sideStep = true;
                    }
                    else
                    {

                        //ConfirmedApproach();
                        if (!leftprocastinate)
                            decisionmakingtree();
                        else
                        {
                            ConfirmedApproach();
                            StartCoroutine(tryToDoLiveMovement());

                        }

                        /* tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

                         if (enemyposinmanager == tempEP)
                         {

                             nav.destination = spot;
                             state = enemyState.approach;
                         }
                         else
                         {
                             Debug.Log("not closest");
                             state = enemyState.spawn;
                             removeFromSpot();
                         }*/
                    }
                }
                else
                {
                    //chance to wait?

                    assignSpot();

                    //check if spot is left or right of player too in reference to be more likely to do something else
                    //   if (ShouldWeWait() )
                    if (ShouldWeWait())
                    {
                        //either literally wait if far enough, otherwise go to decision making tree

                        if (Vector3.Distance(player.position, transform.position) > 2f)
                        {

                            timeTowait = Mathf.Clamp(Random.value * 3f, 1f, 3f);
                            timer = 0;
                            anim.SetFloat("Input", 0);
                            state = enemyState.wait;
                        }
                        else //not far enough to wait
                        {
                            //go to decision make tree *** IMPORTANT

                            //for time being i will leave it as it was to wait

                            decisionmakingtree();
                        }
                    }
                    else
                    {
                        //dont wait but go to decision make tree *** IMPORTANT or decide to approach from that
                        decisionmakingtree();

                        /*   if (returnToApproachState())
                           {

                               nav.destination = spot;
                               state = enemyState.approach;
                           }
                           else
                           {
                               removeFromSpot();
                           }*/
                    }
                }
            }
            else
            {
                //chancetowait or decision making tree? 

                //for time being its told to approach

                sideStep = false;

                decisionmakingtree();
            }
    }

    [HideInInspector]
    public int dmt;

    public void dmtfunct()
    {
        dmt = 0;

        for (int i = 0; i < 4; i++)
        {
            //what about checking if the player is currently hurt?

            //check # of enemies either approaching or attacking?

            if (p.enemyM.positions[i].enemy != null && p.enemyM.positions[i].enemy != this)
            {
                //check if they're approaching too?     **IMPORTANT********************************************************************************* 

                //maybe should check for seperate things and assign values to each?

                if ((p.enemyM.positions[i].enemy.state == enemyState.attacking && (p.playerHit || p.blocked))
                    || p.enemyM.positions[i].enemy.state == enemyState.approach
                    || p.enemyM.positions[i].enemy.state == enemyState.approachthenjumpkick )
                { 
                    //add it to a value?
                    dmt++;
                }
            }
        }
    }

    public void decisionmakingtree()
    {
        dmtfunct();

        treedecisions();
    }

    public void treedecisions()
    {
        rand = Random.value;

        if (rand * 100f < decisionfloat())
        {

            //walk towards player and then jump kick (if far away in x and decent in z? && spot is not on opposite side of player

            //need a random chance to jump kick?
            if (canapproachthenjumpkick() && checkjumpkicks() )
            {


                if (!nav.enabled)
                {
                    StartCoroutine(resetNav());
                }
                assignSpot();
                nav.destination = spot;
                state = enemyState.approachthenjumpkick;
                anim.SetFloat("Input", 1);
            }
            else
            {
                ConfirmedApproach();
                StartCoroutine(tryToDoLiveMovement());
            }
        }
        else
        {
            //temporary?
            // ConfirmedApproach();
            Debug.Log("how the" + state);
            procastinateStates();

            //choose any one of the others?

            //if its only one enemy, tend to return to approach******* otherwise check how many are doing what?

            //maybe?
            // leave spot with a random enemy that isnt attacking if theres more than 4 enemies in game manager?

            //walk constantly pattern with no waiting

            //walk & wait pattern

            //walk 2-3x then wait pattern?

            //walk one spot then approach/decide? (try to make spot be on side we want to be at?)
        }
    }

    float distbasedoffstate()
    {
        switch (state)
        {
            case enemyState.walkconstantly:
              
                return 0.75f;
            case enemyState.walknwait:
      
                return 1.25f;
               
            case enemyState.twothreewait:
            
                return 0.75f;
            case enemyState.onemore:
                
                return 2f;
          
        }

        return 0;
    }

    void pickspottowalkto()
    {
        anim.SetFloat("Input", 0);
        wanderspot = RandomNavSphere(3f, 1);

        while ((efunct.tooclosetoplayer() || Vector3.Distance(transform.position, new Vector3(wanderspot.x, transform.position.y, wanderspot.z)) < distbasedoffstate())
            && (wanderspot.x < cam.transform.position.x + 3.5f && wanderspot.x > cam.transform.position.x - 3.5f))
        {

            wanderspot = RandomNavSphere(3f, 1);

            //    wanderray = new Ray(transform.position, new Vector3(wanderspot.x, 0.8f, wanderspot.z) - transform.position);

            //  cols = Physics.OverlapSphere(new Vector3(wanderspot.x, 0.8f, wanderspot.z), 0.3f, enemyCol);

        }
        anim.SetFloat("Input", 1);
        nav.destination = wanderspot;
    }

    int somevalue;

    public void pickoneofotherstates(int num)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        somevalue = Random.Range(16, 20);
        while(somevalue== num)
        {
            somevalue = Random.Range(16, 20);
        }

        state = enemyState.twothreewait;
        pickspottowalkto();
        timeswalked = 0;

        //     Random.InitState(System.DateTime.Now.Millisecond);
        //  numbertowalk = Random.Range(2, 6);
        lastprocastinatestate = (enemyState)somevalue;
        state = (enemyState)somevalue;
        switch (state)
        {
            case enemyState.walkconstantly:
                //any of other 3
                numbertowalk = Random.Range(2, 6);
                break;
            case enemyState.walknwait:
                //1, 3 n 4
                numbertowalk = Random.Range(2, 4);
                break;
            case enemyState.twothreewait:
                //1,2,4
                numbertowalk = Random.Range(2, 4);
                twothree = 0;
                twoorthree = Random.Range(2,4);
                break;
            case enemyState.onemore:
                //1-3
                numbertowalk = 1;
                break;
        }

    }

    void pickaprocastinatestate()
    {
        switch (lastprocastinatestate)
        {
            case enemyState.walkconstantly:
                //any of other 3
                pickoneofotherstates(16);
                break;
            case enemyState.walknwait:
                //1, 3 n 4
                pickoneofotherstates(17);
                break;
            case enemyState.twothreewait:
                //1,2,4
                pickoneofotherstates(18);
                break;
            case enemyState.onemore:
                //1-3
                pickoneofotherstates(19);
                break;
        }
    }

    public void procastinateStates()
    {

        if (!leftprocastinate)
        {
            pickaprocastinatestate();
        }
        else
        {
            ConfirmedApproach();
            StartCoroutine(tryToDoLiveMovement());
        }
    }

    bool checkjumpkicks()
    {
        for (int i = 0; i < 4; i++)
        {
            if (p.enemyM.positions[i].enemy != null)
            {
                //if anyone jumped recently? or someone currently jumping
                if (p.enemyM.positions[i].enemy.state == enemyState.approachthenjumpkick || p.enemyM.positions[i].enemy.jumpkicked)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public float decisionfloat()
    {
        //      waitvalue = p.enemyM.numberOfEnemies()/100f;

        //wait value used to be just 
        if (p.enemyM.numberOfEnemies() > 1)
            waitvalue = Mathf.Clamp((p.enemyM.numberOfEnemies() - 1) * 33.33f, 10f, 100f);
        else
            waitvalue = Mathf.Clamp((p.enemyM.numberOfEnemies() - 1) * 33.33f, 80f, 100f);
        return waitvalue - (30f * dmt);
    }

    bool canapproachthenjumpkick()
    {
        //unsure if onrightside is needed, may just assign spot as player instead of spot 

        if (onrightside())
            return true;

        return false;
    }

    bool onrightside()
    {
        switch (enemyposinmanager.whichOne)
        {
            case 0:
                if (transform.position.x > player.position.x && transform.position.z < player.position.z)
                {
                    return true;
                }

                break;
            case 1:
                if (transform.position.x > player.position.x && transform.position.z > player.position.z)
                {
                    return true;
                }
                break;
            case 2:
                if (transform.position.x < player.position.x && transform.position.z < player.position.z)
                {
                    return true;
                }
                break;
            case 3:

                if (transform.position.x < player.position.x && transform.position.z > player.position.z)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    public void ConfirmedApproach()
    {
        Debug.Log("?why");
        assignSpot();
        nav.destination = spot;
        state = enemyState.approach;
        anim.SetFloat("Input", 1);
    }

    bool ShouldWeWait()
    {
        chanceToWait = Random.value;

        if (chanceToWait * 100f < enemyWaitChanceMultiplier())
        {
            waiting = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    float enemyWaitChanceMultiplier()
    {
        //      waitvalue = p.enemyM.numberOfEnemies()/100f;
        waitmultiplier = 0;

        //wait value used to be just 
        //  waitvalue = (p.enemyM.numberOfEnemies() - 1) * 25f;

        waitvalue = Mathf.Clamp((p.enemyM.numberOfEnemies() - 1) * 27f, 15f, 81f);
        for (int i = 0; i < 4; i++)
        {

            if (p.enemyM.positions[i].enemy != null && p.enemyM.positions[i].enemy.waiting)
                waitmultiplier++;
        }

        if (p.enemyM.numberOfEnemies() > 3)
            return waitvalue - (32.5f * waitmultiplier);
        else
        {
            return waitvalue - (35f * waitmultiplier);
        }
    }

    public void checkApproach()
    {

        //this is new code, just to make sure to not approach if there are 3 enemies already going for him.

        dmtfunct();
        if(dmt==3)
        {
            leftprocastinate = false;

            decisionmakingtree();
            return;
        }

        //continue after player we are targetting

        //also check if there is a closer available pos to us

        if (!rightSpot)
        {


            tempEP = p.enemyM.returnClosestAvailablePosition(this);
            if (!checkifsomethingisinourspot() && enemyposinmanager == tempEP)
            {

                approach();
            }
            else
            {
                removeFromSpot();
            }
        }
        else
        {


            if (enemyposinmanager.whichOne < 2)
            {
                if (transform.position.x > p.transform.position.x)
                {

                    tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

                    if (!checkifsomethingisinourspot() && enemyposinmanager == tempEP)
                    {

                        approach();
                    }
                    else
                    {
                        removeFromSpot();
                    }
                }
                else
                    approach();

            }
            else
            {
                if (transform.position.x < p.transform.position.x)
                {
                    tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

                    if (!checkifsomethingisinourspot() && enemyposinmanager == tempEP)
                    {

                        approach();
                    }
                    else
                    {
                        removeFromSpot();
                    }
                }
                else
                    approach();
            }
        }
        //old code 

        /*    tempEP = p.enemyM.returnClosestAvailablePosition(this);
            if (!checkifsomethingisinourspot() && enemyposinmanager == tempEP)
            {

                approach();
            }
            else
            {
                Debug.Log("rejoining");
                removeFromSpot();
            }

            }*/
    }

    public IEnumerator resetNav()
    {
        disableobs();
        yield return null;
        nav.enabled = true;
    }

    void activateobs()
    {
        nav.enabled = false;
        enableallobs();
    }

    public void enableallobs()
    {

        for (int i = 0; i < 2; i++)
        {
            obs[i].enabled = true;
        }
    }

    private void Update()
    {
        //go away state code

        /*  p = GameManager.instance.players[0];
          player = p.transform;
          StartCoroutine(resetNav());
          state = enemyState.stayaway;*/

        if (Input.GetKeyDown(KeyCode.L))
        {
            Join();

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            leaveSpot();
        }

        switch (state)
        {
            //when we enter this state, set last distance between us to a high value

            case enemyState.stayaway:
                distanceBetweenUs = Vector3.Distance(transform.position, player.position);
                distx = Mathf.Abs(transform.position.x - player.transform.position.x);
                distz = Mathf.Abs(transform.position.z - player.transform.position.z);


                if (distx <= 2.2f || distanceBetweenUs < 2.1f)
                {
                    if (distanceBetweenUs < 1f)
                    {
                        if (enemyposinmanager.movepositions.x == -1)
                        {
                            Join();
                        }
                        else
                        {
                            ConfirmedApproach();
                            StartCoroutine(tryToDoLiveMovement());
                        }
                    }
                    if (!backup)
                    {
                        nav.ResetPath();

                        efunct.findLocation();

                        if (Vector3.Distance(transform.position, efunct.safeLocation) > 0.1f && cango)
                        {

                            nav.destination = efunct.safeLocation;
                            anim.SetFloat("Input", 1f);
                            backup = true;


                        }
                    }
                    else
                    {

                        //    if (distx < 1.75f && Vector3.Distance(transform.position, player.transform.position) <3f && nav.remainingDistance < 0.5f)  
                        if (distx < 1.75f && distanceBetweenUs < 3f && nav.remainingDistance < 0.5f)
                        {

                            backup = false;
                            anim.SetFloat("Input", 1f);

                            //  anim.SetFloat("Input", 0f);
                            //             Debug.Log("backupfalse");
                            //     backup = false;
                        }
                        else
                        {
                            if (nav.remainingDistance < 0.01f)
                            {

                                anim.SetFloat("Input", 0f);
                            }
                            else if (approaching)
                            {
                                approaching = false;
                                nav.ResetPath();
                                nav.velocity = Vector3.zero;
                                backup = false;
                            }
                        }

                    }

                }
                else if (distx >= 2.5f || Vector3.Distance(transform.position, player.transform.position) > 3f)
                {
                    approaching = true;

                    if (nav.remainingDistance < 0.5f)
                    {
                        //       anim.SetFloat("Input", 0f);
                        if (player.transform.position.x < transform.position.x)
                        {
                            altSpot = new Vector3(Mathf.Clamp(player.transform.position.x + 2.5f, player.transform.position.x + 2.5f, cam.transform.position.x + 3.5f), 1.32f, transform.position.z);
                        }
                        else
                        {
                            altSpot = new Vector3(Mathf.Clamp(player.transform.position.x - 2.5f, cam.transform.position.x - 3.5f, player.transform.position.x - 2.5f), 1.32f, transform.position.z);
                        }

                        //    if (distanceBetweenUs > lastDistanceBetweenUs)
                        //   {
                        //  lastDistanceBetweenUs = distanceBetweenUs;
                        if (Vector3.Distance(transform.position, altSpot) > 1f && distanceBetweenUs > 3f)
                        {
                            anim.SetFloat("Input", 1f);
                            nav.destination = altSpot;
                        }
                        else
                        {
                            if (nav.remainingDistance < 0.01f)
                            {
                                nav.ResetPath();
                                nav.velocity = Vector3.zero;

                                anim.SetFloat("Input", 0f);
                            }
                        }
                    }
                }

                break;

            case enemyState.sidestep:

                if (nav.remainingDistance < 0.1f)
                {
                    //check approach basically but slightly adjusted

                    if (!checkifsomethingisinourspot() && enemyposinmanager == tempEP && !p.floored)
                    {
                        if (p.enemyM.numberOfEnemies() == 1 || Random.value * 100f < 50f)
                        {


                            ConfirmedApproach();

                            //unsure about this******************************************************
                            StartCoroutine(tryToDoLiveMovement());
                        }
                        else
                        {
                            decisionmakingtree();
                        }
                        return;
                    }
                    else
                    {
                        removeFromSpot();
                        return;
                    }
                }
                //dont want a huge box width here
                //  watchforplayer(1f);

                break;

            case enemyState.approach:

                checkApproach();
                break;

            case enemyState.idle:
                raycastCheck();
                if (rayhit)
                {
                    anim.SetFloat("Input", 0);
                    if (nav.enabled)
                    {
                        nav.ResetPath();
                        nav.enabled = false;
                        enableallobs();
                    }
                    Debug.Log("attacking");
                    state = enemyState.attacking;
                    olp.time = 0;
             //       attacking = true;
                    // rand = Random.Range(0f, 100f);
                    rand = Random.value* 100f;
                    //state = enemyState.spawn;

                    //if it doesnt hits random chance for checking player
                    if (rand <= chanceToCheck)
                    {
              

                        dorandomattack();
                    }
                    else
                    {
                        //do something random like crouch and attack or attack etc
                        //      nav.enabled = false;
                        //     obsmain.enabled = true;
                        properAttack();

                    }
                }
                else
                {
                    //return to approach or wandering?

                    if (enemyposinmanager.movepositions.x != -1)
                    {
                        //    disableobs();
                        //     nav.enabled = true;

               

                        if (returnToApproachState())
                        {
            
                            StartCoroutine(endstatefunct());
                            

        /*                   state = enemyState.approach;
                            anim.SetFloat("Input", 1);

                            //idk about this here? just for the live movement cycle to reset?
                            StartCoroutine(tryLiveMovement());*/
                        }
                        else
                            removeFromSpot();
                    }
                    else
                    {
                        StartCoroutine(endstatefunct());
                        //  trytoattackorjoin();

                        /* timestocheckbeforewemove += Time.deltaTime;

                         if (timestocheckbeforewemove > timeBeforeWeWander)
                         {
                             timestocheckbeforewemove = 0;
                             //old code VVVVVVVV
                             //state = enemyState.wander;

                             //new code
                             state = enemyState.spawn;
                             StartCoroutine(RandomPointInNavBounds());
                         }*/
                    }
                }

                break;

            case enemyState.wander:

                timer += Time.deltaTime;

                if (timer >= timeTowait)
                {
                    if (state != enemyState.approach || state != enemyState.approachthenjumpkick || state != enemyState.attacking || state != enemyState.blocking)
                    {
                        StartCoroutine(RandomPointInNavBounds());
                        timer = 0;
                        needstogetaway = false;
                    }
                }
                if ( !p.floored)
                    watchforplayer();

                break;

            case enemyState.walkingtospot:

                //check if we reached out destination
                if (nav.remainingDistance < 0.35f)
                {
                    if (nav.remainingDistance < 0.05f)
                    {
                        nav.velocity = Vector3.zero;
                        nav.enabled = false;

                        enableallobs();
                        anim.SetFloat("Input", 0);
                        state = enemyState.wander;
                        timeTowait = Random.Range(3f, 5.4f);
                        timer = 0;
                        return;
                    }
                    //check for enemies

                    spotcols = Physics.OverlapSphere(new Vector3(spot.x, 0.8f, spot.z), 0.35f, ebox);
                    if (spotcols.Length > 0)
                    {
                        for (int i = spotcols.Length - 1; i > 0; i--)
                        {
                            eref = spotcols[i].transform.root.GetComponent<Enemy>();

                            if (!spotcols[i].transform.IsChildOf(transform.root) && ((eref.state == enemyState.wait || eref.state == enemyState.wander) || (eref.state == enemyState.walkingtospot && Vector3.Distance(eref.spot, spot) < 0.5f)))
                            {
                                nav.velocity = Vector3.zero;
                                anim.SetFloat("Input", 0);
                                state = enemyState.spawn;
                                StartCoroutine(RandomPointInNavBounds());
                                timer = 0;
                                return;
                            }
                        }

                    }
                }
                //check for player collision
                watchforplayer();

                break;

            case enemyState.livemovement:

                if (nav.remainingDistance < 0.35f)
                {
                    //check for enemies

                    spotcols = Physics.OverlapSphere(new Vector3(spot.x, 0.8f, spot.z), 0.35f, ebox);
                    if (spotcols.Length > 0)
                    {
                        for (int i = spotcols.Length - 1; i > 0; i--)
                        {
                            //what if its disabled?

                            if (!spotcols[i].transform.IsChildOf(transform.root) && spotcols[i].transform.root.GetComponent<NavMeshAgent>().velocity == Vector3.zero)
                            {
                                approachOrMoveOtherSide();
                                return;
                            }
                        }
                    }
                    if (nav.remainingDistance < 0.075f)
                    {
                        approachOrMoveOtherSide();
                        return;
                    }
                }
                //check for player collision but no priority picking?
                watchforplayer(1.4f);

                break;

            case enemyState.wait:
                timer += Time.deltaTime;

                spotcols = Physics.OverlapBox(transform.position, new Vector3(1.6f, 1, 0.5f), Quaternion.identity, playerMask);
                if (spotcols.Length > 0 && !p.floored)
                {


                    //try to attack or go to approach idk?
                    waiting = false;

                    trytoattackorjoin();
                    return;

                }

                if (timer >= timeTowait)
                {
                    waiting = false;

                    //do something else next? based off what other enemies are doing

                    //if a lot of enemies are waiting or busy go attack, otherwise give player a break? maybe check if player 2 exists?  IMPORTANT

                    decisionmakingtree();

                    timer = 0;
                    return;
                }

                break;

            case enemyState.approachthenjumpkick:
                assignSpot();

                if (canjumpkick())
                {
                    wanderray = new Ray(new Vector3(transform.position.x, 0.8f, transform.position.z), new Vector3(player.position.x, 0.8f, player.position.z) - wanderray.origin);

                    sidestepcasthit = Physics.RaycastAll(wanderray.origin, wanderray.direction, Vector3.Distance(transform.position, player.position), enemyCol);

                    for (int i = 0; i < sidestepcasthit.Length; i++)
                    {
                        if (sidestepcasthit[i].transform.root != transform)
                        {
                            ConfirmedApproach();
                            StartCoroutine(tryToDoLiveMovement());
                            return;
                        }
                    }

                    StartCoroutine(jumpkick());
                }
                else
                {
                    //check if our spot is still on our side

                    if (enemyposinmanager.whichOne < 2)
                    {
                        if (transform.position.x < player.position.x)
                        {
                            removeFromSpot();
                            return;
                        }
                    }
                    else
                    {
                        if (transform.position.x > player.position.x)
                        {
                            removeFromSpot();
                            return;
                        }
                    }
                    //move closer if the spot position changed since we dont need to find a closer spot
                    if (oldSpot != spot)
                    {

                        nav.destination = spot;
                        oldSpot = spot;
                    }

                }
                break;

            case enemyState.walkconstantly:
                //check if we reached our destination
                if (nav.remainingDistance < 0.35f)
                {
                    //find a new spot again or leave

                    if (timeswalked >= numbertowalk)
                    {
                        //go to approach or decision making tree with stronger chance to approach?
                        if (p.enemyM.numberOfEnemies() == 1)
                        {
                            StartCoroutine(timerforprocastinate());
                            ConfirmedApproach();
                            StartCoroutine(tryToDoLiveMovement());
                        }
                        else
                            decisionmakingtree();
                        return;
                    }
                    else
                    {
                        timeswalked++;
                        Debug.Log(timeswalked);
                        nav.ResetPath();
                        nav.velocity = Vector3.zero;
                        pickspottowalkto();
                        return;
                    }

                }
                watchforplayer();
                break;

            case enemyState.walknwait:
                if (nav.remainingDistance < 0.35f)
                {
                    //find a new spot again or leave
                  
                        timer = 0;
                        timeTowait = Mathf.Clamp(Random.value * 3f, 1f, 3f);
                        timeswalked++;
                        nav.ResetPath();
                        nav.velocity = Vector3.zero;
                    nav.enabled = false;
                    activateobs();
                        state = enemyState.waitafterwalk;
                    anim.SetFloat("Input", 0);
                    somenum = 1;
                        return;
                   
                }
                watchforplayer();

                break;

            case enemyState.waitafterwalk:

                timer += Time.deltaTime;

                if (timer >= timeTowait)
                {
                    StartCoroutine(leftwait());
                    

                }
                else
                    watchforplayer();

                break;

            case enemyState.twothreewait:

                if (nav.remainingDistance < 0.35f)
                {
                    //find a new spot again or leave
                    if (twothree >= twoorthree)
                    {

                        timeswalked++;
                        Debug.Log(numbertowalk);

                        if (timeswalked > numbertowalk)
                        {
                            //go to approach or decision making tree with stronger chance to approach?
                            if (p.enemyM.numberOfEnemies() == 1)
                            {
                                StartCoroutine(timerforprocastinate());
                                ConfirmedApproach();
                                StartCoroutine(tryToDoLiveMovement());
                            }
                            else
                                decisionmakingtree();
                            return;
                        }
                        else
                        {
                            timer = 0;
                            timeTowait = Mathf.Clamp(Random.value * 3f, 1f, 3f);
                            somenum = 0;
                        nav.ResetPath();
                            nav.velocity = Vector3.zero;
                            nav.enabled = false;
                            activateobs();
                            state = enemyState.waitafterwalk;
                            anim.SetFloat("Input", 0);
                        }

                        return;
                    }
                    else
                    {
                        twothree++;
                        nav.ResetPath();
                        nav.velocity = Vector3.zero;
                        pickspottowalkto();
                    }

                }
                watchforplayer();


                break;

            case enemyState.onemore:

                if (nav.remainingDistance < 0.35f)
                {
                    //find a new spot again or leave

                    nav.ResetPath();
                    nav.velocity = Vector3.zero;

                    if (p.enemyM.numberOfEnemies() == 1)
                    {
                        StartCoroutine(timerforprocastinate());
                        ConfirmedApproach();
                        StartCoroutine(tryToDoLiveMovement());
                    }
                    else
                        decisionmakingtree();

                    return;

                }
                watchforplayer();


                break;

        }
    }

    int twothree;

    int twoorthree;

    int somenum;

    IEnumerator leftwait()
    {
        if (state != enemyState.approach || state != enemyState.approachthenjumpkick || state != enemyState.attacking || state != enemyState.wander || state != enemyState.walkingtospot || state != enemyState.blocking)
        { }
        else
            yield break;

            disableobs();
        yield return null;
        nav.enabled = true;

        if (timeswalked >= numbertowalk)
        {

     

                //timer = 0;
                if (p.enemyM.numberOfEnemies() == 1)
                {
                    StartCoroutine(timerforprocastinate());
                    ConfirmedApproach();
                    StartCoroutine(tryToDoLiveMovement());
                }
                else
                    decisionmakingtree();
            
        }
        else
        {

            if (somenum == 1)
                state = enemyState.walknwait;
            else
                state = enemyState.twothreewait;

            pickspottowalkto();
        }
    }

    IEnumerator timerforprocastinate()
    {
        leftprocastinate = true;
        yield return new WaitForSeconds(5f);
        leftprocastinate = false;
    }


    public IEnumerator returntodecisiontree(int i = 0)
    {
        attacking = false;
        anim.SetBool("Attacking", false);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        if (!nav.enabled)
        {
           disableobs();
            yield return null;
           nav.enabled = true;
        }
     needstogetaway = true;

        if (i == 0)
           decisionmakingtree();
        else
        {
            pickaprocastinatestate();
        }
    }

    float timeaway;

    void watchforplayer(float c = 1.6f)
    {
        if (needstogetaway)
        {
            timeaway += Time.deltaTime;
            if (timeaway > 3f)
            {
                timeaway = 0;
                needstogetaway = false;
            }
            return;
        }

        spotcols = Physics.OverlapBox(transform.position, new Vector3(c, 1, 0.5f), Quaternion.identity, playerMask);

        if (spotcols.Length > 0)
        {
            if (!p.floored)
            {
                if (state == enemyState.walkconstantly || state == enemyState.walknwait
                    || state == enemyState.onemore || state == enemyState.twothreewait)
                    StartCoroutine(timerforprocastinate());

                //or return to idle? or take someone elses spot??????????????????????????????????????????????????
                if (enemyposinmanager.movepositions.x == -1)
                    trytoattackorjoin();
                else
                {
                    StartCoroutine(returntoapproachwithnavdisabled());
                }
            }

        }
    }

    IEnumerator returntoapproachwithnavdisabled()
    {
        if(!nav.enabled)
        {
            disableobs();
            yield return null;
            nav.enabled = true;
        }

        ConfirmedApproach();
        StartCoroutine(tryToDoLiveMovement());
    }

    public override void die()
    {
        base.die();
        nav.enabled = false;
        disableobs();
        state = enemyState.down;
        StartCoroutine(deathflash());

    }

    public IEnumerator deathflash()
    {
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Floored death"));
        GameManager.instance.enemies.Remove(this);
        GameManager.instance.nomoreenmies();
       bc[0].enabled = true;
        bc[1].enabled = false;

        for (int i = 0; i < 2; i++)
        {
            Physics.IgnoreCollision(bc[0], p.bc[i]);
        }

   //     bc[1].enabled = false;

        for (float i = 0; i < 0.4f; i+= 0.1f)
        {
            sr.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.5f-i);
            sr.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.2f);
        }
        while(sr.color.a>0f)
        {
            sr.color = new Color(1, 1, 1, sr.color.a-Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void trytoattackorjoin()
    {
        raycastCheck();
        if (!rayhit)
        {
            //force someone to take their spot? if your spot is a priority and are closer 
            state = enemyState.spawn;
            p.enemyM.prioritizespot(this);
            // Join();
        }
        else
        {
            if(state!= enemyState.idle)
            returnToIdle();
            else
            {
                if (!nav.enabled)
                    StartCoroutine(endstatefunct());
                else
                {
                    ConfirmedApproach();
                    StartCoroutine(tryToDoLiveMovement());
                }
            }
        }
    }

    void returnToIdle(float timeset = 0)
    {
        /*       if (nav.enabled)
            nav.ResetPath();
        else
        {
            nav.enabled = false;
            enableallobs();
        }*/
       // timestocheckbeforewemove = 0;
        //timeBeforeWeWander = timeset;
        state = enemyState.idle;
    }

    public void dorandomattack()
    {
        //check if player is crouching or not
        // attBlend = 1;
        // 
        //   comboctrl.punch();

        if (!p.crouch)
        {


            kickorpunch = Random.Range(0, 2);

            if (facingUs())
            {

                randomAtt = Random.Range(-1, 2);

                //if player is flinching back
                if (p.anim.GetCurrentAnimatorStateInfo(0).IsName("flinch back") && cc.kickState + cc.punchState == 2)
                {
                    //if we're told to do a high attack, only kick
                    if (randomAtt == 1)
                    {
                        kickorpunch = 0;
                    }
                    else //otherwise only do mid/low attack
                    {
                        randomAtt = Random.Range(-1, 1);
                    }
                }
                else
                {
                    //change nothing
                }
            }
            else
                randomAtt = Random.Range(-1, 1);


            attBlend = randomAtt;
  

            if (kickorpunch == 0)
            {
                //do kick 
                cc.kick();
                Debug.Log(cc.punchState + cc.kickState);
            }
            else
            {
                //make sure we are in reach to punch?

                //Debug.Log("punch");
                //punch
                cc.punch();
                Debug.Log(cc.punchState + cc.kickState);
            }
        }
        else
        {
            crouchtest = Random.value*100f;

            if (crouchtest <= chanceToCrouch)
            {

                randomAtt = Random.Range(-1, 1);
                kickorpunch = Random.Range(0, 2);
                attBlend = randomAtt;

                //crouch
                if (!crouch)
                    Crouch();
                if (kickorpunch == 0)
                {
                    //do kick 
                    cc.kick();
                }
                else
                {
                    //punch
                    cc.punch();
                }
            }
            else
            {
                //do low kick or medium kick? or low punch if more than 2nd att
            

                randomAtt = Random.Range(-1, 1);
                attBlend = randomAtt;
       

                if (cc.punchState + cc.kickState > 0)
                {
                    kickorpunch = Random.Range(0, 2);

                    if (kickorpunch == 0)
                        cc.kick();
                    else
                        cc.punch();
                }
                else
                {
                    cc.kick();
                }
                    
            }

        }
    }

    public void missAttack()
    {
        if(p.crouch)
        {
            if (!crouch)
            {
                randomAtt = Random.Range(-1, 1);
                attBlend = randomAtt;

                //do any kick body/leg kick, low punch if 2nd attack in combo
                if (cc.punchState + cc.kickState > 0)
                {

                    kickorpunch = Random.Range(0, 2);

                    if (kickorpunch == 0)
                    {
                        cc.kick();
                    }
                    else
                    {
                        //only low punch

                        randomAtt = -1;
                        attBlend = randomAtt;
                        cc.punch();
                    }
                }
                else
                {

                    cc.kick();
                }

            }
            else
            {
                //since crouched, only do punches

                randomAtt = 0;
                attBlend = randomAtt;

                cc.punch();
            }
        }
        else
        {
            if (!crouch)
            {
                //do any body/head attacks :)

                randomAtt = Random.Range(0, 2);
                attBlend = randomAtt;

                kickorpunch = Random.Range(0, 2);

                if (kickorpunch == 0)
                    cc.kick();
                else
                    cc.punch();
            }
            else
            {
                //only do uppercut crouch

                randomAtt = 1;
                attBlend = randomAtt;

                    cc.punch();
            }
        }
    }

   public void properAttack()
    {
        if (p.rolling)
        {
            //low attack obviously
            attBlend = -1;
          
                cc.kick();

            return;
        }
        if(!p.grounded)
        {
            //high kick? since player is jumping etc
            attBlend = 1;

            cc.kick();

            return;
        }
      

        if (!p.crouch)
        {
            //check if blocking
            if (p.blocking)
            {
                //do low kick or any crouch attack other than an uppercut
                if (crouch)
                {
                    randomAtt = Random.Range(-1, 1);
                    attBlend = randomAtt;            

                    kickorpunch = Random.Range(0, 2);

                    if (kickorpunch == 0)
                        cc.kick();
                    else
                        cc.punch();
                }
                else
                {
                    randomAtt = -1;
                    attBlend = randomAtt;

                    if (cc.punchState + cc.kickState > 0)
                    {
                        kickorpunch = Random.Range(0, 2);

                        if (kickorpunch == 0)
                            cc.kick();
                        else
                            cc.punch();
                    }
                    else
                    {

                        cc.kick();
                    }
                }
            }
            else
            {
                //try to go for high kick or high punch otherwise aim for body? since player isnt blocking/crouched
                randomAtt = Random.Range(0, 2);

                kickorpunch = Random.Range(0, 2);

                if (facingUs())
                {
                    if (p.anim.GetCurrentAnimatorStateInfo(0).IsName("flinch back") && cc.kickState + cc.punchState ==2)
                    {
                        //if we're told to do a high attack, only kick
                        if (randomAtt == 1)
                        {
                            kickorpunch = 0;
                        }
                        else //otherwise only do mid
                        {
                            randomAtt = 0;
                        }
                    }
                }
                else
                {
                    if (randomAtt == 1)
                    {
                        kickorpunch = 0;
                    }
                    else //otherwise only do mid
                    {
                        randomAtt = 0;
                    }
                }

                attBlend = randomAtt;
                

                if (kickorpunch == 0)
                    cc.kick();
                else
                    cc.punch();

            }
          

        
        }
        else
        {
            //check if blocking
            if (p.blocking)
            {
                if (!crouch)
                    Crouch();

                //do crouch kick?
                randomAtt = -1;
                attBlend = randomAtt;

                cc.kick();

            }
            else
            {
                //if we hit chance to crouch?
                //do anything except uppercut?

                crouchtest = Random.value * 100f;

                if (crouchtest <= chanceToCrouch)
                {

                    randomAtt = Random.Range(-1, 1);
                    attBlend = randomAtt;
                    
                    kickorpunch = Random.Range(0, 2);

                    //crouch
                    if (!crouch)
                        Crouch();

           
                        if (kickorpunch == 0)
                        {
                            //do kick 
                            cc.kick();
                        }
                        else
                        {
                            //punch
                            cc.punch();
                        }
             
                }
                else
                {
                    //do low kick or medium kick? or low punch if more than 2nd att


                    randomAtt = Random.Range(-1, 1);
                    attBlend = randomAtt;
                    

                    if (cc.punchState + cc.kickState > 0)
                    {
                        kickorpunch = Random.Range(0, 2);

                        if (kickorpunch == 0)
                            cc.kick();
                        else
                            cc.punch();
                    }
                    else
                    {
                        cc.kick();
                    }
                }
    
            }
        }
    }


    public void removeFromSpot()
    {
        //   state = enemyState.spawn;
        enemyposinmanager.enemy = null;
        enemyposinmanager = new enemyPositions();
        enemyposinmanager.movepositions.x = -1;
        //   enemyposinmanager = null;
        Join();
    }

    public void Join()
    {

        if (enemyposinmanager.movepositions.x == -1)
        {
            //change our state to static idk? just till we are in a position
            //     state = enemyState.spawn;


            //need a way to check which player to go attack based off how many are on each etc?  idk? or set a specific target for a time?
            ///\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\ VERY IMPORTANT /\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\


            if (p.enemyM.checkiftherearespots(this) && !p.floored)
            {
                p.enemyM.addToQueue(this);
            }
            else
            {
                if (state != enemyState.walkingtospot && state != enemyState.wander)
                {
                    nav.velocity = Vector3.zero;
                    anim.SetFloat("Input", 0);
                    goWander();
                }

            }
        }
        else
        {

            assignSpot();
            if (!checkifsomethingisinourspot())
            {

                //add a check before going there? like if can retaliate but not flinching
                if (closeenough())
                {


                    raycastCheck();
                    if (!rayhit)
                        StartCoroutine(resetnavAndReturnApproach());
                    else
                    {
                        state = enemyState.idle;
                    }
                }
                else
                {
                    if (!nav.enabled)
                        StartCoroutine(resetnavAndReturnApproach());
                    else
                    {
                        //  disableobs();
                        //  anim.SetFloat("Input", 1);
                        ConfirmedApproach();
                    }
                }
            }
            else
            {

                if (p.enemyM.checkiftherearespots(this))
                {
                    leaveSpot();
                    p.enemyM.addToQueue(this);
                }
                else
                {
                    leaveSpot();
                    anim.SetFloat("Input", 0);
                    goWander();
                    Debug.Log("wander");
                }
            }

        }

    }

    public IEnumerator resetnavAndReturnApproach()
    {
        //used to be a coroutine
        disableobs();
        yield return null;
        nav.enabled = true;
        assignSpot();
        raycastCheck();
        if (!closeenough() || !rayhit)
        {
            anim.SetFloat("Input", 1);
        }

        state = enemyState.approach;
        nav.destination = spot;
    }

    public void goWander()
    {
        needstogetaway = true;
        //wander etc
        //     disableobs();
        StartCoroutine(RandomPointInNavBounds());
    }

    public override void setBlock()
    {
        base.setBlock();

        if (state == enemyState.flinching)
            Join();
    }

    public IEnumerator jumpkick(int val = 1)
    {
        state = enemyState.attacking;
        rightSpot = false;
        if (val != 1)
        {
            efunct.vectTouse = Vector3.zero;
        }
        else
            efunct.vectTouse = Vector3.Normalize(new Vector3(player.position.x + (leftorRight() / 2.8f), 0.8f, player.position.z) - new Vector3(transform.position.x, 0.8f, transform.position.z));

        jumpkicked = true;
        StartCoroutine(jumpkickactive());
        Jump();
        yield return null;
        cc.kick();
    }

    IEnumerator jumpkickactive()
    {
        yield return new WaitForSeconds(7.5f);
        jumpkicked = false;
    }

    public bool canjumpkick(int basenum = 1)
    {
        if (ifCanRetaliate())
        {
            //standing still
            distz = Mathf.Abs(transform.position.z - player.position.z);
            distx = Mathf.Abs(transform.position.x - player.position.x);

            if (basenum != 1)
            {
                if (distz <= 0.3f && allowedtojumpkick() && (facingRight && transform.position.x < player.position.x || !facingRight && transform.position.x > player.position.x))
                    return true;
            }
            else
            {
                // need to also check if we are in the clear to do so

                if (distz <= 0.3f && distx < 1.55f && allowedtojumpkick() && (facingRight && transform.position.x< player.position.x || !facingRight && transform.position.x > player.position.x))
                    return true;
            }
        }

        return false;
    }

    bool allowedtojumpkick()
    {
        if (!rotating && !playerHit && !attacking && !rolling)
            return true;

        return false;
    }

    public Vector3 RandomNavSphere(float distance, int layermask)
    {
        vec = UnityEngine.Random.insideUnitSphere * distance;



        vec += transform.position;

        vec = new Vector3(Mathf.Clamp(vec.x, cam.transform.position.x - 3.5f, cam.transform.position.x + 3.5f), 1.32f, Mathf.Clamp(vec.z, -2f, 2f));

        NavMeshHit navHit;

        NavMesh.SamplePosition(vec, out navHit, distance, layermask);


        return navHit.position;
    }

    public IEnumerator RandomPointInNavBounds()
    {
        if (state != enemyState.approach)
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || (!p.floored && p.enemyM.checkiftherearespots(this)));

        disableobs();
        wanderspot = RandomNavSphere(3f, 1);

        //   cols = Physics.OverlapSphere(new Vector3(wanderspot.x, 0.8f, wanderspot.z), 0.3f, enemyCol);

        //  wanderray = new Ray(transform.position, new Vector3(wanderspot.x, 0.8f, wanderspot.z) - transform.position);

        while (efunct.tooclosetoplayer() || Vector3.Distance(transform.position, new Vector3(wanderspot.x, transform.position.y, wanderspot.z)) < 1.5f)
        {

            wanderspot = RandomNavSphere(3f, 1);

            //    wanderray = new Ray(transform.position, new Vector3(wanderspot.x, 0.8f, wanderspot.z) - transform.position);

            //  cols = Physics.OverlapSphere(new Vector3(wanderspot.x, 0.8f, wanderspot.z), 0.3f, enemyCol);

            yield return null;
        }
        yield return null;
        anim.SetFloat("Input", 1);
        nav.enabled = true;
        spot = wanderspot;
        nav.destination = wanderspot;
        state = enemyState.walkingtospot;
    }

    public void leaveSpot()
    {
        //death or wandering? idk
        if (nav.enabled)
            nav.destination = transform.position;
        if (state != enemyState.down && state != enemyState.approach)
            state = enemyState.spawn;
        enemyposinmanager.enemy = null;
        enemyposinmanager = new enemyPositions();
        enemyposinmanager.movepositions.x = -1;
        //find better spot?

    }

    /*   public int returnUsableSpots()
       {

           usableSpots = new List<Vector3>();
           // jumpAngle = transform.position - pAtt.transform.position;
           //  lastAngle = 500;

           foreach (Vector3 vecs in jumpRollPositions)
           {
               empty = true;

               //   compareAngle = transform.position - (transform.position + new Vector3(transform.GetChild(0).localScale.x * vecs.x, vecs.y, vecs.z));

               spotcols = Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z) + new Vector3(leftorRight() * vecs.x, 0.8f, vecs.z), 0.1f, invisipne);

               wanderray = new Ray(new Vector3(transform.position.x, 0.8f, transform.position.z), ((new Vector3(transform.position.x, 0.8f, transform.position.z) + new Vector3(leftorRight() * vecs.x, 0f, vecs.z)) - new Vector3(transform.position.x, 0.8f, transform.position.z)));

               usablehits = Physics.RaycastAll(wanderray.origin, wanderray.direction, 1f, invisipne);
               if (spotcols.Length == 0)
               {
                   for (int i = 0; i < usablehits.Length; i++)
                   {
                       if (usablehits[i].transform.root != transform)
                       {
                           empty = false;
                           break;
                       }

                   }

                   //     if(rc.transform!=transform)
                   //     u

                   if (empty)
                       usableSpots.Add(vecs);
               }
           }

           if (usableSpots.Count == 2 && usableSpots.Contains(jumpRollPositions[3]) && usableSpots.Contains(jumpRollPositions[4]))
           {
               if (pAtt.transform.position.z <= transform.position.z)
               {
                   vectTouse = (transform.position + (new Vector3(0, 0, 1))) - transform.position;
               }
               else
               {
                   vectTouse = (transform.position + (new Vector3(0, 0, -1))) - transform.position;
               }
           }
           else
           {
               foreach (Vector3 vecs in usableSpots.ToList())
               {
                   if (pAtt.transform.position.z < transform.position.z)
                   {
                       //left
                       if (leftorRight() == 1)
                       {
                           if (vecs.z < 0)
                               usableSpots.Remove(vecs);
                       }
                       else //right
                       {
                           if (vecs.z > 0)
                               usableSpots.Remove(vecs);
                       }

                   }
                   else
                   {
                       if (leftorRight() == 1)
                       {
                           if (vecs.z > 0)
                               usableSpots.Remove(vecs);

                       }
                       else
                       {
                           if (vecs.z < 0)
                               usableSpots.Remove(vecs);
                       }
                   }
               }
               if (usableSpots.Count > 0)
               {
                   vectTouse = (transform.position + (usableSpots[Random.Range(0, usableSpots.Count)] * leftorRight())) - transform.position;
               }
               else
               {
                   //roll away?
               }


           }
           vectTouse = new Vector3(vectTouse.x, 0, vectTouse.z);

           return usableSpots.Count;
       }

       */
}