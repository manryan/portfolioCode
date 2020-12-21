using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enableshieldenemy : EnableShield
{
    Enemy enemy;

    private void Start()
    {
        enemy = (Enemy)player;
    }

    public override void addJumpForce()
    {
        base.addJumpForce();
        player.rb.AddForce(enemy.vectTouse * 2f, ForceMode.Impulse);
    }


}
