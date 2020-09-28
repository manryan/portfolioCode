using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol = 1, Wait, Chasing, Dead }

public class Enemy : Warrior
{
    public CircleCollider2D playerDetection;

    public SpriteRenderer sprite;

    public enemyPositions posRef;

    public NavMeshAgent2D nav;

    public Transform target;

    public Transform dest;

    public EnemyState enemyState;

    public float[] movement = new[] { 0f, 5f };

    public int movementState;

    public float test;

    public List<Transform> patrolpoints = new List<Transform>();

    public int destPoint;

    public bool alerted;


    public Collider2D[] enemiesToDamage;

    public LayerMask player;

    public LayerMask walls;

    public EnemyManager manager;

    public NavMeshPath2D path;

    public List<Loot> regularLoot = new List<Loot>();

    public List<Loot> rareLoot = new List<Loot>();

    public bool gotHit;

    public bool saveDeath;

    public void Start()
    {
        manager = GameObject.Find("Player").GetComponent<EnemyManager>();
     //  nav.agent.autoBraking = false;
        target = patrolpoints[destPoint];
            dest = target;
        nav.destination = new Vector2(patrolpoints[destPoint].position.x, patrolpoints[destPoint].position.y);
        //    destPoint = (destPoint + 1) % patrolpoints.Count;
        enemyState = EnemyState.Patrol;
    }


  
    public void Alerts()
    {
        if (alerted)
        {
            anim.SetBool("Scanning", false);

            anim.Play("idle blend tree 2");

            nav.isStopped = false;
            manager.addEnemyToARing(this);


           // movement[0] = 2;
            target = enemiesToDamage[0].transform;
            //nav.destination = dest.position;
            //enemyState = EnemyState.Chasing;
           // nav.agent.stoppingDistance = 2;
        }
    }


