public class DungeonPopup : PopupUI
{    
    public void GoToDungeon()
    {
        OffDungeanPopup();
        GameManager.instance.GoToDungeonScene();
    }   

    public void OffDungeanPopup()
    {
        UIManager.instance.HideUI<DungeonPopup>();
    }
}
