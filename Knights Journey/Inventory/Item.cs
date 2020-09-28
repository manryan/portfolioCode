using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Descriptors for Items
public enum ItemType
{
    Consumables,
    Gold,
    QuestItems
}

//[RequireComponent(typeof(ItemPickup))]

[System.Serializable]
public abstract partial class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;

    public ItemType itemType;
    [System.NonSerialized]
    public GameObject itemPrefab;
    [System.NonSerialized]//This is set in item pickup on start
    public Sprite sprite;
    public bool defaultItem;
    public int cost;
    public bool stackable;
    public int count;
    public string itemDescription;
    public Sprite defaultSprite;

    //Destroy Item in Inventory

    public virtual void Use()
    {
        //calls the use function from whatever inherits from this
        GameManager.instance.playerInventory.Remove(this, 1);//Destroy Item in Inventory
    }
}
