using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ForgeUIPanel
{      
    public Image image;
    public Image metalImage;
    public TextMeshProUGUI need4MetalTxt;
    public TextMeshProUGUI itemUpgradeCountTxt;
    public Button button;
}

public class ForgeController : MonoBehaviour
{
    public DataManager DM;
    public GameManager GM;
    public InventoryPopup inven;
    public ItemInfoPopup itemInfoPopup;
    private Coroutine updateCoroutine;    
    int needMetalNum;
    bool canUpgrade;

    public List<ShopUIPanel> shopItemUI;
    public List<ForgeUIPanel> forgeItemUI;    
    public TextMeshProUGUI playerGold;     
    
    public GameObject shopitem;
    public ItemPrefab[] shopMetal=new ItemPrefab[2];
    ItemPrefab forgeitem;
    ItemPrefab myHaveMetal;

    private string invenPopupString = "InventoryPopup";
    private string itemInfoPopupString = "ItemInfoPopup";

    void Start()
    {
        DM=DataManager.instance;
        GM=GameManager.instance;
        inven = ResourcesManager.instance.GetUIInDic(invenPopupString) as InventoryPopup;

        forgeItemUI[0].button.onClick.AddListener(() => OnUpgradeItem(0));

        for(int i=0;i<shopItemUI.Count;i++)
        {   
            int num=i;
            shopItemUI[i].button.onClick.AddListener(() => OnBuyItem(num));
            shopItemUI[i].item=Instantiate(shopitem);
            shopItemUI[i].shopItem= shopItemUI[i].item.GetComponent<ItemPrefab>(); 
            shopItemUI[i].item.transform.position=GM.itemPools.position;            
        }

        MetalPisplay();        
    }
    void OnEnable()
    {
        updateCoroutine = StartCoroutine(CoroutineUpdateShop());
    }

    void OnDisable()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
    } 

    void UpdateShop()
    {
        playerGold.text=DM.gold.ToString();

        for(int i=0;i<shopItemUI.Count;i++)
        {               
            if(shopItemUI[i].shopItem.price<=DM.gold)
                shopItemUI[i].button.interactable=true;
            else
                shopItemUI[i].button.interactable=false;
        }
    }

    void UpdataForge()
    {
        itemInfoPopup = ResourcesManager.instance.GetUIInDic(itemInfoPopupString) as ItemInfoPopup;

        if (itemInfoPopup.gameObject.activeSelf)
        {
            forgeitem = itemInfoPopup.item.GetComponent<ItemPrefab>();
            ForgeDisplay();
        }

        else if(!itemInfoPopup.gameObject.activeSelf)
        {
            forgeitem=null;
            forgeItemUI[0].need4MetalTxt.text="0";
            forgeItemUI[0].itemUpgradeCountTxt.text="0";
            forgeItemUI[0].metalImage.sprite=null;
            forgeItemUI[0].image.sprite=null;
            return;
        }

        if(canUpgrade)
            forgeItemUI[0].button.interactable=true;
        else
            forgeItemUI[0].button.interactable=false;
    }

    void ForgeDisplay()
    {           
        if(itemInfoPopup.currentItemSlot.isEquipped) return;
        if(forgeitem.id>=10 && forgeitem.id<40)
        {   
            needMetalNum=forgeitem.upgradeCount*5+5;
            
            forgeItemUI[0].need4MetalTxt.text=needMetalNum.ToString();
            forgeItemUI[0].itemUpgradeCountTxt.text=forgeitem.upgradeCount.ToString();
            forgeItemUI[0].image.sprite=forgeitem.icon;

            if(forgeitem.id>=10 && forgeitem.id<=30) //방어구
            {
                forgeItemUI[0].metalImage.sprite=shopMetal[0].icon;
                CanUpgradeCheck(shopMetal[0]);
            }
            else 
            {   
                forgeItemUI[0].metalImage.sprite=shopMetal[1].icon;
                CanUpgradeCheck(shopMetal[1]);
            }
        }
    }
    
    IEnumerator CoroutineUpdateShop()
    {
        while (true)
        {            
            yield return new WaitForSeconds(0.1f);
            UpdateShop();
            UpdataForge();
            yield return new WaitForSeconds(0.2f); // 1초마다 업데이트, 필요에 따라 조절 가능
        }
    }
    
    public void OnUpgradeItem(int num)
    {   
        if(GM.tutoNum==8)        
            GM.OnplayTutorial();
                
        forgeitem.upgradeCount++;
        forgeitem.Initialize();
        myHaveMetal.quantity-=needMetalNum;
        inven.UpdateInvenSlots();
        itemInfoPopup.UpdateWindow();

        UpdataForge();
    }

    public void OnBuyItem(int num)
    {   
        DM.gold-=shopItemUI[num].shopItem.price;
        inven.TryAddItem(shopItemUI[num].item);
        MetalPisplay();
        UpdateShop();
    }

    void MetalPisplay() //ItemData load
    {   
        for(int i=0;i<shopItemUI.Count;i++)
        {
            DM.Init(shopItemUI[i].shopItem,shopMetal[i]);
            shopItemUI[i].image.sprite=shopItemUI[i].shopItem.icon;
            shopItemUI[i].price.text=shopItemUI[i].shopItem.price.ToString();
        }
    }

    void CanUpgradeCheck(ItemPrefab matelItem) //check Have matelItem
    {
        for(int i=0;i<inven.invenSlots.Length;i++)
        {
            ItemPrefab findItem=inven.invenSlots[i].item.GetComponent<ItemPrefab>();            
            if(findItem.id==matelItem.id) //check matelItem
            {                   
                if(findItem.quantity>=needMetalNum) //Have matelItem >= need matelItem
                {
                    canUpgrade=true;
                    myHaveMetal=findItem;
                }
                else //Have matelItem < need matelItem
                {
                    canUpgrade=false;
                    myHaveMetal=null;
                }
                return;
            }
            else //no have matelItem
            {
                canUpgrade=false;
                myHaveMetal=null;
            }
        }
    } 
}