    public void switching()
    {

        if (health > 0)
        {
            gotHit = false;

            nav.ResetPath();
        //alerted = false;
            enemyState = EnemyState.Patrol;

            destPoint = (destPoint + 1) % patrolpoints.Count;

            nav.destination = new Vector2(patrolpoints[destPoint].position.x, patrolpoints[destPoint].position.y);
            target = patrolpoints[destPoint];
            dest = target;

            nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(dest.position, transform.position) * 20f)), 0, 99);
        }
        //alerted = false;
    }



    public IEnumerator GotoNextPoint()
    {
        nav.agent.avoidancePriority = 99;
     //   nav.isStopped = true;

        // Returns if no points have been set up
        if (patrolpoints.Count == 0)
            yield return null;

        movementStateHandler();

        //   timer = 0;

        while (anim.GetBool("Scanning") == true)
        {
            if(gotHit)
            {
                //id.sneakattackskill check

                enemyState = EnemyState.Chasing;
                Alerts();

                yield break;
            }

            yield return null;
        }

        if (alerted)
        {

                nav.isStopped = false;
                manager.addEnemyToARing(this);
       //     movement[0] = 2;
         //   target = enemiesToDamage[0].transform;
           // nav.destination = dest.position;
     //       enemyState = EnemyState.Chasing;
         //   nav.agent.stoppingDistance = 2;
            yield break;
        }
        if (enemyState == EnemyState.Dead)
            yield break;

        nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(dest.position, transform.position) * 20f)), 0, 99);

        nav.isStopped = false;

        testint = destPoint;
        while(destPoint==testint)
        {
            destPoint = Random.Range(0, patrolpoints.Count-1);
        }
        //destPoint = (destPoint + 1) % patrolpoints.Count;

        nav.destination = new Vector2(patrolpoints[destPoint].position.x, patrolpoints[destPoint].position.y);

        target = patrolpoints[destPoint];

        dest = target;

        enemyState = EnemyState.Patrol;
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
       // destPoint = (destPoint + 1) % patrolpoints.Count;

    }

    public int testint;

    public void movementStateHandler()
    {
        if (!roll)
        {
            transform.GetChild(0).transform.up = target.position - transform.position;
            anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
            anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));

            test = 0;
            movementState = 0;
            while (movementState < movement.Length && (Vector3.Distance(dest.position, transform.position) > movement[movementState]))
            {
                movementState++;
                test += 0.5f;
            }
            if (movementState == 2)
                nav.agent.speed = 2;
            else
                nav.agent.speed = 1;

            if (nav.isStopped || nav.remainingDistance <= 0.01f)
                anim.SetFloat("Blend", 0);
            else
                anim.SetFloat("Blend", test);
        }
    }

    public Vector3 checker;

    public void FixedUpdate()
    {
        if(enemyState == EnemyState.Patrol || enemyState == EnemyState.Chasing)
        movementStateHandler();
    }

    public void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Patrol:
                if(alerted && (manager.potentialc1Positions.Count>1 || manager.potentialc2Positions.Count>1))
                {
                    nav.isStopped = false;
                    manager.addEnemyToARing(this);
                    enemyState = EnemyState.Chasing;
                }
                //       enemyState = EnemyState.Chasing;
                //     movementStateHandler();
                //       if (!nav.pathPending && nav.agent.remainingDistance < 0.1f)
                if (nav.agent.remainingDistance < 0.1f)
                {
                    anim.SetBool("Scanning", true);

                    anim.Play("ScanTree");

                    enemyState = EnemyState.Wait;

                    StartCoroutine(GotoNextPoint());
                    //  enemyState = EnemyState.Wait;
                    nav.isStopped = true;
                    return;
                }
                break;


            //inactive for now
            case EnemyState.Wait:

                enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, 4, player);

                if (enemiesToDamage.Length > 0)
                {
                    Vector3 vectorToCollider = (enemiesToDamage[0].transform.position - transform.position).normalized;



                    // 180 degree arc, change 0 to 0.5 for a 90 degree "pie"
                    if (Vector3.Dot(vectorToCollider, transform.GetChild(0).transform.up) > .5f)
                    {
                        //  RaycastHit2D hit = Physics2D.Raycast(transform.position, enemiesToDamage[0].transform.position - transform.position, walls);
                        //  if (hit.collider == null)
                        Alerts();
                    }
                }


                break;

            case EnemyState.Chasing:
                if(!roll)
                if (Vector3.Distance(transform.position, dest.transform.position) > 0.1f)
                {
                    if (checker != dest.position)
                    {
                        nav.destination = new Vector2( dest.position.x, dest.position.y);
                        checker = dest.position;
                        nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(dest.position, transform.position) * 20f)), 0, 99);
                    }
                    else if(nav.agent.remainingDistance<=0.1f)
                        nav.agent.avoidancePriority = 99;
                }
               // else
                //    nav.agent.avoidancePriority =99;
                //nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(enemy.dest.position, enemy.transform.position) * 20f)), 0, 99);

                //movementStateHandler();

                if(posRef.c2)
                if (canAttack)
                {
                    if ((Vector3.Distance(dest.position, transform.position) <= 0.2f && Vector3.Distance(transform.position, target.position) <= 2))
                    {
                        Fire();

                    }
                }
                        break;
        }
    }

    bool CalculateNewPath()
    {
       NavMeshPath navMeshPath = new NavMeshPath();
        nav.agent.CalculatePath(dest.position, navMeshPath);
       // print("New path calculated");
        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            return true;
        }
    }



    public virtual void Fire()
    {
        if (canAttack)
        {
            if (attackState == 0)
            {
                anim.SetBool("attacking", true);
                //  if (movement != Vector3.zero)
                //   transform.GetChild(0).transform.up = Vector3.zero + movement; 
                attacking = true;
                attackState++;
                StartCoroutine("WeaponCoolDown");
            }
            else if (attackState == 1)
            {
                if (anim.GetBool("attacking") == true)
                {
                    anim.SetBool("Combo", true);
                    attacking = true;
                    canAttack = false;
                    StopCoroutine("WeaponCoolDown");
                    StartCoroutine("ComboCoolDown");
                }
            }

        }
    }

    protected IEnumerator ComboCoolDown()
    {
        // yield return new WaitForSeconds(attCoolDown);
        yield return new WaitUntil(() => anim.GetBool("Combo") == false);
        yield return new WaitForSeconds(attCoolDown);
        if (anim.GetBool("Combo") == false)
        {
            attackState = 0;
            attacking = false;
            canAttack = true;

        }
    }


    public override void DamageEntity(float Damage)
    {
        gotHit = true;
        base.DamageEntity(Damage);
        if(health <=0)
        {
            enemyState = EnemyState.Dead;

            if (posRef != null)
                manager.leaveSpot(this);

            //disable navmeshagent !!

            nav.enabled = false;

            nav.agent.gameObject.SetActive(false);

            //disable damage collider !!
            damageCol.enabled = false;

            playerDetection.enabled = false;

            health = 0;


            anim.SetBool("Dead", true);

         //  anim.Play("Death Tree");

        }
    }

    public void Dead()
    {
        StartCoroutine(fadeOut());
    }

    public IEnumerator fadeOut()
    {
        var col = 1f;

        while (col > 0f)
        {
            col -= 0.05f;
            sprite.color = new Color(0, 0, 0, col);
            yield return null;
        }
        //  StopAllCoroutines();
        dropItems();
        if (saveDeath)
            manager.adjustList(transform.root.gameObject.name);
        else
            manager.checkIfDied(gameObject);
        transform.root.gameObject.SetActive(false);
    }

    public Vector3 placement;

    public float placementIndex;

   public Vector3 whereToPlace(int num, float increment)
    {
        if (increment == 0)
            return new Vector3(transform.position.x, transform.position.y, 0);

        if (increment == 1)
            placementIndex = 4f;

        switch(num)
        {
            case 0:
                placement = new Vector3(transform.position.x, transform.position.y+(placementIndex/10f) , 0);
                break;
            case 1:
                placement =  new Vector3(transform.position.x, transform.position.y - (placementIndex / 10f), 0);
                break;
            case 2:
                placement = new Vector3(transform.position.x - (placementIndex / 10f), transform.position.y, 0);
                break;
            case 3:
                placement = new Vector3(transform.position.x + (placementIndex / 10f), transform.position.y, 0);
                placementIndex += 4f;
                break;

        }

        return placement;
    }

    public void dropItems()
    {

        for (int i = 0; i < regularLoot.Count; i++)
        {
            if (Random.value * 100f < regularLoot[i].spawnChance)
            {

                ItemPickup ip = regularLoot[i].item.GetComponent<ItemPickup>();

                if (ip.item.itemType == ItemType.Gold)
                {
                    if (regularLoot[i].exactAmount)
                        ip.count = regularLoot[i].amountToDrop;
                    else
                        ip.count = Random.Range(50, 151);
                }
                else
                {
                    ip.count = regularLoot[i].amountToDrop;
                }

                //  Vector3 place = whereToPlace(i % 4, i);


                ip.madeFromScratch = true;

                GameManager.instance.player.objPoolManager.DropTable(regularLoot[i].item, whereToPlace(i % 4, i));

            }
        }

        for (int i = 0; i < rareLoot.Count; i++)
        {
            if (Random.value * 100f < rareLoot[i].spawnChance)
            {
                //myBoundary = new Boundary(transform.position.y + (i / 5f), transform.position.y - (i / 5f), transform.position.x - (i / 5f), transform.position.x + (i / 5f));

                ItemPickup ip = rareLoot[i].item.GetComponent<ItemPickup>();

                if (ip.item.itemType == ItemType.Gold)
                {
                    if (rareLoot[i].exactAmount)
                        ip.count = rareLoot[i].amountToDrop;
                    else
                        ip.count = Random.Range(50, 151);
                }
                else
                    ip.count = rareLoot[i].amountToDrop;

                ip.madeFromScratch = true;

                GameManager.instance.player.objPoolManager.DropTable(rareLoot[i].item, whereToPlace(i % 4, i));
                break;
            }
        }
    }

    public bool roll;

    public float angleToTry;

    public float compareAngle;

    public int angleIndex;

    public void rollAway(Transform theplayerpos)
    {
        //  transform.GetChild(0).transform.eulerAngles = theplayerpos.eulerAngles;
        transform.GetChild(0).transform.up = -((theplayerpos.parent.transform.position + theplayerpos.up) - transform.position);
        gameObject.layer = 2;
        spotsList = new List<Vector3>();
        roll = true;
        angleIndex = 0;
        for (int i = 0; i < spots.Length; i++)
        {
            Ray ray = new Ray(transform.position, ( spots[i] + transform.position)- transform.position);
            RaycastHit2D rayhit = Physics2D.Raycast(ray.origin, ray.direction, 1f, wallAndEnemies);

            if (rayhit.collider == null && Vector3.Dot(transform.GetChild(0).transform.up, new Vector3(transform.position.x + spots[i].x, transform.position.y + spots[i].y) - transform.position) > -.25f)
                spotsList.Add(spots[i]);
        }
            angleToTry = Vector3.Angle(transform.GetChild(0).transform.up, (new Vector3(transform.position.x + spotsList[0].x, transform.position.y + spotsList[0].y) - transform.position));
        for (int i = 1; i < spotsList.Count; i++)
        {

         compareAngle = Vector3.Angle(transform.GetChild(0).transform.up, (new Vector3(transform.position.x + spotsList[i].x, transform.position.y + spotsList[i].y) - transform.position));

            if(compareAngle< angleToTry)
            {
                angleToTry = compareAngle;
                angleIndex = i;
            }
        }
        anim.SetBool("Combo", false);
        nav.enabled = false;
        transform.GetChild(0).transform.up = (spotsList[angleIndex]+ transform.position) - transform.position;
        anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
        anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));
        //  rb.AddForce(transform.GetChild(0).transform.up, ForceMode2D.Impulse);
        //anim.Play("roll blend tree");
        anim.SetBool("Roll", true);
        //Debug.Log(spotsList[angleIndex]);
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        rb.AddForce(transform.GetChild(0).transform.up * 2f, ForceMode2D.Impulse);
    }



    public void endRoll()
    {
        anim.SetBool("Roll", false);
        rb.velocity = Vector2.zero;
        nav.agent.transform.position = NavMeshUtils2D.ProjectTo3D(transform.root.position);
        nav.enabled = true;
        roll = false;
        nav.destination = dest.position;
    }

    public LayerMask wallAndEnemies;

    public Vector3[] spots;

    public List<Vector3> spotsList;

    public Rigidbody2D rb;
}

