using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using VIDE_Data; //<--- Import to use easily call VD class
using UnityEngine.SceneManagement;

public class Player : Warrior {

    Vector3 localScale;

    public Transform healthbar;

    public int saveIndex;

    public ItemManager itemManager;

    public DamageEntity dEntity;

    public LayerMask itemMask;

    public TravelManager traveler;

    public GameObject deathScreen;

    Collider2D[] itemsHit;

    bool rolling;

    public NavMeshObstacle2D obstacle;

    //Inventory System
    //[System.NonSerialized]
    public InventorySystem inventory = new InventorySystem();
    Item tempItem;//For picking up items
    int tempItemCount;
    GameObject tempItemGameobject;


    //Reference to object pool
    public ObjectPoolingManager objPoolManager;

    public void save()
    {
        manager.saveEm();
        id.instance.saving = 1; 
        id.instance.lastSceneSaved = SceneManager.GetActiveScene().name;
        id.instance.health = health;
        id.instance.x = transform.root.position.x;
        id.instance.y = transform.root.position.y;
        id.instance.time = DateTime.Now; 
      //   id.instance.saving = 1;
      //  id.instance.lastSceneSaved = SceneManager.GetActiveScene().name;
        SaveInvo();
      itemManager.saveEm();
        id.instance.SavePlayer(Application.persistentDataPath + "/gamesave.save" + id.instance.saveIndex);
        Debug.Log("saved");
    }

    public void saveEverythingExceptEnemies()
    {
      //  manager.saveEm();
        id.instance.saving = 1;
        id.instance.lastSceneSaved = SceneManager.GetActiveScene().name;
        id.instance.health = health;
        id.instance.x = transform.root.position.x;
        id.instance.y = transform.root.position.y;
        id.instance.time = DateTime.Now;
        //   id.instance.saving = 1;
        //  id.instance.lastSceneSaved = SceneManager.GetActiveScene().name;
        SaveInvo();
        itemManager.saveEm();
        id.instance.SavePlayer(Application.persistentDataPath + "/gamesave.save" + id.instance.saveIndex);
        Debug.Log("saved");
    }

    public void SaveInvo()
      {
          //string saveInventory = JsonUtility.ToJson(inventory);
          //PlayerPrefs.SetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory", saveInventory);
          inventory.Save();
      }

      public void LoadInvo()
      {
        //string loadInventory = PlayerPrefs.GetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory");
        //inventory = JsonUtility.FromJson<InventorySystem>(loadInventory);
          inventory.Load();
      }

    public void addToInvo(GameObject obj)
    {
        if (inventory.checkIfWeCanAdd(obj.GetComponent<ItemPickup>().item))
        {
            
            objPoolManager.ObjectPoolAddition(obj);
            ItemPickup ip = obj.GetComponent<ItemPickup>();
            tempItem = ip.item;
            tempItemCount = ip.count;
            inventory.Add(tempItem, tempItemCount);
        }
        else
        {
            objPoolManager.DropTable(obj, transform.position);
        }

        /*  tempItemGameobject = obj;

          Debug.Log(tempItemGameobject.name);
          if (tempItemCount <= 0)
          {
              Debug.LogError(tempItemGameobject.name + " item pickup count is 0 or less then");
          }


              objPoolManager.ReturnFromPool(obj);*/


    }

    public void pickUp()
    {
        if (health > 0)
        {
            itemsHit = Physics2D.OverlapCircleAll(transform.position, 1f, itemMask);

            for (int i = 0; i < itemsHit.Length; i++)
            {

                tempItemGameobject = itemsHit[i].gameObject;


                /*   if (hit[i].gameObject.GetComponent<ItemPickup>() == null)
                   {
                       Debug.Log(tempItemGameobject.name + "Error, Item Pickup Is not implimented onto this object");
                       continue;
                   }
                   if (hit[i].gameObject.GetComponent<ItemPickup>().item == null)
                   {
                       Debug.Log(tempItemGameobject.name + "Error, The Item in Item Pickup component is null on this prefab object");
                       continue;
                   }*/
                if (!inventory.checkIfWeCanAdd(itemsHit[i].GetComponent<ItemPickup>().item))
                    continue;

                //   Debug.Log(tempItemGameobject.name);
                objPoolManager.ObjectPoolAddition(tempItemGameobject);
                ItemPickup ip = itemsHit[i].gameObject.GetComponent<ItemPickup>();
                tempItem = ip.item;
                tempItemCount = ip.count;
                if (tempItemCount <= 0)
                {
                    Debug.LogError(tempItemGameobject.name + " item pickup count is 0 or less then");
                }

                inventory.Add(tempItem, tempItemCount);
                textDisplay.Enqueue("Picked Up " + tempItemCount.ToString() + " " + tempItem.name);
                checkIfWeShouldHandle();
                //        objPoolManager.txtboxManager.StartCoroutine(objPoolManager.txtboxManager.setTo("Picked Up " + tempItemCount.ToString() + " "+ tempItem.name));

            }
            //Add to inventoy Here
        }
    }

