using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboController : ComboControllerRoot
{
  public  Player p;

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


    public void makeEnemiesDodge()
    {

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