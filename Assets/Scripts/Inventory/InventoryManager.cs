using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehavior<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    public List<InventoryItem>[] inventoryLists;

    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        base.Awake();

            // Create Inventory Lists
            CreateInventoryLists();

        // Create item details dictionary
        CreateItemDetailsDictionary();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int) InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        // initialize inventory list capacity array
        inventoryListCapacityIntArray = new int[(int) InventoryLocation.count];

        // initialize inventory list capacity
        inventoryListCapacityIntArray[(int) InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    /// <summary>
    /// Populations the itemDetailsDictionary from the scriptable object items list
    /// </summary>
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// Add an item to the inventory list for the inventoryLocation
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int) inventoryLocation];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int) inventoryLocation]);
    }

    /// <summary>
    /// Add item to the end of the inventory 
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        DebugPrintInventoryList(inventoryList);
    }

    // Adds item at position
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;

        Debug.ClearDeveloperConsole();
        DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Find if an itemCode is already in the inventory. Returns the item position in the inventory list
    /// or -1 is the item is not in the inventory
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int) inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the itemDetails (from the SO_ItemList) for the itemCode, or null if the item code doesn't exist
    /// </summary>

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        foreach (InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("Item description:" +
                      InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription +
                      "   Item Quantity: " + inventoryItem.itemQuantity);
        }
        Debug.Log("***********************************");
    }
}