[System.Serializable]
public class Loot
{
    public GameObject item;

    public float spawnChance;

    public int amountToDrop = 1;

    public bool exactAmount = false;
}


#region SETUP WITH WAITCASE
/*
public enum EnemyState { Patrol = 1, Wait, Chasing, Dead }

public class Enemy : Warrior
{

    public NavMeshAgent2D nav;

    public Transform target;

    public EnemyState enemyState;

    public float[] movement = new[] { 0f, 5f };

    public int movementState;

    public float test;

    public List<Transform> patrolpoints = new List<Transform>();

    public int destPoint;

    public float timer;

    public bool alerted;


    public Collider2D[] enemiesToDamage;

    public LayerMask player;

    public void Awake()
    {
        nav.agent.autoBraking = false;
        target = patrolpoints[destPoint];
        nav.destination = patrolpoints[destPoint].position;
        enemyState = EnemyState.Patrol;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            if (enemyState != EnemyState.Wait)
            {
                nav.isStopped = false;

                movement[0] = 2;
                target = c.transform;
                nav.destination = target.position;
                enemyState = EnemyState.Chasing;
            }
            nav.agent.stoppingDistance = 2;
            alerted = true;
            //   StartCoroutine(startAttacking());
        }
    }

    public void Alerts()
    {
        if (alerted)
        {
            nav.isStopped = false;

            movement[0] = 2;
            target = enemiesToDamage[0].transform;
            nav.destination = target.position;
            enemyState = EnemyState.Chasing;
            nav.agent.stoppingDistance = 2;
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            nav.agent.stoppingDistance = 0;
            movement[0] = 0;
            if (enemyState != EnemyState.Wait)
            {
                nav.ResetPath();
                enemyState = EnemyState.Patrol;
                nav.destination = patrolpoints[destPoint].position;
                target = patrolpoints[destPoint];

            }
            alerted = false;
        }
    }



    public IEnumerator GotoNextPoint()
    {

        nav.isStopped = true;

        // Returns if no points have been set up
        if (patrolpoints.Count == 0)
            yield return null;

        movementStateHandler();

        timer = 0;

        while (timer < 2f)
        {
            timer += Time.deltaTime;
            //    if (alerted)
            //   {
            //       yield break;
            //   }
            yield return null;
        }

        if (alerted)
        {
            nav.isStopped = false;

            movement[0] = 2;
            target = enemiesToDamage[0].transform;
            nav.destination = target.position;
            enemyState = EnemyState.Chasing;
            nav.agent.stoppingDistance = 2;
            yield break;
        }
        nav.isStopped = false;

        enemyState = EnemyState.Patrol;

        nav.destination = patrolpoints[destPoint].position;

        target = patrolpoints[destPoint];
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % patrolpoints.Count;

    }

    public void movementStateHandler()
    {
        transform.GetChild(0).transform.up = target.position - transform.position;
        anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
        anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));

        test = 0;
        movementState = 0;
        while (movementState < movement.Length && Vector3.Distance(target.position, transform.position) > movement[movementState])
        {
            movementState++;
            test += 0.5f;
        }
        if (movementState == 2)
            nav.agent.speed = 2;
        else
            nav.agent.speed = 1;

        if (nav.isStopped)
            anim.SetFloat("Blend", 0);
        else
            anim.SetFloat("Blend", test);
    }

    public void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Patrol:

                movementStateHandler();
                if (!nav.pathPending && nav.agent.remainingDistance < 0.1f)
                {
                    StartCoroutine(GotoNextPoint());
                    enemyState = EnemyState.Wait;

                    //anim.PLay(the scan blend tree);

                }
                break;


            //inactive for now
            case EnemyState.Wait:

                enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, 4, player);

                if (enemiesToDamage.Length > 0)
                {
                    Vector3 vectorToCollider = (enemiesToDamage[0].transform.position - transform.position).normalized;



                    // 180 degree arc, change 0 to 0.5 for a 90 degree "pie"
                    if (Vector3.Dot(vectorToCollider, transform.GetChild(0).transform.up) > 0.75f)
                    {
                        Alerts();
                    }
                }


                break;

            case EnemyState.Chasing:
                if (Vector3.Distance(target.position, transform.position) >= 2F)
                {

                    nav.destination = target.position;
                }

                movementStateHandler();

                if (canAttack && Vector3.Distance(target.position, transform.position) <= 2F)
                    Fire();
                break;

        }
    }

    public virtual void Fire()
    {
        if (canAttack)
        {
            if (attackState == 0)
            {
                anim.SetBool("attacking", true);
                //  if (movement != Vector3.zero)
                //   transform.GetChild(0).transform.up = Vector3.zero + movement; 
                attacking = true;
                attackState++;
                StartCoroutine("WeaponCoolDown");
            }
            else if (attackState == 1)
            {
                if (anim.GetBool("attacking") == true)
                {
                    anim.SetBool("Combo", true);
                    attacking = true;
                    canAttack = false;
                    StopCoroutine("WeaponCoolDown");
                    StartCoroutine("ComboCoolDown");
                }
            }

        }
    }

    protected IEnumerator ComboCoolDown()
    {
        // yield return new WaitForSeconds(attCoolDown);
        yield return new WaitUntil(() => anim.GetBool("Combo") == false);
        yield return new WaitForSeconds(attCoolDown);
        if (anim.GetBool("Combo") == false)
        {
            canAttack = true;
            attackState = 0;
            attacking = false;
        }
    }
}
*/
#endregion

