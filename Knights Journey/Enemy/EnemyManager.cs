using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class enemyPositions
{
    public Transform transform;
    public GameObject enemy;

    public bool c1;
    public bool c2;

    public bool usable = true;
}


[System.Serializable]
public class data
{

    public Vector3 x;

    public float enemyHealth;

    public bool active;
}

[System.Serializable]
public class dataContainer
{
    public List<data> dataHolder;


    public List<string> alive;

    public List<string> dead;
}

public class EnemyManager : MonoBehaviour {

    public dataContainer tester;

    public int time;

    public bool attacking = false;

    public LayerMask walls;

    public GameObject innerRing;

    public GameObject OuterRing;

    public List<enemyPositions> Innerpositions = new List<enemyPositions>();


    public List<enemyPositions> Outerpositions = new List<enemyPositions>();


    public List<enemyPositions> potentialc1Positions = new List<enemyPositions>();

    public List<enemyPositions> potentialc2Positions = new List<enemyPositions>();

    const int INNERMAXPOSITIONS = 4;


    const int OUTERMAXPOSITIONS = 6;

    public List<GameObject> enemies;

    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        string jsonString = PlayerPrefs.GetString("TestSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex);
        tester = JsonUtility.FromJson<dataContainer>(jsonString);
    }

    public void adjustList(string enemy)
    {
        tester.alive.Remove(enemy);
        tester.dead.Add(enemy);
    }

