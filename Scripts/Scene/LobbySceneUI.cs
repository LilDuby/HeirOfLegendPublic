public class LobbySceneUI : SceneUI
{   
    void Start()
    {
        OnKeyGuidePopup();
        
    }
    
    public void OnKeyGuidePopup()
    {
        UIManager.instance.ShowUI<KeyGuidePopup>(UIs.Popup);
    }    

    public void OnDungeanPopup()
    {
        UIManager.instance.ShowUI<DungeonPopup>(UIs.Popup);
        UIManager.instance.HideUI<KeyGuidePopup>();
    }
}
