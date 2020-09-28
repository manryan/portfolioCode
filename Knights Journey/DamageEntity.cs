using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;

public class DamageEntity : MonoBehaviour
{

    public string targettag;

    public Warrior troop;

    public EffectsPool pool;

    public bool isPlayer;

    public LayerMask wall;

     GameObject pfx;

    public void OnTriggerExit2D(Collider2D c)
    {
        if (c.GetComponent<VIDE_Assign>() != null && isPlayer)
        {
            troop.diagUI.StopCoroutine(troop.diagUI.TextAnimator);
            troop.diagUI.EndDialogue(VD.nodeData);
            troop.diagUI.enabled = false;
            troop.inTrigger = null;
        }
        if (c.gameObject.tag == targettag)
        {
            if(hit && !isPlayer)
            Invoke("hitoff",.1f);
        }
    }

    public void hitoff()
    {
        hit = false;
    }

    public void Flip(Transform obj)
    {
        if (transform.position.x > obj.position.x)
        {
            obj.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            obj.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D c)
    {
        if (c.GetComponent<VIDE_Assign>() != null && isPlayer)
        {
            Debug.Log("okay??");
            troop.inTrigger = c.GetComponent<VIDE_Assign>();
            Flip(c.transform);
            troop.diagUI.enabled = true;
            troop.TryInteract();
        }

        if (c.gameObject.tag == targettag)
        {
            Ray ray = new Ray(transform.position, c.transform.parent.position - transform.position);
            RaycastHit2D rayhit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(c.transform.parent.position, transform.position), wall);
            if (rayhit.collider == null)
            {
                if (!isPlayer)
                {
                    if (!hit)
                    {
                        hit = true;
                        Debug.Log("hit " + transform.parent.name);
                      //  var dir = c.gameObject.transform.parent.GetChild(0).up;
                  //      var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                        pfx = pool.GetFx();
                        pfx.SetActive(true);
                        pfx.transform.position = transform.position;
                        pfx.transform.up = c.gameObject.transform.parent.position - transform.parent.position;
                        //     pfx.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        //Instantiate(pfx, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

                        troop.DamageEntity(troop.damage);
                        if (isPlayer && troop.dialogue.activeInHierarchy)
                            troop.diagUI.EndDialogue(VD.nodeData);
                    }
                }

                else
                {
                    Debug.Log("hit " + transform.parent.name);
                    var dir = c.gameObject.transform.parent.GetChild(0).up;
                    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    pfx = pool.GetFx();
                    pfx.SetActive(true);
                    pfx.transform.position = transform.position;
                    pfx.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    //Instantiate(pfx, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

                    troop.DamageEntity(troop.damage);
                    if (isPlayer && troop.dialogue.activeInHierarchy)
                        troop.diagUI.EndDialogue(VD.nodeData);
                }
            }
        }
    }

    public bool hit;
}