#region NO WAITCASE
/*
 * public enum EnemyState { Patrol =1, Wait, Chasing, Dead }

public class Enemy : Warrior {

public NavMeshAgent2D nav;

public Transform target;

public EnemyState enemyState;

public float[] movement = new[] { 0f, 5f };

public int movementState;

public float test;

public List<Transform> patrolpoints = new List<Transform>();

public int destPoint;

public float timer;

public bool alerted;




public void Awake()
{
    nav.agent.autoBraking = false;
    target = patrolpoints[destPoint];
    nav.destination = patrolpoints[destPoint].position;
    enemyState = EnemyState.Patrol;
}

void OnTriggerEnter2D(Collider2D c)
{
    if (c.gameObject.tag == "Player")
    {
        nav.isStopped = false;
        nav.agent.stoppingDistance = 2;
        movement[0] = 2;
        target = c.transform;
        nav.destination = target.position;
        enemyState = EnemyState.Chasing;
        alerted = true;
        //   StartCoroutine(startAttacking());
    }
}

void OnTriggerExit2D(Collider2D c)
{
    if (c.gameObject.tag == "Player")
    {
        nav.agent.stoppingDistance = 0;
        movement[0] = 0;
        nav.ResetPath();
        enemyState = EnemyState.Patrol;
        nav.destination = patrolpoints[destPoint].position;
        target = patrolpoints[destPoint];
        alerted = false;
    }
}



public IEnumerator GotoNextPoint()
{

    nav.isStopped = true;

    // Returns if no points have been set up
    if (patrolpoints.Count == 0)
        yield return null;

    movementStateHandler();

    timer = 0;

    while (timer < 2f)
    {
        timer += Time.deltaTime;
        if (alerted)
        {
            yield break;
        }
        yield return null;
            }

        nav.isStopped = false;

        enemyState = EnemyState.Patrol;

        nav.destination = patrolpoints[destPoint].position;

        target = patrolpoints[destPoint];
        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % patrolpoints.Count;

}

public void movementStateHandler()
{
    transform.GetChild(0).transform.up = target.position - transform.position;
    anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
    anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));

  test = 0;
    movementState = 0;
    while (movementState < movement.Length && Vector3.Distance(target.position, transform.position) > movement[movementState])
    {
        movementState++;
        test += 0.5f;
    }
    if (movementState == 2)
        nav.agent.speed = 2;
    else
        nav.agent.speed = 1;

    if (nav.isStopped)
        anim.SetFloat("Blend", 0);
    else
        anim.SetFloat("Blend", test);
}

public void Update()
{
    switch (enemyState)
    {
        case EnemyState.Patrol:

            movementStateHandler();
            if (!nav.pathPending && nav.agent.remainingDistance < 0.1f)
            {
                StartCoroutine(GotoNextPoint());
                enemyState = EnemyState.Wait;
                //anim.PLay(the scan blend tree);
            }
            break;


            //inactive for now
        case EnemyState.Wait:



            //play the scout animation :)


            break;

        case EnemyState.Chasing:
            if (Vector3.Distance(target.position, transform.position) >= 2F)
            {

                nav.destination = target.position;
            }

            movementStateHandler();

            if (canAttack && Vector3.Distance(target.position, transform.position) <= 2F)
                Fire();
            break;

    }
}

public virtual void Fire()
{
    if (canAttack)
    {
        if (attackState == 0)
        {
            anim.SetBool("attacking", true);
            //  if (movement != Vector3.zero)
            //   transform.GetChild(0).transform.up = Vector3.zero + movement; 
            attacking = true;
            attackState++;
            StartCoroutine("WeaponCoolDown");
        }
        else if (attackState == 1)
        {
            if (anim.GetBool("attacking") == true)
            {
                anim.SetBool("Combo", true);
                attacking = true;
                canAttack = false;
                StopCoroutine("WeaponCoolDown");
                StartCoroutine("ComboCoolDown");
            }
        }

    }
}

public void hideSword()
{
    transform.GetChild(1).gameObject.SetActive(false);
}

public void showSword()
{
    transform.GetChild(1).gameObject.SetActive(true);
}

protected IEnumerator ComboCoolDown()
{
    // yield return new WaitForSeconds(attCoolDown);
    yield return new WaitUntil(() => anim.GetBool("Combo") == false);
      yield return new WaitForSeconds(attCoolDown);
    if (anim.GetBool("Combo") == false)
    {
        canAttack = true;
        attackState = 0;
        attacking = false;
    }
}
}
*/
#endregion

