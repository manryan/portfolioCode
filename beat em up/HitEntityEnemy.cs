using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEntityEnemy : HitEntity
{
    Enemy en;

    public override void OnDisable()
    {
        if(entitiesHit.Count<1)
        {
            //tell enemy to make a decision
        }
        else
        {
            //prepare for combo, back away etc depending on how many are on the player
        }

        base.OnDisable();
    }

    public override void Awake()
    {
        base.Awake();
        en = (Enemy)entity;
    }
}
