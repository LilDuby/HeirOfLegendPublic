using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string playerTag;
    public GameObject player;
    public ObjectPool objectPool { get; private set; }
    public static GameManager instance;
    public CharacterStatsHandler characStatHandler;
    public DataManager DM;
    public CharacterManager CM;
    public int playerClassIndex = 0;
    public bool isPooled;
    public Transform enemyPools;
    public Transform itemPools;
    public Transform bulletPools;
    public ParticleSystem effectParticle;

    public int stage = 0;
    public int clearNum = 0;
    public int difficult = 0;
    public int mobSpawn;
    public int miscItemSpawn;
    public List<Difficulty> Difficulty = new List<Difficulty>();
    public RoomGenerator roomGenerator;
    public List<bool> NPCToggle = new List<bool>();

    private delegate void DifficultyAction();
    private Dictionary<int, DifficultyAction> difficultyActions;

    public CursorController cursorController;
    public bool isTutorial = false;
    public int tutoNum;

    public GameObject tutoPopup;
    public TutorialInfoPopup info;

    private int poolCount;

    [SerializeField] private InventoryPopup inventoryPopup;
    [SerializeField] private ItemInfoPopup itemInfoPopup;    
    private string invenPopupString = "InventoryPopup";
    private string itemInfoPopupString = "ItemInfoPopup";

     private void Awake()
     {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            inventoryPopup = ResourcesManager.instance.LoadUI<InventoryPopup>(UIs.Popup) as InventoryPopup;
            itemInfoPopup = ResourcesManager.instance.LoadUI<ItemInfoPopup>(UIs.Popup) as ItemInfoPopup;
            ResourcesManager.instance.AddUIInDic(invenPopupString, inventoryPopup);
            ResourcesManager.instance.AddUIInDic(itemInfoPopupString, itemInfoPopup);
            DontDestroyOnLoad(inventoryPopup.gameObject);
            DontDestroyOnLoad(itemInfoPopup.gameObject);

            inventoryPopup.gameObject.SetActive(true);
        }
        else
        {
            Destroy(gameObject);
        }
        isPooled = false;
        objectPool = GetComponent<ObjectPool>();
        effectParticle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>();
        cursorController = GetComponent<CursorController>();
        poolCount = objectPool.pools.Count;
    }
    private void Start()
    {
        DM = DataManager.instance;
        CM = CharacterManager.instance;        
        objectPool.Pooling();
        cursorController.SetCursor(0);
        SetDifficultyAction();
    }

    public void ClearPools()
    {
        if (objectPool.pools.Count > poolCount)
        {
            objectPool.pools.Remove(objectPool.pools[objectPool.pools.Count - 1]);
            if(objectPool.poolDictionary.ContainsKey("PlayerCloseAttack"))
                objectPool.poolDictionary.Remove("PlayerCloseAttack");
            else if (objectPool.poolDictionary.ContainsKey("PlayerBullet"))
                objectPool.poolDictionary.Remove("PlayerBullet");
        }
        ClearItemPools();
        for (int i = 0; i < enemyPools.childCount; i++)
        {
            enemyPools.GetChild(i).gameObject.SetActive(false);
            //CharacterStatsHandler enemyStat=enemyPools.GetChild(i).gameObject.GetComponent<CharacterStatsHandler>();
            //enemyStat.RemoveAllStatModifier();
        }
        for (int i = 0; i < bulletPools.childCount; i++)
        {
            bulletPools.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ClearItemPools()
    {
        for (int i = 0; i < itemPools.childCount; i++)
        {
            itemPools.GetChild(i).gameObject.SetActive(true);
            itemPools.GetChild(i).gameObject.GetComponent<ItemPrefab>().Initialize();
            itemPools.GetChild(i).gameObject.GetComponent<ItemPrefab>().upgradeCount = 0;
            itemPools.GetChild(i).gameObject.GetComponent<ItemPrefab>().quantity = 0;
            itemPools.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {   
        AudioManager.instance.BgmClipPlay();
        DM.CreateCharacterfinish();
        AllMoveItemPool();
        ClearPools();
        DifficultGame();
        SceneManager.LoadScene("LobbyScene");
        StartCoroutine(DelayStartGame());
    }

    IEnumerator DelayStartGame()
    {
        yield return new WaitForSeconds(0.1f);
        if (SceneManager.GetActiveScene().name != "TitleScene")
        {
            Instantiate(CM.player[DM.saveData.ClassNum]);
            StartCoroutine(UpdatePlayerUI());
            CM.MakePlayerPools();
            if (GameObject.FindGameObjectWithTag(playerTag) != null)
            {
                for (int i = 0; i < DM.saveData.StatModifiers.Count; i++)
                {
                    characStatHandler.AddStatModifier(DM.saveData.StatModifiers[i]);
                }
                itemInfoPopup.CheckGetInven();
                for(int i=0;i<inventoryPopup.invenSlots.Length;i++)
                {   
                    InvenSlot slot=inventoryPopup.invenSlots[i].GetComponent<InvenSlot>();
                    if (slot.isEquipped)
                    {
                        itemInfoPopup.EquipInvenItem(slot.index);
                    }
                }
            }
        }
        if(SceneManager.GetActiveScene().name == "LobbyScene") 
        {
            UIManager.instance.ShowUI<KeyGuidePopup>(UIs.Popup);
        }
        if(SceneManager.GetActiveScene().name == "DungeonScene") // 던전 씬일때 임의 스폰위치 받아오는 구문
        {
            roomGenerator.testplayer();
            CM.SetSpawnPositions();
            if(isTutorial)
            {
                DM.gold=1000; 
                OnTutorial(); //TutorialPopup load
                info.ChengChapter(2); //Tutorial Chapter 2
                OnplayTutorial(); //TextLoad
            }
        }
    }

    IEnumerator UpdatePlayerUI()
    {
        yield return new WaitForSeconds(0.1f);
        CM.UpdateHealthUI();
        CM.UpdateStaminaUI();
    }

    public void GoToTitleScene()
    {
        if (CM.gameOverUI.activeSelf || CM.gameClearUI.activeSelf)
        {
            CM.gameOverUI.SetActive(false);
            CM.gameClearUI.SetActive(false);
        }
        ClearPools();
        SceneManager.LoadScene("TitleScene");        
        Time.timeScale = 1f;
        StartCoroutine(DelayStartGame());
        ResourcesManager.instance.ClearDic();
        ResourcesManager.instance.AddUIInDic(invenPopupString, inventoryPopup);
        ResourcesManager.instance.AddUIInDic(itemInfoPopupString, itemInfoPopup);

        AudioManager.instance.TitleClipPlay();
    }

    public void GoToLobbyScene()
    {
        ClearPools();
        SceneManager.LoadScene("LobbyScene");
        StartCoroutine(DelayStartGame());
    }

    public void GoToDungeonScene()
    {
        ClearPools();
        SceneManager.LoadScene("DungeonScene");
        StartCoroutine(DelayStartGame());
    }

    public void ClearItemPool()
    {
        objectPool.OffAllObjects("Item");
    }

    public void ItemPrefabPool(ItemPrefab _itemPrefab,Vector2 pos)
    {   
        objectPool.SpawnFromItemPool("Item",_itemPrefab,pos);
    }

    public void AllMoveItemPool()
    {
       objectPool.ItemAllMoveInventory("Item");
    }

    public void DamageTxtPool()
    {           
        objectPool.SpawnFromPool("DamageTxt");
    }

    void SetDifficultyAction()
    {
        difficultyActions = new Dictionary<int, DifficultyAction>
        {
            { 0, () => ResetDifficulty() },
            { 1, () => mobSpawn += 3 },
            { 2, () => EnemysAddStat2Difficult(true) },
            { 3, () => miscItemSpawn -= 2 },
            { 4, () => EnemysAddStat2Difficult(true) },
            { 5, () => AddBossSpawn()},
            { 6, () => EnemysAddStat2Difficult(true)},
            { 7, () => miscItemSpawn -= 2},
            { 8, () => AddBossSpawn()}
        };
    }

    public void AddBossSpawn()
    {   
    }

    public void DifficultGame()
    {
        if (difficult == 0)
            difficultyActions[0]();
        else
        {
            for (int i = 1; i <= difficult; i++)
            {
                if (difficultyActions.ContainsKey(i))
                {
                    difficultyActions[i]();
                }
            }
        }        
    }

    void EnemysAddStat2Difficult(bool isAdd) //true=Add false=Clear
    {  
        for (int i = 0; i < enemyPools.childCount; i++)
        {
            GameObject obj = enemyPools.GetChild(i).gameObject;
            AddStat2Difficult(isAdd,obj);
        }

        // foreach (GameObject obj in objectPool.poolDictionary["CloseEnemys"]) //개별 추가적용 할때
        // {   
        //     statHandler = obj.GetComponent<CharacterStatsHandler>();
        //     if(typeNum==1)
        //         statHandler.AddStatModifier(stat);
        //     else 
        //         statHandler.RemoveAllStatModifier();
        // }
    }
    public void AddStat2Difficult(bool isAdd,GameObject obj)
    {
        CharacterStat stat = new CharacterStat
        {
            maxHealth = 20,
            defense = 5,
            power = 10
        };

        CharacterStatsHandler statHandler=obj.GetComponent<CharacterStatsHandler>();
        if(isAdd)
            statHandler.AddStatModifier(stat);
        
        else //(!isAdd)
            statHandler.RemoveAllStatModifier();
    }

    public void ResetDifficulty()
    {
        mobSpawn = 0;
        miscItemSpawn = 0;
        EnemysAddStat2Difficult(false); //Clear Enemys statHandler
    }

    public void GetLegacyPoint(int typeNum)
    {
        if(typeNum==0) //Enemys Die
        {
            if(CM.enemyCount==0)
            DM.saveData.Legacy+= 5+(difficult*1);
        }
        else if(typeNum==1) //Boss Die
            DM.saveData.Legacy+= 20+(difficult*5);
        else if(typeNum==2) //Game Clear!
            DM.saveData.Legacy+= 50+(difficult*20);
    }

    public void OnTutorial()
    {
        UIManager.instance.ShowUI<TutorialInfoPopup>(UIs.Popup);
        tutoPopup=UIManager.instance.FindUI<TutorialInfoPopup>(UIs.Popup);
        info=tutoPopup.GetComponent<TutorialInfoPopup>();
    }

    public void OnplayTutorial()
    {
        if(isTutorial)
        {
            tutoNum++;
            info.Initiailzer();
        }
    }

    public bool TutoPlayLockCheck(int _NpcNum)
    {
        if(isTutorial)
        {
            if(_NpcNum==0 && tutoNum>=4 && tutoNum<=6)
            {            
                if(tutoNum==4)
                    OnplayTutorial(); 
                
                return false; //ok
            }            
            else if(_NpcNum==1 && tutoNum>=7 && instance.tutoNum<=9)
            {
                if (tutoNum==7)
                    OnplayTutorial();
                
                return false; //ok
            }
            else             
                return true; //Lock[Tutorial]
        }
        else
            return false; //ok
    }
}