#region Updated with scan
/*

public enum EnemyState { Patrol = 1, Wait, Chasing, Dead }

public class Enemy : Warrior
{

public NavMeshAgent2D nav;

public Transform target;

public EnemyState enemyState;

public float[] movement = new[] { 0f, 5f };

public int movementState;

public float test;

public List<Transform> patrolpoints = new List<Transform>();

public int destPoint;

public bool alerted;


public Collider2D[] enemiesToDamage;

public LayerMask player;

public LayerMask walls;

public void Awake()
{
    nav.agent.autoBraking = false;
    target = patrolpoints[destPoint];
    nav.destination = patrolpoints[destPoint].position;
//    destPoint = (destPoint + 1) % patrolpoints.Count;
    enemyState = EnemyState.Patrol;
}

void OnTriggerEnter2D(Collider2D c)
{
    if (c.gameObject.tag == "Player")
    {
        if (enemyState != EnemyState.Wait)
        {
            nav.isStopped = false;

            movement[0] = 2;
            target = c.transform;
            nav.destination = target.position;
            enemyState = EnemyState.Chasing;
        }
        nav.agent.stoppingDistance = 2;
        alerted = true;
        //   StartCoroutine(startAttacking());
    }
}

public void Alerts()
{
    if (alerted)
    {
        nav.isStopped = false;

        movement[0] = 2;
        target = enemiesToDamage[0].transform;
        nav.destination = target.position;
        enemyState = EnemyState.Chasing;
        nav.agent.stoppingDistance = 2;
    }
}

void OnTriggerExit2D(Collider2D c)
{
    if (c.gameObject.tag == "Player")
    {
        nav.agent.stoppingDistance = 0;
        movement[0] = 0;
     //   if (enemyState != EnemyState.Wait)
       // {
            nav.ResetPath();
            enemyState = EnemyState.Patrol;
            nav.destination = patrolpoints[destPoint].position;
            target = patrolpoints[destPoint];

       // }
        alerted = false;
    }
}



public IEnumerator GotoNextPoint()
{

 //   nav.isStopped = true;

    // Returns if no points have been set up
    if (patrolpoints.Count == 0)
        yield return null;

    movementStateHandler();

    //   timer = 0;

    while (anim.GetBool("Scanning") == true)
    {
        // timer += Time.deltaTime;
        //    if (alerted)
        //   {
        //       yield break;
        //   }
        yield return null;
    }

    if (alerted)
    {
        nav.isStopped = false;

        movement[0] = 2;
        target = enemiesToDamage[0].transform;
        nav.destination = target.position;
        enemyState = EnemyState.Chasing;
        nav.agent.stoppingDistance = 2;
        yield break;
    }
    nav.isStopped = false;

    destPoint = (destPoint + 1) % patrolpoints.Count;

    nav.destination = patrolpoints[destPoint].position;

    target = patrolpoints[destPoint];

    enemyState = EnemyState.Patrol;
    // Choose the next point in the array as the destination,
    // cycling to the start if necessary.
   // destPoint = (destPoint + 1) % patrolpoints.Count;

}

public void movementStateHandler()
{
    transform.GetChild(0).transform.up = target.position - transform.position;
    anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
    anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));

    test = 0;
    movementState = 0;
    while (movementState < movement.Length && Vector3.Distance(target.position, transform.position) > movement[movementState])
    {
        movementState++;
        test += 0.5f;
    }
    if (movementState == 2)
        nav.agent.speed = 2;
    else
        nav.agent.speed = 1;

    if (nav.isStopped)
        anim.SetFloat("Blend", 0);
    else
        anim.SetFloat("Blend", test);
}

public void Update()
{
    switch (enemyState)
    {
        case EnemyState.Patrol:

            movementStateHandler();
            if (!nav.pathPending && nav.agent.remainingDistance < 0.1f)
            {
                anim.SetBool("Scanning", true);

                anim.Play("ScanTree");

                enemyState = EnemyState.Wait;

                StartCoroutine(GotoNextPoint());
                //  enemyState = EnemyState.Wait;
                nav.isStopped = true;
                return;
            }
            break;


        //inactive for now
        case EnemyState.Wait:

            enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, 4, player);

            if (enemiesToDamage.Length > 0)
            {
                Vector3 vectorToCollider = (enemiesToDamage[0].transform.position - transform.position).normalized;



                // 180 degree arc, change 0 to 0.5 for a 90 degree "pie"
                if (Vector3.Dot(vectorToCollider, transform.GetChild(0).transform.up) > 0.75f)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, enemiesToDamage[0].transform.position - transform.position, walls);
                    if (hit.collider == null)
                        Alerts();
                }
            }


            break;

        case EnemyState.Chasing:
            if (Vector3.Distance(target.position, transform.position) >= 2F)
            {

                nav.destination = target.position;
            }

            movementStateHandler();

            if (canAttack && Vector3.Distance(target.position, transform.position) <= 2F)
                Fire();
            break;

    }
}

public virtual void Fire()
{
    if (canAttack)
    {
        if (attackState == 0)
        {
            anim.SetBool("attacking", true);
            //  if (movement != Vector3.zero)
            //   transform.GetChild(0).transform.up = Vector3.zero + movement; 
            attacking = true;
            attackState++;
            StartCoroutine("WeaponCoolDown");
        }
        else if (attackState == 1)
        {
            if (anim.GetBool("attacking") == true)
            {
                anim.SetBool("Combo", true);
                attacking = true;
                canAttack = false;
                StopCoroutine("WeaponCoolDown");
                StartCoroutine("ComboCoolDown");
            }
        }

    }
}

protected IEnumerator ComboCoolDown()
{
    // yield return new WaitForSeconds(attCoolDown);
    yield return new WaitUntil(() => anim.GetBool("Combo") == false);
    yield return new WaitForSeconds(attCoolDown);
    if (anim.GetBool("Combo") == false)
    {
        canAttack = true;
        attackState = 0;
        attacking = false;
    }
}

}*/
#endregion

