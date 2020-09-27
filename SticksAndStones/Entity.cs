using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum teamNumber { green = 1, red = 2, white = 3, blue };
public abstract class Entity : MonoBehaviour
{
    public Animator anim;

    public Rigidbody2D rb;

    public SpriteRenderer sr;
    
    [System.NonSerialized]
    public bool facingRight = true;

    public bool grounded;

    protected bool jumping;

    public Transform reflectionChild;


    public attributes attribute;

    public bool canWalk = false;

    [System.NonSerialized]
    public float moveInput;

    public MatchManager mm;

    public Vector2 startPos;

    public MovementControl mc;

    public TextMeshPro tmp;

    public Transform healthBar;

    public GameObject grenadeholder;

    public GameObject gunholder;

    public GameObject LandMine;

    public bool aiming = false;

    public bool placeMine;

    public bool shooting;

    public bool kicking;

    public teamNumber team;

    public bool blownup;

    public BoxCollider2D[] cols;

   public void blowUp()
    {
        if (!blownup)
        {
          blownup = true;
           anim.SetBool("Blownup", true);
            cols[0].enabled = false;
            cols[1].enabled = true;
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;

        if(facingRight)
        {
            reflectionChild.localScale = new Vector2(1, 1);
          //  sr.flipX = false;
        }
        else
        {
            reflectionChild.localScale = new Vector2(-1, 1);
            //  sr.flipX = true;
        }
    }

    public void endKick()
    {
        anim.SetBool("kick", false);
        anim.SetBool("kickstance", false);
    }

    public CapsuleCollider2D kickcol;



    private void Awake()
    {
        attribute.health = 100;
        attribute.maxHealth = 100;
        Physics2D.IgnoreLayerCollision(8, 8);
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(8, 11);        Physics2D.IgnoreLayerCollision(8, 14);
    }

    public void assignName()
    {
        tmp.text = attribute.name;
    }

  public  GameObject deathsign;


    float p;


    public virtual void modifyHealth(float Damage)
    {

        //negative to reduce health, positive to gain health
        attribute.health += Damage;
      p = attribute.health / attribute.maxHealth;
      //  float userHpBarLength = p * 0.2f;
       healthBar.localScale= new Vector2( p,1);


        mm.tm.adjustTeamHealthBar((int)team -1);

        if (attribute.health <= 0)
        {
            // kill us
            if(grounded)
            StartCoroutine(waitUntilWeStop());

            mm.deadentities.Add(this);

            if (this == mm.activeEntity)
            {
                mm.goToMenu(6);
                mc.enabled = false;
                moveInput = 0;
                anim.SetFloat("Input", 0);
            }

            healthBar.localScale = new Vector2(0, 1);
        }
        else
          mm.hitentities.Add(this);
        if (attribute.health >= 100)
        {
            attribute.health = attribute.maxHealth;
            healthBar.localScale = new Vector2(1, 1);
        }

    }

   public IEnumerator waitUntilWeStop()
    {
        yield return new WaitUntil(() => rb.velocity==Vector2.zero);

        deathsign = mm.op.GetDeathSign();
        deathsign.gameObject.SetActive(true);
        if (transform.position.y <= 4)
            deathsign.transform.position = transform.position + new Vector3(0, 1, 0);
        else
        {
            deathsign.transform.position = transform.position;
        }
        deathsign.GetComponent<DeathAnimation>().floatUp(this);
        if (inactive)
            gameObject.SetActive(false);
    }

    public void instantDeath()
    {
        if (mm.activeEntity == this)
            mm.goToMenu(6);


        deathsign = mm.op.GetDeathSign();
        deathsign.gameObject.SetActive(true);
        if (transform.position.y <= 4)
            deathsign.transform.position = transform.position + new Vector3(0, 1, 0);
        else
        {
            deathsign.transform.position = transform.position;
        }
        deathsign.GetComponent<DeathAnimation>().floatUp(this);
        if (inactive)
            gameObject.SetActive(false);
    }


    public bool inactive;


    public void JumpDone()
    {
        anim.SetBool("Jump", false);
    }

    public void addJumpForce()
    {
        grounded = false;

        jumping = false;


        rb.AddForce(new Vector2(0, 4f), ForceMode2D.Impulse);
        handleTravelBar();
    }

    public void addSideForce()
    {
        //if(moveInput==0)

       if (moveInput == 0 && Vector2.Distance(startPos, transform.position) * 30f<200f)
        {
            if (facingRight)
            {
                rb.AddForce(new Vector2(3f, 0), ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(-3f, 0), ForceMode2D.Impulse);
            }
            handleTravelBar();
        }
    }

    public void jump()
    {
        if (!jumping && grounded)
        {
            SoundManager.call.PlaySingleSound(mm.jump);

            jumping = true;
            anim.SetBool("Jump", true);
            rb.velocity = Vector2.zero;
        }
    }

    public void faceRightWay()
    {
        if (moveInput > 0 && !facingRight)
        {

            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {

            Flip();
        }
    }

    bool twoxdamage;

    public float doubleDamage()
    {
        if (twoxdamage)
        {
            twoxdamage = false;
            return 2f;
        }
        else
            return 1f;
    }


    public bool handleTravelBar()
    {
        mm.travelBar.sizeDelta = new Vector2(Vector2.Distance(startPos, transform.position) * 30f, 24);

        if (mm.travelBar.sizeDelta.x >= 20000f)
        {
            //go to next menu
            canWalk = false;

            if(grounded)
            rb.velocity = Vector2.zero;
            anim.SetFloat("Input", 0);

            mm.goToMenu(2);

            return true;
        }
        return false;
    }

    RaycastHit2D wallcheck;


    public bool checkIfSafe(Transform t)
    {
        wallcheck = Physics2D.Raycast(t.position, transform.position-t.position, Vector2.Distance(t.position, transform.position), mm.platforms);
        if (wallcheck.collider == null)
            return false;


        for (int i = 0; i < 3; i++)
        {
           wallcheck= Physics2D.Raycast(t.position, new Vector3(transform.position.x, transform.position.y + mm.hitpositions[i]) - t.position, Vector2.Distance(t.position, new Vector2(transform.position.x, transform.position.y + mm.hitpositions[i])), mm.platforms);

          

            if (wallcheck.collider == null)
                return false;
            
        }
        Debug.Log("true");

        return true;
    }

}
[System.Serializable]
public struct attributes
{
    public string name;

    public float health;

    public float maxHealth;
}