using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComboCtrl : ComboControllerRoot
{
  public  Enemy enemy;

    public override void setAttBlend()
    {
        base.setAttBlend();
        attBlend = enemy.attBlend;
        player.anim.SetFloat("AttackBlend", attBlend);
    }

    public override void rotateplayer(int num)
    {
        base.rotateplayer(num);
        enemy.rotating = false;
       // if(player.anim.GetBool("rotate")==false)
       //     if(enemy.nav.enabled)
        //enemy.nav.isStopped = false;
    }

    public override void stopWalk()
    {
       
        if (enemy.nav.enabled)
        {
            enemy.nav.isStopped = true;
            enemy.nav.velocity = Vector3.zero;
        }
    }

    public override void resumeWalk()
    {
     
        if (enemy.nav.enabled)
        {
            enemy.nav.isStopped = false;
        }
    }

}