#region most recent
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol = 1, Wait, Chasing, Dead }

public class Enemy : Warrior
{
public CircleCollider2D playerDetection;

public SpriteRenderer sprite;

public enemyPositions posRef;

public NavMeshAgent2D nav;

public Transform target;

public Transform dest;

public EnemyState enemyState;

public float[] movement = new[] { 0f, 5f };

public int movementState;

public float test;

public List<Transform> patrolpoints = new List<Transform>();

public int destPoint;

public bool alerted;


public Collider2D[] enemiesToDamage;

public LayerMask player;

public LayerMask walls;

public EnemyManager manager;

public NavMeshPath2D path;

public void Start()
{
    manager = GameObject.Find("player").GetComponent<EnemyManager>();
 //  nav.agent.autoBraking = false;
    target = patrolpoints[destPoint];
    nav.destination = patrolpoints[destPoint].position;
    //    destPoint = (destPoint + 1) % patrolpoints.Count;
        enemyState = EnemyState.Patrol;
        dest = target;
}



public void Alerts()
{
    if (alerted)
    {
        anim.SetBool("Scanning", false);

        anim.Play("idle blend tree 2");

        nav.isStopped = false;
        manager.addEnemyToARing(this);


       // movement[0] = 2;
        target = enemiesToDamage[0].transform;
        //nav.destination = dest.position;
        //enemyState = EnemyState.Chasing;
       // nav.agent.stoppingDistance = 2;
    }
}


public void switching()
{


    nav.ResetPath();
    enemyState = EnemyState.Patrol;

    destPoint = (destPoint + 1) % patrolpoints.Count;

    nav.destination = patrolpoints[destPoint].position;
    target = patrolpoints[destPoint];
    dest = target;

    nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(dest.position, transform.position) * 20f)), 0, 99);

    //alerted = false;
}



