using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ShopUIPanel
{  
    public GameObject item; 
    public ItemPrefab shopItem;
    public Image image;
    public TextMeshProUGUI price;
    public Button button;
}

public class ShopController : MonoBehaviour
{
    public DataManager DM;
    public GameManager GM;
    public GameObject itemPrefab;
    public ItemPrefab potion;
    public List<ShopUIPanel> shopItemUI;
    public TextMeshProUGUI playerGold;
    private Coroutine updateCoroutine;
    private InventoryPopup inven;
    private string inventoryPopupString = "InventoryPopup";    

    public GameObject infoPanel;
    public TextMeshProUGUI infoTxt;
    public List<GameObject> infoObject=new List<GameObject>();    
    
    void Start()
    {
        DM=DataManager.instance;
        GM=GameManager.instance;
        inven = ResourcesManager.instance.GetUIInDic(inventoryPopupString) as InventoryPopup;

        for(int i=0;i<shopItemUI.Count;i++)
        {   
            int num=i;
            shopItemUI[i].button.onClick.AddListener(() => OnBuyItem(num));
            shopItemUI[i].item=Instantiate(itemPrefab);
            shopItemUI[i].shopItem= shopItemUI[i].item.GetComponent<ItemPrefab>(); 
            shopItemUI[i].item.transform.position=GM.itemPools.position;
        }        
        RandomItemDisplay();
        PotionDisplay();
        InfoPanel();
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

    public void RandomItemDisplay() //나중에 버튼으로 구현 하려나..
    {
        for(int i=0;i<shopItemUI.Count-1;i++)
        { 
            DM.EquipmentsRandomItem();
            DM.Init(shopItemUI[i].shopItem,DM.itemPrefab);
            DM.ClearItemData();
            shopItemUI[i].image.sprite=shopItemUI[i].shopItem.icon;
            shopItemUI[i].price.text=shopItemUI[i].shopItem.price.ToString();
        }
    }

    void PotionDisplay()
    {   
        DM.Init(shopItemUI[3].shopItem,potion); 
        shopItemUI[3].image.sprite=shopItemUI[3].shopItem.icon;
        shopItemUI[3].price.text=shopItemUI[3].shopItem.price.ToString();
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
    
    IEnumerator CoroutineUpdateShop()
    {
        while (true)
        {   
            yield return new WaitForSeconds(0.1f);
            UpdateShop();
            yield return new WaitForSeconds(0.2f); 
        }
    }

    public void OnBuyItem(int num)
    {
        if(num==3)
        {
            if(GM.isTutorial)
            {
                if(GM.tutoNum==6)
                {
                    GM.OnplayTutorial();
                    buyItemPotion(num);
                    PotionDisplay();
                }
            }else             
                buyItemPotion(num);
        }
        
        if(num!=3)
        {
            if(GM.isTutorial)
            {
                if(GM.tutoNum==5) 
                {
                    GM.OnplayTutorial();
                    buyItemEquip(num);                    
                }
            }else buyItemEquip(num);
        }
    
        UpdateShop();
    }

    void buyItemPotion(int num)
    {
        DM.gold-=shopItemUI[num].shopItem.price;
        inven.TryAddItem(shopItemUI[num].item);
        PotionDisplay();
    }

    void buyItemEquip(int num)
    {
        DM.gold-=shopItemUI[num].shopItem.price;
        inven.TryAddItem(shopItemUI[num].item);
        Soldout(num);
    }

    void Soldout(int num)
    {
        shopItemUI[num].item=null;
        shopItemUI[num].image.sprite=null;
        shopItemUI[num].price.text="";
    }    

    void InfoPanel()
    {
        for (int i = 0; i < infoObject.Count; i++)
        {
            int index = i; 
            EventTrigger trigger = infoObject[i].AddComponent<EventTrigger>();

            // 마우스 엔터 이벤트 추가
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnPointerEnter(index); });
            trigger.triggers.Add(entryEnter);

            // 마우스 나가기 이벤트 추가
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { OnPointerExit(); });
            trigger.triggers.Add(entryExit);
        }

        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(int index)
    {
        if(infoPanel != null)
        {            
            if (index >= 0 && index < infoObject.Count && shopItemUI[index].item !=null)
            {
                infoPanel.SetActive(true);
                infoTxt.text = Itemdata(index);
            }
        }
    }

    // 마우스가 이미지에서 나갔을 때 호출되는 메소드
    public void OnPointerExit()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    string Itemdata(int index)
    {
        string txt="";
        txt=$"{shopItemUI[index].shopItem.itemName}\n{shopItemUI[index].shopItem.itemDescription}";
        return txt;
    }
}
