using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunctions : MonoBehaviour
{
    Enemy enemy;

    int opposite;

    [HideInInspector]
    public Vector3 safeLocation;

    Collider[] enemyInSafeLocation;

    Ray invisiWallCheck;

    bool goNorth;

    bool GoSouth;

    Ray raynorthsouth;

    float northDist;

    float southDist;

    bool noneFound;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.efunct = this;
    }

    public void stepaside()
    {
        enemy.newspot = false;

        if (!enemy.nav.enabled)
        {
            enemy.disableobs();
            enemy.nav.enabled = true;
        }
        enemy.anim.SetFloat("Input", 1f);
        enemy.nav.ResetPath();

        //   tryOtherSpots();

        enemy.spot = transform.position + ((transform.position - enemy.pAtt.transform.position) * 0.5f);

       enemy.sideStepSpot = enemy.spot;
        enemy.sidestepcols = Physics.OverlapSphere(new Vector3(enemy.spot.x, 0.8f, enemy.spot.z), 0.2f, enemy.playernenemies);

        if (enemy.sidestepcols.Length < 1)
        {
            //do a raycast check first
            enemy.sidestepray = new Ray(new Vector3(transform.position.x, 0.8f, transform.position.z), new Vector3(transform.position.x, 0.8f, transform.position.z) - new Vector3(enemy.pAtt.transform.position.x, 0.8f, enemy.pAtt.transform.position.z));
            //if nothing hit, go there
            enemy.sidestepcasthit = Physics.RaycastAll(enemy.sidestepray.origin, enemy.sidestepray.direction, Vector3.Distance(enemy.sidestepray.origin, enemy.sidestepray.direction), enemy.allobjects);
            for (int i = 0; i < enemy.sidestepcasthit.Length; i++)
            {
                if (enemy.sidestepcasthit[i].transform.root != transform)
                {
                    enemy.newspot = true;
                    break;
                }
            }
            if (!enemy.newspot && opposite < 1)
            {
              opposite++;
                enemy.state = enemyState.sidestep;
                enemy.nav.destination = enemy.spot;
            }
            else
            {
                if (opposite > 2)
                    opposite = 0;
                else
                    opposite++;
                //find new spot
                tryOtherSpots();
            }
        }
        else
        {
            //try the live position spots? if greater in z/less in z etc
            //step left/right? idk

            tryOtherSpots();
        }
    }

    void tryOtherSpots()
    {
        //   Debug.Log("other spot");

        //south
        if (transform.position.z < enemy.pAtt.transform.position.z)
        {
            //we want to go south
            goSouthOrNorth(-1);
        }
        else //north
        {
            //  goNorth();
            goSouthOrNorth(1);
        }
    }



    void goSouthOrNorth(float northOrSouth)
    {
        enemy.newspot = false;
        enemy.spot = transform.position + new Vector3(0, 0, northOrSouth * 0.85f);
        if (northOrSouth == 1)
            enemy.spot = new Vector3(enemy.spot.x, 1.32f, Mathf.Clamp(enemy.spot.z, enemy.spot.z, northOrSouth * 2f));
        else
        {

            enemy.spot = new Vector3(enemy.spot.x, 1.32f, Mathf.Clamp(enemy.spot.z, northOrSouth * 2f, enemy.sideStepSpot.z));
        }
        enemy.sideStepSpot = enemy.spot;
        enemy.sidestepcols = Physics.OverlapSphere(new Vector3(enemy.sideStepSpot.x, 0.8f, enemy.sideStepSpot.z), 0.1f, enemy.allobjects);

        if (enemy.sidestepcols.Length < 1)
        {
            //do a raycast check first
            enemy.sidestepray = new Ray(new Vector3(transform.position.x, 0.8f, transform.position.z), new Vector3(enemy.sideStepSpot.x, 0.8f, enemy.sideStepSpot.z) - new Vector3(transform.position.x, 0.8f, transform.position.z));

            //if nothing hit, go there
            enemy.sidestepcasthit = Physics.RaycastAll(enemy.sidestepray.origin, enemy.sidestepray.direction, 0.75f, enemy.allobjects);

            for (int i = 0; i < enemy.sidestepcasthit.Length; i++)
            {
                if (enemy.sidestepcasthit[i].transform.root != transform)
                {
                    enemy.newspot = true;
                    break;
                }
            }
            if (!enemy.newspot)
            {
                enemy.nav.destination = enemy.spot;
                enemy.state = enemyState.sidestep;
            }
            else
            {
                keepFindingNewSideSpot(northOrSouth);
            }
        }
        else
        {
            //    state = enemyState.spawn;
            //   nav.ResetPath();
            keepFindingNewSideSpot(northOrSouth);
        }



    }

    RaycastHit sidesteprayhit;



    void keepFindingNewSideSpot(float northorsouth)
    {

        //do a raycast south/north to wall to check how close, if too close make enemy walk east/west

        enemy.sidestepray = new Ray(new Vector3(transform.position.x, 0.6f, transform.position.z), new Vector3(transform.position.x, 0.6f, transform.position.z) + new Vector3(0, 0, northorsouth));


        if (!Physics.Raycast(enemy.sidestepray, out sidesteprayhit, 0.3f, enemy.wallsOnly))
        {

            enemy.newspot = true;

            while (enemy.newspot)
            {

                enemy.newspot = false;

                enemy.spot = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(northorsouth * 0.3f, northorsouth * 2f));
                if (northorsouth == 1)
                    enemy.spot = new Vector3(enemy.spot.x, 1.32f, Mathf.Clamp(enemy.spot.z, enemy.spot.z + (0.2f * northorsouth * -1), northorsouth * 2f));
                else
                {

                    enemy.spot = new Vector3(enemy.spot.x, 1.32f, Mathf.Clamp(enemy.spot.z, northorsouth * 2f, enemy.spot.z + (0.2f * northorsouth * -1)));
                }
                enemy.sideStepSpot = enemy.spot;

                //find a new spot around this spot
                enemy.sidestepcols = Physics.OverlapSphere(new Vector3(enemy.spot.x, 0.6f, enemy.spot.z), 0.1f, enemy.allobjects);

                if (enemy.sidestepcols.Length < 1)
                {
                    //do a raycast check first
                    enemy.sidestepray = new Ray(new Vector3(transform.position.x, 0.6f, transform.position.z), new Vector3(enemy.spot.x, 0.6f, enemy.spot.z) - new Vector3(transform.position.x, 0.6f, transform.position.z));

                    //if nothing hit, go there
                    enemy.sidestepcasthit = Physics.RaycastAll(enemy.sidestepray.origin, enemy.sidestepray.direction, Mathf.Abs(transform.position.z - enemy.spot.z), enemy.invisipne);

                    for (int i = 0; i < enemy.sidestepcasthit.Length; i++)
                    {
                        if (enemy.sidestepcasthit[i].transform.root != transform)
                        {
                           enemy.newspot = true;
                            break;
                        }
                    }
                    if (!enemy.newspot)
                    {
                        //          Debug.Log(spot);
                        enemy.state = enemyState.sidestep;
                        enemy.nav.destination = enemy.spot;
                        break;
                    }
                }
                else
                   enemy.newspot = true;
            }
        }
        else
        {
            //   make enemy walk east/ west or stand still for a second?

            //left
            if (enemy.leftorRight() == 1)
            {
                enemy.spot = transform.position + Vector3.right;
                enemy.sidestepray = new Ray(new Vector3(transform.position.x, 0.6f, transform.position.z), Vector3.right + new Vector3(transform.position.x, 0.6f, transform.position.z));
            }
            else
            {
                enemy.spot = transform.position - Vector3.right;
                enemy.sidestepray = new Ray(new Vector3(transform.position.x, 0.6f, transform.position.z), new Vector3(transform.position.x, 0.6f, transform.position.z) - Vector3.right);
            }
            //check if the spot is  available otherwise, stand still for a second?




            //if nothing hit, go there
            enemy.sidestepcasthit = Physics.RaycastAll(enemy.sidestepray.origin, enemy.sidestepray.direction, Vector3.Distance(enemy.sidestepray.origin, enemy.sidestepray.direction), enemy.allobjects);

            if (enemy.sidestepcasthit.Length < 1)
            {

                enemy.state = enemyState.sidestep;
                enemy.nav.destination = enemy.spot;
            }
            else
            {
                //stand still for a second
                StartCoroutine(continueToPlayer());
            }
        }
    }

    public void keepDistance()
    {
        StartCoroutine(enemy.resetNav());
        enemy.state = enemyState.stayaway;
    }

    public IEnumerator continueToPlayer()
    {
       // Debug.Log("continue to player");

        yield return new WaitForSeconds(1f);

        enemy.ConfirmedApproach();
    }

    Ray wallcheckray;

    RaycastHit wallhitbyray;

    [HideInInspector]
    public float distToThatWall;


    bool enemyinsafelocationcheck()
    {
        for (int i = 0; i < enemyInSafeLocation.Length; i++)
        {
            if (enemyInSafeLocation[i].transform.root != transform)
            {
                return false;
            }
        }
        return true;
    }


    //east
    void movens(int ns)
    {
       noneFound = true;

        wallcheckray = new Ray(transform.position, (transform.position + new Vector3(0, 0, ns)) - transform.position);

        if (Physics.Raycast(wallcheckray, out wallhitbyray, 5f, enemy.wallsOnly))
        {
            distToThatWall = Mathf.Abs(transform.position.z - wallhitbyray.point.z);
        }

        for (float i = 0f; i < distToThatWall; i += 0.2f)
        {
            //first we overlapsphere to check if theres anyone there?

            if (ns == 1)
            {
                safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f, enemy.player.transform.position.x + 2.2f, enemy.cam.transform.position.x + 4.6f), transform.position.y, transform.position.z + i);
            }
            else
            {
                safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f, enemy.player.transform.position.x + 2.2f, enemy.cam.transform.position.x + 4.6f), transform.position.y, transform.position.z - i);
            }
            enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, safeLocation.z), 0.2f, enemy.rollh.nboxnobj);



            if (enemyinsafelocationcheck())
            {



                //           safeLocation = new Vector3(Mathf.Clamp(safeLocation.x, safeLocation.x, cam.transform.position.x + 5f), 0.4f, transform.position.z + i);
                if (Vector3.Distance(enemy.p.transform.position, safeLocation) > 3f)
                {

                    noneFound = false;
                    break;
                }
            }
        }

        if (!noneFound)
        {

            //  nav.velocity = Vector3.zero;
            enemy.cango = true;
            return;
            // nav.destination = safeLocation;


        }
        else
        {

            goOpp(-1);
            //try to go somehwere northwest/west of us?

            //do all mandatory checks again?
        }

    }


    void movensWest(int ns)
    {
        noneFound = true;

        wallcheckray = new Ray(transform.position, (transform.position + new Vector3(0, 0, ns)) - transform.position);

        if (Physics.Raycast(wallcheckray, out wallhitbyray, 5f, enemy.wallsOnly))
        {
            distToThatWall = Mathf.Abs(transform.position.z - wallhitbyray.point.z);
        }

        for (float i = 0f; i < distToThatWall; i += 0.2f)
        {
            //first we overlapsphere to check if theres anyone there?

            if (ns == 1)
            {
                safeLocation = new Vector3(Mathf.Clamp(enemy.p.transform.position.x - 2.2f, enemy.cam.transform.position.x - 4.6f, enemy.p.transform.position.x - 2.2f), transform.position.y, transform.position.z + i);
            }
            else
            {
                safeLocation = new Vector3(Mathf.Clamp(enemy.p.transform.position.x - 2.2f, enemy.cam.transform.position.x - 4.6f, enemy.p.transform.position.x - 2.2f), transform.position.y, transform.position.z - i);
            }
            enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, safeLocation.z), 0.2f, enemy.rollh.nboxnobj);



            if (enemyinsafelocationcheck())
            {



                //           safeLocation = new Vector3(Mathf.Clamp(safeLocation.x, safeLocation.x, cam.transform.position.x + 5f), 0.4f, transform.position.z + i);
                if (Vector3.Distance(enemy.p.transform.position, safeLocation) > 3f)
                {

                    noneFound = false;
                    break;
                }
            }
        }

        if (!noneFound)
        {

            //  nav.velocity = Vector3.zero;
            enemy.cango = true;
            return;
            // nav.destination = safeLocation;


        }
        else
        {

            goOpp(1);
            //try to go somehwere northwest/west of us?

            //do all mandatory checks again?
        }

    }

   public void checkforwall()
    {
        //this is from player, not enemy.

        //check north and south and return both values

        wallcheckray = new Ray(enemy.player.position, (enemy.player.position + new Vector3(0, 0, 1)) - enemy.player.position);

        if (Physics.Raycast(wallcheckray, out wallhitbyray, 5f, enemy.wallsnobj))
        {
          wallN = Mathf.Abs(enemy.player.position.z - wallhitbyray.point.z);
        }

        wallcheckray = new Ray(enemy.player.position, (enemy.player.position + new Vector3(0, 0, -1)) - enemy.player.position);

        if (Physics.Raycast(wallcheckray, out wallhitbyray, 5f, enemy.wallsnobj))
        {
         wallS = Mathf.Abs(enemy.player.position.z - wallhitbyray.point.z);
        }
    }


    bool closeToWall(int lor)
    {
        if (Mathf.Abs(enemy.player.transform.position.x - (enemy.cam.transform.position.x + (lor * 4.6f))) > 1f)
        {
            return true;
        }
        else
            return false;
    }

    bool barelyAnyDist()
    {
        goNorth = true;
        GoSouth = true;

        enemy.distz = Mathf.Abs(transform.position.z - enemy.player.transform.position.z);
        enemy.distx = Mathf.Abs(transform.position.x - enemy.spot.x);
        for (int i = -1; i < 2; i += 2)
        {



            raynorthsouth = new Ray(transform.position, (transform.position + new Vector3(0, 0, i)) - transform.position);

            if (!Physics.Raycast(raynorthsouth, 0.5f, enemy.wallsOnly))
            {
                continue;
            }
            else
            {
                if (i == -1)
                {
                    GoSouth = false;
                }
                else
                {
                    goNorth = false;
                }
            }

        }


        if (enemy.distz > 0.2f)
        {
            if (transform.position.z > enemy.player.transform.position.z)
            {
                GoSouth = false;
            }
            else
            {
                goNorth = false;
            }


            //return false;
        }



        // if (goNorth && GoSouth)
        //     return true;

        return false;
    }



    //handles moving enemy to a safe location
    public void findLocation()
    {
        enemy.cango = false;

        enemy.approaching = false;

        //left of us
        if (enemy.player.transform.position.x < transform.position.x || !closeToWall(-1))
        {
         //   Debug.Log("left of us");

            //if true allows us to go either north or south since distaance in z between enemy.player and to both walls north as south is insignificant, otherwise only go one way or the other

            if (!barelyAnyDist())
            {


                //      safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f, enemy.player.transform.position.x + 2.2f, cam.transform.position.x + 5f), transform.position.y, transform.position.z);
                safeLocation = new Vector3(enemy.player.transform.position.x + 2.2f, transform.position.y, transform.position.z);
                //first we overlapsphere to check if theres anyone there?

                enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, safeLocation.z), 0.2f, enemy.rollh.nboxnobj);

                //if no enemy there, then
                if (enemyinsafelocationcheck())
                {


                    //if enemy.player can move camera forward
                    //         if (GameManager.instance.canMoveCamera || transform.position.x < cam.transform.position.x+ 5f)
                    if (GameManager.instance.canMoveCamera || safeLocation.x < enemy.cam.transform.position.x + 4.6f)
                    {
                        //set safelocation                
                        //     nav.velocity = Vector3.zero;
                        safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f, enemy.player.transform.position.x + 2.2f, enemy.cam.transform.position.x + 4.6f), transform.position.y, transform.position.z);
                        enemy.cango = true;
                        return;
                    }
                    else
                    {

                        //raycast check for invisible walls

                        invisiWallCheck = new Ray(transform.position, (transform.position + Vector3.right) - transform.position);

                        //if there is no invisible wall
                        if (!Physics.Raycast(invisiWallCheck, Vector3.Distance(transform.position, safeLocation), enemy.invisibleWalls))
                        {

                            //set safelocation
                            safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f, enemy.player.transform.position.x + 2.2f, enemy.cam.transform.position.x + 4.6f), transform.position.y, transform.position.z);
                            //         safeLocation = new Vector3(Mathf.Clamp(player.transform.position.x + 2f, player.transform.position.x + 2f, cam.transform.position.x + 5f), 0.4f, transform.position.z + i);
                            enemy.cango = true;
                            return;
                            //    nav.destination = safeLocation;
                        }
                        else
                        {


                            if (goNorth) //north
                            {
                                movens(1);




                            }
                            else if (GoSouth)
                            {
                              //  Debug.Log("go south");
                                movens(-1);
                            }
                            else
                            {
                                //cant go north or south?

                                goOpp(-1);


                            }
                        }

                    }

                }
                else
                {
                    if (goNorth) //north
                    {
                        movens(1);




                    }
                    else if (GoSouth)
                    {
                      //  Debug.Log("go south");
                        movens(-1);
                    }
                    else
                    {
                        //cant go north or south?

                        goOpp(-1);


                    }
                }
            }
            else
            {

                if (Random.value * 100f < 50f)
                {
                    movens(1);
                }
                else
                    movens(-1);
                //we can go either  north or south, doesnt matter
            }

        }
        else //right of us
        {
            //if true allows us to go either north or south since distaance in z between player and to both walls north as south is insignificant, otherwise only go one way or the other

            if (!barelyAnyDist())
            {


                //      safeLocation = new Vector3(Mathf.Clamp(player.transform.position.x + 2.2f, player.transform.position.x + 2.2f, cam.transform.position.x + 5f), transform.position.y, transform.position.z);
                safeLocation = new Vector3(enemy.player.transform.position.x - 2.2f, transform.position.y, transform.position.z);
                //first we overlapsphere to check if theres anyone there?

                enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, safeLocation.z), 0.2f, enemy.rollh.nboxnobj);

                //if no enemy there, then
                if (enemyinsafelocationcheck())
                {


                    //if player can move camera forward
                    //         if (GameManager.instance.canMoveCamera || transform.position.x < cam.transform.position.x+ 5f)
                    if (safeLocation.x > enemy.cam.transform.position.x - 4.6f)
                    {
                        //set safelocation                
                        //     nav.velocity = Vector3.zero;
                        safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x - 2.2f, enemy.cam.transform.position.x - 4.6f, enemy.player.transform.position.x - 2.2f), transform.position.y, transform.position.z);
                        enemy.cango = true;
                        return;
                    }
                    else
                    {

                        //raycast check for invisible walls

                        invisiWallCheck = new Ray(transform.position, (transform.position - Vector3.right) - transform.position);

                        //if there is no invisible wall
                        if (!Physics.Raycast(invisiWallCheck, Vector3.Distance(transform.position, safeLocation), enemy.invisibleWalls))
                        {

                            //set safelocation
                            safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x - 2.2f, enemy.cam.transform.position.x - 4.6f, enemy.player.transform.position.x - 2.2f), transform.position.y, transform.position.z);
                            //         safeLocation = new Vector3(Mathf.Clamp(player.transform.position.x + 2f, player.transform.position.x + 2f, cam.transform.position.x + 5f), 0.4f, transform.position.z + i);
                            enemy.cango = true;
                            return;
                            //    nav.destination = safeLocation;
                        }
                        else
                        {


                            if (goNorth) //north
                            {
                                movensWest(1);




                            }
                            else if (GoSouth)
                            {
                            //    Debug.Log("go south");
                                movensWest(-1);
                            }
                            else
                            {
                                //cant go north or south?

                                goOpp(1);


                            }
                        }

                    }

                }
                else
                {

                    if (goNorth) //north
                    {
                        movensWest(1);




                    }
                    else if (GoSouth)
                    {
                      //  Debug.Log("go south");
                        movensWest(-1);
                    }
                    else
                    {
                        //cant go north or south?

                        goOpp(1);


                    }
                }
            }
            else
            {

                if (Random.value * 100f < 50f)
                {
                    movensWest(1);
                }
                else
                    movensWest(-1);
                //we can go either  north or south, doesnt matter
            }
        }
    }

    void goOpp(int leftOrRight)
    {
        for (int i = -1; i < 2; i += 2)
        {



            raynorthsouth = new Ray(transform.position, (transform.position + new Vector3(0, 0, i)) - transform.position);

            if (!Physics.Raycast(raynorthsouth, out wallhitbyray, 5f, enemy.wallsOnly))
            {
                continue;
            }
            else
            {
                if (i == -1)
                {
                    northDist = Mathf.Abs(transform.position.z - wallhitbyray.point.z);
                }
                else
                {
                    southDist = Mathf.Abs(transform.position.z - wallhitbyray.point.z);
                }
            }

        }


        raynorthsouth = new Ray(transform.position, (transform.position + (Vector3.right * leftOrRight)) - transform.position);

        if (Physics.Raycast(raynorthsouth, out wallhitbyray, 10f, enemy.invisibleWalls))
        {

            distToThatWall = Mathf.Abs(transform.position.x - wallhitbyray.point.x);
        }
        //depending on where we are standing relative to the player, try to go west/northwest/southwesT?

        for (float i = 0f; i < distToThatWall; i += 0.2f)
        {
            //first we overlapsphere to check if theres anyone there?
            if (leftOrRight == -1)
            {
                if (southDist > northDist) //south wall is further
                {

                    safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x - 2.2f - i, enemy.cam.transform.position.x - 4.6f, enemy.player.transform.position.x - 2.2f - i), 1.32f, Mathf.Clamp(transform.position.z + i, transform.position.z + i, 2f));
                }
                else
                {
                    safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x - 2.2f - i, enemy.cam.transform.position.x - 4.6f, enemy.player.transform.position.x - 2.2f - i), 1.32f, Mathf.Clamp(transform.position.z - i, -2f, transform.position.z - i));
                }
                // enemyInSafeLocation = Physics.OverlapSphere(new Vector3(Mathf.Clamp(safeLocation.x, player.transform.position.x - 2.2f - i, safeLocation.x), 0.4f, transform.position.z + i), 0.2f, rollh.enemyBox);
                //       enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, transform.position.z + i), 0.2f, rollh.enemyBox);
            }
            else
            {
                if (southDist > northDist)//south wall is further
                {
                    safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f + i, enemy.player.transform.position.x + 2.2f + i, enemy.cam.transform.position.x + 4.6f), 1.32f, Mathf.Clamp(transform.position.z + i, transform.position.z + i, 2f));
                }
                else
                {
                    safeLocation = new Vector3(Mathf.Clamp(enemy.player.transform.position.x + 2.2f + i, enemy.player.transform.position.x + 2.2f + i, enemy.cam.transform.position.x + 4.6f), 1.32f, Mathf.Clamp(transform.position.z - i, -2f, transform.position.z - i));
                }
                //   enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, transform.position.z + i), 0.2f, rollh.enemyBox);
            }
            enemyInSafeLocation = Physics.OverlapSphere(new Vector3(safeLocation.x, 0.4f, transform.position.z + i), 0.2f, enemy.rollh.nboxnobj);
            if (enemyinsafelocationcheck())
            {


                //           safeLocation = new Vector3(Mathf.Clamp(safeLocation.x, safeLocation.x, cam.transform.position.x + 5f), 0.4f, transform.position.z + i);
                if (Vector3.Distance(enemy.player.transform.position, safeLocation) > 2.1f)
                {
                    noneFound = false;
                    break;
                }


            }
        }

        if (!noneFound)
        {

            //  nav.velocity = Vector3.zero;
            enemy.cango = true;
            return;
            // nav.destination = safeLocation;


        }
    }


  float wallN;

  float wallS;

   public bool alternativeSideSpot()
    {
        checkforwall();
        enemy.distx = Mathf.Abs(transform.position.x - enemy.spot.x);

        //check if we are north or south

        if (transform.position.z < enemy.player.position.z)
        {
            //check distance between us

            if (enemy.distx > 2f)
            {

                //try either north or south
                if (wallN > 2f)
                {
                    return up();

                }
                else
                {
                    return down();
                }

            }
            else
            {
                // go with the one in our direction if its empty, otherwise try other way

                if (wallS > 2f)
                {
                    return down();
                }
                else
                {
                    return up();
                }
            }
        }
        else
        {
            if (enemy.distx > 2f)
            {

                //try either north or south
                if (wallN > 2f)
                {
                    return up();

                }
                else
                {
                    return down();
                }
            }
            else
            {
                if (wallN > 2f)
                {
                    return up();
                }
                else
                {
                    return down();
                }
            }
        }

    }

    bool up()
    {
        for (float i = 0.5f; i < 2; i += 0.5f)
        {
            enemy.livespotref = new Vector3(enemy.player.position.x, 0, enemy.player.position.z + i);

            enemy.spotcols = Physics.OverlapSphere(new Vector3(enemy.livespotref.x, 0.7f, enemy.livespotref.z), 0.75f, enemy.objectsnplayer);

            if (enemy.spotcols.Length < 1)
            {

                return true;
            }

        }
        return false;
    }

    bool down()
    {
        for (float i = 0.5f; i < 2; i += 0.5f)
        {
            enemy.livespotref = new Vector3(enemy.player.position.x, 0, enemy.player.position.z - i);

            enemy.spotcols = Physics.OverlapSphere(new Vector3(enemy.livespotref.x, 0.7f, enemy.livespotref.z), 0.75f, enemy.objectsnplayer);

            if (enemy.spotcols.Length < 1)
            {
                //   Debug.Log("found a spot");

                return true;
            }

        }
        return false;
    }



    public float enemymvaluesub(int whichOne)
    {
        if (whichOne > 1)
        {
            if (enemy.facingUs())
                return -1 * 0.5f;
            else
                return -0.34f;
        }
        else
        {
            if (enemy.facingUs())
                return 1 * 0.5f;
            else
                return 0.34f;

        }
    }

    public float enemymvaluesubsmaller(int whichOne)
    {
        if (whichOne > 1)
        {
            if (enemy.facingUs())
                return -1 * 0.5f;
            else
                return -0.28f;
        }
        else
        {
            if (enemy.facingUs())
                return 1 * 0.5f;
            else
                return 0.28f;

        }
    }


    public float valuesub()
    {
        if (enemy.enemyposinmanager.whichOne > 1)
        {
            if (enemy.facingUs())
                return -1 * 0.5f;
            else
                return -0.34f;
            //-.37f

            /*     if (facingUs())
                     return -1 * 0.45f;
                 else
                     return -1 * 0.3f;*/
        }
        else
        {
            if (enemy.facingUs())
                return 1 * 0.5f;
            else
                return 0.34f;

            /*    if (facingUs())
                return 1 * 0.45f;
            else
                return 1 * 0.3f;*/
        }
    }

    public float valuesubsmaller()
    {
        if (enemy.enemyposinmanager.whichOne > 1)
        {
            if (enemy.facingUs())
                return -1 * 0.5f;
            else
                return -0.28f;
            //-.37f

            /*     if (facingUs())
                     return -1 * 0.45f;
                 else
                     return -1 * 0.3f;*/
        }
        else
        {
            if (enemy.facingUs())
                return 1 * 0.5f;
            else
                return 0.28f;

            /*    if (facingUs())
                return 1 * 0.45f;
            else
                return 1 * 0.3f;*/
        }
    }
}
