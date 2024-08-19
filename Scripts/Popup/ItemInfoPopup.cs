using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPopup : BaseUI
{
    public int curItemSlotIndex;

    [Header("UI Components")]
    public Transform windowPanel;
    public Canvas canvas;
    public Image BG;
    public TextMeshProUGUI Title;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image itemIcon;
    public TextMeshProUGUI tradable;
    public TextMeshProUGUI price;
    public TextMeshProUGUI amount;
    public TextMeshProUGUI maxStack;
    public TextMeshProUGUI stats;

    [Header("Buttons")]
    public Button equipBtn;
    public Button unequipBtn;
    public Button quickOnFirstBtn;
    public Button quickOnSecondBtn;
    public Button quickOffBtn;
    public Button purchaseBtn;
    public Button sellBtn;
    public Button useBtn;
    public Button discardBtn;

    [Header("Variables")]
    public InventoryPopup inventory;
    public GameObject item;
    public ItemPrefab itemPrefab;
    public Vector2 spawnPosition = Vector2.zero;

    public InvenSlot currentItemSlot;
    private string inventoryPopupString = "InventoryPopup";

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        CheckGetInven();
    }

    public void CheckGetInven()
    {
        inventory = inventory == null ? ResourcesManager.instance.GetUIInDic(inventoryPopupString) as InventoryPopup : inventory;
    }

    public void SetItemPrefab()
    {
        itemPrefab = item.GetComponent<ItemPrefab>();
    }

    public void SetTexts()
    {
        currentItemSlot = inventory.invenSlots[curItemSlotIndex];

        itemIcon.sprite = itemPrefab.icon;
        itemName.text = itemPrefab.itemName;
        itemDescription.text = itemPrefab.itemDescription;
        tradable.text = itemPrefab.tradable ? "tradable" : "untradeable";
        price.text = $"Price: {itemPrefab.price}G";
        amount.text = itemPrefab.quantity >= 1 ? $"amount: {itemPrefab.quantity}" : string.Empty;
        maxStack.text = $"Up to {itemPrefab.maxStackAmount}";

        switch (itemPrefab.itemType)
        {
            case ItemType.Equipment_Weapon_Sword:
            case ItemType.Equipment_Weapon_Bow:
            case ItemType.Equipment_Armer_Top:
            case ItemType.Equipment_Armer_Bottom:
                if (itemPrefab != null)
                {
                    stats.text = GetEquipStatsText();
                }
                break;
            default:
                stats.text = string.Empty;
                break;
        }
        if (stats.text == string.Empty)
        {
            stats.gameObject.SetActive(false);
        }
        else
        {
            stats.gameObject.SetActive(true);
        }
    }

    public string GetEquipStatsText()
    {
        int maxHealth = itemPrefab.characterStat.maxHealth;
        float speed = itemPrefab.characterStat.speed;
        int defense = itemPrefab.characterStat.defense;
        float power = itemPrefab.characterStat.power;

        string str;

        string str1 = maxHealth != 0 ? $"MaxHealth {maxHealth} Up" : string.Empty;
        string str2 = speed != 0 ? $"speed {speed} Up" : string.Empty;
        string str3 = defense != 0 ? $"defense {defense} Up" : string.Empty;
        string str4 = power != 0 ? $"power {power} Up" : string.Empty;

        EquipmentItemData equipData = (EquipmentItemData)itemPrefab.itemData;
        string str5 = "";
        if (equipData.classLimit)
        {
            str5 = equipData.classNum == 0 ? "전사만 착용할 수 있다." : "궁수만 착용할 수 있다.";
            str = $"{str1}\n{str2}\n{str3}\n{str4}\n{str5}";
            return str;
        }

        str = $"{str1}\n{str2}\n{str3}\n{str4}";
        return str;
    }

    public void ExitInfoWindow()
    {
        windowPanel.gameObject.SetActive(false);
    }

    public void SetButtons()
    {
        HideButtons();
        ActivateInvenButtons();
    }

    public void HideButtons()
    {
        equipBtn.gameObject.SetActive(false);
        unequipBtn.gameObject.SetActive(false);
        quickOnFirstBtn.gameObject.SetActive(false);
        quickOnSecondBtn.gameObject.SetActive(false);
        quickOffBtn.gameObject.SetActive(false);
        purchaseBtn.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        useBtn.gameObject.SetActive(false);
        discardBtn.gameObject.SetActive(false);
    }

    public void ActivateInvenButtons()
    {
        switch (itemPrefab.itemType)
        {
            case ItemType.Consumable_NoUse:
                discardBtn.gameObject.SetActive(true);
                break;
            case ItemType.Consumable_CanUse:
                useBtn.gameObject.SetActive(true);
                CheckQuickOnOff();
                discardBtn.gameObject.SetActive(true);
                break;
            case ItemType.Equipment_Weapon_Bow:
            case ItemType.Equipment_Weapon_Sword:
            case ItemType.Equipment_Armer_Top:
            case ItemType.Equipment_Armer_Bottom:
                CheckEquipOnOff();
                discardBtn.gameObject.SetActive(true);
                break;
        }

        CheckTradeOnOff();
        
    }

    public InvenSlot GetCurrentInvenSlot()
    {
        return inventory.invenSlots[curItemSlotIndex];
    }

    public ItemSlot GetCurrentEquipSlot()
    {
        switch (itemPrefab.itemType)
        {
            case ItemType.Equipment_Weapon_Bow:
            case ItemType.Equipment_Weapon_Sword:
                return inventory.equipSlots[0];
            case ItemType.Equipment_Armer_Top:
                return inventory.equipSlots[1];
            case ItemType.Equipment_Armer_Bottom:
                return inventory.equipSlots[2];
            default:
                return null;
        }
    }

    public ItemSlot CheckGetQuickSlot()
    {
        if (currentItemSlot.isOnHotkey == false) return null;
        return inventory.quickSlots[currentItemSlot.quickSlotNumber];
    }


    public void CheckQuickOnOff()
    {
        if (GetCurrentInvenSlot().isOnHotkey)
        {   
            quickOffBtn.gameObject.SetActive(true);
        }
        else
        {
            quickOnFirstBtn.gameObject.SetActive(true);
            quickOnSecondBtn.gameObject.SetActive(true);
        }
    }

    public void CheckEquipOnOff()
    {
        if (GetCurrentInvenSlot().isEquipped)
        {
            unequipBtn.gameObject.SetActive(true);
        }
        else
        {
            equipBtn.gameObject.SetActive(true);
        }
    }

    public void CheckTradeOnOff()
    {
        if(GameManager.instance.NPCToggle.Count>0)
        {
            if (GameManager.instance.NPCToggle[0] == false) return;

            if (!itemPrefab.itemData.tradable) return;

            sellBtn.gameObject.SetActive(true);
        }
    }

    public void OnEnrollQuick(int index)
    {
        QuickSlot quickSlot = inventory.quickSlots[index];

        if(quickSlot.item != null)
        {
            InvenSlot previousInvenSlot = quickSlot.curItemSlot;
            previousInvenSlot.isOnHotkey = false;
            previousInvenSlot.quickSlotNumber = -1;
            inventory.UpdateSlot(previousInvenSlot);
        }
        
        quickSlot.curItemSlot = GetCurrentInvenSlot();
        quickSlot.curItemSlot.isOnHotkey = true;
        quickSlot.curItemSlot.quickSlotNumber = index;
        quickSlot.item = quickSlot.curItemSlot.item;

        inventory.UpdateSlot(quickSlot.curItemSlot);
        inventory.UpdateSlot(quickSlot);

        UpdateWindow();
    }

    public void OnQuickOff()
    {
        QuickSlot quickSlot;
        InvenSlot invenSlot = GetCurrentInvenSlot();
        if (invenSlot.quickSlotNumber == 0)
        {
            quickSlot = inventory.quickSlots[0];
        }
        else if(invenSlot.quickSlotNumber == 1)
        {
            quickSlot = inventory.quickSlots[1];
        }
        else
        {
            return;
        }

        quickSlot.Clear();

        invenSlot.isOnHotkey = false;
        invenSlot.quickSlotNumber = -1;
        inventory.UpdateSlot(invenSlot);

        UpdateWindow();
    }

    public void OnPurchase(GameObject item)
    {
        int itemPrice = item.gameObject.GetComponent<ItemPrefab>().price;

        if (DataManager.instance.gold < itemPrice) return;

        DataManager.instance.gold -= itemPrice;
        inventory.TryAddItem(item);
    }

    public void OnSell()
    {
        int count;

        if (itemPrefab.quantity == 1)
        {
            count = 1;
        }
        else
        {
            count = itemPrefab.quantity;
        }

        switch (itemPrefab.itemType)
        {
            case ItemType.Equipment_Weapon_Bow:
            case ItemType.Equipment_Weapon_Sword:
            case ItemType.Equipment_Armer_Top:
            case ItemType.Equipment_Armer_Bottom:
                if(currentItemSlot.isEquipped)
                {
                    GetCurrentEquipSlot().Clear();
                }
                break;
            case ItemType.Consumable_CanUse:
                if (CheckGetQuickSlot())
                {
                    CheckGetQuickSlot().Clear();
                }
                break;
            case ItemType.Consumable_NoUse:
                break;
        }
        
        DataManager.instance.gold += count * itemPrefab.itemData.price;
        currentItemSlot.Clear();
        ExitInfoWindow();
    }

    public void OnUse()
    {
        ((ConsumableItemData)itemPrefab.itemData).Use();

        if (itemPrefab.quantity == 1)
        {
            if (CheckGetQuickSlot())
            {
                CheckGetQuickSlot().Clear();
            }
            currentItemSlot.Clear();
            ExitInfoWindow();
        }
        else
        {
            itemPrefab.quantity -= 1;
            inventory.UpdateSlot(currentItemSlot);
            UpdateWindow();
        }
    }

    public void OnDiscard()
    {
        GameManager.instance.ItemPrefabPool(itemPrefab,GameManager.instance.player.transform.position);
        item.transform.position = GameManager.instance.player.transform.position;

        switch (itemPrefab.itemData.itemType)
        {
            case ItemType.Equipment_Weapon_Bow:
            case ItemType.Equipment_Weapon_Sword:
                inventory.equipSlots[0].Clear();
                break;
            case ItemType.Equipment_Armer_Top:
                inventory.equipSlots[1].Clear();
                break;
            case ItemType.Equipment_Armer_Bottom:
                inventory.equipSlots[2].Clear();
                break;
            case ItemType.Consumable_CanUse:
                if (currentItemSlot.isOnHotkey)
                {
                    QuickSlot quickSlot = currentItemSlot == inventory.quickSlots[0] ? inventory.quickSlots[0] : inventory.quickSlots[1];
                    quickSlot.Clear();
                }
                break;
        }
        currentItemSlot.Clear();

        ExitInfoWindow();
    }

    public void EquipInvenItem(int index)
    {        
        DataManager dataManager = DataManager.instance;
        InvenSlot invenSlot = inventory.invenSlots[index];
        ItemPrefab itemPrefab = inventory.invenSlots[index].item.GetComponent<ItemPrefab>();

        if (itemPrefab == null)
        {
            return;
        }

        EquipSlot equipSlot = inventory.GetEquipSlotWithDataType(itemPrefab.itemData);

        if (equipSlot.item != null)
        {
            InvenSlot previousItemSlot = inventory.equipSlots[CheckEquipType(itemPrefab.itemData.itemType)].curItemSlot;
            previousItemSlot.isEquipped = false;
            dataManager.UnEquipItem(previousItemSlot.item.GetComponent<ItemPrefab>());
            inventory.UpdateSlot(previousItemSlot);
        }

        equipSlot.curItemSlot = invenSlot;
        equipSlot.curItemSlot.isEquipped = true;
        equipSlot.item = invenSlot.item;
        dataManager.EquipItem(itemPrefab);

        inventory.UpdateSlot(invenSlot);
        inventory.UpdateSlot(equipSlot);
    }

    public void OnEquip()
    {
        DataManager dataManager = DataManager.instance;
        EquipSlot equipSlot = inventory.equipSlots[CheckEquipType()];
        EquipmentItemData curEquipmentItemData = (EquipmentItemData)item.GetComponent<ItemPrefab>().itemData;

        if (curEquipmentItemData.classLimit)
        {
            if (curEquipmentItemData.classNum != GameManager.instance.playerClassIndex)
            {
                return;
            }
        }

        if (equipSlot.item != null)
        {
            InvenSlot previousItemSlot = inventory.equipSlots[CheckEquipType()].curItemSlot;
            previousItemSlot.isEquipped = false;
            dataManager.UnEquipItem(previousItemSlot.item.GetComponent<ItemPrefab>());
            inventory.UpdateSlot(previousItemSlot);
        }

        equipSlot.curItemSlot = GetCurrentInvenSlot();
        equipSlot.curItemSlot.isEquipped = true;
        equipSlot.item = equipSlot.curItemSlot.item;
        dataManager.EquipItem(equipSlot.item.GetComponent<ItemPrefab>());

        inventory.UpdateSlot(equipSlot.curItemSlot);
        inventory.UpdateSlot(equipSlot);

        UpdateWindow();

        CharacterManager.instance.UpdateHealthUI();
        CharacterManager.instance.UpdateStaminaUI();

    }

    public int CheckEquipType()
    {
        switch(itemPrefab.itemData.itemType)
        {
            case ItemType.Equipment_Weapon_Sword:
            case ItemType.Equipment_Weapon_Bow:
                return 0;
            case ItemType.Equipment_Armer_Top:
                return 1;
            case ItemType.Equipment_Armer_Bottom:
                return 2;
            default:
                return -1;
        }
    }

    public int CheckEquipType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Equipment_Weapon_Sword:
            case ItemType.Equipment_Weapon_Bow:
                return 0;
            case ItemType.Equipment_Armer_Top:
                return 1;
            case ItemType.Equipment_Armer_Bottom:
                return 2;
            default:
                return -1;
        }
    }

    public void OnUnequip()
    {
        DataManager dataManager = DataManager.instance;
        EquipSlot equipSlot = inventory.equipSlots[CheckEquipType()];

        dataManager.UnEquipItem(equipSlot.item.GetComponent<ItemPrefab>());
        equipSlot.curItemSlot = GetCurrentInvenSlot();
        equipSlot.curItemSlot.isEquipped = false;
        equipSlot.item = null;

        inventory.UpdateSlot(equipSlot.curItemSlot);
        inventory.UpdateSlot(equipSlot);

        UpdateWindow();

        CharacterManager.instance.UpdateHealthUI();
        CharacterManager.instance.UpdateStaminaUI();
    }

    public void UpdateWindow()
    {
        SetTexts();
        SetButtons();
    }

    public void UseQuick(int index)
    {   
        QuickSlot selectedQuickSlot = inventory.quickSlots[index];
        
        if (selectedQuickSlot.item == null) return;

        InvenSlot referringInvenSlot = selectedQuickSlot.curItemSlot;

        referringInvenSlot.item.GetComponent<ItemPrefab>().itemData.Use();

        ItemPrefab usingItemPrefab = referringInvenSlot.item.GetComponent<ItemPrefab>();

        if (usingItemPrefab.quantity == 1)
        {
            selectedQuickSlot.Clear();
            referringInvenSlot.Clear();
            if (currentItemSlot == selectedQuickSlot.curItemSlot) ExitInfoWindow();
        }
        else
        {
            usingItemPrefab.quantity -= 1;
            referringInvenSlot.Set();
            if (currentItemSlot == selectedQuickSlot.curItemSlot) UpdateWindow();
        }
    }

}
