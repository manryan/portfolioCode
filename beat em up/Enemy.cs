using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum enemyState { spawn = 1, idle = 2, approach = 3, wander, livemovement, attack, walkingtospot, flinching, wait, down, jumping, rolling, stayaway, sidestep, alternativelivemove };

public class Enemy : Entity
{
    public NavMeshAgent nav;

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
    public  bool newspot;


    public List<GameObject> objectsinstep;

    [HideInInspector]
    public Vector3 oldSpot;

    public Transform player;

    public Player p;

    Vector3 wanderspot;

    public Player temp;

    public enemyPositions enemyposinmanager;

    public NavMeshObstacle obsmain;

    bool reached;

    public LayerMask playerMask;
    [HideInInspector]
    public Overlapping olp;

    bool rayhit;

    [HideInInspector]
    public float rand;

    public int chanceToCheck;

    public float timeBeforeWeWander;

    public Camera cam;

    Vector3 vec;

    RaycastHit[] usablehits;

    bool empty;

    Collider[] cols;

    [HideInInspector]
    int spotindex;

    public LayerMask playernenemies;

    float timer;

    public enemyState state;

    RaycastHit hit;

    Ray ray;

    RaycastHit wanderhit;

    public List<Collider> anyoneThere;

    float randomAtt;

    [HideInInspector]
    public bool canrotate = true;

    int kickorpunch;

    public float chanceToCrouch;

    public float crouchtest;

    Ray wanderray;

    [HideInInspector]
    public bool canswitchtarg = true;

    [HideInInspector]
    public Collider[] spotcols;

    float timestocheckbeforewemove;

    [HideInInspector]
    public float attBlend;

    float fu;

    public LayerMask objectsnplayer;

    public LayerMask enemyCol;

    public List<Vector3> livePosition = new List<Vector3>();

    public float chanceToReact;

    public float chanceRoll;

    public LayerMask allobjects;

    public bool smartAi;

    bool dynamic;

    [HideInInspector]
    public int timesWentDynamic;

    [HideInInspector]
    public Player pAtt;



    Vector3 compareAngle;

   // float lastAngle;

    public List<Vector3> jumpRollPositions;

    public List<Vector3> usableSpots;

    [HideInInspector]
    public Vector3 vectTouse;

    public LayerMask invisipne;

    public LayerMask wallsOnly;

    public RollHandle rollh;

    List<Vector3> liveposAlt;

    public bool AllowRs = true;

    RaycastHit wallcheck;

    Ray wallRay;

    Vector3 secondTravelPath;

    [HideInInspector]
    public bool sideStep;

    public LayerMask objects;

    [HideInInspector]
    public bool waiting;

    float chanceToWait;

    enemyPositions tempEP;

    [HideInInspector]
    public Vector3 livespotref;

