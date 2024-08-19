using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public int index;
    public GameObject item;

    public Button button;
    public Image image;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI EquipText;
    public TextMeshProUGUI HotkeyText;

    protected InventoryPopup inventory;
    protected ItemInfoPopup itemInfoPopup;

    protected string inventoryPopupString = "InventoryPopup";
    protected string itemInfoPopupString = "ItemInfoPopup";


    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }

    protected virtual void Start()
    {
        inventory = ResourcesManager.instance.GetUIInDic(inventoryPopupString) as InventoryPopup;
        itemInfoPopup = ResourcesManager.instance.GetUIInDic(itemInfoPopupString) as ItemInfoPopup;
    }


    public virtual void Set()
    {   
    }

    public virtual void Clear()
    {
    }

    public virtual void OnSelect()
    {

    }

}