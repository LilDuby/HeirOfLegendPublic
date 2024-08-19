using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataController : MonoBehaviour
{
    public DataManager DM;
    public StartDataController startData;
    public bool[] fileExists=new bool[3]{false,false,false};
    public Button[] loadDataBtn=new Button[3];    
    public Button startGameBtn;
    public GameObject SaveInfoPanel;
    public GameObject CheckPanel;    

    void Start()
    {
        DM=DataManager.instance;
        startData=SaveInfoPanel.GetComponent<StartDataController>();        
        Initiailzer();
    } 

    public void Initiailzer() //정보창 최신화
    { 
        for(int i = 0; i < 3;i++)
        {            
            if(File.Exists(DM.path[i]))
            {
                fileExists[i]=true;                
            }
            else 
            {
                fileExists[i]=false;                
            }
        }
    }
    public void OnChoiseNum(int num)
    {   
        BackBtn(); // 초기화
        DM.SaveDataNum=num; //선택한 세이브파일 번호
        if(!fileExists[num]) //저장위치에 저장파일이 없다면
        {
            DM.StartNewGame(); //새게임데이터 넣기
            DM.SaveGame(num); //빈세이브파일 생성
            CheckSaveDataCharacter(); //케릭터 있는지 확인
        }
        else
        {            
            LoadGame(num); //기존게임데이터 불러오기
            CheckSaveDataCharacter(); //케릭터 있는지 확인
        }
        
    }

    void CheckSaveDataCharacter()
    {
        if(!DM.saveData.character) //기존케릭터 없음
        {
            CheckPanel.SetActive(true); //새케릭만들기
        }

        else //기존케릭있음
        {
            DM.LoadCharacter(); //케릭터 직업로드
            DM.LoadItem(); //케릭터 아이템로드
            DM.LoadStatModifiers(); //케릭터 스텟로드
            DM.LoadSaveData(); //케릭터 돈,유산,난이도 정보로드            
            SaveInfoPanel.SetActive(true); //케릭터 정보창
            startData.ShowData(); //정보창 text변화
        }
    }
    public void BackBtn()
    {
        DM.saveData=null; //나가기 할 경우 기존 정보 지우기
        DM.ResetData(); 
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
            DM.saveData= data; //역직렬화한거 데이터매니저에 넣기
            return true;
        }
        else
        {
            DM.saveData=null;
            return false;
        }
    }

    public void OnDeletSaveData() //세이브 데이터 삭제
    {
        File.Delete(DM.path[DM.SaveDataNum]);
        DM.saveData=null;
        Initiailzer(); //UI창 최신화
    }    
}