 /*   public bool checkIfLastWasTheSame(string text)
    {
        return objPoolManager.txtboxManager.checkIfTheLastWasTheSame(text);
    }*/

    public void checkIfWeShouldHandle()
    {
        if (!handlingDisplay)
        {
            handlingDisplay = true;
            StartCoroutine(handleTextDisplay());
        }
    }

    public IEnumerator handleTextDisplay()
    {

        while(textDisplay.Count>0)
        {
            objPoolManager.txtboxManager.StartCoroutine(objPoolManager.txtboxManager.setTo(textDisplay.Peek()));
            textDisplay.Dequeue();
            yield return new WaitForSeconds(0.5f);
        }
        handlingDisplay = false;
    }

    public Queue<string> textDisplay = new Queue<string>();
    public bool handlingDisplay;

    Vector3 movement;

    public EnemyManager manager;

    public Rigidbody2D rb;

    public void setQuest1Active()
    {
        quest1 = true;
    }

    // Update is called once per frame
    void FixedUpdate () {

        Movement();


    }

    public void TakeAway(Item item, int num)
    {
        inventory.Remove(item, num);
        //itemManager.tester.pool.Remove(item.name);
    }

    /*
    public void TakeAway(GameObject item, int num)
    {

        inventory.Remove(item.GetComponent<ItemPickup>().item, num);
        itemManager.tester.pool.Remove(item.name);
    }*/

        public bool checkIfEnough(Item item, int num)
    {
        if (inventory.inventory.Contains(item))
        {
            Item temp = inventory.inventory.Find(x => x.itemName == item.itemName);//.count -= item.count;
            if (temp.count>=num)
                return true;
            else
                return false;
            //  VD.assigned.overrideStartNode = 13;
            //VD.SetNode(10);
            //item.Use();
        }
        else
        {
            return false;
            //  VD.SetNode(11);
        }
    }

  /*  public bool Check(Item item)
    {
        if(inventory.inventory.Contains(item))
        {
            return true;
          //  VD.assigned.overrideStartNode = 13;
            //VD.SetNode(10);
            //item.Use();
        }
        else
        {
            return false;
          //  VD.SetNode(11);
        }

    }*/


public void Start()
    {

        // saveIndex = id.instance.saveIndex;

        anim.SetFloat("rot", Mathf.RoundToInt(transform.GetChild(0).transform.up.x));
        anim.SetFloat("roty", Mathf.RoundToInt(transform.GetChild(0).transform.up.y));

        inventory = new InventorySystem();
        GameManager.instance.playerInventory = inventory;
        objPoolManager = GameManager.instance.objPoolManager;


        if(id.instance.saving==1 )
        {
            saveIndex = id.instance.saveIndex;
            health = id.instance.health;
            float p = health / maxHealth;
            float userHpBarLength = p * 1f;
            localScale = new Vector3( userHpBarLength, 1, 1);
            healthbar.localScale = localScale;

            if(id.instance.lastSceneSaved == SceneManager.GetActiveScene().name)
            transform.root.position = new Vector2(id.instance.x, id.instance.y);
            else
            {
                if(!id.instance.fastTravelling) 
                transform.root.position = id.instance.travelPosition;
            }

            if(id.instance.fastTravelling)
            {
                id.instance.fastTravelling = false;
                transform.root.position = new Vector3(id.instance.x, id.instance.y);
            }

            if (manager.tester != null) 
                manager.StartCoroutine(manager.loadEnemies());

            GameManager.instance.playerInventory = inventory;
            LoadInvo();
        }
        else
        {
            localScale = healthbar.localScale;
        }
        if (manager.tester == null)
        {
            manager.initializeEnemies();
        }

        if (itemManager.tester==null)
        {
            itemManager.initializeEnemies();
        }
        else
        {
            itemManager.loadEnemies();
        }
    }

    public void moveDialogue()
    {
        dialogue.GetComponent<RectTransform>().transform.position = new Vector3(0, -10000, 0);
    }

    public void returnDialogue()
    {
        dialogue.GetComponent<RectTransform>().transform.position = new Vector2(dialogue.transform.parent.position.x, 0);
    }

   /* public void wait()
    {
        manager.loadEnemies();
        
    }*/

    public void Update()
    {
     /*   if (Input.GetKeyDown(KeyCode.K))
            Time.timeScale = 0;
        if (Input.GetKeyDown(KeyCode.U))
            Time.timeScale = 1;*/

        if (Input.GetKeyDown(KeyCode.Space))
        {
                Fire();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.timeScale != 0)
            {
                if(inTrigger)
                dEntity.Flip(inTrigger.transform);
                TryInteract();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            pickUp();
        }

        if (Input.GetKeyDown(KeyCode.R))
            Roll();
        //manager.checkSpot();
    }

