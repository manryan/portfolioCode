using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEntityEnemy : HitEntity
{
    Enemy en;

    float val;

   // System.Func<>

   string hi()
    {
        return "hi";
    }
    
    float amountattacking()
    {
        val = 0;

        //float f = (val != 4) ? (val =5) : (val = 6);

        //string concatenation

        //  string s = $"{hi()} hello";   

        //func<int, int>              

        for (int i = 0; i < 4; i++)
        {
            if(en.p.enemyM.positions[i].enemy && en.p.enemyM.positions[i].enemy !=en)
            if(en.p.enemyM.positions[i].enemy.state== enemyState.attacking)
            {
                val++;
            }
        }
        return val;
    }

    public override void OnDisable()
    {
        if (en.grounded && !en.playerHit && !en.floored && !en.p.floored)
            if (en.closeenough())
            {
                if (entitiesHit.Count < 1)
                {
                    
                    //tell enemy to make a decision since we missed

                    //first check if enemy isnt floored or hit

                 

      
                            if (en.p.enemyM.numberOfEnemies() > 1)
                            {
                                //decision tree
                                en.StartCoroutine(en.returntodecisiontree());

                            }
                            else //return to approach
                                en.StartCoroutine(en.endstatefunctattack());
                

                }
                else
                {

                  
                        if (en.cc.kickState + en.cc.punchState == 3)
                        {
                        //player blocked all hits so back away/roll etc?
                        en.purposelymiss = false;
                        en.StartCoroutine(en.returntodecisiontree(1));
                        }
                        else
                        {
                            //prepare for combo, sidestep, decision tree etc depending on how many are on the player

                            //check how many attacks if we've done? using kick/punch state

                            if (en.cc.kickState + en.cc.punchState < 2)
                            {
                                //if player blocked

                                if (en.p.blocked)
                                {
                                        // how many are on the player?
                                        if (amountattacking() < 2)
                                        {
                                   

                                            // do proper attack if high intelligence else random att?
                                            if (en.smartAi)
                                            {
                                                en.properAttack();
                                            }
                                            else
                                            {
                                                // can purposely make player block all?

                                                if (Random.value * 100f < 30f)
                                                {
                                                    en.purposelymiss = true;
                                                    en.missAttack();
                                                }
                                                else
                                                {

                                                alternatives();
                                                }
                                            }

                                        }
                                        else
                                        {
                                            //procastinate states
                                            en.StartCoroutine(en.returntodecisiontree(1));

                                        }
                                     
                                }
                                else
                                {

                                // how many are on the player?
                                if (amountattacking() < 2)
                                {
                                    if (en.smartAi)
                                    {
                                        en.properAttack();
                                    }
                                    else
                                    {
                                        alternatives();
                                    }
                                }
                                else
                                {
                                    //procastinate states
                                    en.StartCoroutine(en.returntodecisiontree(1));

                                }
                                }

                            }
                            else
                            {
                                //might as well continue our combo if player isnt floored etc, unless told to miss attacks

                                    if (en.purposelymiss)
                                    {
                                        en.missAttack();

                                    }
                                    else
                                    {


                                    //proper att if high intelligence eitherwise random? ****************************
                                    if (amountattacking() < 2)
                                    {
                                        if (Random.value * 100f < 100f)
                                        {
                                            en.properAttack();
                                        }
                                        else
                                            en.dorandomattack();
                                    }
                                    else
                                    {
                                        en.StartCoroutine(en.returntodecisiontree(1));
                                    }
                                    }
                                
                            }
                        }
                    
                }
            }
            else if (!en.p.floored && amountattacking() < 2 && (en.grounded && !en.playerHit && !en.floored)) //move closer to our spot since not close enough and player isnt floored
                en.StartCoroutine(en.endstatefunctattack());


        base.OnDisable();
    }

    float rand;

    //temporary, need a better method for checking if should continue attacking in alternatives?

    float chancetoattack()
    {
        val = 0;
        for (int i = 0; i < 4; i++)
        {
            if (en.p.enemyM.positions[i].enemy && en.p.enemyM.positions[i].enemy != en)
                if (en.p.enemyM.positions[i].enemy.state == enemyState.attacking)
                {
                    val++;
                }
        }

        return -20+ (val * 10f);
    }

    public void alternatives()
    {

        rand = Random.value * 100f;
        if(rand + chancetoattack() <33.33f)
        {
            //continue combo?
            if (en.cc.kickState + en.cc.punchState == 3)
            {
                en.StartCoroutine(en.returntodecisiontree(1));
            }
            else
                en.dorandomattack();
        }
        else if (rand < 66.66f)
        {
            //go to a procastinate state?
            en.StartCoroutine(en.returntodecisiontree(1));
        }
        else
        {
            //side step?
            en.stepaside();
        }
        Debug.Log(rand);
    }

    public override void landed(Collider c)
    {
        base.landed(c);
        if(!empty && !en.grounded)
        {
            //not sure if ill keep this?

            en.rb.velocity = new Vector3(0, en.rb.velocity.y, 0);
        }
    }

    public override void Awake()
    {
        base.Awake();
        en = (Enemy)entity;
    }
}
