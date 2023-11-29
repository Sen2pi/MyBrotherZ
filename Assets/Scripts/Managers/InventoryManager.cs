using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using MyBrotherZ.Quest;
using SaveAndLoad;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

[Serializable]
public class InventoryManager : Singleton<InventoryManager>, IDataPersisence,IPredicateEvaluator
{
    [SerializeField] private List<CraftingRecipe> recipes = new List<CraftingRecipe>();
    [SerializeField] private GameObject itemCursor;
    [SerializeField] public GameObject slotHolder;
    [SerializeField] private GameObject hotBarHolder;
    [SerializeField] private GameObject craftSlotPrefab;
    [SerializeField] private GameObject craftSlotHolder;


    public int SelectedSlotIndex
    {
        get => selectedSlotIndex;
        set => selectedSlotIndex = value;
    }

    [SerializeField] private Slot[] startingItems;
    [SerializeField] private int selectedSlotIndex = 0;
    [SerializeField] private GameObject hotBarSelector;

    private Slot[] items;

    public Slot[] Items
    {
        get => items;
    }

    private GameObject[] recipesSlots;
    private GameObject[] slots;
    private GameObject[] hotbarSlots;
    private Slot movingSlot;
    private Slot tempSlot;
    private Slot originalSlot;
    bool isMovingItem = false;
    private bool isLoading = false;


    public ItemClass selectedItem;
    #region SUF

    private void Start()
    {
        //Inventory
        
            slots = new GameObject[slotHolder.transform.childCount];
            
            if (!isLoading)
            {
                items = new Slot[slotHolder.transform.childCount];
            }
            
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = slotHolder.transform.GetChild(i).gameObject;
                if (!isLoading)
                {
                    items[i] = new Slot();
                }
                
            }

            if (!isLoading)
            {
                for (int i = 0; i < startingItems.Length; i++)
                {
                    if (startingItems[i].Item == items[i].Item) break;
                    if (items[i].Item == null)
                    {
                        items[i] = startingItems[i];
                    }
            
                }
            }
            
        
        Debug.Log("Start Metod");
        
        //set all slots
        for (int i = 0; i < slotHolder.transform.childCount; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }

