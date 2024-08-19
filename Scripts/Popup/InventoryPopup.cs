using UnityEngine;

public class InventoryPopup : BaseUI
{
    public EquipSlot[] equipSlots;
    public QuickSlot[] quickSlots;
    public InvenSlot[] invenSlots;

    public GameObject inventoryWindow;
    public Transform equipSlotPanel;
    public Transform quickSlotPanel;
    public Transform invenSlotPanel;
    public Transform dropPosition;

    private void Start()
    {
        equipSlots = new EquipSlot[equipSlotPanel.childCount];
        quickSlots = new QuickSlot[quickSlotPanel.childCount];
        invenSlots = new InvenSlot[invenSlotPanel.childCount];

        for (int i = 0; i < equipSlots.Length; i++)
        {
            equipSlots[i] = equipSlotPanel.GetChild(i).GetComponent<EquipSlot>();
            equipSlots[i].index = i;
        }

        for (int i = 0; i < quickSlots.Length; i++)
        {
            quickSlots[i] = quickSlotPanel.GetChild(i).GetComponent<QuickSlot>();
            quickSlots[i].index = i;
        }

        for (int i = 0; i < invenSlots.Length; i++)
        {
            invenSlots[i] = invenSlotPanel.GetChild(i).GetComponent<InvenSlot>();
            invenSlots[i].index = i;
            ItemPrefab itemPrefab = invenSlots[i].item.GetComponent<ItemPrefab>();
            itemPrefab.Initialize();
        }

        UpdateUI();
        this.gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        UpdateEquipSlots();
        UpdateQuickSlot();
        UpdateInvenSlots();
    }

    public void UpdateEquipSlots()
    {   
        for (int i = 0; i < equipSlots.Length; i++)
        {
            ItemData itemData = invenSlots[i].item?.GetComponent<ItemPrefab>()?.itemData;

            if (itemData == null || itemData.id == 0)
            {
                equipSlots[i].Clear();
                continue;
            }
            else
            {
                equipSlots[i].Set();
            }
        }
    }

    public void UpdateQuickSlot()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            ItemData itemData = invenSlots[i].item?.GetComponent<ItemPrefab>()?.itemData;

            if (itemData == null || itemData.id == 0)
            {
                quickSlots[i].Clear();
                continue;
            }

            quickSlots[i].Set();
        }
    }

    public void UpdateInvenSlots()
    {
        for (int i = 0; i < invenSlots.Length; i++)
        {
            ItemPrefab itemPrefab = invenSlots[i].item?.GetComponent<ItemPrefab>();
            ItemData itemData = invenSlots[i].item?.GetComponent<ItemPrefab>()?.itemData;

            if (itemPrefab == null || itemData == null || itemPrefab.quantity == 0 || itemData.id == 0)
            {
                invenSlots[i].Clear();
                continue;
            }

            invenSlots[i].Set();
        }
    }

    public void UpdateSlot(ItemSlot itemSlot)
    {
        ItemData itemData = itemSlot.item?.GetComponent<ItemPrefab>()?.itemData;

        if (itemData == null || itemData.id == 0)
        {
            itemSlot.Clear();
            return;
        }

        itemSlot.Set();
    }

    public void UpdateInvenZeroSlots()
    {
        for (int i = 0; i < invenSlots.Length; i++)
        {
            if (invenSlots[i].item.GetComponent<ItemPrefab>().quantity > 0)
            {
                invenSlots[i].Set();
            }
            else
            {
                invenSlots[i].Clear();
            }
        }
    }

    public bool TryAddItem(GameObject _item)
    {
        ItemPrefab itemPrefab = GetItemPrefab(_item);

        for (int i = 0; i < invenSlots.Length; i++)
        { 
            ItemPrefab slotItemPrefab = GetItemPrefab(invenSlots[i].item);

            if (slotItemPrefab.id == 0) continue; 
            
            if (slotItemPrefab.id != itemPrefab.id) continue;

            if (slotItemPrefab.quantity < slotItemPrefab.maxStackAmount)
            {
                slotItemPrefab.quantity += itemPrefab.quantity;
                
                UpdateSlot(invenSlots[i]);
                ItemPoolDataClear(_item);
                return true;
            }
        }

        for (int i = 0; i < invenSlots.Length; i++)
        {               
            ItemPrefab slotItemPrefab = GetItemPrefab(invenSlots[i].item);

            if (slotItemPrefab.id != 0) continue;
            
            Init(slotItemPrefab,itemPrefab);
            slotItemPrefab.Initialize();

            invenSlots[i].isEquipped = false;
            invenSlots[i].isOnHotkey = false;

            UpdateSlot(invenSlots[i]);  

            ItemPoolDataClear(_item);          
            return true;
        }

        return false;
    }

    public bool isOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public ItemPrefab GetItemPrefab(GameObject item)
    {
        if (item.TryGetComponent(out ItemPrefab itemPrefab))
        {
            return itemPrefab;
        }
        else
        {
            return null;
        }
    }
    void Init(ItemPrefab itemobj, ItemPrefab newItemPrefab)
    { 
        itemobj.itemData = newItemPrefab.itemData;
        itemobj.upgradeCount = newItemPrefab.upgradeCount;
        itemobj.quantity = newItemPrefab.quantity;
    }   
    public void ItemPoolDataClear(GameObject itemOBj)
    {   
        SpriteRenderer _sprite=itemOBj.GetComponentInChildren<SpriteRenderer>();        
        ItemPrefab itemPrefab=itemOBj.GetComponent<ItemPrefab>();
        itemPrefab.itemData=null;
        itemPrefab.Initialize();
        _sprite.sprite=itemPrefab.icon;
        itemOBj.SetActive(false);
    }

    public EquipSlot GetEquipSlotWithDataType(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.Equipment_Weapon_Sword:
            case ItemType.Equipment_Weapon_Bow:
                return equipSlots[0];
            case ItemType.Equipment_Armer_Top:
                return equipSlots[1];
            case ItemType.Equipment_Armer_Bottom:
                return equipSlots[2];
            default:
                return null;
        }
    }

    public void Open()
    {
        UIManager.instance.ShowUI<InventoryPopup>(UIs.Popup);
    }

    public void Close()
    {
        UIManager.instance.HideUI<InventoryPopup>();
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}