    public void Roll()
    {
        if(health>0f)
        if (!rolling)
        {
                obstacle.carve = true;
            rolling = true;
            rollSpeed = 2;
         //   rb.AddForce(transform.GetChild(0).transform.up * 3f, ForceMode2D.Impulse);
            anim.Play("roll blend tree");
        }
    }

    public void rollForce()
    {
        rb.AddForce(transform.GetChild(0).transform.up * 3f, ForceMode2D.Impulse);
    }

    public void stopRoll()
    {
        rolling = false;
        rb.velocity = Vector2.zero;
        rollSpeed = 1;
    }

    public override void DamageEntity(float Damage)
    {
        base.DamageEntity(Damage);

        if (health > 100f)
            health = 100;

        float p = health / maxHealth;
        float userHpBarLength = p * 1f;
        localScale.x = userHpBarLength;
        healthbar.localScale = localScale;

        //update healthbar for us:) none for enemy

        if (health <= 0)
        {
            health = 0;
            rb.velocity = Vector2.zero;
            obstacle.carve = true;
            p = health / maxHealth;
            userHpBarLength = p * 1f;
            localScale.x = userHpBarLength;
            healthbar.localScale = localScale;

            damageCol.enabled = false;

            manager.clearEnemies();
            walls = Physics2D.OverlapCircleAll(transform.position, 1f, LayerMask.NameToLayer("wall"));
            if (walls.Length > 0)
            {
                GetComponent<CapsuleCollider2D>().offset = new Vector2(0, -0.2238213f);
                GetComponent<CapsuleCollider2D>().size = new Vector2(1f, 1.200648f);
            }
            obstacle.size = new Vector2(1f, .75f);
            anim.Play("Death Tree");

            //make enemies in enemy manager stop attacking and leave:)

        //disactivate our damage col & maybe navmesh obstacle
        }

    }

    Collider2D[] walls;

    public void loadDeathScreen()
    {
        deathScreen.SetActive(true);
    }


    public virtual void Fire()
    {
        if (!dialogue.activeInHierarchy)
            if (canAttack && health>0)
        {
            if (attackState == 0)
            {
                anim.SetBool("attacking", true);
              //  if (movement != Vector3.zero)
                 //   transform.GetChild(0).transform.up = Vector3.zero + movement; 
                attacking = true;
                attackState++;
                StartCoroutine("WeaponCoolDown");
            }
            else if (attackState == 1)
            {
                if (anim.GetBool("attacking") == true)
                {
                    anim.SetBool("Combo", true);
                    attacking = true;
                    canAttack = false;
                    StopCoroutine("WeaponCoolDown");
                    StartCoroutine("ComboCoolDown");
                }
            }

        }
    }

    public int rollSpeed= 1;

    public void Movement()
    {
        if (health > 0)
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            // float vv = Vector3.Angle(transform.up, transform.GetChild(0).transform.up);
            //    vv = Mathf.Round(vv / 45.0f) * 45.0f;

            if (rollSpeed == 1)
            {
                if (movement != Vector3.zero)
                {
                    transform.GetChild(0).transform.up = Vector3.zero + movement;
                    anim.SetFloat("rot", Mathf.Round(transform.GetChild(0).transform.up.x));
                    anim.SetFloat("roty", Mathf.Round(transform.GetChild(0).transform.up.y));

                    //use this for projectile direction :)
                    //          transform.GetChild(transform.childCount - 1).transform.up = new Vector2(Mathf.Round(transform.GetChild(0).transform.up.x) / 2f, Mathf.Round(transform.GetChild(0).transform.up.y) / 2f);


                    //   anim.SetFloat("Horizontal", movement.x);
                    //   anim.SetFloat("Vertical", movement.y);
                }
                float test = 0.5f;
                if (Input.GetKey(KeyCode.LeftShift))
                    test = 1f;
                anim.SetFloat("Blend", Mathf.Clamp(movement.magnitude * test, 0, 1f));

                //if(attacking==false) -> for no movement either normal attack or combo
                //
                //use if (canAttack) for movement with normal attack but none with combo
                if (canAttack)
                {
                    if (movement != Vector3.zero)
                        obstacle.carve = false;
                    else
                    {
                        obstacle.carve = true;
                    }
                    rb.MovePosition(new Vector2(transform.parent.position.x +  movement.x * Time.deltaTime * 2f * test, transform.parent.position.y + movement.y *Time.deltaTime*2f * test));
                }
            }
                //   transform.position += (movement * Time.deltaTime * 2f*test);

            // if (movement == Vector3.zero)
            //transform.up = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y) - new Vector2(transform.position.x, transform.position.y);
            // else
            // transform.up = Vector3.zero + movement;
        }
    }

   public IEnumerator ComboCoolDown()
    {
        // yield return new WaitForSeconds(attCoolDown);
        yield return new WaitUntil(() => anim.GetBool("Combo") == false);
        //   yield return new WaitForSeconds(attCoolDown);
        if (anim.GetBool("Combo") == false)
        {
            canAttack = true;
            attackState = 0;
            attacking = false;
        }
    }
}
