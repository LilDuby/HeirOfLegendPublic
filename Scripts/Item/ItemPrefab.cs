using TMPro;
using UnityEngine;

public class ItemPrefab : MonoBehaviour
{   
    public ItemData itemData;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public PlayerController playerController;

    public const float YOffSet = 0.4f;

    private TextMeshProUGUI nameTextInstance;

    [Header("Info")]
    public int id;
    public string itemName;
    public string itemDescription;
    public Sprite icon;
    public ItemType itemType;

    [Header("Stacking")]
    public int quantity= 1;
    public int maxStackAmount;

    [Header("Trading")]
    public bool tradable = true;
    public int price;

    [Header("Stat")]
    public CharacterStat characterStat;
    public int upgradeCount=0;
    public int MaxUpgradeCount;
    
    [Header("Consumable")]
    public float healAmount;

    private InventoryPopup inventoryPopup;
    private string inventoryPopupString = "InventoryPopup";


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        inventoryPopup = ResourcesManager.instance.GetUIInDic(inventoryPopupString) as InventoryPopup;
    }

    void Start()
    {   
        Initialize();
        boxCollider.size = new Vector2(0.5f, 0.5f);
    }

    private void OnEnable()
    {
        if (GameManager.instance.player != null) playerController = GameManager.instance.player.GetComponent<PlayerController>();
        Initialize();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = icon;
        }
    }

    public void Initialize()
    {        
        if (itemData != null)
        {
            id = itemData.id;
            itemName = itemData.itemName;
            itemDescription = itemData.itemDescription;
            icon = itemData.icon;
            itemType = itemData.itemType;
            if (quantity == 0) quantity = 1;
            maxStackAmount = itemData.maxStackAmount;
            tradable = itemData.tradable;
            price = itemData.price;

            if(itemData is EquipmentItemData Equip)
            {
                characterStat=new CharacterStat(Equip.characterStat);
                MaxUpgradeCount=Equip.MaxUpgradeCount;
                if(itemData.itemType==ItemType.Equipment_Armer_Top || itemData.itemType==ItemType.Equipment_Armer_Bottom )
                {
                    characterStat.maxHealth=(upgradeCount+1)*Equip.characterStat.maxHealth;
                    characterStat.defense=(upgradeCount+1)*Equip.characterStat.defense;
                }
                else if(itemData.itemType==ItemType.Equipment_Weapon_Sword || itemData.itemType==ItemType.Equipment_Weapon_Bow)
                {
                    characterStat.power=(upgradeCount+1)*Equip.characterStat.power;                    
                    if(Equip.characterStat.attackSO!=null)characterStat.attackSO=Equip.characterStat.attackSO;
                }
            }
        }
        else
        {
            id =0;
            itemName ="";
            itemDescription ="";
            icon =null;
            itemType = ItemType.Consumable_NoUse;
            quantity = 0;
            maxStackAmount = 0;
            tradable = true;
            price = 0;
            characterStat=null;
            upgradeCount=0;
            MaxUpgradeCount=0;
            healAmount=0;
        }
    }

    public void ResetSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("ItemCircleCollider")) return;
        if (TextNameObjectPool.instance == null) return;
        if (nameTextInstance != null) return;

        switch (itemType)
        {
            case ItemType.Consumable_CanUse:
            case ItemType.Consumable_NoUse:
                if (inventoryPopup.TryAddItem(gameObject)) return;
                break;
        }

        playerController.nearbyItems.Add(gameObject);
        nameTextInstance = TextNameObjectPool.instance.GetTextObject();
        if (itemData != null)
        {
            nameTextInstance.text = itemData.itemName;
        }
        else
        {
            ItemPrefab finditem = GetComponent<ItemPrefab>();
            nameTextInstance.text = finditem.itemName;
        }
            
        nameTextInstance.transform.position = transform.position + new Vector3(0, YOffSet, 0);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("ItemCircleCollider")) return;
        if (TextNameObjectPool.instance == null) return;
        if (nameTextInstance == null) return;

        if (playerController.nearbyItems.Contains(gameObject))
        {
            playerController.nearbyItems.Remove(gameObject);
        }
        TextNameObjectPool.instance.ReturnTextObject(nameTextInstance);
        nameTextInstance = null;
    }

    public void ScaleSprite()
    {
        Bounds parentBounds = boxCollider.bounds;
        Bounds spriteBounds = spriteRenderer.bounds;

        float scale = Mathf.Min(parentBounds.size.x , spriteBounds.size.x);

        spriteRenderer.transform.localScale = Vector3.one * scale;

        spriteRenderer.transform.position = boxCollider.transform.position;
    }
}
