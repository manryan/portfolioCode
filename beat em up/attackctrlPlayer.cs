using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackctrlPlayer : AttackController
{
    Player myp;
    public override void knockedOver(GameObject obj)
    {
        playa.floored = true;
        base.knockedOver(obj);

        //tell enemies in my player manager to leave me alone/keep their distance/etc for a bit?
        myp.leaveMeAlone();
        
    }

    private void Start()
    {
        myp = (Player)playa;
    }

}
