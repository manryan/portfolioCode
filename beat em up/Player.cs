using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public EnemyManager enemyM;

    public string inputx;

    public string inputy;

    BoxCollider colBox;

    public BoxCollider otherplayercol;


    float x;
    float z;

    private void Start()
    {

        GameManager.instance.players.Add(this);
        enemyM.p = this;
       // colBox = transform.GetChild(0).GetChild(9).GetComponent<BoxCollider>();

        //    GetComponent<BoxCollider>().contactOffset = 0.01f;

    //    Physics.IgnoreCollision(colBox, otherplayercol);
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Boundary")
            Boundary = true;
    }



    public bool checkIfCanAttack()
    {
        if(!playerHit)
        {
            return true;
        }
        return false;
    }



    private void OnCollisionExit(Collision c)
    {
        if (c.gameObject.tag == "Boundary")
            Boundary = false;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    public override void Crouch()
    {
        base.Crouch();
        if(crouch)
        {
            anim.SetBool("Crouch", true);
        }
        else
        {
            anim.SetBool("Crouch", false);
        }
    }

    public override void Jump()
    {
        base.Jump();
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if(Input.GetKeyDown(KeyCode.B))
        {

            Block();
        }
        if(Input.GetKeyUp(KeyCode.B))
        {
            Block();
        }
        if (Input.GetKeyDown(KeyCode.R))
            Roll();

      /*  if (Input.GetKeyUp(KeyCode.LeftShift))
        {
                running = false;
            anim.SetFloat("speed", walkSpeed);
                resetSpeed();
        }*/
    }
    /*public bool canRun()
    {
        if (!crouch)
            return true;
        else
            return false;
    }*/

    public override void Roll()
    {
        base.Roll();
        if(shouldroll)
        {
            rollcontent();
            movement = new Vector2(Input.GetAxis(inputx), Input.GetAxis(inputy));

            shouldroll = false;
            rb.velocity = Vector3.zero;
            anim.SetBool("rotate", false);

            if (facingRight)
                x = 1;
            else
                x = -1;

            //   if (movement != Vector2.zero && es.myHE.attackPos < 1)
            if(movement!=Vector2.zero)
            {
              //  x = 0;
                z = 0;
                if (movement.x != 0)
                {
                    x =  Mathf.Sign(movement.x);
                }

              
                if (movement.y != 0)
                {

                    z = Mathf.Sign(movement.y);
                    if (movement.x != 0)
                    {
                        rb.AddForce(x * Mathf.Sqrt(12.5f), 0, z * Mathf.Sqrt(12.5f), ForceMode.Impulse);
                    }
                    else
                    {
                        rb.AddForce(0, 0, z* rollForce, ForceMode.Impulse);
                    }
                }
                else
                {
                    rb.AddForce(x * rollForce, 0, 0, ForceMode.Impulse);
                }
            }
            else
            {
                if (facingRight)
                    rb.AddForce(new Vector3(1f * rollForce, 0, 0), ForceMode.Impulse);
                else
                    rb.AddForce(new Vector3(-1f * rollForce, 0, 0), ForceMode.Impulse);
            }

            if (x > 0 && !facingRight)
            {

                Flip();
            }
            else if (x < 0 && facingRight)
            {

                Flip();
            }
        }
    }


    [HideInInspector]
    public bool canWalk;

    public bool canMove()
    {
        //&& Mathf.Abs(movement.x)>0.4f
        if (!crouch)
        {
            if (!floored && movement != Vector2.zero && (!blocking || blocking && jumping) && !playerHit && !rolling && !attacking && (Mathf.Abs(movement.x) > 0.3f || Mathf.Abs(movement.y) > 0.3f))
            {
                return true;
            }
            else
                return false;
        }
        else
        {
            if (!floored && movement != Vector2.zero && (!blocking || blocking && jumping) && !playerHit && !rolling && !attacking && (Mathf.Abs(movement.x) > 0.3f || Mathf.Abs(movement.y) > 0.3f) && canWalk)
            {
                return true;
            }
            else
                return false;
        }
    }

    public bool canFaceDifferentDirection()
    {
        if (!playerHit && !rolling && allowed && !floored)
            return true;
        else
            return false;
    }

    public override void landGround()
    {
        base.landGround();
        rb.velocity = Vector3.zero;
    }

    void rotate()
    {
        rb.velocity = Vector3.zero;
        attacking = true;

        //uh?
        cc.resetStates();

        cc.actions.Clear();

          //  allowed = true;

        canWalk = true;
        rotating = true;
        anim.SetBool("rotate", true);
    }

    void handleRotation()
    {
        if(blocking && !playerHit)
        {
            faceRightWay();
        }

        if (canFaceDifferentDirection())
        {
            //arent attacking
            if (cc.actions.Count<1 && grounded && anim.GetBool("Jump") == false)
            {

                if (movement.x > 0 && !facingRight)
                {
                    rotate();
                }
                else if (movement.x < 0 && facingRight)
                {
                    rotate();

                }
            }
            else
            {
                //we are attacking so allow flips

                faceRightWay();
            }
        }

        
    }

    void faceRightWay()
    {
        if (movement.x > 0 && !facingRight)
        {

            Flip();
        }
        else if (movement.x < 0 && facingRight)
        {

            Flip();
        }
    }

    public void Move()
    {
        movement = new Vector2(Input.GetAxis(inputx), Input.GetAxis(inputy));
   
          //  rb.MovePosition(new Vector3(transform.position.x + movement.x * moveSpeed, transform.position.y , transform.position.z + movement.y *  moveSpeed));

        handleRotation();
        if (canMove())
        {
            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.y * moveSpeed);
        }


            if (movement.x != 0)
                anim.SetFloat("Input", Mathf.Abs(movement.x));
            else if (movement.y != 0 && !Boundary)
            {
                anim.SetFloat("Input", Mathf.Abs(movement.y));
            }
            else
            {
                anim.SetFloat("Input", 0);
            }
 //       }
    }

    public void leaveMeAlone()
    {
        for (int i = 0; i < 4; i++)
        {
           if( enemyM.positions[i].enemy)
            {
                eref = enemyM.positions[i].enemy;
                eref.state = enemyState.spawn;
             eref.leaveSpot();
                eref.anim.SetFloat("Input", 0);
                eref.goWander();
                //new code
         //       eref.Join();
            }
        }
    }

    Enemy eref;
}
