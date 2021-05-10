using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboController : ComboControllerRoot
{
  public  Player p;

    public LayerMask enemyBox;

  Collider[] hitboxes;

    Enemy eref;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
                punch();
            
        }
        if (Input.GetKeyDown(KeyCode.K))
        {

                kick();
            
        }
    }
    public override void Awake()
    {
        Debug.Log("???");
        p = transform.root.GetComponent<Player>();
        base.Awake();
    }

    public override void endCombo()
    {
        base.endCombo();

        //tell any enemies we hit that are blocking to do something
        StartCoroutine(enemiesblocking());
    }

    public override void didnthitenemies()
    {
        base.didnthitenemies();
        StartCoroutine(enemiesblocking());
    }

    IEnumerator enemiesblocking()
    {

        hitboxes = Physics.OverlapSphere(new Vector3(1.6f * (transform.parent.localScale.x / 2f) + transform.root.position.x, 1.32f, transform.position.z), 1.3f, enemyBox);

        for (int i = 0; i < hitboxes.Length; i++)
        {
            eref = hitboxes[i].transform.root.GetComponent<Enemy>();

            if (entitiesHit.Contains(eref.gameObject))
            {
                if (eref.blocking && !eref.playerHit && !eref.floored && eref.health>0)
                {
                    //tell em to side step or roll away?

                    //make sure we've actually hit them?

                    //   yield return new WaitUntil(() => !eref.blocked);
                        yield return new WaitUntil(() => !eref.blocked);

                    if (!player.attacking)
                    {
                        if((Mathf.Abs(transform.root.position.x - eref.transform.position.x) >1.7f && p.crouch )|| Mathf.Abs(transform.root.position.x - eref.transform.position.x) > 1.25f && !p.crouch)
                        {
                            if (!eref.nav.enabled)
                            {
                                eref.disableobs();
                                yield return null;
                                eref.nav.enabled = true;
                            }
                            eref.blocking = false;
                            eref.anim.SetBool("Block", false);
                            //for the time being we only did return to approach, but should actually possible go to decision tree?
                            //***********************
                            eref.ConfirmedApproach();
                            eref.StartCoroutine(eref.tryToDoLiveMovement());
                        }
                                else
                        eref.Roll();
                    }
                    
                }
            }
            else
            {
                if (eref.blocking)
                {
                    if (eref.blocked)
                    {
                        yield return new WaitUntil(() => !eref.blocked);
                    }

                    //continue approach/break outta blocking state lol, maybe go to decision tree?
                    if (actions.Count == 0)
                    {
                 

                            if (!eref.nav.enabled)
                        {
                            eref.disableobs();
                            yield return null;
                            eref.nav.enabled = true;
                        }
                        eref.blocking = false;
                        eref.anim.SetBool("Block", false);
                        //for the time being we only did return to approach, but should actually possible go to decision tree?
                        //***********************
                        eref.ConfirmedApproach();
                        eref.StartCoroutine(eref.tryToDoLiveMovement());
                    }
                    else
                    {
                        //side step etc
                        if (!eref.nav.enabled)
                        {
                            eref.disableobs();
                            yield return null;
                            eref.nav.enabled = true;
                        }
                        eref.blocking = false;
                        eref.anim.SetBool("Block", false);

                        eref.stepaside();
                    }
                }
            }
        }
    }

    public override void stopWalk()
    {
        p.canWalk = false;
        p.rb.velocity = Vector3.zero;
    }

    public override void resumeWalk()
    {
        p.canWalk = true;
    }

    public override void setAttBlend()
    {
        base.setAttBlend();
        if (Input.GetAxis(p.inputy) != 0)
            attBlend = Mathf.Sign(Input.GetAxis(p.inputy));
        player.anim.SetFloat("AttackBlend", attBlend);
    }
}