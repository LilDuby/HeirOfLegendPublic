public class SaveInfoPopup : PopupUI
{
    StartDataController startData;
    void OnEnable() 
    {
        DataManager.instance.LoadCharacter(); //케릭터 직업로드
        DataManager.instance.LoadItem(); //케릭터 아이템로드
        DataManager.instance.LoadStatModifiers(); //케릭터 스텟로드
        DataManager.instance.LoadSaveData(); //케릭터 돈,유산,난이도 정보로드
        startData=GetComponent<StartDataController>();
        startData.ShowData(); //정보창 text변화
    }
}
