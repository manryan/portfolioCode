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

    public override void cantcrouchorflip(int num)
    {
        myHE.attackPos = num;
        player.allowed = false;
    }


    public override void addJumpForce()
    {
        base.addJumpForce();
        player.rb.AddForce(enemy.efunct.vectTouse * 2.25f, ForceMode.Impulse);
    }

    public void jumpuppercut()
    {
        rb.isKinematic = false;
        rb.AddForce(transform.root.up * jumpForce, ForceMode.Impulse);
    }

}
