using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]

public class Consumables : Item {

    //public bool stackable;
    [Header("Consumable Info")]
    public int hPHealed;

    public Consumables()
    {
        itemType = ItemType.Consumables;//ties the item type to be correct        
    }

    public override void Use()
    {
        if(itemName != "Gold")
        {
            if (GameManager.instance.player.health < 100f)
            {
                base.Use();
                GameManager.instance.player.DamageEntity(-hPHealed);
                GameManager.instance.player.textDisplay.Enqueue("Consumed " + itemName);
                GameManager.instance.player.checkIfWeShouldHandle();
                Debug.Log("Consumable used");
            }
            else
            {
               if(! GameManager.instance.player.objPoolManager.txtboxManager.checkIfTheLastWasTheSame("health already full"))
                {
                    GameManager.instance.player.textDisplay.Enqueue("health already full");
                   GameManager.instance.player.checkIfWeShouldHandle();
                }
               else
                Debug.Log("health already full");
            }
        }
    }
}
