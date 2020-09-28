using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    #region Variables

    GameObject _inventoryUI;//This is a reference to its own gameobject

    GameObject player;//Reference to the player to grab the list of items currently on him
    List<Item> tempItemList;//a copy of the list to be applied

    GameObject[] equipmentSlots;
    GameObject[] inventorySlots;

    //this is for the slots and temporary attaching the UI slots and adding in all the information
    Image tempImage;
    Item tempItem;
    Text tempItemName;
    Text tempItemCount;
    Image tempBackground;//adds button functionality, the button aspect will not work with this inactive
    Button tempBackgroundButton;

    //When you select a item with a item type of Consumable
    Transform itemConsumableSelectionMenu;
    Button itemConsumableUseButton;
    Button itemConsumableDropButton;
    Button itemConsumableBackButton;
    Image itemConsumableSpritePlaceholder;
    Text itemConsumableItemNameText;
    Text itemConsumableItemTypeText;//probably useless but will leave here just incase
    Text itemConsumableItemDescriptionText;

    //Item Selection Menu => Remove
    [Header("Drop Selection Menu")]
    Transform dropSelectionMenu;
    Button dropPlusButton;
    Button dropMinusButton;
    Button dropCancelButton;
    Button dropDropButton;
    Image dropSpritePlaceholder;
    Text dropItemCount;
    int droppingHowMany;

    //Temporary Variables
    GameObject tempItemGameobject;

    //Constants
    const int GOLD_SLOT_LOCATION = 15;
    const int WATER_SLOT_LOCATION = 14;

    #endregion

    // Use this for initialization
    void Start () {


        #region Variable Setters

        _inventoryUI = GameObject.Find("Canvas/InventoryUI");

        player = GameObject.Find("Update").transform.GetChild(0).gameObject;

        //Item Options Menu => when a item is selected of itemtype Consumables
        itemConsumableSelectionMenu = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground").transform;

        itemConsumableUseButton = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/Use_Button").GetComponent<Button>();

        itemConsumableDropButton = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/Drop_Button").GetComponent<Button>();

        itemConsumableBackButton = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/Back_Button").GetComponent<Button>();

        itemConsumableSpritePlaceholder = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/SpritePlaceHolder").GetComponent<Image>();

        itemConsumableItemNameText = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/ItemName_Text").GetComponent<Text>();

        itemConsumableItemTypeText = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/ItemType_Text").GetComponent<Text>();

        itemConsumableItemDescriptionText = GameObject.Find("Canvas/InventoryUI/ConsumableOptionsBackground/ItemDescription_Text").GetComponent<Text>();

        //Item Options Menu => Consumables/Equipment => when a item is selected to drop regardless of its previous type
        dropSelectionMenu = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground").transform;

        dropPlusButton = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground/Plus_Button").GetComponent<Button>();

        dropMinusButton = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground/Minus_Button").GetComponent<Button>();

        dropCancelButton = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground/Back_Button").GetComponent<Button>();

        dropDropButton = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground/Drop_Button").GetComponent<Button>();

        dropSpritePlaceholder = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground/SpritePlaceHolder").GetComponent<Image>();

        dropItemCount = GameObject.Find("Canvas/InventoryUI/DropOptionsBackground/DropCount_Text").GetComponent<Text>();
        #endregion

        #region Slot Setter

        //Items
        int slotCounter = GameObject.Find("Canvas/InventoryUI/InventoryBackground/ItemsBackground").transform.childCount;

        inventorySlots = new GameObject[slotCounter];

        for (int i = 0; i < slotCounter; i++)
        {
            inventorySlots[i] = GameObject.Find("Canvas/InventoryUI/InventoryBackground/ItemsBackground/InventorySlot (" + i + ")");
        }

        #endregion

        if(_inventoryUI.activeInHierarchy)
        {
            CloseInventoryMenu();
        }

    }

    #region InventoryUI Main Systems

    public void UpdateUI()
    {
        ClearUIItems();
        tempItemList = player.GetComponent<Player>().inventory.inventory;
        InventoryList(tempItemList);
        //EquipmentList(player.GetComponent<Unit>().equipmentSystem.currentEquipment);
        ItemOptionsCleanup();
    }

    void InventoryList(List<Item> currentInventory)
    {
        int correctorForSetLocations = 0;

        for (int i = 0; i < currentInventory.Count; i++)
        {
            if(currentInventory[i].itemType == ItemType.Gold)
            {
                AddNewItem(currentInventory[i], GOLD_SLOT_LOCATION);
                correctorForSetLocations++;
            }
          /*  else if(currentInventory[i].itemType == ItemType.Water)
            {
                AddNewItem(currentInventory[i], WATER_SLOT_LOCATION);
                correctorForSetLocations++;
            }*/
            else
            {
                AddNewItem(currentInventory[i], i-correctorForSetLocations);
            }
        }

    }

    public void ClearUIItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            ClearSlotInventory(i);
        }
    }

    //Inventory
    public void AddNewItem(Item newItem, int slotIndex)//Adds the inventory item into the slot position on the inventory UI
    {
        TempAttachingChildrenInventory(slotIndex);//This shall attach the items pictures, funtion and text to the proportionate positions

        tempImage.sprite = newItem.sprite;
        tempItemName.text = newItem.itemName;

        if (newItem.count == 0)
        {
            tempItemCount.text = ("x " + "1");
        }
        else
        {
            tempItemCount.text = ("x " + newItem.count.ToString());
        }
        tempImage.enabled = true;
        tempBackground.enabled = true;
        tempBackgroundButton.onClick.AddListener(() => { ItemOptions(slotIndex,newItem); });
    }

    public void ClearSlotInventory(int slotIndex)
    {
        TempAttachingChildrenInventory(slotIndex);
        tempImage.sprite = null;
        tempItemName.text = null;
        tempItemCount.text = null;
        tempImage.enabled = false;
        tempBackground.enabled = false;
        tempBackgroundButton.onClick.RemoveAllListeners();
    }

    public void TempAttachingChildrenInventory(int slotIndex)//This shall attach the items pictures, funtion and text to the proportionate positions
    {
        tempImage = inventorySlots[slotIndex].transform.GetChild(1).GetChild(0).GetComponent<Image>();
        tempItemName = inventorySlots[slotIndex].transform.GetChild(1).GetChild(1).GetComponent<Text>();
        tempItemCount = inventorySlots[slotIndex].transform.GetChild(1).GetChild(2).GetComponent<Text>();
        tempBackground = inventorySlots[slotIndex].transform.GetChild(1).GetComponent<Image>();//adds button functionality, the button aspect will not work with this inactive
        tempBackgroundButton = inventorySlots[slotIndex].transform.GetChild(1).GetComponent<Button>();
    }

    #endregion

    #region Item Options Menu

    public void ItemOptions(int slotIndex, Item newItem)
    {
        if (newItem.itemType == ItemType.Consumables)
        {
            itemConsumableSelectionMenu.gameObject.SetActive(true);
            itemConsumableUseButton.gameObject.SetActive(true);
            ItemOptionsConsumableSelectedUpdateUI(newItem);
            itemConsumableUseButton.onClick.AddListener(() => { UseItem(newItem); });
            if (newItem.stackable == false)
            {
                itemConsumableDropButton.onClick.AddListener(() =>
                {
                    droppingHowMany = 1;
                    DropItem(slotIndex, newItem);
                    itemConsumableSelectionMenu.gameObject.SetActive(false);
                });
            }
            else
            {
                if(newItem.count == 1)
                {
                    itemConsumableDropButton.onClick.AddListener(() =>
                    {
                        droppingHowMany = 1;
                        DropItem(slotIndex, newItem);
                        itemConsumableSelectionMenu.gameObject.SetActive(false);
                    });
                }
                else
                {
                    itemConsumableDropButton.onClick.AddListener(() =>
                    {
                        droppingHowMany = newItem.count;
                        DropOptions(slotIndex, newItem);
                        itemConsumableSelectionMenu.gameObject.SetActive(false);
                    });
                }
            }

            itemConsumableBackButton.onClick.AddListener(() => 
            {
                itemConsumableSelectionMenu.gameObject.SetActive(false);
                ItemOptionsCleanup();
            });
        }
        else if (newItem.itemType == ItemType.QuestItems)
        {
            itemConsumableSelectionMenu.gameObject.SetActive(true);
            ItemOptionsConsumableSelectedUpdateUI(newItem);
            itemConsumableUseButton.gameObject.SetActive(false);
         //   itemConsumableUseButton.onClick.AddListener(() => { UseItem(newItem); });
            if (newItem.stackable == false)
            {
                itemConsumableDropButton.onClick.AddListener(() =>
                {
                    droppingHowMany = 1;
                    DropItem(slotIndex, newItem);
                    itemConsumableSelectionMenu.gameObject.SetActive(false);
                });
            }
            itemConsumableBackButton.onClick.AddListener(() =>
            {
                itemConsumableSelectionMenu.gameObject.SetActive(false);
                ItemOptionsCleanup();
            });
        }
      /*  else if(newItem.itemType == ItemType.Water)
        {
            if (newItem.count > 0)
            {
                newItem.Use();
                UpdateUI();
            }
        }*/
      //Gold Will Do Fuck All
    }

    void ItemOptionsConsumableSelectedUpdateUI(Item newItem)
    {
        itemConsumableSpritePlaceholder.sprite = newItem.sprite;
        itemConsumableItemNameText.text = newItem.itemName;
        itemConsumableItemTypeText.text = newItem.itemType.ToString();
        itemConsumableItemDescriptionText.text = newItem.itemDescription;
    }

    void ItemOptionsCleanup()//removes all listeners and cleans up ui
    {
        //Consumables
        itemConsumableUseButton.onClick.RemoveAllListeners();
        itemConsumableDropButton.onClick.RemoveAllListeners();
        itemConsumableBackButton.onClick.RemoveAllListeners();
        itemConsumableSpritePlaceholder.sprite = null;
        itemConsumableItemNameText.text = null;
        itemConsumableItemTypeText.text = null;
        itemConsumableItemDescriptionText.text = null;
    }

    #endregion

    #region Use Item Function

    public void UseItem(Item newItem)
    {
        if (newItem != null)
        {
            newItem.Use();

            UpdateUI();
            itemConsumableSelectionMenu.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("New Item is Null");
        }
    }

    #endregion

    #region Drop Menu Functions

    public void DropItem(int slotIndex,Item newItem)//If this item is not stackable call this function
    {
        if (droppingHowMany > 0)
        {
            player.GetComponent<Player>().inventory.Remove(newItem,droppingHowMany);
            GameManager.instance.objPoolManager.ObjectPoolPlayerDrop(newItem.itemPrefab, droppingHowMany);
        }

        dropSelectionMenu.gameObject.SetActive(false);
        UpdateUI();
    }

    public void DropOptions(int slotIndex, Item newItem)
    {
        dropSelectionMenu.gameObject.SetActive(true);

        DropOptionsRemoveListeners();

        dropDropButton.onClick.AddListener(() => DropItem(slotIndex, newItem));
        dropPlusButton.onClick.AddListener(() => UpDropCount(slotIndex, newItem));
        dropMinusButton.onClick.AddListener(() => DownDropCount(slotIndex, newItem));

        dropSpritePlaceholder.sprite = newItem.sprite;
        dropItemCount.text = droppingHowMany.ToString();

        dropCancelButton.onClick.AddListener(() =>
        {
            dropSelectionMenu.gameObject.SetActive(false);
            ItemOptionsCleanup();
        });
    }

    void DropOptionsRemoveListeners()
    {
        dropDropButton.onClick.RemoveAllListeners();
        dropCancelButton.onClick.RemoveAllListeners();
        dropPlusButton.onClick.RemoveAllListeners();
        dropMinusButton.onClick.RemoveAllListeners();
    }

    public void UpDropCount(int slotIndex, Item newItem)
    {
        droppingHowMany++;
        if (droppingHowMany > newItem.count)
        {
            droppingHowMany = 1;
        }
        dropItemCount.text = droppingHowMany.ToString();
    }

    public void DownDropCount(int slotIndex, Item newItem)
    {
        droppingHowMany--;
        if (droppingHowMany < 1)
        {
            droppingHowMany = newItem.count;
        }
        dropItemCount.text = droppingHowMany.ToString();
    }

    #endregion

    #region Open/Close InventoryMenu

    public void OpenInventoryMenu()
    {
        _inventoryUI.SetActive(true);
        UpdateUI();
    }

    public void CloseInventoryMenu()
    {
        _inventoryUI.SetActive(false);
        itemConsumableSelectionMenu.gameObject.SetActive(false);
        dropSelectionMenu.gameObject.SetActive(false);
    }

    #endregion

    #region Testing/To Be Removed

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(_inventoryUI.activeInHierarchy)
            {
                CloseInventoryMenu();
            }
            else
            {
                OpenInventoryMenu();
            }
        }
    }

    #endregion
}