    public void initializeEnemies()
    {
        tester = new dataContainer();
        tester.dead = new List<string>();
        tester.alive = new List<string>();
        tester.dataHolder = new List<data>();

        foreach (GameObject enemy in enemies)
        {
                tester.alive.Add(enemy.name);
                data storage = new data { x = enemy.transform.position };
                tester.dataHolder.Add(storage);
                storage.active = true;
        }
        string json = JsonUtility.ToJson(tester);
        PlayerPrefs.SetString("TestSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex, json);

        PlayerPrefs.Save();
    }


    public void saveEm()
     {
         // string jsonString = PlayerPrefs.GetString("TestSave" + saveIndex);
         //tester = JsonUtility.FromJson<dataContainer>(jsonString);

             tester.dataHolder.Clear();

         foreach (GameObject obj in enemies)
         {
            if (tester.alive.Contains(obj.name))
            {
                data storage = new data { x = obj.GetComponent<NavMeshAgent2D>().agent.transform.position, enemyHealth = obj.transform.GetChild(0).GetChild(0).GetComponent<Warrior>().health };
                tester.dataHolder.Add(storage);
                storage.active = true;
            }

         }
        string json = JsonUtility.ToJson(tester);
        PlayerPrefs.SetString("TestSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex, json);
        PlayerPrefs.Save();
     }


    public void checkIfDied(GameObject obj)
    {
       if( tester.alive.Contains(obj.name))
        {
            tester.dataHolder[tester.alive.IndexOf(obj.name)].enemyHealth = 0;
        }
    }

    public IEnumerator loadEnemies()
    {
        int i = 0;

        string jsonString = PlayerPrefs.GetString("TestSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex);
        tester = JsonUtility.FromJson<dataContainer>(jsonString);

        foreach (GameObject obj in enemies)
        {
            if (tester.alive.Contains(obj.name))
            {
                Enemy enemy = obj.transform.GetChild(0).GetChild(0).GetComponent<Enemy>();
                if(!enemy.saveDeath && tester.dataHolder[i].enemyHealth >0f)
                {
                    enemy.health = tester.dataHolder[i].enemyHealth;
                    enemy.nav.agent.transform.position = tester.dataHolder[i].x;
                }
                if (enemy.saveDeath)
                {
                    enemy.health = tester.dataHolder[i].enemyHealth;
                    enemy.nav.agent.transform.position = tester.dataHolder[i].x;
                }
                i++;
            }
            else
            {
                obj.SetActive(false);
            }
        }

        yield return null;

        transform.root.GetComponent<NavMeshObstacle2D>().enabled = true;

              /*  foreach (string enemy in tester.alive)
        {
            GameObject reference = GameObject.Find(enemy);
            reference.transform.position = tester.dataHolder[i].x;
            i++;
        }
        foreach (string enemy in tester.dead)
        {
            GameObject reference = GameObject.Find(enemy);
            reference.SetActive(false);
        }*/
    }

    public void Start()
    {
        for (int i = 0; i < INNERMAXPOSITIONS; i++)
        {
            enemyPositions newpos = new enemyPositions();
            newpos.transform = innerRing.transform.GetChild(i);
            newpos.c1 = true;

            Innerpositions.Add(newpos);
        }
        for (int i = 0; i < OUTERMAXPOSITIONS; i++)
        {
            enemyPositions newpos = new enemyPositions();
            newpos.transform = OuterRing.transform.GetChild(i);
            newpos.c2 = true;

            Outerpositions.Add(newpos);
        }

        foreach (enemyPositions pos in Innerpositions)
        {

            if (pos.enemy != null)
                Debug.Log("not empty");
            else
            {
                potentialc1Positions.Add(pos);
            }
        }

        foreach (enemyPositions pos in Outerpositions)
        {
            if (pos.enemy != null)
                Debug.Log("not empty");
            else
            {
                potentialc2Positions.Add(pos);
            }
        }
    }

   Transform ClosestC1(Enemy enemy, List<enemyPositions> thePositions)
    {

        bool trial = false;
        if (!trial)
            foreach (enemyPositions pos in Outerpositions)
            {
                if (pos.enemy == enemy.gameObject)
                {
                    trial = true;
                    break;
                }
            }
        if (!trial)
            foreach (enemyPositions pos in Innerpositions)
            {
                if (pos.enemy == enemy.gameObject)
                {
                    trial = true;
                    break;
                }
            }

        if(trial)
            return null;

        //compare squared distance of threshold and the sqrMagnitude to the target.
        var closestDistance = 0f;

        //which place we want in the array
        var targetNumber = 0;
        //loop through entire player transform array
        for (int i = 0; i < thePositions.Count; i++)
        {
            if (thePositions[i].usable)
            {

                //compare squared distance of different index positions other than 0 (first)  and the sqrMagnitude to the target.

                var thisDistance = (thePositions[i].transform.position - enemy.transform.position).sqrMagnitude;

                if (closestDistance == 0)
                    closestDistance = thisDistance;

                //if first comparison is less than the other,  set the comparisons to equal each other 
                //and select what position in the array it is otherwise select the first players position

                if (thisDistance < closestDistance)
                {
                    closestDistance = thisDistance;

                    targetNumber = i;
                }
            }
        }
            thePositions[targetNumber].enemy = enemy.gameObject;
            enemy.posRef = thePositions[targetNumber];
            //return it to the serialized field.
            return thePositions[targetNumber].transform;
    }


    public void checkSpot()
    {
        foreach (enemyPositions pos in Innerpositions)
        {
            Ray ray = new Ray(transform.position, pos.transform.position - transform.position);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, pos.transform.position), walls);

            if (rayHit.collider != null)
            {
                pos.usable = false;
                potentialc1Positions.Remove(pos);

                if(pos.enemy!=null)
                {
                    GameObject temp = pos.enemy;
                    leaveSpot(pos.enemy.GetComponent<Enemy>());
                    addEnemyToARing(temp.GetComponent<Enemy>());
                    pos.enemy = null;
                    
                }
                //   Debug.Log("ray hit");
            }
            else
            {
                pos.usable = true;

                if (pos.enemy == null && !potentialc1Positions.Contains(pos))
                    potentialc1Positions.Add(pos);
            }
        }
        foreach (enemyPositions pos in Outerpositions)
        {
            Ray ray = new Ray(transform.position, pos.transform.position - transform.position);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, pos.transform.position), walls);

            if (rayHit.collider != null)
            {
                pos.usable = false;
              //  Debug.Log("ray hit");
                potentialc2Positions.Remove(pos);

                if (pos.enemy != null)
                {
                    GameObject temp = pos.enemy;
                    leaveSpot(pos.enemy.GetComponent<Enemy>());
                    addEnemyToARing(temp.GetComponent<Enemy>());
                    pos.enemy = null;

                }
            }
            else
            {
                pos.usable = true;


                if (pos.enemy == null && !potentialc2Positions.Contains(pos))
                    potentialc2Positions.Add(pos);

            }
        }

