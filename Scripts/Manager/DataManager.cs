using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class GameData
{
    public bool character=false;
    public int Legacy;
    public int ClassNum;
    public int Difficulty;
    public int Gold;
    public int[] Pos=new int[2];//플래이어 위치값 [몇층에 몇번방 같은 정보]
    public List<CharacterStat> StatModifiers = new List<CharacterStat>();
    public List<InventoryData> inventory = new List<InventoryData>();    
    public string saveTime;
}
[Serializable]
public class InventoryData
{
    public int id;
    public int num;
    public bool isEquipped;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public GameManager GM;
    public CharacterManager CM;
    public string[] path=new string[3]{"","",""};
    public GameData saveData;
    public AttackSO attackSO;
    public List<ClassData>classList=new List<ClassData>();
    public List<ItemPrefab> consumablelist=new List<ItemPrefab>();
    public List<ItemPrefab> equipmentslist=new List<ItemPrefab>();    

    int random;
    public ItemPrefab itemPrefab;
    public int SaveDataNum;
    public InventoryPopup inventoryPopup;

    public CharacterStat stat;    
    public int playerlegacy;    
    public int gold;

    public CharacterStat top;
    public CharacterStat bottom;
    public CharacterStat weapon;

    private string inventoryPopupString = "InventoryPopup";

