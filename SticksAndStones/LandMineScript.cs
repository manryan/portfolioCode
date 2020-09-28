using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineScript : MonoBehaviour
{

    public Animator anim;

    Collider2D[] cols;

    public LayerMask entitylayer;

    public Entity entityToDamage;

    public Rigidbody2D rb;

    public Entity root;

    Transform myparent;

    BoxCollider2D bc;

    public bool exploded;

    private void Awake()
    {
        myparent = transform.parent;
        bc = GetComponent<BoxCollider2D>();

       // entitylayer += 11;
    }

    bool checkIfTheresSomeoneOtherThanOurEntity()
    {
        for (int i = 0; i < cols.Length; i++)
        {
                entityToDamage = cols[i].GetComponent<Entity>();

            if(entityToDamage!=null)
                if (entityToDamage != root && entityToDamage.team != root.team)
                    return true;
            
        }

        return false;
    }

    RaycastHit2D groundcheck;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.tag == "Ground")
        {
          groundcheck=  Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y-0.05f), -Vector3.up,1f, root.mm.platforms);

            if (groundcheck.collider != null)
            {
                bc.isTrigger = true;

                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector3.zero;

                myparent.gameObject.SetActive(false);
                root.anim.SetBool("grenade", false);

                cols = Physics2D.OverlapCircleAll(transform.position, 0.13f, entitylayer);

                if (checkIfTheresSomeoneOtherThanOurEntity())
                {
                    Explode();
                }
                root.mm.StartCoroutine(root.mm.waitBeforePassingUpTurn());
            }
            //    Invoke("disablerootarms", 1f);
        }


    }

   void OnTriggerEnter2D(Collider2D collision)
    {
                    if (collision.gameObject.tag== "entitycol" || collision.gameObject.tag =="Grenade")
        {
            if(collision.transform.root.GetComponent<Entity>().team!=root.team && rb.bodyType==RigidbodyType2D.Kinematic)
            {
                Explode();
            }
        }

            if(collision.gameObject.tag=="killzone")
        {
            disableMe();
        }
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;

            bc.enabled = false;
            

            SoundManager.call.PlaySingleSound(root.mm.explosionSfx, 0.1f);

            cols = Physics2D.OverlapCircleAll(transform.position, 1.5f, entitylayer);

            anim.SetBool("explode", true);

            for (int i = 0; i < cols.Length; i++)
            {
                //apply force to each one and make them go into hurt animation state?

                entityToDamage = cols[i].transform.GetComponent<Entity>();
                if (entityToDamage != null)
                {
                    if (!entityToDamage.checkIfSafe(transform))
                    {

                        if (entityToDamage.mm.activeEntity == entityToDamage)
                        {
                            root.mm.goToMenu(6);
                        }


                        entityToDamage.canWalk = false;
                        entityToDamage.rb.AddForce((cols[i].transform.position - transform.position) * 5f * (1f / Vector3.Distance(transform.position, cols[i].transform.position)), ForceMode2D.Impulse);
                        entityToDamage.modifyHealth((-1f / Vector3.Distance(transform.position, cols[i].transform.position)) * 30f);
                        entityToDamage.blowUp();
                        if (entityToDamage.mm.activeEntity == entityToDamage)
                        {
                            root.mm.StartCoroutine(root.mm.waitBeforePassingUpTurn());
                            root.mm.stopFollowing = true;
                            entityToDamage.mc.enabled = false;
                            entityToDamage.moveInput = 0;
                            entityToDamage.anim.SetFloat("Input", 0);
                        }
                    }
                }
                else
                {
                    cols[i].transform.GetComponent<LandMineScript>().Explode();
                }
            }
        }
    }


    public void disableMe()
    {
        gameObject.SetActive(false);
      
    }
}
