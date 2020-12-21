using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackctrlEnemy : AttackController
{
    public Enemy enemy;
    public override void knockedOver(GameObject obj)
    {
        enemy.state = enemyState.down;
        enemy.leaveSpot();
        enemy.disableobs();
        enemy.nav.enabled = false;
        base.knockedOver(obj);
        enemy.enableallobs();
        enemy.resetdynamic();
    }



    public override void setStateGotHit()
    {
        base.setStateGotHit();
        enemy.ResetBlock();
        if (enemy.nav.enabled)
        {
            enemy.nav.ResetPath();
            enemy.nav.velocity = Vector3.zero;
        }
        enemy.state = enemyState.flinching;
    }
}
