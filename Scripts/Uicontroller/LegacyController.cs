using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LegacyController : MonoBehaviour
{        
    public DataManager DM;     
    public TextMeshProUGUI maxHpTxt;
    public TextMeshProUGUI atkTxt;
    public TextMeshProUGUI defTxt;    
    public TextMeshProUGUI goldTxt;
    public TextMeshProUGUI legacyTxt;    
    public List<LegacyPoint> legacylist=new List<LegacyPoint>();    
    public ItemPrefab finditem;
    public Image itemImg;
    ShowInfoOnHover showInfoOnHover;
    bool isUp=true;
    void Start()
    {
        showInfoOnHover=gameObject.GetComponent<ShowInfoOnHover>();        
    }
    
    void OnEnable()
    {   
        DM=DataManager.instance;
        DM.playerlegacy=DM.saveData.Legacy;
        UpdataLog();
    }

    public void UpdataLog()
    {   
        legacyTxt.text = DM.playerlegacy.ToString();
        maxHpTxt.text = DM.stat.maxHealth.ToString();
        atkTxt.text = DM.stat.power.ToString();
        defTxt.text = DM.stat.defense.ToString();
        goldTxt.text = DM.gold.ToString();
    }

    public void OnUpDownToggle(bool isUp)
    {
        this.isUp=isUp;
    }

    public void OnUpDownBtn(int num)
    {   
        LegacyPoint point=legacylist[num];
        if(isUp) //(!isUp) 
        {   
            if(DM.playerlegacy>=point.payCost)
            {   
                UpDownMath(point,1);
            }
        }
        else //(!isDown) 
        {   
            if(DM.playerlegacy<DM.saveData.Legacy)
            { 
                UpDownMath(point,-1);
            }
        }
        UpdataLog();
    }

    void UpDownMath(LegacyPoint point,int UPDown)
    {
        int valueChange = point.Value * UPDown;
        bool isCheck = ApplyStatChange(point.statsType, valueChange);

        if (isCheck)
        {
            DM.playerlegacy -= point.payCost * UPDown;
        }
    }
    private bool ApplyStatChange(StatsType statType, int valueChange)
    {        
        switch (statType)
        {
            case StatsType.MaxHp:
                return ApplyChange(ref DM.stat.maxHealth, valueChange);
            case StatsType.Atk:
                return ApplyChange(ref DM.stat.power, valueChange);
            case StatsType.Def:
                return ApplyChange(ref DM.stat.defense, valueChange);
            case StatsType.Gold:
                return ApplyChange(ref DM.gold, valueChange);
            default:
                return false;
        }
    }

    private bool ApplyChange(ref int statValue, int valueChange)
    {        
        int newValue = statValue + valueChange;
        
        if (newValue < 0) 
        {
            return false;
        }

        else
        {
            statValue = newValue;
            return true;
        }        
    }

    public void OnResetItem()
    {        
        DM.EquipmentsRandomItem();
        UpdataLog();
        ShowItem();
    }

    void ShowItem()
    {           
        finditem = DM.itemPrefab.GetComponent<ItemPrefab>();
        finditem.Initialize();
        itemImg.sprite=finditem.icon;        
        showInfoOnHover.itemInfo=Itemdata();
    }

    string Itemdata()
    {
        string txt="";
        txt=$"{finditem.itemName}\n{finditem.itemDescription}";
        return txt;
    }

}



