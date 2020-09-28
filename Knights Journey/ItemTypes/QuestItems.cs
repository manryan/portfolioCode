using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "QuestItems", menuName = "Item/QuestItems")]


public class QuestItems : Item
{

    public QuestItems()
    {
        itemType = ItemType.QuestItems;
    }

    public override void Use()
    {
     //   if (itemName != "Gold")
     //   {

                base.Use();
        //        GameManager.instance.player.DamageEntity(-hPHealed);
     //   }
    }
}
