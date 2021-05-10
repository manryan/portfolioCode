using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyAttackState { LightAttack = 1, HeavyAttack }

public class HitEntity : MonoBehaviour
{

    public Entity entity;

    public int attackPos;

    public List<GameObject> entitiesHit = new List<GameObject>();

    public int damage;

    public EnemyAttackState attackStatus;

    AttackController ctrl;

    public string EntityWeAttack = "";

    public List<GameObject> hitEffects;

     CapsuleCollider cc;


    [System.NonSerialized]
    public SphereCollider sc;

    bool shouldExpand = true;


    public virtual void Awake()
    {
        cc = transform.GetComponent<CapsuleCollider>();
        sc = transform.GetComponent<SphereCollider>();
    }

    public float damageCalculator()
    {
        if(entity.crouch)
        {
            if (attackPos > 2)
                return 2f;
            else
                return 1f;
        }
        else
        {
            switch(attackPos)
            {
                case 1:
                    return returnPkValue();
                    case 2:
                        if(entity.cc.attBlend ==1) //upwards
                        {
                        if (returnPkValue() == 2)
                            return 2.5f;
                        else
                            return 1.5f;
                        }
                        else
                        {
                        return returnPkValue();
                        }
                case 3:
                    return 1f;
                case 4:
                    return 2f;
                case 5://jump kick so deal a bit more
                    return 2.5f;
                default: //means they are kicking so no need?
                    return 2f;
            }
        }


    }

    float returnPkValue()
    {
        if (checkIfPunchingOrKicking())
        {
            return 2f;
        }
        else
        {
            return 1f;
        }
    }

    bool checkIfPunchingOrKicking()
    {
        if (entity.anim.GetBool("attacktype") == true)
        {
            //kicking
            return true;
        }
        else
            return false;
    }

    public GameObject hiteffectPrefab;

    public GameObject hitEffect(List<GameObject> objs,GameObject objtocreate)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            if(!objs[i].activeInHierarchy)
            {
                objs[i].SetActive(true);
                return objs[i];
            }
        }
        if (shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(objtocreate);
            obj.GetComponent<HitEffectDisable>().parent = transform.parent.parent;
            //  obj.SetActive(false);
            objs.Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

    public Vector3 returnHitEffectPosition()
    {
     /*   if(entity.crouch)
        {
            if(attackPos>2)
            {
                return new Vector3(cc.center.x + (entity.collisionParent.localScale.x * 0.1f), cc.center.y);
            }
            else
            {
                return new Vector3(sc.center.x, sc.center.y);
            }

        }
        else
        {
            return new Vector3(sc.center.x, sc.center.y);
        }
        */
                        return new Vector3(sc.center.x, sc.center.y);

    }

    [HideInInspector]
    public bool empty;

    public virtual void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == EntityWeAttack)
        {
            //check if we already havent hit them

            //for enemies if we land a hit and are mid air, maybe stop velocity in x/z? 

            landed(c);

        }

    }

    public virtual void landed(Collider c)
    {
        checkifhit(c);
    }

    void checkifhit(Collider c)
    {

        if (!entitiesHit.Contains(c.transform.root.gameObject))
        {

            //add them to the list
            entitiesHit.Add(c.transform.root.gameObject);

            //check sibling index to see if we hit head, body or legs

            if (c.transform.GetSiblingIndex() == 0)
            {
                //        Debug.Log("Hit head");
                damage = 3;
            }
            else if (c.transform.GetSiblingIndex() <= 2)
            {
                //      Debug.Log("Hit Body");
                damage = 2;
            }
            else
            {
                //    Debug.Log("hit legs");
                damage = 1;
            }

            ctrl = c.transform.root.GetComponent<AttackController>();
            ctrl.enemyRef = this;
            ctrl.HandleBlocks((attacktype() + damage + damageCalculator())*10f);
            empty = false;
            return;

        }
        empty = true;
    }

    float attacktype()
    {
        switch(attackStatus)
        {
           case EnemyAttackState.HeavyAttack:
            return 1f;
            default:
                return 0.5f;
        }
    }


    public virtual void OnDisable()
    {
      //  if(entitiesHit.Count<1)
     //   entity.missedAttack();
     
        //clear entities after
        entitiesHit.Clear();
    }
}
