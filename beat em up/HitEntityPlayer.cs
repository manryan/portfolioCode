using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEntityPlayer : HitEntity
{
    SodaMachine sm;

    public  List<GameObject> objecthiteffects;

    public GameObject objhiteffect;

    GameObject oeffect;

    public override void OnTriggerEnter(Collider c)
    {
        base.OnTriggerEnter(c);
        if (c.gameObject.tag == "sodaMachine")
        {
            if (!entitiesHit.Contains(c.transform.gameObject))
            {
                Debug.Log("soda");
                entitiesHit.Add(c.gameObject);
                oeffect = hitEffect(objecthiteffects, objhiteffect);
                if(entity.facingRight)
                oeffect.transform.position = transform.position + new Vector3(sc.center.x, sc.center.y, 0);
                else
                {
                    oeffect.transform.position = transform.position - new Vector3(sc.center.x, sc.center.y, 0);
                }
                oeffect.transform.parent = null;
                sm = c.transform.root.GetComponent<SodaMachine>();
                sm.setTexture();
            }

            //handle smh

        }
    }
}