        updateEnemies();

        /*if (potentialc1Positions.Count >0 && potentialc2Positions.Count < OUTERMAXPOSITIONS)
            foreach (enemyPositions pos in Outerpositions)
        {
            if (pos.enemy != null)
            {
                GameObject temp = pos.enemy;
                leaveSpot(pos.enemy.GetComponent<Enemy>());
                addEnemyToARing(temp.GetComponent<Enemy>());
                pos.enemy = null;

            }
            if (potentialc1Positions.Count < 1 && potentialc2Positions.Count >= OUTERMAXPOSITIONS)
                break;

        }*/
    }

    public void addEnemyToARing(Enemy enemy)
        {
        checkSpot();

        if (potentialc1Positions.Count > 0)
        {
            Transform test = ClosestC1(enemy, potentialc1Positions);
            if (test != null)
            {
                enemy.dest = test;
                enemy.target = transform;
                enemy.enemyState = EnemyState.Chasing;
                // enemy.nav.destination = enemy.dest.position;

                enemy.nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(enemy.dest.position, enemy.transform.position) * 20f)), 0, 99);
            }
            foreach (enemyPositions pos in potentialc1Positions.ToList())
            {
                if (pos.enemy != null || !pos.usable)
                    potentialc1Positions.Remove(pos);
            }

        }
        else if (potentialc2Positions.Count > 0)
        {
            Transform test = ClosestC1(enemy, potentialc2Positions);
            if (test != null)
            {
                enemy.dest = test;
                enemy.target = transform;
                enemy.enemyState = EnemyState.Chasing;
                enemy.nav.agent.avoidancePriority = Mathf.Clamp(Mathf.RoundToInt(99f - (Vector3.Distance(enemy.dest.position, enemy.transform.position) * 20f)), 0, 99);
                //enemy.nav.destination = enemy.dest.position;
            }
            foreach (enemyPositions pos in potentialc2Positions.ToList())
            {
                if (pos.enemy != null || !pos.usable)
                    potentialc2Positions.Remove(pos);
            }

        }
        else
        {
            enemy.switching();
        }
        if (!attacking)
            StartCoroutine(Attack());
    }

    public void leaveSpot(Enemy enemy)
    {


        enemyPositions test = enemy.posRef;

        test.enemy = null;

        enemy.posRef = null;

        //add additional positions
        if (test.c1)
        foreach (enemyPositions pos in Innerpositions.ToList())
        {
            if (pos.enemy == null && !potentialc1Positions.Contains(pos))
                potentialc1Positions.Add(pos);
        }
        else
        foreach (enemyPositions pos in Outerpositions.ToList())
        {
            if (pos.enemy == null && !potentialc2Positions.Contains(pos))
                potentialc2Positions.Add(pos);
        }
        checkSpot();

        updateEnemies();
    }

 //  for moving enemies from outer to inner spot

    public void updateEnemies()
    {
        foreach(enemyPositions pos in Outerpositions)
        {
            if (potentialc1Positions.Count > 0)
            {
                if (pos.enemy != null)
                {
                   GameObject temp = pos.enemy;
                    leaveSpot(pos.enemy.GetComponent<Enemy>());
                    addEnemyToARing(temp.GetComponent<Enemy>());
                    //pos.enemy = null;
                }
            }
            else
                break;
        }
    }

    //need a function for removing the enemy that left?

    //update enemies positions that are in outer circle if the inner circle is free :) whenever an enemy dies/leaves etc.

    public IEnumerator Attack()
    {
        attacking = true;
        while (potentialc1Positions.Count < 4)
        {
            for (int i = 0; i < Innerpositions.Count; i++)
            {

                if (Innerpositions[i].enemy != null)
                {
                    Enemy enemy = Innerpositions[i].enemy.GetComponent<Enemy>();

                    //might change so its only if the enemies close to the target :) makes it harder
                    //  if (Vector3.Distance(enemy.dest.position, enemy.transform.position) <= 0.2f && Vector3.Distance(enemy.transform.position, enemy.target.position) <= 2)
                    if (Vector3.Distance(enemy.transform.position, enemy.target.position) <= 2)
                    {
                       // RaycastHit2D = 

                        if (Random.value * 100f < 75f)
                        {
                            enemy.Fire();
                            yield return new WaitUntil(() => enemy.anim.GetBool("attacking") == false);
                        }
                        else
                        {
                            enemy.Fire();
                            enemy.Fire();
                            yield return new WaitUntil(() => enemy.anim.GetBool("Combo") == false);
                        }
                    }
                 //   yield return new WaitUntil(() => enemy.anim.GetBool("attacking")==false);

                    List<enemyPositions> pos = new List<enemyPositions>();
                    foreach (enemyPositions posts in Innerpositions)
                    {
                        if(posts.usable && posts.enemy!=null)
                        pos.Add(posts);
                    }
                    if (pos.Count == 1)
                        time = 0;
                    else
                        time = 1;
                    yield return new WaitForSeconds(time);
                }
            }
            yield return null;
        }
        attacking = false;
      //  yield return new WaitUntil(() => potentialc1Positions.Count < 4);
        //StartCoroutine(Attack());


    }

    public void clearEnemies()
    {
        foreach(enemyPositions pos in Innerpositions)
        {
            if(pos.enemy!=null)
            {
                Enemy enemy = pos.enemy.GetComponent<Enemy>();
                enemy.posRef = null;
                pos.enemy = null;
                enemy.switching();
                enemy.anim.SetBool("Combo", false);
            }
        }
        foreach (enemyPositions pos in Outerpositions)
        {
            if (pos.enemy != null)
            {
                Enemy enemy = pos.enemy.GetComponent<Enemy>();
                enemy.posRef = null;
                pos.enemy = null;
                enemy.switching();
            }
        }
    }

}
#region PreWallsCheck
/*[System.Serializable]
public class enemyPositions
{
    public Transform transform;
    public GameObject enemy;

    public bool c1;
    public bool c2;

    public bool usable = true;
}


public class EnemyManager : MonoBehaviour {

    public int time;

    public LayerMask walls;

    public GameObject innerRing;

    public GameObject OuterRing;

    public List<enemyPositions> Innerpositions = new List<enemyPositions>();


    public List<enemyPositions> Outerpositions = new List<enemyPositions>();


    public List<enemyPositions> potentialc1Positions = new List<enemyPositions>();

    public List<enemyPositions> potentialc2Positions = new List<enemyPositions>();

    const int INNERMAXPOSITIONS = 4;


    const int OUTERMAXPOSITIONS = 12;

    public void Start()
    {
        for (int i = 0; i < INNERMAXPOSITIONS; i++)
        {
            enemyPositions newpos = new enemyPositions();
            newpos.transform = innerRing.transform.GetChild(i);
            newpos.c1 = true;

            Innerpositions.Add(newpos);
        }
        for (int i = 0; i < OUTERMAXPOSITIONS; i++)
        {
            enemyPositions newpos = new enemyPositions();
            newpos.transform = OuterRing.transform.GetChild(i);
            newpos.c2 = true;

            Outerpositions.Add(newpos);
        }

        foreach (enemyPositions pos in Innerpositions)
        {

            if (pos.enemy != null)
                Debug.Log("not empty");
            else
            {
                potentialc1Positions.Add(pos);
            }
        }

        foreach (enemyPositions pos in Outerpositions)
        {
            if (pos.enemy != null)
                Debug.Log("not empty");
            else
            {
                potentialc2Positions.Add(pos);
            }
        }
    }

    Transform ClosestCattempt(Enemy enemy, List<enemyPositions> thePositions)
    {
        //compare squared distance of threshold and the sqrMagnitude to the target.
        var closestDistance = 0f;

        //which place we want in the array
        var targetNumber = 0;
        //loop through entire player transform array
        for (int i = 0; i < thePositions.Count; i++)
        {
            if (thePositions[i].usable)
            {

                //compare squared distance of different index positions other than 0 (first)  and the sqrMagnitude to the target.

                var thisDistance = (thePositions[i].transform.position - enemy.transform.position).sqrMagnitude;

                if (closestDistance == 0)
                    closestDistance = thisDistance;

                //if first comparison is less than the other,  set the comparisons to equal each other 
                //and select what position in the array it is otherwise select the first players position

                if (thisDistance < closestDistance)
                {
                    closestDistance = thisDistance;

                    targetNumber = i;
                }
            }
        }
        thePositions[targetNumber].enemy = enemy.gameObject;
        enemy.posRef = thePositions[targetNumber];
        //return it to the serialized field.
        return thePositions[targetNumber].transform;
    }

//function to compare distance to those of the player transforms

Transform ClosestC1(Enemy enemy, List<enemyPositions> thePositions)
{
    //compare squared distance of threshold and the sqrMagnitude to the target.
    var closestDistance = (thePositions[0].transform.position - enemy.transform.position).sqrMagnitude;

    //which place we want in the array
    var targetNumber = 0;
    //loop through entire player transform array
    for (int i = 1; i < thePositions.Count; i++)
    {
        //compare squared distance of different index positions other than 0 (first)  and the sqrMagnitude to the target.

        var thisDistance = (thePositions[i].transform.position - enemy.transform.position).sqrMagnitude;

        //if first comparison is less than the other,  set the comparisons to equal each other 
        //and select what position in the array it is otherwise select the first players position

        if (thisDistance < closestDistance)
        {
            closestDistance = thisDistance;

            targetNumber = i;
        }
    }
    thePositions[targetNumber].enemy = enemy.gameObject;
    enemy.posRef = thePositions[targetNumber];
    //return it to the serialized field.
    return thePositions[targetNumber].transform;
}

public void checkSpot()
{
    foreach (enemyPositions pos in Innerpositions)
    {
        Ray ray = new Ray(transform.position, pos.transform.position - transform.position);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, pos.transform.position), walls);

        if (rayHit.collider != null)
            pos.usable = true;
        else
            pos.usable = false;
    }
}

public void addEnemyToARing(Enemy enemy)
{

    if (potentialc1Positions.Count > 0)
    {

        enemy.dest = ClosestC1(enemy, potentialc1Positions);
        enemy.target = transform;
        enemy.nav.destination = enemy.dest.position;



        foreach (enemyPositions pos in potentialc1Positions.ToList())
        {
            if (pos.enemy != null)
                potentialc1Positions.Remove(pos);
        }
        if (potentialc1Positions.Count == 3)
            StartCoroutine(Attack());
    }
    else if (potentialc2Positions.Count > 0)
    {
        enemy.dest = ClosestC1(enemy, potentialc2Positions);
        enemy.target = transform;
        enemy.nav.destination = enemy.dest.position;

        foreach (enemyPositions pos in potentialc2Positions.ToList())
        {
            if (pos.enemy != null)
                potentialc2Positions.Remove(pos);
        }
    }
    else
    {
        enemy.switching();
    }


}

public void leaveSpot(Enemy enemy)
{


    enemyPositions test = enemy.posRef;

    enemy.posRef = null;

    test.enemy = null;

    //add additional positions
    if (test.c1)
        foreach (enemyPositions pos in Innerpositions.ToList())
        {
            if (pos.enemy == null && !potentialc1Positions.Contains(pos))
                potentialc1Positions.Add(pos);
        }
    else
        foreach (enemyPositions pos in Outerpositions.ToList())
        {
            if (pos.enemy == null && !potentialc2Positions.Contains(pos))
                potentialc2Positions.Add(pos);
        }


    updateEnemies();
}

//   public GameObject temp;

public void updateEnemies()
{
    foreach (enemyPositions pos in Outerpositions)
    {
        if (potentialc1Positions.Count > 0)
        {
            if (pos.enemy != null)
            {
                GameObject temp = pos.enemy;
                leaveSpot(pos.enemy.GetComponent<Enemy>());
                addEnemyToARing(temp.GetComponent<Enemy>());
                //pos.enemy = null;
            }
        }
        else
            break;
    }
}

//need a function for removing the enemy that left?

//update enemies positions that are in outer circle if the inner circle is free :) whenever an enemy dies/leaves etc.

public IEnumerator Attack()
{
    while (potentialc1Positions.Count < 4)
    {
        for (int i = 0; i < Innerpositions.Count; i++)
        {
            if (Innerpositions[i].enemy != null)
            {
                Enemy enemy = Innerpositions[i].enemy.GetComponent<Enemy>();
                if (Vector3.Distance(enemy.dest.position, enemy.transform.position) <= 0.2f && Vector3.Distance(enemy.transform.position, enemy.target.position) <= 2)
                {
                    enemy.Fire();
                    enemy.Fire();
                }
                yield return new WaitUntil(() => enemy.anim.GetBool("Combo") == false);

                if (potentialc1Positions.Count == 3)
                    time = 0;
                else
                    time = 1;
                yield return new WaitForSeconds(time);
            }
        }
    }
}
}
*/
#endregion

