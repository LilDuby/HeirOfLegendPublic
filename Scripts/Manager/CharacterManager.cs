using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    public HealthSystem playerHealthSystem;

    public HealthSystem finalBossHealthSystem;
    public HealthSystem bossHealthSystem1;
    public HealthSystem bossHealthSystem2;
    public HealthSystem bossHealthSystem3;

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
        inSafeZone = false;
    }

    public ObjectPool pool;   
    public GameObject[] player;
    public List<GameObject> BossPrefebs = new List<GameObject>();    
    public GameObject[] playerAttackPrefab;
    public Transform playerAttackSpawnPoolTransform;
    public Transform spawnPositionsRoot;
    private List<Transform> spawnPositions = new List<Transform>();
    public int enemyCount=0;
    public bool inSafeZone;    
    public Slider hpSlider;
    public Slider staminaSlider;

    public Slider finalBossHpSlider;
    public Slider bossHpSlider1;
    public Slider bossHpSlider2;
    public Slider bossHpSlider3;

    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public TextMeshProUGUI hpTxt;
    public TextMeshProUGUI staminaTxt;

    public void MakePlayerPools()
    {
        if (DataManager.instance.saveData.ClassNum == 0)
        {
            pool.pools.Add(new ObjectPool.Pool("PlayerCloseAttack", playerAttackPrefab[0], 8, playerAttackSpawnPoolTransform));
            GameManager.instance.cursorController.SetCursor(2);
        }
        else if(DataManager.instance.saveData.ClassNum == 1)
        {
            pool.pools.Add(new ObjectPool.Pool("PlayerBullet", playerAttackPrefab[1], 25, playerAttackSpawnPoolTransform));
            GameManager.instance.cursorController.SetCursor(3);
        }
        pool.PoolingPlayer();
    }    

    public void AddActionEvent()
    {
        playerHealthSystem.OnDamage += UpdateHealthUI;
        playerHealthSystem.OnHeal += UpdateHealthUI;
        playerHealthSystem.OnDeath += EndGameCheck;
    }

    public void AddBossActionEvent()
    {
        if(finalBossHealthSystem != null)
        {
            finalBossHealthSystem.OnDamage += UpdateBossHealthUI;
            finalBossHealthSystem.OnHeal += UpdateBossHealthUI;
        }
        if (bossHealthSystem1 != null)
        {
            bossHealthSystem1.OnDamage += UpdateBossHealthUI;
            bossHealthSystem1.OnHeal += UpdateBossHealthUI;
        }
        if(bossHealthSystem2 != null)
        {
            bossHealthSystem2.OnDamage += UpdateBossHealthUI;
            bossHealthSystem2.OnHeal += UpdateBossHealthUI;
        }
        if (bossHealthSystem3 != null)
        {
            bossHealthSystem3.OnDamage += UpdateBossHealthUI;
            bossHealthSystem3.OnHeal += UpdateBossHealthUI;
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        enemyCount =0;
        GameManager.instance.stage = 0;
        GameManager.instance.clearNum = 0;
        GameManager.instance.tutoNum=0;        
        GameManager.instance.isTutorial=false;
        GameManager.instance.ResetDifficulty();
        ClearSpawnPositions();
        string invenString = "InventoryPopup";
        InventoryPopup inven = ResourcesManager.instance.GetUIInDic(invenString) as InventoryPopup;
        for(int i = 0; i < inven.invenSlots.Length; i++)
        {
            InvenSlot slot=inven.invenSlots[i].GetComponent<InvenSlot>();
            slot.Clear();
        } 
        inven.UpdateUI();
        AudioManager.instance.GameOverClipPlay();
    }

    public void EndGameCheck()
    {
        if(playerHealthSystem.curHealth<=0) //player die
        {            
            if(!gameClearUI.activeSelf)
            {
                gameOverUI.SetActive(true);
                GameOver();
            }
        }
        else 
        {            
            GameManager.instance.GetLegacyPoint(2);            
            gameClearUI.SetActive(true);
            GameOver();
            DataManager.instance.PlayerDataClear();
        }
    }

    public void UpdateHealthUI()
    {
        if (playerHealthSystem.curHealth >= playerHealthSystem.maxHealth) playerHealthSystem.curHealth = playerHealthSystem.maxHealth;
        hpSlider.value = playerHealthSystem.curHealth / playerHealthSystem.maxHealth;
        hpTxt.text = $"{playerHealthSystem.curHealth} / {playerHealthSystem.maxHealth}";
    }

    public void UpdateStaminaUI()
    {
        if (playerHealthSystem.curStamina >= playerHealthSystem.maxStamina) playerHealthSystem.curStamina = playerHealthSystem.maxStamina;
        staminaSlider.value = playerHealthSystem.curStamina / playerHealthSystem.maxStamina;
        staminaTxt.text = $"{playerHealthSystem.curStamina} / {playerHealthSystem.maxStamina}";
    }

    private void UpdateBossHealthUI()
    {
        if(finalBossHpSlider != null) finalBossHpSlider.value = finalBossHealthSystem.curHealth / finalBossHealthSystem.maxHealth;
        if(bossHpSlider1 != null) bossHpSlider1.value = bossHealthSystem1.curHealth / bossHealthSystem1.maxHealth;
        if(bossHpSlider2 != null) bossHpSlider2.value = bossHealthSystem2.curHealth / bossHealthSystem2.maxHealth;
        if(bossHpSlider3 != null) bossHpSlider3.value = bossHealthSystem3.curHealth / bossHealthSystem3.maxHealth;
    }

    public void SetSpawnPositions()
    {
        spawnPositionsRoot=GameObject.Find("SpawnPoint").transform;
        for (int i = 0; i < spawnPositionsRoot.childCount; i++)
        {
            spawnPositions.Add(spawnPositionsRoot.GetChild(i));
        }
    }
    public void ClearSpawnPositions()
    {
        spawnPositions.Clear();
    }

    public void SpawnEnemiesPos(int battleEnemyNum)
    {   
        enemyCount=battleEnemyNum;
        spawnPositionsRoot.transform.position = GameManager.instance.player.transform.position;
        int posIdx;
        for (int i = 0; i < battleEnemyNum; i++)
        {
            posIdx = UnityEngine.Random.Range(0, spawnPositions.Count);
            SpawnEnemyAtPosition(posIdx);                      
        }
        if(GameManager.instance.stage == 10 || GameManager.instance.stage == 20)
            BossSpawnPosition();
    }

    void SpawnEnemyAtPosition(int posIdx)
    {
        int prefabIdx = UnityEngine.Random.Range(0, 11);
        if (prefabIdx <= 2)
        {
            GameObject closeEnemy = GameManager.instance.objectPool.SpawnFromPool("CloseEnemys");
            closeEnemy.transform.position = spawnPositions[posIdx].position;
        }
        else if (prefabIdx > 2 && prefabIdx <= 5)
        {
            GameObject rangedEnemy = GameManager.instance.objectPool.SpawnFromPool("RangedEnemys");
            rangedEnemy.transform.position = spawnPositions[posIdx].position;
        }
        else if (prefabIdx > 5 && prefabIdx <= 7)
        {
            GameObject rangedEnemy = GameManager.instance.objectPool.SpawnFromPool("NinjaEnemys");
            rangedEnemy.transform.position = spawnPositions[posIdx].position;
        }
        else if (prefabIdx > 7 && prefabIdx <= 9)
        {
            GameObject rangedEnemy = GameManager.instance.objectPool.SpawnFromPool("SniperEnemys");
            rangedEnemy.transform.position = spawnPositions[posIdx].position;
        }
        else if (prefabIdx > 9 && prefabIdx <= 10)
        {
            GameObject rangedEnemy = GameManager.instance.objectPool.SpawnFromPool("BombEnemys");
            rangedEnemy.transform.position = spawnPositions[posIdx].position;
        }
    }
    public void BossSpawnPosition()
    {        
        if(GameManager.instance.difficult >= 5)
        {
            AddBossSpawn();
        }
        if(GameManager.instance.difficult == 8)
        {
            AddBossSpawn();
        }
        if (GameManager.instance.stage == 10)
        {
            enemyCount++;
            CheckDifficult(MidBossSpawn());
            return;
        }
        enemyCount++;
        CheckDifficult(FinalBossSpawn());
    }
    void AddBossSpawn()
    {
        enemyCount++;            
        CheckDifficult(MidBossSpawn());
    }

    void CheckDifficult(GameObject bossObj)
    {
        if(GameManager.instance.difficult >= 2)
            GameManager.instance.AddStat2Difficult(true,bossObj);
        if(GameManager.instance.difficult >= 4)
            GameManager.instance.AddStat2Difficult(true,bossObj);
        if(GameManager.instance.difficult >= 6)
            GameManager.instance.AddStat2Difficult(true,bossObj);
    }

    GameObject MidBossSpawn()
    {        
        GameObject bossObj=Instantiate(BossPrefebs[0], spawnPositions[0].position, Quaternion.identity); // MidBoss       
        return bossObj;
    }

    GameObject FinalBossSpawn()
    {
        GameObject bossObj=Instantiate(BossPrefebs[1], spawnPositions[0].position, Quaternion.identity); // FinalBoss
        return bossObj;        
    }

    public void DamageTxtPool(int damage,Vector2 spawnPosition)
    {           
        GameObject damageTxt = GameManager.instance.objectPool.SpawnFromPool("DamageTxt");        
        Vector2 newpos= new Vector2(spawnPosition.x,spawnPosition.y+1);
        damageTxt.transform.position = newpos;
        
        DamageUI damagevalue = damageTxt.GetComponent<DamageUI>();
        damagevalue.DamageTxt(damage);
    }
}
