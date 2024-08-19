public class EquipSlot : ItemSlot
{
    public InvenSlot curItemSlot;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Set()
    {
        base.Set();

        ItemPrefab itemPrefab = item.GetComponent<ItemPrefab>();

        if (!image.gameObject.activeSelf) image.gameObject.SetActive(true);
        image.sprite = itemPrefab.icon;
    }

    public override void Clear()
    {
        base.Clear();
        item = null;
        image.gameObject.SetActive(false);    
    }

    public override void OnSelect()
    {
    }

}