#region brokenabit

/*sing System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class enemyPositions
{
public Transform transform;
public GameObject enemy;

public bool c1;
public bool c2;

public bool usable = true;
}


public class EnemyManager : MonoBehaviour {

public int time;

public bool attacking = false;

public LayerMask walls;

public GameObject innerRing;

public GameObject OuterRing;

public List<enemyPositions> Innerpositions = new List<enemyPositions>();


public List<enemyPositions> Outerpositions = new List<enemyPositions>();


public List<enemyPositions> potentialc1Positions = new List<enemyPositions>();

public List<enemyPositions> potentialc2Positions = new List<enemyPositions>();

const int INNERMAXPOSITIONS = 4;


const int OUTERMAXPOSITIONS = 6;

public void Start()
{
    for (int i = 0; i < INNERMAXPOSITIONS; i++)
    {
        enemyPositions newpos = new enemyPositions();
        newpos.transform = innerRing.transform.GetChild(i);
        newpos.c1 = true;

        Innerpositions.Add(newpos);
    }
    for (int i = 0; i < OUTERMAXPOSITIONS; i++)
    {
        enemyPositions newpos = new enemyPositions();
        newpos.transform = OuterRing.transform.GetChild(i);
        newpos.c2 = true;

        Outerpositions.Add(newpos);
    }

    foreach (enemyPositions pos in Innerpositions)
    {

        if (pos.enemy != null)
            Debug.Log("not empty");
        else
        {
            potentialc1Positions.Add(pos);
        }
    }

    foreach (enemyPositions pos in Outerpositions)
    {
        if (pos.enemy != null)
            Debug.Log("not empty");
        else
        {
            potentialc2Positions.Add(pos);
        }
    }
}

Transform ClosestC1(Enemy enemy, List<enemyPositions> thePositions)
{
    //compare squared distance of threshold and the sqrMagnitude to the target.
    var closestDistance = 0f;

    //which place we want in the array
    var targetNumber = 0;
    //loop through entire player transform array
    for (int i = 0; i < thePositions.Count; i++)
    {
        if (thePositions[i].usable)
        {

            //compare squared distance of different index positions other than 0 (first)  and the sqrMagnitude to the target.

            var thisDistance = (thePositions[i].transform.position - enemy.transform.position).sqrMagnitude;

            if (closestDistance == 0)
                closestDistance = thisDistance;

            //if first comparison is less than the other,  set the comparisons to equal each other 
            //and select what position in the array it is otherwise select the first players position

            if (thisDistance < closestDistance)
            {
                closestDistance = thisDistance;

                targetNumber = i;
            }
        }
    }
    if (thePositions[targetNumber].enemy == null && thePositions[targetNumber].usable)
    {
        thePositions[targetNumber].enemy = enemy.gameObject;
        enemy.posRef = thePositions[targetNumber];
        if (potentialc1Positions.Contains(enemy.posRef))
            potentialc1Positions.Remove(enemy.posRef);
        if (potentialc2Positions.Contains(enemy.posRef))
            potentialc2Positions.Remove(enemy.posRef);

        //return it to the serialized field.
        return thePositions[targetNumber].transform;
    }
    else
    {
        return null;
    }
}


public void checkSpot()
{
    foreach (enemyPositions pos in Innerpositions)
    {
        Ray ray = new Ray(transform.position, pos.transform.position - transform.position);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, pos.transform.position), walls);

        if (rayHit.collider != null)
        {
            pos.usable = false;
            potentialc1Positions.Remove(pos);

            if(pos.enemy!=null)
            {
                GameObject temp = pos.enemy;
                leaveSpot(pos.enemy.GetComponent<Enemy>());
                addEnemyToARing(temp.GetComponent<Enemy>());
                pos.enemy = null;

            }
            //   Debug.Log("ray hit");
        }
        else
        {
            pos.usable = true;

            if (pos.enemy == null && !potentialc1Positions.Contains(pos))
                potentialc1Positions.Add(pos);

          //  if(pos.enemy==null && )
        }
    }
    foreach (enemyPositions pos in Outerpositions)
    {
        Ray ray = new Ray(transform.position, pos.transform.position - transform.position);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(transform.position, pos.transform.position), walls);

        if (rayHit.collider != null)
        {
            pos.usable = false;
          //  Debug.Log("ray hit");
            potentialc2Positions.Remove(pos);

            if (pos.enemy != null)
            {
                GameObject temp = pos.enemy;
                leaveSpot(pos.enemy.GetComponent<Enemy>());
                addEnemyToARing(temp.GetComponent<Enemy>());
                pos.enemy = null;

            }
        }
        else
        {
            pos.usable = true;


            if (pos.enemy == null && !potentialc2Positions.Contains(pos))
                potentialc2Positions.Add(pos);

        }
    }

    if (potentialc1Positions.Count >0 && potentialc2Positions.Count < OUTERMAXPOSITIONS)
        foreach (enemyPositions pos in Outerpositions)
    {
        if (pos.enemy != null)
        {
            GameObject temp = pos.enemy;
            leaveSpot(pos.enemy.GetComponent<Enemy>());
            addEnemyToARing(temp.GetComponent<Enemy>());
            pos.enemy = null;

        }
        if (potentialc1Positions.Count < 1 && potentialc2Positions.Count >= OUTERMAXPOSITIONS)
            break;

    }
}

public void addEnemyToARing(Enemy enemy)
    {
    checkSpot();

    if (potentialc1Positions.Count > 0)
    {

        enemy.dest = ClosestC1(enemy, potentialc1Positions.ToList());
        if (enemy.dest != null)
        {

            enemy.target = transform;
            enemy.enemyState = EnemyState.Chasing;
            // enemy.nav.destination = enemy.dest.position;



            foreach (enemyPositions pos in potentialc1Positions.ToList())
            {
                if (pos.enemy != null || !pos.usable)
                    potentialc1Positions.Remove(pos);
            }
            if (!attacking)
                StartCoroutine(Attack());
        }
        else
        {
            enemy.switching();
        }
    }
    else if (potentialc2Positions.Count > 0)
    {
        enemy.dest = ClosestC1(enemy, potentialc2Positions.ToList());
        if (enemy.dest != null)
        {
            enemy.target = transform;
            enemy.enemyState = EnemyState.Chasing;
            //enemy.nav.destination = enemy.dest.position;

            foreach (enemyPositions pos in potentialc2Positions.ToList())
            {
                if (pos.enemy != null || !pos.usable)
                    potentialc2Positions.Remove(pos);
            }
        }
        else
        {
            enemy.switching();
        }
    }
    else
    {
        enemy.switching();
    }

    if (attacking == false)
        StartCoroutine(Attack());
}

public void leaveSpot(Enemy enemy)
{


    enemyPositions test = enemy.posRef;

    test.enemy = null;

    enemy.posRef = null;


    //add additional positions
    if(test.c1)
    foreach (enemyPositions pos in Innerpositions.ToList())
    {
        if (pos.enemy == null && !potentialc1Positions.Contains(pos))
            potentialc1Positions.Add(pos);
    }
    else
    foreach (enemyPositions pos in Outerpositions.ToList())
    {
        if (pos.enemy == null && !potentialc2Positions.Contains(pos))
            potentialc2Positions.Add(pos);
    }
  //  checkSpot();

    updateEnemies();
}

//  for moving enemies from outer to inner spot

public void updateEnemies()
{
    foreach(enemyPositions pos in Outerpositions)
    {
        if (potentialc1Positions.Count > 0)
        {
            if (pos.enemy != null)
            {
               GameObject temp = pos.enemy;
                leaveSpot(pos.enemy.GetComponent<Enemy>());
                addEnemyToARing(temp.GetComponent<Enemy>());
                //pos.enemy = null;
            }
        }
        else
            break;
    }
}

//need a function for removing the enemy that left?

//update enemies positions that are in outer circle if the inner circle is free :) whenever an enemy dies/leaves etc.

public IEnumerator Attack()
{
    attacking = true;
    while (potentialc1Positions.Count < 4)
    {
        for (int i = 0; i < Innerpositions.Count; i++)
        {
            if (Innerpositions[i].enemy != null)
            {
                Enemy enemy = Innerpositions[i].enemy.GetComponent<Enemy>();

                //might change so its only if the enemies close to the target :) makes it harder
                //  if (Vector3.Distance(enemy.dest.position, enemy.transform.position) <= 0.2f && Vector3.Distance(enemy.transform.position, enemy.target.position) <= 2)
                if (Vector3.Distance(enemy.transform.position, enemy.target.position) <= 2)
                {

                    if (Random.value * 100f < 75f)
                    {
                        enemy.Fire();
                        yield return new WaitUntil(() => enemy.anim.GetBool("attacking") == false);
                    }
                    else
                    {
                        enemy.Fire();
                        enemy.Fire();
                        yield return new WaitUntil(() => enemy.anim.GetBool("Combo") == false);
                    }
                }
             //   yield return new WaitUntil(() => enemy.anim.GetBool("attacking")==false);

                List<enemyPositions> pos = new List<enemyPositions>();
                foreach (enemyPositions posts in Innerpositions)
                {
                    if(posts.usable && posts.enemy!=null)
                    pos.Add(posts);
                }
                if (pos.Count == 1)
                    time = 0;
                else
                    time = 1;
                yield return new WaitForSeconds(time);
            }
        }
    }
    attacking = false;
}

public void clearEnemies()
{
    foreach(enemyPositions pos in Innerpositions)
    {
        if(pos.enemy!=null)
        {
            Enemy enemy = pos.enemy.GetComponent<Enemy>();
            enemy.posRef = null;
            pos.enemy = null;
            enemy.switching();
        }
    }
    foreach (enemyPositions pos in Outerpositions)
    {
        if (pos.enemy != null)
        {
            Enemy enemy = pos.enemy.GetComponent<Enemy>();
            enemy.posRef = null;
            pos.enemy = null;
            enemy.switching();
        }
    }
}

}
*/
#endregion

/* public void saveEm()
{
 // string jsonString = PlayerPrefs.GetString("TestSave" + saveIndex);
 //tester = JsonUtility.FromJson<dataContainer>(jsonString);

 if (tester == null)
     tester = new dataContainer();

 if (tester.dead == null)
     tester.dead = new List<string>();
 else
     tester.dead.Clear();

 if (tester.alive == null)
     tester.alive = new List<string>();
 else
     tester.alive.Clear();

 if (tester.dataHolder == null)
     tester.dataHolder = new List<data>();
 else
     tester.dataHolder.Clear();

 foreach (GameObject enemy in enemies)
 {
     if (enemy.activeInHierarchy)
     {
         tester.alive.Add(enemy.name);
         data storage = new data { x = enemy.transform.position };
         tester.dataHolder.Add(storage);
         storage.active = true;
     }
     else
         tester.dead.Add(enemy.name);

     string json = JsonUtility.ToJson(tester);
     PlayerPrefs.SetString("TestSave" + id.instance.saveIndex, json);
 }
 PlayerPrefs.Save();
}
*/