public IEnumerator GotoNextPoint()
{
    nav.agent.avoidancePriority = 99;
 //   nav.isStopped = true;

    // Returns if no points have been set up
    if (patrolpoints.Count == 0)
        yield return null;

    movementStateHandler();

    //   timer = 0;

    while (anim.GetBool("Scanning") == true)
    {
        // timer += Time.deltaTime;
        //    if (alerted)
        //   {
        //       yield break;
        //   }
        yield return null;
    }

    if (alerted)
    {

            nav.isStopped = false;
            manager.addEnemyToARing(this);
   //     movement[0] = 2;
     //   target = enemiesToDamage[0].transform;
       // nav.destination = dest.position;
 //       enemyState = EnemyState.Chasing;
     //   nav.agent.stoppingDistance = 2;
        yield break;
    }
    nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(dest.position, transform.position) * 20f)), 0, 99);

    nav.isStopped = false;

    destPoint = (destPoint + 1) % patrolpoints.Count;

    nav.destination = patrolpoints[destPoint].position;

    target = patrolpoints[destPoint];

    enemyState = EnemyState.Patrol;
    // Choose the next point in the array as the destination,
    // cycling to the start if necessary.
   // destPoint = (destPoint + 1) % patrolpoints.Count;

}

public void movementStateHandler()
{
    transform.GetChild(0).transform.up = target.position - transform.position;
    anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
    anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));

    test = 0;
    movementState = 0;
    while (movementState < movement.Length && (Vector3.Distance(target.position, transform.position) > movement[movementState]|| Vector3.Distance(dest.position, transform.position) > movement[movementState]))
    {
        movementState++;
        test += 0.5f;
    }
    if (movementState == 2)
        nav.agent.speed = 2;
    else
        nav.agent.speed = 1 ;

    if (nav.isStopped || nav.remainingDistance<=0.01f)
        anim.SetFloat("Blend", 0);
    else
        anim.SetFloat("Blend", test);
}

