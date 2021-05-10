using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackctrlEnemy : AttackController
{
    public Enemy enemy;
    public override void knockedOver(GameObject obj)
    {
        enemy.state = enemyState.down;
        enemy.anim.SetBool("Block", false);
        enemy.leaveSpot();
        enemy.nav.enabled = false;
        base.knockedOver(obj);
        enemy.enableallobs();
        enemy.resetdynamic();
    }

    public override void blocks()
    {
        base.blocks();
        if (playa.rb.isKinematic)
            playa.rb.isKinematic = false;
        // playa.rb.AddForce(-((enemy.player.position - new Vector3(transform.position.x, enemy.player.position.y, transform.position.z) )
        //    .normalized * pushBackForce) / returnplayeratt(), ForceMode.Impulse);

        playa.rb.AddForce( new Vector2( (enemy.leftorRight() * pushBackForce / returnplayeratt()), 0), ForceMode.Impulse);


    }

    float returnplayeratt()
    {
        if(enemy.pAtt.es.myHE.attackPos == 5 || enemy.pAtt.es.myHE.attackPos == 1)
        {
            //player is kicking so we need a
            //lower value for a higher blowback force when blocking a hit :)
            return 2f;
        }
        return 3f;
    }

    public override void setStateGotHit()
    {
        base.setStateGotHit();
        enemy.returnregularboxsize();
        enemy.ResetBlock();
        if (enemy.nav.enabled)
        {
            enemy.nav.ResetPath();
            enemy.nav.velocity = Vector3.zero;
        }
        enemy.state = enemyState.flinching;
    }

    public override void resetrb()
    {
        if(enemy.grounded)
        enemy.rb.isKinematic = true;
    }
}
