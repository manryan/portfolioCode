using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public abstract class Entity : MonoBehaviour
{
    public Rigidbody rb;

    public Vector2 movement;

    public float moveSpeed;

    public float runSpeed;

    public float crouchSpeed;

    public float walkSpeed;

    public Animator anim;


    [HideInInspector]
    public bool rotating;

    [HideInInspector]
    public bool shouldroll;

    public bool facingRight = true;

    public SpriteRenderer sr;

    public Transform collisionParent;



    [HideInInspector]
    protected bool Boundary;

 
    public bool crouch;

    public AttackController attCtrl;

    [HideInInspector]
    public bool blocking;


    public int hitStack;


    public bool playerHit = false;


    public bool grounded = true;

    [HideInInspector]
    public bool canAttack;


    public bool rolling;

    public float rollForce;

   // public bool running;

      public BoxCollider[] bc;

    public ComboControllerRoot cc;

  
    public bool attacking = false;

 
    public bool allowed = true;

    [HideInInspector]
    public bool floored;

    public EnableShield es;

    public virtual void setBlock()
    {

        if (playerHit)
        {
            anim.SetBool("playerhit", false);
            playerHit = false;
        }

        if (blocking)
        {
            anim.SetBool("Block", true);
        }
    }


    public virtual void landGround()
    {
        //temp variable
        attacking = true;
    }



    public virtual void Flip()
    {
        //     bc.center = new Vector3(-bc.center.x, bc.center.y, bc.center.z);
        facingRight = !facingRight;
        collisionParent.localScale = new Vector3(collisionParent.transform.localScale.x * -1f, 1, 1);

        if (facingRight)
        {
            collisionParent.transform.position = new Vector3(transform.position.x + 0.18f, transform.position.y, transform.position.z);
        }
        else
        {
            collisionParent.transform.position = new Vector3(transform.position.x - 0.18f, transform.position.y, transform.position.z);
        }
        //   sr.flipX = !sr.flipX;
    }

    public void resetSpeed()
    {
        if (crouch)
            moveSpeed = crouchSpeed;
        else
            moveSpeed = walkSpeed;

        anim.SetFloat("speed", moveSpeed);
    }

    public void Block()
    {     
        if (!blocking)
        {

            if (anim.GetBool("Jump") == false && grounded && !attacking && !rolling && !playerHit)
            {
                es.shield.SetActive(true);
                rb.velocity = Vector2.zero;
                cc.resetStates();
            }
            anim.SetBool("Block", true);
            blocking = true;
        }
        else
        {
       
            attCtrl.shield.SetActive(false);
            anim.SetBool("Block", false);
            blocking = false;
        }
    }

    public List<NavMeshObstacle> obs;

    public void enablesecondBox()
    {
       if(crouch)
        {
            bc[0].enabled = false;
            bc[1].enabled = true;
            obs[0].gameObject.SetActive(false);
            obs[1].gameObject.SetActive(true);
        }
       else
        {
            bc[0].enabled = true;
            bc[1].enabled = false;
            obs[1].gameObject.SetActive(false);
            obs[0].gameObject.SetActive(true);
        }
    }

    public virtual void stopRoll()
    {
        rolling = false;
        anim.SetBool("Roll", false);
        rb.velocity = Vector3.zero;
   //     shouldroll = false;
        if (blocking && !playerHit)
            attCtrl.shield.SetActive(true);

    }


    public virtual void Crouch()
    {

            if (!crouch)
            {
                //    anim.SetBool("Crouch", true);
                crouch = true;
                anim.SetFloat("Input", 0);
                moveShield();
            if (grounded)
                resetSpeed();
                if (allowed && !playerHit)
                {
                anim.SetLayerWeight(1, 1);
                    anim.SetFloat("crouchfloat", 1);
                    enablesecondBox();
                    //    anim.SetBool("Crouch", true);
                }
            }
            else
            {
                crouch = false;
            // anim.SetBool("Crouch", false);
            moveShield();
            if(grounded)
                moveSpeed = walkSpeed;

                if (allowed && !playerHit)
                {
            anim.SetLayerWeight(1, 0);
                    anim.SetFloat("crouchfloat", 0);
                    enablesecondBox();
                }
            }



    }


    /*  public virtual void Crouch()
      {
          if (anim.GetLayerWeight(1) == 0)
          {
              if (!crouch)
              {
                  //    anim.SetBool("Crouch", true);
                  crouch = true;
                  anim.SetFloat("Input", 0);
                  moveShield();
                  anim.SetLayerWeight(1, 1);
                  resetSpeed();
                  if (allowed)
                  {
                      anim.SetFloat("crouchfloat", 1);
                      enablesecondBox();
                      //    anim.SetBool("Crouch", true);
                  }
              }
              else
              {
                  crouch = false;
                  // anim.SetBool("Crouch", false);
                  moveShield();
                  moveSpeed = walkSpeed;

                  if (allowed)
                  {
                      anim.SetFloat("crouchfloat", 0);
                      enablesecondBox();
                  }
              }
          }
          else
          {
              crouch = false;
              // anim.SetBool("Crouch", false);
              moveShield();
              anim.SetLayerWeight(1, 0);
              moveSpeed = walkSpeed;

              if (allowed)
              {
                  anim.SetFloat("crouchfloat", 0);
                  enablesecondBox();
              }
          }


      }*/

    /*  public void resetAttributes()
      {
          rolling = false;
          anim.SetBool("Roll", false);
          anim.SetBool("Jump", false);
      }*/

    public  void moveShield()
    {
        if (crouch)
            attCtrl.shield.transform.position = new Vector3(attCtrl.shield.transform.position.x, attCtrl.shield.transform.position.y - 1f, attCtrl.shield.transform.position.z);
        else
            attCtrl.shield.transform.position = new Vector3(attCtrl.shield.transform.position.x, attCtrl.shield.transform.position.y + 1f, attCtrl.shield.transform.position.z);
    }

    public virtual void Jump()
    {
        if (grounded && !playerHit && !rolling && anim.GetBool("Blocked") == false && !attacking)
        {
            // resetAnimatorLayersWhenJumping();
            // grounded = false;
            anim.SetBool("Jump", true);
            anim.SetLayerWeight(2, 1);
            if (blocking)
                attCtrl.shield.SetActive(false);

            attacking = false;
            anim.SetBool("Attacking", false);
            cc.actions.Clear();
            cc.resetStates();
            //  rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }



    bool canRoll()
    {
        if (grounded && !playerHit && !rolling)
            return true;
        else
            return false;
    }

    public virtual void Roll()
    {

        if (canRoll())
        {
           rolling = true;
            shouldroll = true;
            allowed = true;
            rb.velocity = Vector3.zero;
            cc.actions.Clear();
            cc.resetStates();
            anim.SetBool("Roll", true);

            attacking = false;
            anim.SetBool("Attacking", false);
            cc.actions.Clear();
            cc.resetStates();

            if (attCtrl.shield.activeInHierarchy)
                attCtrl.shield.SetActive(false);
        }

           
        
    }

}