    float leftOfEnemy()
    {
        if (player.transform.position.x > transform.position.x)
        {
            return 1.5f;
        }
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

    private void OnEnable()
    {
        //  GameManager.instance.enemies.Add(this);
        Invoke("setRbKinematic", 0.1f);
        //ignore collisions with the invisible walls on the camera boundaries :) **8

        //******   ^^^ VERY IMPORTANT ^^^^^^  *****

    }

    private void Start()
    {
        GameManager.instance.enemies.Add(this);
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
            Vector3 temp = livePosition[i];
            int randomIndex = Random.Range(i, livePosition.Count);
            livePosition[i] = livePosition[randomIndex];
            livePosition[randomIndex] = temp;
        }
    }
    public override void Jump()
    {
        state = enemyState.jumping;
        rb.isKinematic = false;
        olp.time = 0;
        StartCoroutine(activateobs());
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
                Debug.Log("found a spot");
                actuallyRoll();

                //find good roll spot && add force :)
            }
            else
            {


                if (rollh.tryCardinals())
                {
                    Debug.Log("cardinals");
            //        base.Roll();
                    actuallyRoll();
                }
                else
                {
                    Debug.Log("movehim");

                    if (rollh.tryToMoveAnEnemy())
                    {
                 //       base.Roll();
                        rollh.enemyToCheck.Roll();
                        actuallyRoll();
                    }
                    else
                    {
                        Debug.Log("no spot available for other enemy");

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

        StartCoroutine(activateobs());

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

        bc[0].size = new Vector3(bc[0].size.x, bc[0].size.y, 0.15f);
        bc[1].size = new Vector3(bc[1].size.x, bc[1].size.y, 0.15f);

        if (!nav.enabled)
        {
            //     obsmain.enabled = false;
            disableobs();
            nav.enabled = true;
            nav.ResetPath();
        }

        handleEndState();
    }

    //for ending jump/roll
    public void handleEndState()
    {
        if (!playerHit && state != enemyState.down)
        {
            if (enemyposinmanager.movepositions.x == -1)
            {
                Join();
            }
            else
            {
                if (!nav.enabled)
                {
                    //   StartCoroutine(resetNav());
                    disableobs();
                    nav.enabled = true;
                }

                ConfirmedApproach();
                anim.SetFloat("Input", 1);
            }
        }
    }

    public IEnumerator allowRightSpot()
    {
        AllowRs = false;
        yield return new WaitForSeconds(10f);
        AllowRs = true;
    }

    public float leftorRight()
    {
        if (pAtt.transform.position.x < transform.position.x)
        {
            return 1;
        }
        else
            return -1;
    }

    public int returnUsableSpots()
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
                resetStateBeforeRetaliating();

                //change our state for our knowledge/if checks
                if (crouch)
                {
                    // if (distxfromp > 1.65f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.5f && distx > 0.8f) || (distxfromp > 1f && distyfromp > 0.35f)))))
                    if (distxfromp > 1.65f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.5f && distx > 0.9f) || (distxfromp > 1f && distyfromp > 0.35f)))))
                    {
                        state = enemyState.spawn;
                        if (nav.enabled)
                        {
                            nav.ResetPath();
                            nav.velocity = Vector3.zero;
                        }
                       efunct.stepaside();
                        //side step/stop in your spot?
                    }
                    else
                    {
                        retaliate();
                    }
                }
                else
                {
                    if ((distxfromp > 0.5f && distyfromp > 0.5f) || distxfromp >= 1.58f || distyfromp >= 0.95f || ((Vector3.Distance(transform.position, pAtt.transform.position) > 1.34f && ((distyfromp > 0.5f && distx > 0.8f) || (distxfromp > 1f && distyfromp > 0.35f) || (distxfromp > 0.6f && distyfromp > 0.6f)))))
                    {
                        //      Debug.Log(Vector3.Distance(transform.position, pAtt.transform.position));
                        //       Debug.Log("sidestep");
                        state = enemyState.spawn;
                        if (nav.enabled)
                        {
                            nav.ResetPath();
                            nav.velocity = Vector3.zero;
                        }
                       efunct.stepaside();
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

    //  public abstract void implementme();


    //incomplete
    bool ifCanRetaliate()
    {
        //if not blocking obviously..
        if (pAtt != p)
        {
            if (state == enemyState.sidestep)
                return true;
        }

        if (grounded && !playerHit && state != enemyState.down && anim.GetBool("Blocked") == false && !rolling
            && state != enemyState.jumping && state != enemyState.flinching && state != enemyState.stayaway)
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

    public virtual void retaliate()
    {
        //  Debug.Log(Vector3.Distance(transform.position, pAtt.transform.position));
        //  1.136486f
        //1.11f
        if (pAtt.crouch)
        {
            if (crouch)
            {

                switch (pAtt.es.myHE.attackPos)
                {
                    case 1://ignore or attack back since player is uppercutting
                        break;
                    case 2://block since can block punch
                        if (!blocking)
                            Block();
                        break;
                    case 3://roll away/jump since player kick
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
                    case 1://block since player is uppercutting

                        //smart ai may be able to crouch? :D

                        //if smart ai and didnt block
                        Debug.Log("crouch");
                        //  Block();
                        Crouch();

                        //else not smart ai. so block

                        break;
                    case 2:// player punches, sidestep (if far)?/ Roll
                        Block();
                        break;
                    case 3:// player kicks, roll away/ Jump?


                        //if far enough but not too far, make jump
                        //          distx = Mathf.Abs(transform.position.x - pAtt.transform.position.x);
                        //used to be 0.6f
                        if (distxfromp > 0.9f)
                        {
                            if (returnUsableSpots() >= 1)
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

                }


            }

        }
        else
        {
            //
            //player is standing
            //

            if (crouch)
            {
                if (pAtt.es.myHE.attackPos >= 3)
                {
                    //Block since player does a low hit
                }
                else
                {
                    //ignore or attack?
                }

            }
            else
            {
                //
                //enemy is standing
                //

                if (pAtt.es.myHE.attackPos >= 3)
                {
                    // player does a low attack, so sideStep?/Roll away
                }
                else
                {
                    if (pAtt.es.myHE.attackPos != 1)
                    {
                        //Block since we can
                        Debug.Log("block");
                        //smart ai may be able to crouch before the attack if its not a med kick? :D (player attpos !=1)

                    }
                    else
                    {

                        //Block since we can

                    }

                }


            }

        }
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

        if (vec.z > 0)
        {
            fu = 1;
        }
        else
        {
            fu = -1;
        }

        //return new Vector3(Mathf.Clamp(transform.position.x + (vec.x * collisionParent.localScale.x), cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Random.Range(Mathf.Clamp(transform.position.z + vec.z, -2f, 2f), Mathf.Clamp(transform.position.z + vec.z + (fu), -2f, 2f)));

        if (p.transform.position.x < transform.position.x)
            return new Vector3(Mathf.Clamp(Random.Range(transform.position.x + (vec.x * -1f), transform.position.x + ((vec.x + 0.5f) * collisionParent.localScale.x)), cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Random.Range(Mathf.Clamp(transform.position.z + vec.z, -2f, 2f), Mathf.Clamp(transform.position.z + vec.z + (fu), -2f, 2f)));
        else
        {
            return new Vector3(Mathf.Clamp(Random.Range(transform.position.x + (vec.x * 1f), transform.position.x + ((vec.x + 0.5f) * collisionParent.localScale.x)), cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Random.Range(Mathf.Clamp(transform.position.z + vec.z, -2f, 2f), Mathf.Clamp(transform.position.z + vec.z + (fu), -2f, 2f)));

        }
    }


    [HideInInspector]
    public  int canTry;

    IEnumerator waitBeforeCanRepeatLive()
    {
        yield return new WaitForSeconds(3f);
        canTry = 0;
    }

    float chanceToSideStep()
    {
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

    bool areWeWithinCameraBounds()
    {
        if (transform.position.x > cam.transform.position.x + 5f || transform.position.x < cam.transform.position.x - 5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


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
                        if (Vector3.Distance(transform.position, spot) > 2f && Vector3.Distance(transform.position, spot) < 8f && canTry == 0)
                        {
                            liveSpotPercent = Random.value * 100f;

                            //if we are to the right and our spot is to the left, give a higher chance of side stepping? and vice verse


                            if (liveSpotPercent - chanceToSideStep() <= chanceToGoLiveSpot)
                            {
                                //change state and go to our spot

                                if (rightSpot)
                                {
                                    //roll again to see if its gonna be a regular livemovement or an alt movement

                                    liveSpotPercent = Random.value * 100f;

                                    if (liveSpotPercent < 50f)
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
                                    else
                                    {
                                        //we arent side stepping, so just assign a spot near our target  so its easy to go to other side of him
                                        if (efunct.alternativeSideSpot()) //alt sidespot** is kinda rushed towards the spot
                                        {
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

            if (state == enemyState.spawn || state == enemyState.idle)
                dynamic = false;
        }
        else
        {
            timesWentDynamic++;

            if (timesWentDynamic > 5)
            {
                resetdynamic();
            }
        }

        //or not down/hit? idk??
        /*  if (state != enemyState.livemovement || state != enemyState.spawn || state!= enemyState.down)
          {
              yield return new WaitUntil(() => state == enemyState.approach);
              yield return new WaitForSeconds(10);
              StartCoroutine(tryToDoLiveMovement());
          }*/
    }

    public float chanceToGoLiveSpot;

    float liveSpotPercent;


    public void handleRotationEnemy()
    {
        if (state != enemyState.down && !playerHit && !rolling && !rotating && state != enemyState.sidestep)
            if (canrotate || closeenough() || p.rolling || Mathf.Abs( transform.position.x-player.position.x)>1f)
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
        rotatecooldown = 2f;
       while(rotatecooldown>0f)
        {
            rotatecooldown -= Time.deltaTime;
            if (Vector3.Distance(transform.position, spot)<1.5f)
                break;

            yield return null;
        }
        canrotate = true;
    }

    float rotatecooldown;

    private void FixedUpdate()
    {
        //if player or wandering?

        // if (nav.enabled && nav.velocity == Vector3.zero)
        //      anim.SetFloat("Input", 0);

        if (player)
                handleRotationEnemy();
            
    }


    IEnumerator handleNavNObs()
    {
        disableobs();
        yield return null;
        nav.enabled = true;
        nav.destination = spot;

        state = enemyState.approach;
    }



    public void slowNavAgent()
    {
        nav.speed = Mathf.Clamp(Vector3.Distance(transform.position, spot), 1.5f, 2);

        /* if(Vector3.Distance(transform.position, spot) > 1f)
         {
             nav.spe
         }*/
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

    float raydist()
    {
        if (crouch)
        {
            if (p.crouch)
            {
                if (facingUs())
                {
                    return 0.9f;
                }
                else
                {
                    return 0.9f;
                }
            }
            else
                return 0.92f;
        }
        else
        {
            if (p.crouch)
            {
                if (facingUs())
                {
                    return 0.85f;
                }
                else
                {
                    return 1f;
                }
            }
            else
                return 0.75f;
        }

        /*  if (crouch || p.crouch)
          {
              return 0.85f;
          }
          else return 0.75f;*/
    }

    public void raycastCheck()
    {

        //if it is, raycast three times to the front of enemy, and 2 diagonal to it within the hit radius
        rayhit = false;

        for (int i = -1; i < 2; i++)
        {
            ray = new Ray(transform.position, (transform.position + new Vector3(-0.6f * -collisionParent.localScale.x, 0, (i / 20f) * 1.1f)) - transform.position);

            Debug.DrawRay(ray.origin, ray.direction * raydist(), Color.blue, 3f);

            if (Physics.Raycast(ray, out hit, raydist(), playerMask))
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


    [HideInInspector]
    public float distx;
    [HideInInspector]
    public float distz;

    bool closeenough()
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
            else if(crouch && !p.crouch)
            {
                if ((distz <= 0.011f && distx < 0.01f) || (distz <= 0.024f && Mathf.Abs(transform.position.x - player.position.x) > 0.3f && distx < 0.73f))
                    return true;

                /*  if (enemyposinmanager.whichOne < 1)
                  {
                      if (facingUs())
                      {
                          if ((distz <= 0.01f && Mathf.Abs(transform.position.x - player.position.x) > 0.84f && distx < 0.4f && transform.position.x <= spot.x))
                          {
                              Debug.Log("close enough");
                              return true;
                          }
                      }
                      else
                      {
                          if ((distz <= 0.01f && Mathf.Abs(transform.position.x - player.position.x) >0.74f && distx < 0.4f && transform.position.x <= spot.x))
                          {
                              Debug.Log("close enough");
                              return true;
                          }
                      }
                  }
                  else
                  {
                      if (facingUs())
                      {
                          if ((distz <= 0.01f && Mathf.Abs(transform.position.x - player.position.x) > 0.94f && distx < 0.4f && transform.position.x >= spot.x))
                          {
                              Debug.Log("close enough");
                              return true;
                          }
                      }
                      else
                      {
                          if ((distz <= 0.01f && Mathf.Abs(transform.position.x - player.position.x) > 0.74f && distx < 0.4f && transform.position.x >= spot.x))
                          {
                              Debug.Log("close enough");
                              return true;
                          }
                      }
                  }*/
            }
            else
            {
                if ((distz <= 0.011f && distx < 0.01f) || (distz <= 0.024f && Mathf.Abs(transform.position.x - player.position.x) > 0.93f && distx < 0.4f) )
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
            //check for last time we did a fight shuffle

            //give chance to shuffle if we havent in a while.

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

    Enemy refObj;

    public LayerMask enemiesnobjs;

    //also accounts for objects like crates etc
    bool checkifanenemyfromotherplayerisinourspot()
    {
        //used to be .115f
          spotcols = Physics.OverlapSphere(new Vector3(spot.x, 0.8f, spot.z), 0.115f, enemiesnobjs);
      //  spotcols = Physics.OverlapBox(new Vector3(spot.x, 0.8f, spot.z), new Vector3(1, 1, 0.115f), Quaternion.identity, enemiesnobjs);

        if (spotcols.Length > 0)
        {

            for (int i = 0; i < spotcols.Length; i++)
            {

                //add and not wandering
                if (spotcols[i].transform.root.GetComponent<Enemy>() != null)
                {
                    refObj = spotcols[i].transform.root.GetComponent<Enemy>();

                    //distance originally was .2f
                    if (refObj != this && refObj.p != p && (refObj.state != enemyState.wander || refObj.state != enemyState.walkingtospot || refObj.state != enemyState.down) && Vector3.Distance(refObj.spot, spot) < 0.35f)
                    {
                        //leave our spot;

                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

        }
        return false;
    }

    void assignSpot()
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
        if (!sideStep)
        {
            if (rightSpot)
            {
                //check if we can travel there are there migth be an obstacle :)

                if (liveSpotPercent - chanceToSideStep() <= chanceToGoLiveSpot)
                {

                    secondTravelPath = new Vector3(Mathf.Clamp(player.transform.position.x + leftOfEnemy(), cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Mathf.Clamp(transform.position.z + Random.Range(-0.2f, 0.2f), -2f, 2f));

                    spotcols = Physics.OverlapSphere(new Vector3(secondTravelPath.x, 0.8f, secondTravelPath.z), 0.1f, objects);

                    while (spotcols.Length > 0)
                    {
                        secondTravelPath = new Vector3(Mathf.Clamp(Random.Range(player.transform.position.x + leftOfEnemy(), player.transform.position.x), cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Mathf.Clamp(transform.position.z + Random.Range(-0.2f, 0.2f), -2f, 2f));
                        spotcols = Physics.OverlapSphere(new Vector3(secondTravelPath.x, 0.8f, secondTravelPath.z), 0.1f, objects);
                    }

                    nav.destination = secondTravelPath;
                    sideStep = true;
                }
                else
                {
                    tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

                    if (enemyposinmanager == tempEP)
                    {

                        nav.destination = spot;
                        state = enemyState.approach;
                    }
                    else
                    {
                        removeFromSpot();
                    }
                }
            }
            else
            {
                //chance to wait?

                assignSpot();
                if (ShouldWeWait())
                {

                    //either literally wait, keep your distance, or choose another spot (zig zag)?

                    timeTowait = Mathf.Clamp(Random.value * 3f, 1f, 3f);
                    timer = 0;
                    anim.SetFloat("Input", 0);
                    state = enemyState.wait;
                }
                else
                {
                    if (returnToApproachState())
                    {

                        nav.destination = spot;
                        state = enemyState.approach;
                    }
                    else
                    {
                        removeFromSpot();
                    }
                }
            }
        }
        else
        {
            //chancetowait

            /*    if (ShouldWeWait())
                {
                    timer = 0;
                    state = enemyState.wait;
                }
                else
                {
                }*/
            if (returnToApproachState())
            {
                ConfirmedApproach();
            }
            else
            {
                removeFromSpot();
            }
        }

    }

    public void ConfirmedApproach()
    {
        assignSpot();
        nav.destination = spot;
        state = enemyState.approach;
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

    float waitvalue;

    float waitmultiplier;


    float enemyWaitChanceMultiplier()
    {
        //      waitvalue = p.enemyM.numberOfEnemies()/100f;
        waitmultiplier = 0;

        waitvalue = (p.enemyM.numberOfEnemies() - 1) * 25f;
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

        //continue after player we are targetting

        //also check if there is a closer available pos to us

        if (!rightSpot)
        {
        tempEP = p.enemyM.returnClosestAvailablePosition(this);
            if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
            {

                approach();
            }
            else
            {
                if (enemyposinmanager != tempEP)
                    Debug.Log(tempEP.whichOne);

                removeFromSpot();

            }
        }
        else
        {
            tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

            if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
            {

                approach();
            }
           /* else if(enemyposinmanager != tempEP)
            {
                //something breaking here?

                enemyposinmanager.enemy = null;
                enemyposinmanager = tempEP;
                enemyposinmanager.enemy= this;
                approach();
            }*/
            else
            {
                removeFromSpot();
            }
        }
        //old code 

        /*    tempEP = p.enemyM.returnClosestAvailablePosition(this);
            if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
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

    IEnumerator activateobs()
    {
        nav.enabled = false;
        yield return null;
enableallobs();
    }

    public void enableallobs()
    {

        for (int i = 0; i < 2; i++)
        {
            obs[i].enabled = true;
        }
    }


    public LayerMask invisibleWalls;

    //temporary bools VVVVV

    bool canMoveCam;
    //only written for logic^^^

    [HideInInspector]
    public bool cango;


    float distanceBetweenUs;


    bool backup;

    [HideInInspector]
    public bool approaching;

    Vector3 altSpot;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            /*  p = GameManager.instance.players[0];
              player = p.transform;
              StartCoroutine(resetNav());
              state = enemyState.stayaway;*/

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
                if (GameManager.instance.players.Count>1)
                {
                    //check whos closer

                        temp = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];

                    if (p != temp)
                    {


                            p = temp;
                            player = p.transform;
                            nav.ResetPath();
                            
                            //stay away from this player now
                        
                    }
                }
                else
                {


                    if (distx <= 2.2f || distanceBetweenUs< 2.1f)
                    {
                        if (distanceBetweenUs<1f)
                        {
                            if (enemyposinmanager.movepositions.x==-1)
                            {
                                Join();
                            }
                            else
                            {
                                ConfirmedApproach();
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
                    else if(distx>=2.5f || Vector3.Distance(transform.position, player.transform.position)>3f)
                    {

               
                            approaching = true;

                            if (nav.remainingDistance < 0.5f)
                            {
                                //       anim.SetFloat("Input", 0f);
                                if (player.transform.position.x < transform.position.x)
                                {
                                    altSpot = new Vector3(Mathf.Clamp( player.transform.position.x + 2.5f, player.transform.position.x + 2.5f, cam.transform.position.x +4.6f), 1.32f, transform.position.z);
                                }
                                else
                                {
                                    altSpot = new Vector3(Mathf.Clamp( player.transform.position.x - 2.5f, cam.transform.position.x- 4.6f, player.transform.position.x - 2.5f), 1.32f, transform.position.z);
                                }

                                //    if (distanceBetweenUs > lastDistanceBetweenUs)
                                //   {
                                //  lastDistanceBetweenUs = distanceBetweenUs;
                                if (Vector3.Distance(transform.position, altSpot) > 1f && distanceBetweenUs>3f)
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
                    
                }
                break;

            case enemyState.sidestep:
                if (nav.remainingDistance < 0.1f)
                {
                    //check approach basically but slightly adjusted
                   
                    if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
                    {



                        ConfirmedApproach();

                        //unsure about this******************************************************
                        StartCoroutine(tryToDoLiveMovement());
                    }
                    else
                    {                
                        removeFromSpot();
                    }
                }
             else if (GameManager.instance.players.Count > 1)
                {

                    if (Vector3.Distance(transform.position, spot) > 1f)
                        temp = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];

                    if (p != temp && temp.enemyM.checkiftherearespots())
                    {
                        if (Vector3.Distance(transform.position, enemyposinmanager.movepositions + p.transform.position) > temp.enemyM.returnspotdist(this))

                        {

                            p = temp;
                            player = p.transform;
                            nav.ResetPath();
                            removeFromSpot();
                        }
                    }
                }


                break;

            case enemyState.approach:


                if (GameManager.instance.players.Count > 1)
                {

                    if (Vector3.Distance(transform.position, spot) > 1f)
                        temp = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];

                    if (p != temp && temp.enemyM.checkiftherearespots() && canswitchtarg)
                    {
                        if (Vector3.Distance(transform.position, enemyposinmanager.movepositions + p.transform.position) > temp.enemyM.returnspotdist(this))

                        {
                            Debug.Log("swtch targ?");
                            canswitchtarg = false;
                            StartCoroutine(allowtargetswitch());


                            p = temp;
                            player = p.transform;
                            //return state to spawn :)

                            //  nav.enabled = false;
                            // obsmain.enabled = true;
                            //   state = enemyState.idle;

                            //find new spot
                            removeFromSpot();
                        }
                        else
                        {
                            checkApproach();
                        }
                    }
                    else
                    {
                        checkApproach();
                    }
                }
                else
                {
                    checkApproach();
                }
                break;

            case enemyState.idle:
                raycastCheck();
                if (rayhit)
                {
                    olp.time = 0;

                    rand = Random.Range(0f, 100f);
                    state = enemyState.spawn;
                    //if it doesnt hits random chance for checking player
                    if (rand <= chanceToCheck)
                    {        
                        anim.SetFloat("Input", 0);
                        StartCoroutine(activateobs());
                  //      nav.enabled = false;
                 //       obsmain.enabled = true;
     //                   Debug.Log("random attack");
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

                        if(!nav.enabled)
                        {
                            StartCoroutine(resetNav());
                        }

                        if (returnToApproachState())
                        {
                            state = enemyState.approach;
                            anim.SetFloat("Input", 1);

                            //idk about this here?
                            StartCoroutine(tryToDoLiveMovement());
                        }
                        else
                            removeFromSpot();
                    }
                    else
                    {
                        timestocheckbeforewemove += Time.deltaTime;

                        if (timestocheckbeforewemove > timeBeforeWeWander)
                        {
                            timestocheckbeforewemove = 0;
                            state = enemyState.wander;
                        }
                    }
                }

                break;

            case enemyState.wander:


                timer += Time.deltaTime;

                //     spotcols = Physics.OverlapSphere(transform.position, 1.5f, playerMask);
                spotcols = Physics.OverlapBox(transform.position, new Vector3(1, 1.5f, 0.3f), Quaternion.identity, playerMask);
                if (spotcols.Length > 0)
                {
                    p = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];
                    if (p.movement != Vector2.zero && needstogetaway)
                    {
                        //try to attack

                        returnToIdle();
                        return;
                    }
                }

                if (timer >= 3f)
                {

                    StartCoroutine(RandomPointInNavBounds());
                    timer = 0;
                    needstogetaway = false;
                }


                //check for other enemies

                break;

            case enemyState.walkingtospot:
                if (nav.remainingDistance < 0.05f)
                {
                    nav.velocity = Vector3.zero;
                    nav.enabled = false;
            enableallobs();
                    anim.SetFloat("Input", 0);
                    state = enemyState.wander;
                    return;
                }

                if (nav.remainingDistance < 0.15f)
                {
                    spotcols = Physics.OverlapSphere(spot, 0.1f, playernenemies);
                    if (spotcols.Length > 0)
                    {
                        anyoneThere = spotcols.ToList();

                        foreach (Collider col in spotcols)
                        {


                            if (col.transform.IsChildOf(transform.root))
                            {
                                anyoneThere.Remove(col);
                            }
                        }

                        if (anyoneThere.Count > 0)
                        {
                            //stop and find a new spot
                            nav.velocity = Vector3.zero;
                            nav.enabled = false;
                        enableallobs();
                            anim.SetFloat("Input", 0);
                            state = enemyState.wander;
                        }
                    }
                }
                else
                {
                    spotcols = Physics.OverlapBox(transform.position, new Vector3(1, 1, 0.3f), Quaternion.identity, playerMask);
                    if (spotcols.Length > 0)
                    {


                        if (GameManager.instance.players.Count > 1)
                        {
                            p = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];
                            player = p.transform;
                        }

                        //try to attack
                        if (p.movement != Vector2.zero)
                            returnToIdle(1);

                    }
                }

                break;

            case enemyState.livemovement:

                if (nav.remainingDistance < 0.05f)
                {
                    //check if right spot
                    approachOrMoveOtherSide();
                }
                else if (Vector3.Distance(transform.position, spot) < 15f)
                {
                    if (GameManager.instance.players.Count > 1)
                    {
                        temp = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];
                        if (temp != p && temp.enemyM.checkiftherearespots())
                        {
                            if (Vector3.Distance(transform.position, enemyposinmanager.movepositions + p.transform.position) > temp.enemyM.returnspotdist(this))
                            {
                                p = temp;
                                player = p.transform;
                              //  state = enemyState.spawn;

                                removeFromSpot();
                                return;
                            }
                        }
                    }

                    spotcols = Physics.OverlapSphere(transform.position, 0.5f, playerMask);

                    if (spotcols.Length > 0)
                    {

                        anyoneThere = spotcols.ToList();

                        if (anyoneThere.Count > 0)
                        {

                            //  nav.velocity = Vector3.zero;

                            if (GameManager.instance.players.Count > 1)
                            {
                                temp = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];
                                if (temp != p)
                                {
                                    if (temp.enemyM.checkiftherearespots())
                                    {
                                        p = temp;
                                        player = p.transform;

                                        removeFromSpot();
                                    }
                                    else
                                    {
                                        nav.ResetPath();
                                        //     nav.velocity = Vector3.zero;
                                        returnToIdle(0.5f);
                                    }
                                }
                                else
                                {
                                    if(enemyposinmanager.movepositions.x==-1)
                                    {
                                        returnToIdle(0.5f);
                                    }
                                    else
                                    {
                                        checkApproach();
                                    }
                                //    makeenemystopgoingtootherside();
                                }
                            }
                            else
                            {
                                // makeenemystopgoingtootherside();
                                if (enemyposinmanager.movepositions.x == -1)
                                {
                                    returnToIdle(0.5f);
                                }
                                else
                                {
                                    checkApproach();
                                }
                            }
                        }
                    }
                }
                break;

            case enemyState.wait:
                timer += Time.deltaTime;

                //     spotcols = Physics.OverlapSphere(transform.position, 1.5f, playerMask);
                spotcols = Physics.OverlapBox(transform.position, new Vector3(1, 1, 0.3f), Quaternion.identity, playerMask);
                if (spotcols.Length > 0)
                {
                    p = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];

                    //try to attack or go to approach idk?
                    waiting = false;

                    returnToIdle();
                    return;

                }

                if (timer >= timeTowait)
                {
                    waiting = false;

                    if (returnToApproachState())
                    {

                        nav.destination = spot;
                        state = enemyState.approach;
                    }
                    else
                    {
                        removeFromSpot();
                    }

                    timer = 0;
                    return;
                }

                break;
        }
    }

    float timeTowait;

    public void makeenemystopgoingtootherside()
    {
        if (rightSpot)
            rightSpot = false;

        assignSpot();

      //  if (!checkIfWeShouldFindCloserSpot())
    //    {
            if (returnToApproachState())
            {
                state = enemyState.approach;
                nav.destination = spot;
            }
            else
                removeFromSpot();
//}

    }


    void returnToIdle(float timeset = 0)
    {
        anim.SetFloat("Input", 0);
        if (nav.enabled)
        {
            nav.ResetPath();
        }
        else
        {
            nav.enabled = false;
            enableallobs();
        }
        timestocheckbeforewemove = 0;
        timeBeforeWeWander = timeset;
        state = enemyState.idle;
    }

    void dorandomattack()
    {
        //check if player is crouching or not
        // attBlend = 1;
        // cc.setAttBlend();
        //   comboctrl.punch();

        if (!p.crouch)
        {


            kickorpunch = Random.Range(0, 2);



            if(player.transform.position.x<transform.position.x) //player is left
            {
                if(!p.facingRight )
                {
                  randomAtt = Random.Range(-1, 2);
                }
                else
                {
                    randomAtt = Random.Range(-1, 1);
                }
            }
            else
            {
                if (p.facingRight)
                {
                    randomAtt = Random.Range(-1, 1);
                }
                else
                {
                    randomAtt = Random.Range(-1, 2);
                }
            }

            attBlend = randomAtt;
            cc.setAttBlend();

            if (kickorpunch == 0)
            {
                //do kick 
                cc.kick();
            }
            else
            {
                //make sure we are in reach to punch

                Debug.Log("punch");
                //punch
                cc.punch();
            }
        }
        else
        {
            crouchtest = Random.Range(0, 100f);

            if (crouchtest <= chanceToCrouch)
            {

                randomAtt = Random.Range(-1, 1);
                kickorpunch = Random.Range(1, 2);
                attBlend = randomAtt;
                cc.setAttBlend();

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
                //do low kick or medium kick? 
                    randomAtt = Random.Range(-1, 1);
                attBlend = randomAtt;
                cc.setAttBlend();
                cc.kick();
            }

        }
    }

    void properAttack()
    {
        if (p.rolling)
        {
            //low attack obviously

            return;
        }

        //WHAT ABOUT if jumping?

        if (!p.crouch)
        {
            //check if blocking
            if (p.blocking)
            {
                //do low kick or any crouch attack other than an uppercut
            }
            else
            {
                //try to go for high kick or high punch otherwise aim for body?
            }
        }
        else
        {
            //check if blocking
            if (p.blocking)
            {
                //do crouch kick?
            }
            else
            {
                //if we hit chance to crouch?
                //do anything except uppercut?

                //else 
                //do anything except for high kick/ or med/high punch
            }
        }
    }


    void chooseDifferentPathway()
    {
        //tell enemy to walk towards a spot inm the same z depth but to the other side of the player
    }

    public bool rightSpot;

    bool checkIfWeShouldFindCloserSpot()
    {
        if (!rightSpot)
        {


            spotindex = enemyposinmanager.whichOne;
            if (spotindex <= 1)
            {


                if (transform.position.x >= player.transform.position.x)
                {
                    return false;
                }
                else
                {
                    if (p.enemyM.checkiftherearespots())
                    {

                        //if there is something blocking the closer spot/we cant get a different spot

                        if (p.enemyM.checkSpotsOnOtherSide(this))
                        {
                            removeFromSpot();
                            return true;
                        }
                    }

                    return false;
                }

                /*   if (spotindex <= 1)
                   {
                       return false;
                   }
                   else
                   {

                       removeFromSpot();
                       return true;
                   }*/
            }
            else
            {
                if (transform.position.x < player.transform.position.x)
                {
                    return false;
                }
                else
                {
                    if (p.enemyM.checkiftherearespots())
                    {
                        //if there is something blocking the closer spot/we cant get a different spot
                        if (p.enemyM.checkSpotsOnOtherSide(this))
                        {
                            Debug.Log("available");
                            removeFromSpot();
                            return true;
                        }
                    }

                    return false;
                }
            }

        }
        else
        {

            timeInRightSpot += Time.deltaTime;
            if (timeInRightSpot > 3f)
            {
                rightSpot = false;
                timeInRightSpot = 0;
                StartCoroutine(allowRightSpot());
            }

            return false;
        }


    }

    float timeInRightSpot;

    IEnumerator allowtargetswitch()
    {
       yield return new WaitForSeconds(1f);
        canswitchtarg = true;
    }

   public void removeFromSpot()
    {

        Debug.Log("removing");
        //   state = enemyState.spawn;
        enemyposinmanager.enemy = null;
        enemyposinmanager = new enemyPositions();
        enemyposinmanager.movepositions.x = -1;
        //   enemyposinmanager = null;
        Join();

        //find better spot?

    }

    public void Join()
    {

        if (enemyposinmanager.movepositions.x == -1)
        {
       //     state = enemyState.spawn;

            if (canswitchtarg)
            {
                p = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];
                temp = p;
                player = p.transform;
            }


            if (p.enemyM.checkiftherearespots() && !p.floored)
            {
                p.enemyM.addToQueue(this);
            }
            else
            {
                 
                if (GameManager.instance.players.Count > 1)
                {

                    //check other player for spots
                    for (int i = 0; i < GameManager.instance.players.Count; i++)
                    {
                        if (p != GameManager.instance.players[i])
                        {
                            p = GameManager.instance.players[i];
                            player = p.transform;
                            temp = p;
                            break;
                        }
                    }

                    if (p.enemyM.checkiftherearespots() && !p.floored)
                    {
                        p.enemyM.addToQueue(this);

                    }
                    else
                    {
                        if (state != enemyState.walkingtospot && state != enemyState.wander)
                            goWander();
                    }
                }
                else
                {
                    if(state!=enemyState.walkingtospot && state!=enemyState.wander)
                    goWander();
                }
            }
        }
        else
        {

                assignSpot();
                if (!checkifanenemyfromotherplayerisinourspot())
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
                            anim.SetFloat("Input", 1);
                            ConfirmedApproach();
                        }
                    }
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

    public void goWander()
    {
        temp = null;
        needstogetaway = true;
        //wander etc
        //     disableobs();
        StartCoroutine(RandomPointInNavBounds());
    }

    IEnumerator resetnavAndReturnApproach()
    {
        disableobs();
        yield return null;
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

    public override void setBlock()
    {
        base.setBlock();

        if (state == enemyState.flinching)
            Join();
    }


    /* public IEnumerator RandomPointInBounds()
     {

         vec =randPnt();

         cols = Physics.OverlapSphere(vec, 1f, playernenemies);

         while (cols.Length>0)
         {

             vec =randPnt();

             cols = Physics.OverlapSphere(vec, 1f, playernenemies);

             yield return null;
         }

         nav.destination = vec;
         //call circle overlap all and check for player/enemies

     }

     Vector3 randPnt()
     {

         return new Vector3(Random.Range(cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Random.Range(-1.9f, 2f));
     }*/


    public Vector3 RandomNavSphere(float distance, int layermask)
    {
        vec = UnityEngine.Random.insideUnitSphere * distance;



        vec += transform.position;

        vec = new Vector3(Mathf.Clamp(vec.x, cam.transform.position.x - 5f, cam.transform.position.x + 5f), 1.32f, Mathf.Clamp(vec.z, -2f, 2f));

        NavMeshHit navHit;

        NavMesh.SamplePosition(vec, out navHit, distance, layermask);


        return navHit.position;
    }

    Collider[] playerCols;

    public IEnumerator RandomPointInNavBounds()
    {
        if(state!=enemyState.approach)
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || (!p.floored && p.enemyM.checkiftherearespots()));

        disableobs();
        wanderspot = RandomNavSphere(3f, 1);

        cols = Physics.OverlapSphere(new Vector3(wanderspot.x, 0.8f, wanderspot.z), 0.3f, enemyCol);


        wanderray = new Ray(transform.position, new Vector3(wanderspot.x, 1.32f, wanderspot.z) - transform.position);



        while ( tooclosetoplayer()|| cols.Length > 0 || Vector3.Distance(transform.position, new Vector3(wanderspot.x, transform.position.y, wanderspot.z)) < 1.5f || Physics.Raycast(wanderray, out wanderhit, Vector3.Distance(transform.position, new Vector3(wanderspot.x, 0.8f, wanderspot.z)), playerMask))
        {

            wanderspot = RandomNavSphere(3f, 1);

            wanderray = new Ray(transform.position, new Vector3(wanderspot.x, 1.32f, wanderspot.z) - transform.position);

            cols = Physics.OverlapSphere(new Vector3(wanderspot.x, 0.8f, wanderspot.z), 0.3f, enemyCol);

           

            yield return null;
        }
        yield return null;
        anim.SetFloat("Input", 1);
        nav.enabled = true;
        spot = wanderspot;
        nav.destination = wanderspot;
        state = enemyState.walkingtospot;
    }

    bool tooclosetoplayer()
    {
        playerCols = Physics.OverlapSphere(transform.position, 3f, playerMask);

        if (playerCols.Length > 0)
        {
            for (int i = 0; i < playerCols.Length; i++)
            {
                if(Vector3.Dot( playerCols[i].transform.root.transform.position - transform.position, new Vector3(wanderspot.x, 1.32f, wanderspot.z)- new Vector3( transform.position.x, 1.32f, transform.position.z)) >0.75f)
                {
                    return true;
                }
            }
        }
        else
        {
            return false;
        }
        return false;
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


}