     private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }        
    } 
    void Start()
    {
        GM=GameManager.instance;
        CM=CharacterManager.instance;

        path[0]=Path.Combine(Application.persistentDataPath,"gameData0.json");
        // Debug.Log(Application.persistentDataPath);
        path[1]=Path.Combine(Application.persistentDataPath,"gameData1.json");
        path[2]=Path.Combine(Application.persistentDataPath,"gameData2.json");

        inventoryPopup = ResourcesManager.instance.GetUIInDic(inventoryPopupString) as InventoryPopup;

        for(int i=0;i<equipmentslist.Count;i++)
        {  
            equipmentslist[i].Initialize();
        }
        for(int i=0;i<consumablelist.Count;i++)
        {   
            consumablelist[i].Initialize();
        }        
    } 

    public void SaveData(GameData target,int _num)
    {
        var t = JsonUtility.ToJson(target);        
        File.WriteAllText(path[_num],t);
    } 

    public void SaveGame(int pathNum)
    {           
        saveData.Gold=gold;
        saveData.Difficulty=GM.difficult;
        saveData.ClassNum=GM.playerClassIndex;
        saveData.Pos[0]=0; //floor
        saveData.Pos[1]=GM.stage; //room
        saveData.saveTime=DateTime.Now.ToString(("yyyy-MM-dd HH:mm"));

        if(GM.player!=null)
        {           
            for(int i=0;i<inventoryPopup.invenSlots.Length;i++)
            {   
                ItemPrefab findItem=inventoryPopup.invenSlots[i].item.GetComponent<ItemPrefab>();
                InventoryData newItemData = new InventoryData
                {
                    id = findItem.id
                };

                if(newItemData.id==0) continue;
                
                if (findItem.id >= 10 && findItem.id <= 40)
                {                    
                    InvenSlot slot=inventoryPopup.invenSlots[i].GetComponent<InvenSlot>();
                    newItemData.num=findItem.upgradeCount;
                    newItemData.isEquipped=slot.isEquipped;
                }
                else
                {
                    newItemData.num=findItem.quantity;
                    newItemData.isEquipped=false;
                }
                    
                saveData.inventory.Add(newItemData);
            }
        }
        instance.SaveData(saveData,pathNum);
    }

    public void CreateCharacterfinish()
    {           
        OnAddLegacyStatModifiers();
        GM.ItemPrefabPool(itemPrefab,GM.itemPools.position);
        gold += saveData.Gold;
        saveData.character=true;
        SaveGame(SaveDataNum);
    }

    bool RandomPercent(int num)
    {
        int percent=UnityEngine.Random.Range(0,10);
        if(percent<=num)
        {           
            return true;
        }
        else 
        {
            return false;
        }
    }

    public ItemPrefab EquipmentsRandomItem()
    {   
        random=UnityEngine.Random.Range(0, equipmentslist.Count);
        Init(itemPrefab,equipmentslist[random]);
        return itemPrefab;
    }

    public ItemPrefab MiscRandomItem(int percent)
    {   
        if(RandomPercent(percent+GM.miscItemSpawn))
        {
            random=UnityEngine.Random.Range(0, consumablelist.Count);
            Init(itemPrefab,consumablelist[random]);
            return itemPrefab;
        }
        else return null;
    }

    public void EquipItem(ItemPrefab _itemData)
    {
        if(_itemData==null) return;
        
        
        if(_itemData.itemType==ItemType.Equipment_Armer_Bottom)
        {
            bottom = _itemData.characterStat;
            if(GM.characStatHandler!=null)
                GM.characStatHandler.AddStatModifier(bottom);
            return;
        }
        else if(_itemData.itemType==ItemType.Equipment_Armer_Top)
        {            
            top = _itemData.characterStat;
            if(GM.characStatHandler!=null)
                GM.characStatHandler.AddStatModifier(top);
            return;            
        }
        else if(_itemData.itemType==ItemType.Equipment_Weapon_Sword ||_itemData.itemType==ItemType.Equipment_Weapon_Bow )
        {               
            weapon = _itemData.characterStat;
            if(GM.characStatHandler!=null)
                GM.characStatHandler.AddStatModifier(weapon);
            return;
        }
    }

    public void UnEquipItem(ItemPrefab _itemData)
    {        
        if (_itemData.itemType == ItemType.Equipment_Armer_Bottom)
        {
            GM.characStatHandler.RemoveStatModifier(bottom);
            bottom = null;
            return;
        }
        else if (_itemData.itemType == ItemType.Equipment_Armer_Top)
        {
            GM.characStatHandler.RemoveStatModifier(top);
            top = null;
            return;
        }
        else if (_itemData.itemType == ItemType.Equipment_Weapon_Sword ||_itemData.itemType==ItemType.Equipment_Weapon_Bow )
        {
            GM.characStatHandler.RemoveStatModifier(weapon);
            weapon = null;
            return;
        }               
    }

    public void LoadItem()
    {           
        for(int i=0;i<saveData.inventory.Count;i++)
        {
            InventoryData saveItem= saveData.inventory[i];
            if (saveItem.id >= 10 && saveItem.id <= 40)
            {   
                foreach (ItemPrefab listitem in equipmentslist)
                {   
                    if(listitem.id==saveItem.id)
                    {                           
                        Init(itemPrefab,listitem);
                        itemPrefab.upgradeCount=saveItem.num;
                        GM.ItemPrefabPool(itemPrefab,GM.itemPools.position);
                    }
                }
            }
            else
            {
                foreach (ItemPrefab listitem in consumablelist)
                {  
                    if(listitem.id==saveItem.id)
                    {   
                        Init(itemPrefab,listitem);
                        itemPrefab.quantity=saveItem.num;
                        GM.ItemPrefabPool(itemPrefab,GM.itemPools.position);
                    }
                }
            }            

            ClearItemData(); //한개의 아이템데이터 불러왓으면 청소
        }
        ClearItemData(); //로드 끝나고 마지막 으로 불러온 아이템데이터 청소
    }   

    public void ClearItemData()
    {
        itemPrefab.itemData=null; 
        itemPrefab.Initialize();
    }
    
    public void LoadCharacter()
    {   
        stat.maxHealth=classList[saveData.ClassNum].classMaxHp;
        stat.power=classList[saveData.ClassNum].classAtk;
        stat.defense=classList[saveData.ClassNum].classDef;
        stat.speed=classList[saveData.ClassNum].classSpeed;        
    }
    
    public void LoadStatModifiers()
    {        
        for(int i = 0;i<saveData.StatModifiers.Count;i++)
        {
            stat.maxHealth+=saveData.StatModifiers[i].maxHealth;
            stat.power+=saveData.StatModifiers[i].power;
            stat.defense+=saveData.StatModifiers[i].defense;
            stat.speed+=saveData.StatModifiers[i].speed;
        }
    }

    public void LoadSaveData()
    {
        GM.playerClassIndex=saveData.ClassNum;
        gold=saveData.Gold;
        saveData.Gold=0;
        playerlegacy=saveData.Legacy;
        GM.difficult=saveData.Difficulty;
        GM.stage=saveData.Pos[1];
    }

    public void ResetData()
    {   
        stat=new CharacterStat();
        gold=0;
        playerlegacy=0;
        GM.difficult=0;
        GM.stage=0;
        top=null;
        bottom=null;
        weapon=null;
        GM.ClearItemPool();
        if(GM.characStatHandler!=null)
        GM.characStatHandler.RemoveAllStatModifier();
    }    

    public void OnAddLegacyStatModifiers()
    {
        CharacterStat characStat = new CharacterStat
        {
            maxHealth = 0,
            defense = 0,
            speed = 0,
            power = 0
        };

        if(!saveData.character && saveData.Legacy!=playerlegacy)
        {            
            characStat.maxHealth = stat.maxHealth;
            characStat.defense = stat.defense;
            characStat.speed = stat.speed;
            characStat.power = stat.power;

            saveData.StatModifiers.Add(characStat);
        }
        else if(!saveData.character)
            saveData.StatModifiers.Add(characStat);
    }

    public void InGameSaveData()
    {   
        CharacterStatsHandler statsHandler= GM.player.GetComponent<CharacterStatsHandler>();
        saveData.StatModifiers.Clear();
        if(statsHandler.statModifiers.Count>0)
            saveData.StatModifiers.Add(statsHandler.statModifiers[0]); //LegacyStat
        saveData.inventory.Clear();
    }
    
    public void Init(ItemPrefab itemobj, ItemPrefab newItemPrefab)
    {
        itemobj.itemData = newItemPrefab.itemData;
        itemobj.upgradeCount = newItemPrefab.upgradeCount;
        itemobj.quantity = newItemPrefab.quantity;
        itemobj.Initialize();
    }

    public void SaveFileDelet()
    {
        if(File.Exists(path[SaveDataNum]))
        {   
            File.Delete(path[SaveDataNum]);
            saveData.inventory.Clear();
        }
    }
    public void PlayerDataClear() //player reset || die
    {
        SaveFileDelet();
        
        int getPlayedLegacy = saveData.Legacy;
        StartNewGame(); //DM.playerData clear
        saveData.Legacy=getPlayedLegacy;
        saveData.character=false;
        gold=saveData.Gold;
        SaveGame(SaveDataNum);//new saveFile
    }

    public void OnDeletSaveData() //세이브 데이터 삭제
    {        
        File.Delete(path[SaveDataNum]);
    }   

    public void StartNewGame()
    {
        saveData = new GameData
        {           
            Difficulty=0,
            Legacy=50,
            ClassNum=0,
            Gold=50,
            StatModifiers=new List<CharacterStat>(),
            inventory=new List<InventoryData>()
        }; //new saveData
    }

    public void OnCreateCharacter(bool ok)
    {   
        saveData.character= ok? true:false;
    }
}