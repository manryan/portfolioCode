using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboControllerRoot : MonoBehaviour
{
    public Entity player;

    public int kickState;

    public int punchState;

    public int kickIndex;

    public int punchIndex;

    public float attBlend;

    public float resetTime;

    public float time;

    public Queue<IEnumerator> actions = new Queue<IEnumerator>();

    public void lg()
    {
        player.landGround();
    }

    public void off()
    {
        player.attacking = false;
    }

    public virtual void stopWalk()
    {

    }

    public virtual void resumeWalk()
    {

    }

    public virtual void rotateplayer(int num)
    {
        if (num == 0)
        {
            if (!player.crouch)
            {
                if (!player.rolling)
                {
                    player.rotating = false;
                    player.anim.SetBool("rotate", false);
                    player.Flip();
                    player.attacking = false;
                }
            }
        }
        else
        {
            if (player.crouch)
            {
                if (!player.rolling)
                {
                    player.rotating = false;
                    player.anim.SetBool("rotate", false);
                    player.Flip();
                    player.attacking = false;
                }
            }
        }
    }

    public void handleAtt()
    {
        if (kickState == 0 && punchState == 0)
        {
            player.attacking = true;
            player.anim.SetBool("Attacking", true);
            setAttBlend();
            StartCoroutine(actions.Peek());
            actions.Dequeue();
            StartCoroutine(firstHit());
        }
        else if (player.attacking == false)
        {
            //    StopCoroutine(returnToIdle());
            StartCoroutine(firstHit());
            player.attacking = true;
            player.anim.SetBool("Attacking", true);
            setAttBlend();
            StartCoroutine(waitBeforeDequeue());
            //   StartCoroutine(actions.Peek());
            //   actions.Dequeue();
        }
    }

    public virtual void kick()
    {
        //need to update this for jump attacks

        if (checkIfWeCanAttack())
        {
            kickIndex++;
            enqueueState(increaseKickState());
            handleAtt();
            player.rb.velocity = Vector3.zero;
        }
        else if(!player.rotating && !player.rolling && !player.playerHit && punchIndex + kickIndex < 3 && player.anim.GetBool("Jump") == true)
        {
            //do jump attack
            kickIndex++;
            enqueueState(increaseKickState());
            handleAtt();
        }
    }

    public virtual void punch()
    {
        if (checkIfWeCanAttack())
        {
            punchIndex++;
            enqueueState(increasePunchState());
            handleAtt();
            player.rb.velocity = Vector3.zero;
        }
    }

    public bool canAttack()
    {
        return false;
    }


    public bool checkIfWeCanAttack()
    {
        if (!player.rotating && player.grounded && !player.rolling && !player.playerHit && punchIndex + kickIndex < 3 && player.anim.GetBool("Jump") == false)
        {
          //  player.rb.velocity = Vector3.zero;
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator waitBeforeDequeue()
    {
        yield return null;
        if (actions.Count > 0)
        {
            StartCoroutine(actions.Peek());
            actions.Dequeue();
        }
    }

    public IEnumerator firstHit()
    {

        yield return null; ;
        player.attCtrl.shield.SetActive(false);
        player.anim.SetBool("FirstHit", true);
  //      yield break;
        // return null;
        //   yield return null;
        //
        //     player.anim.SetBool("FirstHit", false);
    }




    public void callNextCoroutine()
    {
        if (actions.Count > 0)
        {
            StartCoroutine(actions.Peek());
            actions.Dequeue();
            player.attacking = true;
            player.anim.SetBool("Attacking", true);
        }
        else
        {
            player.anim.Play("Idle");
            StartCoroutine(returnToIdle());
        }
    }

    public IEnumerator returnToIdle()
    {
        player.anim.SetBool("Attacking", false);
        player.attacking = false;
        time = 1;
        while (time > 0f)
        {
            time -= Time.deltaTime;

            if (player.attacking || (punchState == 0 && kickState == 0))
                yield break;
            yield return null;
        }
        resetStates();
        actions.Clear();
    }


    public void endCombo()
    {
        resetStates();
        player.anim.SetBool("Attacking", false);
        player.attacking = false;
        if(player.grounded)
        player.anim.Play("Idle");
    }

    public IEnumerator increaseKickState()
    {
        player.anim.SetBool("attacktype", true);
        kickState++;
        player.anim.SetInteger("KickState", kickState);
        yield return null;
    }

    public IEnumerator increasePunchState()
    {
        player.anim.SetBool("attacktype", false);
        punchState++;
        player.anim.SetInteger("PunchState", punchState);
        yield return null;
    }

    public void enqueueState(IEnumerator aAction)
    {
        actions.Enqueue(aAction);
    }
    public void resetStates()
    {
        kickIndex = 0;
        punchIndex = 0;
        kickState = 0;
        punchState = 0;
        player.anim.SetInteger("KickState", 0);
        player.anim.SetInteger("PunchState", 0);
    }
    public virtual void setAttBlend()
    {
        attBlend = 0;
      /*  if (Input.GetAxis("Vertical") != 0)
            attBlend = Mathf.Sign(Input.GetAxis("Vertical"));
        player.anim.SetFloat("AttackBlend", attBlend);*/
    }

  
}
