using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePhysics : MonoBehaviour
{
    public Rigidbody2D rb;

    int bouncesNumber;

    public Vector2 startpos;

    public float power;

  public  bool firstbounce =true;

    public PhysicsMaterial2D bouncygrenade;

    float time;

    CircleCollider2D cc;

    public Animator anim;

    Entity en;

    private void Awake()
    {
        cc = transform.GetComponent<CircleCollider2D>();
        en = transform.root.GetComponent<Entity>();
    }

    public void startOff(float force)
    {
        rb.constraints = RigidbodyConstraints2D.None;
        time = 0;
        startpos = transform.position;
        power = force;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            time += Time.deltaTime;
            if(time>1f)
            {
                explode();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {



            bouncesNumber++;
            if (bouncesNumber > 2)
            {

                explode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       

        if(collision.transform.tag == "Ground")
        {


                if (firstbounce)
                {
                    firstbounce = false;
                    bouncesNumber = 0;
                    if (Vector2.Distance(startpos,transform.position)>2f)
                    {
                        rb.drag = 0.2f;
                    bouncygrenade.bounciness = 0.8f;
                    /*   if (power > 30f)
                           bouncygrenade.bounciness = 0.8f;
                       else
                           bouncygrenade.bounciness = 0.4f;*/
                    cc.enabled = false;
                        cc.enabled = true;
                    }
                    else
                    {

                        //    rb.drag = 0.5f;
                        //    bouncygrenade.bounciness = 0.4f;
                        if (power > 30f)
                            rb.drag = 0.2f;
                        else
                            rb.drag = 1f;

                    //  bouncygrenade.bounciness = power / 120f; 
                        bouncygrenade.bounciness = Mathf.Clamp( power / 120f, 0.4f,0.8f);
                //    bouncygrenade.bounciness = 0.4f;
                    //bouncygrenade.bounciness = 0.4f;
                        cc.enabled = false;
                        cc.enabled = true;


                    }
                }

            

        }
        if(collision.transform.tag=="killzone")
        {
            disableme();
        }
    }

    Collider2D[] cols;

    public LayerMask entitylayer;

    public void explode()
    {

        //disable grenade and return it to the original spot





        setValues();
        anim.SetBool("explode", true);
    //    gameObject.SetActive(false);
    }

    void setValues()
    {
        firstbounce = true;
        rb.velocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.drag = 0.1f;
        bouncygrenade.bounciness = 0.8f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.localEulerAngles = Vector3.zero;
    }

    public void disableme()
    {
        setValues();
        gameObject.SetActive(false);
    }

    Entity entityToDamage;

    public void handleExplosion()
    {
        SoundManager.call.PlaySingleSound(en.mm.explosionSfx,0.1f);

        cols = Physics2D.OverlapCircleAll(transform.position, 1.5f, entitylayer);

        for (int i = 0; i < cols.Length; i++)
        {
            //apply force to each one and make them go into hurt animation state?

            entityToDamage = cols[i].transform.GetComponent<Entity>();
            if (entityToDamage != null)
            {
                if (entityToDamage.checkIfSafe(transform)==false)
                {
                    entityToDamage.rb.AddForce((cols[i].transform.position - transform.position) * 5f * (1f / Vector3.Distance(transform.position, cols[i].transform.position)), ForceMode2D.Impulse);
                    entityToDamage.modifyHealth((-1f / Vector3.Distance(transform.position, cols[i].transform.position)) * 30f);

                    entityToDamage.blowUp();
                }

            }
            else
            {
                if(!checkIfLandmineIsSafe(cols[i].transform.root))
                cols[i].transform.root.GetComponent<LandMineScript>().Explode();

            }
        }
    }

    RaycastHit2D wallcheck;

    Vector2 temp;

    bool checkIfLandmineIsSafe(Transform t)
    {

        wallcheck = Physics2D.Raycast(transform.position, t.position, Vector3.Distance(t.position, transform.position), en.mm.platforms);

        if (wallcheck.collider == null)
        {

            temp = new Vector2(transform.position.x, t.position.y);
            wallcheck = Physics2D.Raycast(temp, t.position, Vector3.Distance(temp, t.position), en.mm.platforms);


            if (wallcheck.collider == null)
            {
                return false;
            }



        }
        else
            return false;

        return true;
    }
}
