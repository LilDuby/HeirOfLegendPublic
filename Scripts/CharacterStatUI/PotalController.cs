using System.Collections.Generic;
using UnityEngine;

public class PotalController : MonoBehaviour
{   
    GameManager GM;
    ItemFind itemFind;
    bool isOpen=false;
    int randomNum=0;
    public Transform rooms;
    public DungeonSceneUI dungeonScene;
    List<Transform> roomList=new List<Transform>();

    void Start()
    {
        GM=GameManager.instance;
        for(int i=0;i<rooms.childCount;i++)        
            roomList.Add(rooms.GetChild(i).transform);
        
        itemFind=roomList[1].GetComponentInChildren<ItemFind>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {           
        if(collider.gameObject.CompareTag("Player"))
        {   
            CheckEnemies();
            if(TutorialCheck()) return;
            if(isOpen)
            {
                if(GM.stage == 9 || GM.stage == 19) //now 9 stat >> next Boss stat
                {
                    randomRoom(rooms.childCount-1); //Boss stat
                    AudioManager.instance.BossClipPlay();
                }
                else if(GM.stage == 20)
                {
                    CharacterManager.instance.EndGameCheck();
                    return;
                }
                else
                {   
                    AudioManager.instance.BgmClipPlay();
                    
                    randomNum=Random.Range(0, rooms.childCount-1);
                    while(GM.clearNum==randomNum) //같은방 2번연속 입장 방지
                    {   
                        randomNum=Random.Range(0, rooms.childCount-1);
                    }

                    if(GM.clearNum>=0 && GM.clearNum <= 2) //이전방이 긍정형 방일때
                    {
                        randomNum=Random.Range(3, 4); //전투방으로
                    }
                    
                    randomRoom(randomNum);
                }
                
                isOpen=false;
            }
        }
    }

    void randomRoom(int num)
    {
        GameManager.instance.ClearItemPools();
        dungeonScene.OnSaveGame();
        MoveRoom(num);
        GM.stage++;
        GM.clearNum=num; //clearstage
        switch(num)
        {
            case 0: //로비,상점             
            
            break;

            case 1: //상자방
            itemFind.isOpen=false;            
            break;

            case 2: //회복소

            break;

            case 3: //일반전투,일반 보상 
            Battle(5);
            break;

            case 4: //큰전투,좋은 보상 
            Battle(10);
            break;

            default: //보스, 클리어             
            Battle(10);
            break;
        }
    }

    void MoveRoom(int num)
    {           
        Vector3 newPosition = roomList[num].position;        
        GameManager.instance.player.transform.position=newPosition;
        newPosition.z = Camera.main.transform.position.z; // 기존 z축 위치 유지
        newPosition.y = Camera.main.transform.position.y;
        Camera.main.transform.position = newPosition;
    }

    void Battle(int num)
    {   
        CharacterManager.instance.SpawnEnemiesPos(GM.stage + num + GM.mobSpawn);
    }

    void CheckEnemies()
    {
        if(CharacterManager.instance.enemyCount==0)
        {
            isOpen=true;
        }
    }

    bool TutorialCheck()
    {
        if(GM.tutoNum==9) 
        {
            GM.info.ChengChapter(3);
            GM.OnplayTutorial();
            randomRoom(3);
            isOpen=false;
            return true;
        }
        else if(GM.tutoNum==10)
        {
            if(isOpen)
            {
                UIManager.instance.HideUI<TutorialInfoPopup>();
                GM.isTutorial=false;
                GM.tutoNum=0;
            }
        }
        return false;
    }
}
