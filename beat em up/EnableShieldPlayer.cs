using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableShieldPlayer : EnableShield
{
    public Collider[] hitboxes;

    public LayerMask enemyBox;

    Player pref;


     void Start()
    {
        pref = (Player)player;
    }

    public override void cantcrouchorflip(int num)
    {
        myHE.attackPos = num;
        player.allowed = false;
        overlapeEnemies();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3( 1.6f*(transform.parent.localScale.x/2f )+ transform.root.position.x, 1.32f, transform.position.z), 1.3f);
    }

    float diffx;

    void overlapeEnemies()
    {
        hitboxes = Physics.OverlapSphere(new Vector3(1.6f *(transform.parent.localScale.x / 2f) + transform.root.position.x, 1.32f, transform.position.z), 1.3f, enemyBox);

        for (int i = 0; i < hitboxes.Length; i++)
        {
            //  Debug.Log("over");

            diffx = Mathf.Abs(hitboxes[i].transform.root.transform.position.x - transform.position.x);

            if (player.facingRight)
            {
                if (hitboxes[i].transform.root.transform.position.x > transform.position.x)
                    hitboxes[i].transform.root.GetComponent<Enemy>().handleDodges(pref);
                else if(diffx<0.4f)
                {
                    hitboxes[i].transform.root.GetComponent<Enemy>().handleDodges(pref);
                }
            }
            else
            {

                if (hitboxes[i].transform.root.transform.position.x < transform.position.x)
                    hitboxes[i].transform.root.GetComponent<Enemy>().handleDodges(pref);
                else if (diffx < 0.4f)
                {
                    hitboxes[i].transform.root.GetComponent<Enemy>().handleDodges(pref);
                }
            }

        }



        //check for enemies outside the box that may get hit (directly parallel to us? (positive z)
    }
}
