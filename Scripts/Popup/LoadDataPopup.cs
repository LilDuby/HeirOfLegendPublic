using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataPopup : PopupUI
{
    public DataManager DM;
    public GameManager GM;    
    public bool[] fileExists=new bool[3]{false,false,false};
    public Button[] loadDataBtn=new Button[3];
    public GameObject SaveInfoPanel;
    public List<GameData> loadData = new List<GameData>();
    public List<TextMeshProUGUI> loadBtnText=new List<TextMeshProUGUI>();
    
    void Start()
    {
        DM=DataManager.instance;
        GM=GameManager.instance;
        Initiailzer();
    } 

    public void Initiailzer() //정보창 최신화
    { 
        for(int i = 0; i < 3;i++)
        {            
            if(File.Exists(DM.path[i]))
            {
                fileExists[i] = true;
                LoadGame(i);
                LoadBtnText(i);
            }
            else 
            {
                fileExists[i] = false;
                loadData.Add(new GameData());
                loadBtnText[i].fontSize=80;
                loadBtnText[i].text="No SaveFile";
            }
        }
    }
    public void OnChoiseNum(int num)
    {           
        LoadResetData(); // 초기화
        DM.SaveDataNum=num; //선택한 세이브파일 번호
        if(!fileExists[num]) //저장위치에 저장파일이 없다면
        {
            DM.StartNewGame(); //새게임데이터 넣기
            DM.gold=DM.saveData.Gold;
            DM.SaveGame(num); //빈세이브파일 생성
            DM.gold=0;
            loadData[num]=DM.saveData;
        }
        else
        {
            DM.saveData=loadData[num]; //기존게임데이터 불러오기
        }
        CheckSaveDataCharacter(num); //케릭터 있는지 확인   
        Initiailzer();            
    }

    void CheckSaveDataCharacter(int num)
    {   
        if(!loadData[num].character) //기존케릭터 없음
        {   
            OnNewGameCheckPopup();
        }
        else //기존케릭있음
        {
            OnSaveInfoPopup(); //케릭터 정보창 
        }
    }

    public void LoadResetData()
    {
        DM.saveData=null; //나가기 할 경우 기존 정보 지우기
        DM.ResetData(); 
    }

    public void LoadBtnText(int num)
    {
        string className="없음";
        loadBtnText[num].fontSize=36;
        if(loadData[num].character)
        {
            className = DM.classList[loadData[num].ClassNum].className;
        }
        loadBtnText[num].text=$"캐릭터:{className} 보유유산: {loadData[num].Legacy} \n[{loadData[num].saveTime}]";
    }

    public bool TryLoadData(out GameData data,int num)
    {
        if(fileExists[num])
        {
            var jsonData= File.ReadAllText(DM.path[num]); //역직렬화
            data=JsonUtility.FromJson<GameData>(jsonData); //역직렬화 한거 게임데이터 형식으로 변환
            return true;
        }
        else
        {
            data=null;
            return false;
        }
    }
    
    public bool LoadGame(int num) //num번 세이브 데이터 클릭
    {
        if(TryLoadData(out GameData data,num))
        {
            loadData.Add(data); //역직렬화한거 데이터매니저에 넣기
            return true;
        }
        else
        {
            loadData.Add(null);
            return false;
        }
    }

    public void OnClickStart()
    {   
        if(!GM.isTutorial)
        GM.StartGame();
    }

    public void OnNewGameCheckPopup()
    {   
        GM.OnplayTutorial();
        UIManager.instance.ShowUI<NewGameCheckPopup>(UIs.Popup);
    }
    
    public void OnSaveInfoPopup()
    {   
        if(!GM.isTutorial)
        UIManager.instance.ShowUI<SaveInfoPopup>(UIs.Popup);
    }    

    public void OnDeletPopup()
    {   
        if(!GM.isTutorial)
        UIManager.instance.ShowUI<DeletPopup>(UIs.Popup);
    }

    public void OnClickBack()
    { 
        if(!GM.isTutorial)
        {
            LoadResetData();
            UIManager.instance.HideUI<LoadDataPopup>();
            OffSaveinfoPopup();
        }
    }

    public void OffSaveinfoPopup()
    {
        if(!GM.isTutorial)
        {
            GameObject Popup = UIManager.instance.FindUI<SaveInfoPopup>(UIs.Popup);
            if(Popup!=null && Popup.activeSelf)
                UIManager.instance.HideUI<SaveInfoPopup>();
        }        
    }
}
