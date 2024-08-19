using UnityEngine;

public class InvenSlot : ItemSlot
{
    public bool isEquipped;
    public bool isOnHotkey;

    public int equipSlotNumber;
    public int quickSlotNumber;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Set()
    {
        base.Set();

        InventoryPopup inventory = ResourcesManager.instance.GetUIInDic(inventoryPopupString) as InventoryPopup;
        ItemInfoPopup itemInfoPopup = ResourcesManager.instance.GetUIInDic(itemInfoPopupString) as ItemInfoPopup;
        ItemPrefab itemPrefab = item.GetComponent<ItemPrefab>();
        
        if (!image.gameObject.activeSelf) image.gameObject.SetActive(true);
        image.sprite = itemPrefab.icon;
        quantityText.text = itemPrefab.quantity > 1 ? itemPrefab.quantity.ToString() : string.Empty;

        SetEquipMark();
        SetHotKeyMark();
    }

    public void SetEquipMark()
    {
        if (isEquipped)
        {
            if (!EquipText.gameObject.activeSelf) EquipText.gameObject.SetActive(true);
        }
        else
        {
            if (EquipText.gameObject.activeSelf) EquipText.gameObject.SetActive(false);
        }
    }

    public void SetHotKeyMark()
    {
        if (isOnHotkey)
        {
            if (!HotkeyText.gameObject.activeSelf) HotkeyText.gameObject.SetActive(true);
        }
        else
        {
            if (HotkeyText.gameObject.activeSelf) HotkeyText.gameObject.SetActive(false);
        }
    }

    public override void Clear()
    {
        base.Clear();
        
        ItemPrefab itemPrefab=item.GetComponent<ItemPrefab>();
        itemPrefab.itemData =null;
        itemPrefab.Initialize();

        isEquipped = false;
        isOnHotkey = false;

        quickSlotNumber = -1;

        image.gameObject.SetActive(false);
        quantityText.text = string.Empty;
        EquipText.gameObject.SetActive(false);
        HotkeyText.gameObject.SetActive(false);
    }

    public override void OnSelect()
    {
        base.OnSelect();
    
        ItemPrefab itemPrefab = item.GetComponent<ItemPrefab>();

        if (itemPrefab.id == 0)
        {
            return;
        }

        if (itemInfoPopup.gameObject.activeSelf == false)
        {
            itemInfoPopup.gameObject.SetActive(true);
            itemInfoPopup.GetComponent<RectTransform>().anchoredPosition = itemInfoPopup.spawnPosition;
        }

        itemInfoPopup.item = item;
        itemInfoPopup.curItemSlotIndex = index;

        itemInfoPopup.CheckGetInven();
        itemInfoPopup.SetItemPrefab();
        itemInfoPopup.SetTexts();
        itemInfoPopup.SetButtons();
    }

}