        //Hotbar
        hotbarSlots = new GameObject[hotBarHolder.transform.childCount];

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i] = hotBarHolder.transform.GetChild(i).gameObject;
        }

        //Craft
        RefresUI();
    }

    private void Update()
    {
        isLoading = false;
        // Check if the itemCursor GameObject is null before accessing it
        if (itemCursor != null)
        {
            itemCursor.SetActive(isMovingItem);
            itemCursor.transform.position = Input.mousePosition;
            if (isMovingItem)
            {
                // Check if the movingSlot or its Item is null before accessing them
                if (movingSlot != null && movingSlot.Item != null)
                {
                    itemCursor.GetComponent<Image>().sprite = movingSlot.Item.itemIcon;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isMovingItem)
            {
                EndItemMove();
            }
            else
            {
                BeginItemMove();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex + 1, 0, hotbarSlots.Length - 1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex - 1, 0, hotbarSlots.Length - 1);
        }

        if (hotBarSelector != null && hotbarSlots.Length > 0)
        {
            hotBarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
        }
        selectedItem = items[selectedSlotIndex + (hotbarSlots.Length * 5)].Item;
        RefresUI();
    }


    #endregion

    #region UI

    public void RefresUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(1).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(1).GetComponent<Image>().sprite = items[i].Item.itemIcon;
                slots[i].transform.GetChild(3).GetComponent<TMP_Text>().enabled = true;
                slots[i].transform.GetChild(3).GetComponent<TMP_Text>().text = items[i].Item.itemName;
                if (items[i].Item.isStackable)
                {
                    slots[i].transform.GetChild(2).GetComponent<TMP_Text>().enabled = true;
                    slots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = items[i].Quantity.ToString();
                }
                else
                {
                    slots[i].transform.GetChild(2).GetComponent<TMP_Text>().enabled = true;
                    slots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                }
            }
            catch
            {
                slots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                slots[i].transform.GetChild(1).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(3).GetComponent<TMP_Text>().text = "";
            }
        }

        RefreshHotBar();
        RefresCraft();
    }

    public void RefreshHotBar()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            try
            {
                if (hotbarSlots[i] != null)
                {
                    hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite =
                        items[i + (hotbarSlots.Length * 5)].Item.itemIcon;
                    hotbarSlots[i].transform.GetChild(2).GetComponent<TMP_Text>().enabled = true;
                    hotbarSlots[i].transform.GetChild(2).GetComponent<TMP_Text>().text =
                        items[i + (hotbarSlots.Length * 5)].Item.itemName;
                    if (items[i + (hotbarSlots.Length * 5)].Item.isStackable)
                    {
                        hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().enabled = true;
                        hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text =
                            items[i + (hotbarSlots.Length * 5)].Quantity.ToString();
                    }
                    else
                    {
                        hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().enabled = false;
                        hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                    }
                }
            }
            catch
            {
                hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarSlots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = ""; // Handle any exceptions or errors here (if needed).
            }
        }
    }

    public void RefresCraft()
    {
        
        List<CraftingRecipe> canCraftRecipes = CraftList(); // Obtenha a lista de receitas que podem ser criadas

        if (recipesSlots == null || recipesSlots.Length != canCraftRecipes.Count)
        {
            // Destrua os slots existentes, se houver, e crie novos slots para cada receita
            if (recipesSlots != null)
            {
                for (int i = 0; i < recipesSlots.Length; i++)
                {
                    Destroy(recipesSlots[i]);
                }
            }

            recipesSlots = new GameObject[canCraftRecipes.Count];

            for (int i = 0; i < canCraftRecipes.Count; i++)
            {
                var craftSlot = Instantiate(craftSlotPrefab, craftSlotHolder.transform);
                recipesSlots[i] = craftSlot;
            }
        }

        // Atualize os slots de receita com as informações corretas
        for (int i = 0; i < recipesSlots.Length; i++)
        {
            try
            {
                recipesSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                recipesSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = canCraftRecipes[i].icon;
                recipesSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().enabled = true;
                recipesSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text =
                    canCraftRecipes[i].RecipeDescription();
                recipesSlots[i].transform.GetChild(2).GetComponent<TMP_Text>().enabled = true;
                recipesSlots[i].transform.GetChild(2).GetComponent<TMP_Text>().text =
                    canCraftRecipes[i].outputItem.Item.itemName;
            }
            catch
            {
                recipesSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                recipesSlots[i].transform.GetChild(2).GetComponent<TMP_Text>().text = "";
                recipesSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                recipesSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            }
        }
    }

    #endregion

    #region CraftingSystem

    public List<CraftingRecipe> CraftList()
    {
        List<CraftingRecipe> craftableRecipes = new List<CraftingRecipe>();

        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe.CanCraft(this))
            {
                craftableRecipes.Add(recipe);
            }
        }
        return craftableRecipes;
    }

    public void CraftSelected(CraftingRecipe recipe)
    {
        if (recipe.CanCraft(this))
        {
            recipe.Craft(this);
        }
        else
        {
        }
    }

    public void Craft()
    {
        List<CraftingRecipe> canCraftRecipe = CraftList();

        if (canCraftRecipe.Count > 0)
        {
            int count = 0;
            foreach (var recipe in canCraftRecipe)
            {
                if (craftSlotHolder.transform.GetChild(count).transform.GetChild(2).GetComponent<TMP_Text>().text ==
                    recipe.outputItem.Item.itemName)
                {
                    CraftSelected(recipe);
                    return;
                }

                count++;
            }
        }
    }

    #endregion

    #region Create Remove Find

    public bool IsFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Item == null)
            {
                return false;
            }
        }

        return true;
    }

    public bool Add(ItemClass item, int quantity)
    {
        // Check if inventory contains item
        Slot slot = Contains(item);
        if (slot != null && slot.Item.isStackable)
        {
            slot.Quantity += quantity;
        }
        else
        {
            // Find the first available slot
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Item == null)
                {
                    items[i].AddItem(item, 1);
                    break;
                }
            }
        }

        RefresUI();
        return true;
    }

    public bool Remove(ItemClass item)
    {
        Slot temp = Contains(item);
        if (temp != null)
        {
            if (temp.Quantity > 1)
            {
                temp.Quantity -= 1;
            }
            else
            {
                int slotRemoveIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Item == item)
                    {
                        slotRemoveIndex = i;
                        break;
                    }
                }

                items[slotRemoveIndex].Clear();
            }
        }
        else
        {
            return false;
        }

        RefresUI();
        return true;
    }

    public bool Remove(ItemClass item, int quantity)
    {
        Slot temp = Contains(item);

        if (temp != null)
        {
            if (temp.Quantity > 1)
            {
                temp.Quantity -= quantity;
            }
            else
            {
                int slotRemoveIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Item == item)
                    {
                        slotRemoveIndex = i;
                        break;
                    }
                }

                items[slotRemoveIndex].Clear();
            }
        }
        else
        {
            return false;
        }

        RefresUI();
        return true;
    }

    public Slot Contains(ItemClass item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Item == item)
            {
                return items[i];
            }
        }

        return null;
    }

    public bool Contains(ItemClass item, int quantity)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Item == item && items[i].Quantity == quantity)
            {
                return true;
            }
        }

        return false;
    }
    public bool HasItem(ItemClass item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(items[i].Item, item))
            {
                return true;
            }
        }
        return false;
    }
    public int GetQuantity(ItemClass item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(items[i].Item, item))
            {
                return items[i].Quantity;
            }
        }
        return 0;
    }
    

    #endregion

    #region Selects and drag

    private bool BeginItemMove()
    {
        if (GameObject.Find("UI_Inventory").GetComponent<Canvas>().isActiveAndEnabled)
        {
            originalSlot = GetClosestSlot();
            if (originalSlot == null || originalSlot.Item == null)
            {
                return false;
            }

            movingSlot = new Slot(originalSlot);
            originalSlot.Clear();
            isMovingItem = true;
            RefresUI();
            return true;
        }

        return false;
    }

    private bool EndItemMove()
    {
        if (GameObject.Find("UI_Inventory").GetComponent<Canvas>().isActiveAndEnabled)
        {
            originalSlot = GetClosestSlot();
            if (originalSlot.Item == null)
            {
                originalSlot.AddItem(movingSlot.Item, movingSlot.Quantity);
                movingSlot.Clear();
            }
            else
            {
                if (originalSlot.Item != null)
                {
                    if (originalSlot.Item == movingSlot.Item)
                    {
                        if (originalSlot.Item.isStackable)
                        {
                            originalSlot.Quantity += movingSlot.Quantity;
                            movingSlot.Clear();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        tempSlot = new Slot(originalSlot); // a = b
                        originalSlot.AddItem(movingSlot.Item, movingSlot.Quantity); // b = c
                        movingSlot.AddItem(tempSlot.Item, tempSlot.Quantity); // a = c

                        RefresUI();
                        return true;
                    }
                }
                else
                {
                    originalSlot.AddItem(movingSlot.Item, movingSlot.Quantity);
                    movingSlot.Clear();
                }
            }
        }

        // Make sure to set isMovingItem to false even if the above condition fails
        isMovingItem = false;
        RefresUI();
        return true;
    }


    public Slot GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
            {
                return items[i];
            }
        }

        return null;
    }

    #endregion

    #region Load And Save

    public void LoadData(GameData data)
    {
        
        try
        {
            selectedSlotIndex = data.selectedSlotIndex;
            
            List<SlotData> inventoryList = new List<SlotData>();
            for (int i = 0; i < data.inventoryItems.Length; i++)
            {
                inventoryList.Add(data.inventoryItems[i]);
            }

            if (inventoryList == null)
            {
                return;
            }

            isLoading = true;

            int count = 0;
            items = new Slot[inventoryList.Count];
            foreach (SlotData objectState in inventoryList)
            {
                items[count] = new Slot(objectState.item, objectState.quantity);
                count++;
            }
        }
        catch
        {
            return;
        }
    }
    


    public void SaveData(ref GameData data)
    {
        
        // Save the items in the inventory to the GameData object
        data.selectedSlotIndex = selectedSlotIndex;
        data.inventoryItems = new SlotData[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            data.inventoryItems[i] = new SlotData
            {
                item = items[i].Item,
                quantity = items[i].Quantity
            };
        }
        
    }

    #endregion

    public bool? Evaluate(string predicate, string[] parameters)
    {
        switch (predicate)
        {
            case "HasInventoryItem":
                return HasItem(ItemClass.GetFromID(parameters[0]));
            
        }

        return null;
    }

    
}

[System.Serializable]
public class SlotData
{
    public ItemClass item;
    public int quantity;
}