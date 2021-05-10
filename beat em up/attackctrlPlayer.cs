using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackctrlPlayer : AttackController
{
    Player myp;

    public LayerMask enemyBox;

    Collider[] hitboxes;

    Enemy eref;

    public override void knockedOver(GameObject obj)
    {
        playa.floored = true;
        base.knockedOver(obj);

        //tell enemies in my player manager to leave me alone/keep their distance/etc for a bit?
        StartCoroutine(pissoff());

    }

    IEnumerator pissoff()
    {
        yield return null;
        myp.leaveMeAlone();
    }

    private void Start()
    {
        myp = (Player)playa;
    }

    public override void resetrb()
    {
        StartCoroutine(handleblockers());
    }


    

    IEnumerator handleblockers()
    {
        hitboxes = Physics.OverlapSphere(new Vector3(1.6f * (transform.GetChild(0).localScale.x / 2f) + transform.root.position.x, 1.32f, transform.position.z), 1.3f, enemyBox);

        //this is to make enemies that are blocking when i got hit to continue attacking/approach etc once they are done blocking


        for (int i = 0; i < hitboxes.Length; i++)
        {
            eref = hitboxes[i].transform.root.GetComponent<Enemy>();

            if (eref.blocking)
            {
                //tell em to side step or roll away?

                //make sure we've actually hit them?
                yield return new WaitUntil(() => !eref.blocked);
                if(eref.enemyposinmanager.movepositions.x == -1)
                {
                    //go wander? join if we can?
                }
                else
                {
                        if(eref.blocking)
                        {
                            eref.blocking = false;
                            eref.anim.SetBool("Block", false);

                            //go to continue approach, or decision tree?
                        }
                }
            }
        }
    }
}

/*if (enemyposinmanager.whichOne < 2)
            {
                if(transform.position.x>p.transform.position.x )
                {

                    tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

                    if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
                    {

                        approach();
                    }
                    else
                    {
                        removeFromSpot();
                    }
                }

            }
            else
            {
                if (transform.position.x < p.transform.position.x)
                {
                    tempEP = p.enemyM.returnClosestAvailablePositionOnOtherSide(this);

                    if (!checkifanenemyfromotherplayerisinourspot() && enemyposinmanager == tempEP)
                    {

                        approach();
                    }
                    else
                    {
                        removeFromSpot();
                    }
                }
            }


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

        spotcols = Physics.OverlapSphere(new Vector3(spot.x, 0.8f, spot.z), 0.115f, playerMask);

        if (spotcols.Length > 0)
        {
            for (int i = 0; i < spotcols.Length; i++)
            {
                aref = spotcols[i].transform.root.GetComponent<Player>();

                //added if floored check

                if (aref != p && !aref.floored)
                {
                    return true;
                }
            }
        }
            return false;
    }
*/