public Vector3 checker;

public void FixedUpdate()
{
    if(enemyState != EnemyState.Patrol || enemyState != EnemyState.Chasing)
    movementStateHandler();
}

public void Update()
{
    switch (enemyState)
    {
        case EnemyState.Patrol:
            if(alerted && (manager.potentialc1Positions.Count>1 || manager.potentialc2Positions.Count>1))
            {
                nav.isStopped = false;
                manager.addEnemyToARing(this);
                enemyState = EnemyState.Chasing;
            }
         //       enemyState = EnemyState.Chasing;
       //     movementStateHandler();
            if (!nav.pathPending && nav.agent.remainingDistance < 0.1f)
            {
                anim.SetBool("Scanning", true);

                anim.Play("ScanTree");

                enemyState = EnemyState.Wait;

                StartCoroutine(GotoNextPoint());
                //  enemyState = EnemyState.Wait;
                nav.isStopped = true;
                return;
            }
            break;


        //inactive for now
        case EnemyState.Wait:

            enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, 4, player);

            if (enemiesToDamage.Length > 0)
            {
                Vector3 vectorToCollider = (enemiesToDamage[0].transform.position - transform.position).normalized;



                // 180 degree arc, change 0 to 0.5 for a 90 degree "pie"
                if (Vector3.Dot(vectorToCollider, transform.GetChild(0).transform.up) > 0.75f)
                {
                    //  RaycastHit2D hit = Physics2D.Raycast(transform.position, enemiesToDamage[0].transform.position - transform.position, walls);
                    //  if (hit.collider == null)
                    Alerts();
                }
            }


            break;

        case EnemyState.Chasing:
            if (Vector3.Distance(transform.position, dest.transform.position) > 0.1f)
            {
                if (checker != dest.position)
                {
                    nav.destination = dest.position;
                    checker = dest.position;
                    nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(dest.position, transform.position) * 20f)), 0, 99);
                }
                else if(nav.agent.remainingDistance<=0.1f)
                    nav.agent.avoidancePriority = 99;
            }
           // else
            //    nav.agent.avoidancePriority =99;
            //nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(enemy.dest.position, enemy.transform.position) * 20f)), 0, 99);

            //movementStateHandler();

            if(posRef.c2)
            if (canAttack)
            {
                if ((Vector3.Distance(dest.position, transform.position) <= 0.2f && Vector3.Distance(transform.position, target.position) <= 2)|| Vector3.Distance(transform.position, target.position) <= 2)
                {
                    Fire();

                }
            }
                    break;
    }
}

bool CalculateNewPath()
{
   NavMeshPath navMeshPath = new NavMeshPath();
    nav.agent.CalculatePath(dest.position, navMeshPath);
   // print("New path calculated");
    if (navMeshPath.status != NavMeshPathStatus.PathComplete)
    {
        return false;
    }
    else
    {
        return true;
    }
}

public virtual void Fire()
{
    if (canAttack)
    {
        if (attackState == 0)
        {
            anim.SetBool("attacking", true);
            //  if (movement != Vector3.zero)
            //   transform.GetChild(0).transform.up = Vector3.zero + movement; 
            attacking = true;
            attackState++;
            StartCoroutine("WeaponCoolDown");
        }
        else if (attackState == 1)
        {
            if (anim.GetBool("attacking") == true)
            {
                anim.SetBool("Combo", true);
                attacking = true;
                canAttack = false;
                StopCoroutine("WeaponCoolDown");
                StartCoroutine("ComboCoolDown");
            }
        }

    }
}

protected IEnumerator ComboCoolDown()
{
    // yield return new WaitForSeconds(attCoolDown);
    yield return new WaitUntil(() => anim.GetBool("Combo") == false);
    yield return new WaitForSeconds(attCoolDown);
    if (anim.GetBool("Combo") == false)
    {
        attackState = 0;
        attacking = false;
        canAttack = true;

    }
}


public override void DamageEntity(float Damage)
{
    base.DamageEntity(Damage);
    if(health <=0)
    {
        if (posRef != null)
            manager.leaveSpot(this);

        enemyState = EnemyState.Dead;

        //disable navmeshagent !!

        nav.enabled = false;

        nav.agent.gameObject.SetActive(false);

        //disable damage collider !!
        damageCol.enabled = false;

        playerDetection.enabled = false;

        health = 0;


        anim.SetBool("Dead", true);

     //  anim.Play("Death Tree");

    }
}

public void Dead()
{
    StartCoroutine(fadeOut());
}

public IEnumerator fadeOut()
{
    var col = 1f;

    while (col > 0f)
    {
        col -= 0.05f;
        sprite.color = new Color(0, 0, 0, col);
        yield return null;
    }
    //  StopAllCoroutines();
    manager.adjustList(gameObject.name);
    gameObject.SetActive(false);
}



}

 */
#endregion