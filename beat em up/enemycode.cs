using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemycode : MonoBehaviour
{

    public LayerMask enemiesnobjs;

    //also accounts for objects like crates etc
    bool checkifanenemyfromotherplayerisinourspot()
    {
        spotcols = Physics.OverlapSphere(new Vector3(spot.x, 0.8f, spot.z), 0.115f, enemiesnobjs);

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

                    /*    if (spotcols[i].transform.root.GetComponent<explode>() != null)
                        {
                            Debug.Log("something in our spot");
                            return true;
                        }
                        else if (spotcols[i].transform.root.GetComponent<Player>() != null & spotcols[i].transform.root != p.transform)
                        {
                            Debug.Log("something in our spot");
                            return true;
                        }*/
                }
            }

        }
        return false;
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


    private void Update()
    {



        /*  if (Input.GetKeyDown(KeyCode.F))
          {
              if (checkliveSpots())
              {
                  nav.velocity = Vector3.zero;
                  nav.destination = spot;
              }
          }*/


        switch (state)
        {
            //when we enter this state, set last distance between us to a high value

            case enemyState.stayaway:
                distanceBetweenUs = Vector3.Distance(transform.position, player.position);
                distx = Mathf.Abs(transform.position.x - player.transform.position.x);
                distz = Mathf.Abs(transform.position.z - player.transform.position.z);
                if (GameManager.instance.players.Count > 1)
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
                                assignSpot();
                                state = enemyState.approach;
                            }
                        }
                        if (!backup)
                        {


                            nav.ResetPath();


                            findLocation();

                            if (Vector3.Distance(transform.position, safeLocation) > 0.1f && cango)
                            {

                                nav.destination = safeLocation;
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
                                altSpot = new Vector3(Mathf.Clamp(player.transform.position.x + 2.5f, player.transform.position.x + 2.5f, cam.transform.position.x + 4.6f), 1.32f, transform.position.z);
                            }
                            else
                            {
                                altSpot = new Vector3(Mathf.Clamp(player.transform.position.x - 2.5f, cam.transform.position.x - 4.6f, player.transform.position.x - 2.5f), 1.32f, transform.position.z);
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

                }
                break;

            case enemyState.sidestep:
                if (nav.remainingDistance < 0.1f)
                {
                    //check approach basically but slightly adjusted

                    if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
                    {
                        //unsure about this******************************************************
                        StartCoroutine(tryToDoLiveMovement());



                        assignSpot();
                        nav.destination = spot;
                        state = enemyState.approach;

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
                        //    Debug.Log("attack");
                        //   anim.SetFloat("Input", 0);
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

                        if (!nav.enabled)
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


            //     case enemyState.spawn:
            // p.enemyM.StartCoroutine(p.enemyM.chooseSpotLeft());
            //      break;


            case enemyState.wander:


                timer += Time.deltaTime;

                //     spotcols = Physics.OverlapSphere(transform.position, 1.5f, playerMask);
                spotcols = Physics.OverlapBox(transform.position, new Vector3(1, 1, 0.3f), Quaternion.identity, playerMask);
                if (spotcols.Length > 0)
                {
                    p = GameManager.instance.players[GameManager.instance.findclosestPlayer(this)];
                    if (p.movement != Vector2.zero)
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
                }

                break;

            case enemyState.walkingtospot:
                if (nav.remainingDistance < 0.05f)
                {
                    nav.enabled = false;
                    enableallobs();
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
                            nav.enabled = false;
                            enableallobs();
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

                    //choose a new spot before approaching again

                    //    state = enemyState.approach;
                }

                //check if a player is very close to us?


                //check if we bump another enemy

                //if we are a good distance from the last spot?

                //  if(!checkIfWeShouldFindCloserSpot())
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
                                state = enemyState.spawn;

                                removeFromSpot();
                                return;
                            }
                        }
                    }


                    //            spotcols = Physics.OverlapBox(transform.position, new Vector3(1, 1, 0.3f), Quaternion.identity, playerMask);

                    spotcols = Physics.OverlapSphere(transform.position, 1f, playerMask);

                    if (spotcols.Length > 0)
                    {

                        anyoneThere = spotcols.ToList();


                        /* foreach(Collider col in spotcols)
                         {


                             if (col.transform.IsChildOf(transform.root))
                             {
                                 anyoneThere.Remove(col);
                             }
                         }*/

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
                                    makeenemystopgoingtootherside();
                                }
                            }
                            else
                            {
                                makeenemystopgoingtootherside();

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
            if (livespotCast(liveposAlt))
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

                    if (livespotCast(livePosition))
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
}
